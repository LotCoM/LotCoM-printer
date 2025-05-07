using System.Drawing;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Creates a Print Job that can generate a Bitmap image Label and spool a print job to the printing system.
/// </summary>
/// <param name="Capture">A validated InterfaceCapture object.</param>
/// <param name="Header">The string to use as the Label's Header text.</param>
public class LabelPrintJob(InterfaceCapture Capture, string Header) 
{
    /// <summary>
    /// The InterfaceCapture object to use as the source of Data for this Label Print Job.
    /// </summary>
    private readonly InterfaceCapture Capture = Capture;

    /// <summary>
    /// The formatted Header string to apply to the top of this Label.
    /// </summary>
    private readonly string Header = Header;

    /// <summary>
    /// A Bitmap Label image.
    /// </summary>
    private Bitmap? Label = null;

    /// <summary>
    /// Creates a Label object and Bitmap image from the Job's saved information. 
    /// Stores the generated Bitmap image in _label property.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="LabelBuildException">Thrown if the LabelGenerator failed to create a Label.</exception>
    private async Task GenerateLabelImage() 
    {
        // generate a new Label image and store it in the _label property
        try 
        {
            Label = await LabelGenerator.GenerateLabelAsync(Capture, Header);
        // there was an unexpected error in the Label generation
        } 
        catch (LabelBuildException _ex) 
        {
            throw new LabelBuildException(_ex.Message);
        }
    }

    /// <summary>
    /// Performs Serial Number processing logic at the end of a Print Job. 
    /// Calculates whether to consume or cache the Serial Number. 
    /// </summary>
    /// <param name="PrintResult"></param>
    /// <returns></returns>
    private async Task ProcessSerialNumber(bool PrintResult) 
    {
        // retrieve data from the Capture to improve processing time
        Process SelectedProcess = Capture.Process;
        string PartNumber = Capture.Part.PartNumber;
        SerializationModes Serialization = SelectedProcess.Serialization;
        // retrieve the Serial Number from the Capture data
        string SerialNumber;
        if (Serialization == SerializationModes.JBK) 
        {
            SerialNumber = Capture.VariableFields.JBKNumber.ToString()!;
        } 
        else 
        {
            SerialNumber = Capture.VariableFields.LotNumber!;
        }
        // prepare the serial cache for the end of the print job
        SerialCacheController SerialCache = new SerialCacheController();
        // the print was successful; remove the cached serial number here (if the label was full)
        if (PrintResult) 
        {
            await SerialCache.RemoveCachedSerialNumber(SerialNumber, PartNumber);
        // the print failed; cache the serial number
        } 
        else 
        {
            await SerialCache.CacheSerialNumber(SerialNumber, PartNumber);
        }
    }

    /// <summary>
    /// Runs the Print Job (creates a Handler for the Job and spools it to the OS' printing system).
    /// </summary>
    /// <remarks>
    /// Throws LabelBuildException if the LabelGenerator failed to create a Label.
    /// Throws PrintRequestException if there was an error communicating with the Printer or Printing System.
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    /// <exception cref="PrintRequestException"></exception>
    public async Task<bool> Run() 
    {
        // generate a Label from the saved Label information
        try 
        {
            await GenerateLabelImage();
        // there was an unexpected error in the label build; pass the error on
        } 
        catch (LabelBuildException _ex) 
        {
            throw new LabelBuildException(_ex.Message);
        }
        // create a PrintHandler object for the new Label
        bool Printed = false;
        PrintHandler LabelPrinter = new PrintHandler(Label!);
        // try to print the Label
        try 
        {
            await LabelPrinter.PrintLabelAsync();
            Printed = true;
        // handle errors thrown by the PrintLabelAsync() method
        } 
        catch (PrintRequestException _ex)
        {
            throw new PrintRequestException(_ex.Message);
        }
        // process the serial number attached to this Label
        await ProcessSerialNumber(Printed);
        // log successful print jobs
        if (Printed) 
        {
            try 
            {
                await PrintLogger.LogPrintEvent(Capture);
            // the print logging was forced to default on its bulk logging; report this to user
            } 
            catch (Exception _ex) 
            {
                throw new PrintLogException(_ex.Message);
            }
        }
        return Printed;
    }
}