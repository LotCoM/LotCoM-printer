using System.Drawing;
using LotCoMPrinter.Models.Serialization;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Labels;
using LotCoMPrinter.Models.Validators;
using LotCoMPrinter.Models.Datasources;

namespace LotCoMPrinter.Models.Printing;

/// <summary>
/// Creates a Print Job that can generate a Bitmap image Label and spool a print job to the printing system.
/// </summary>
/// <param name="Capture">A validated InterfaceCapture object.</param>
/// <param name="Header">The string to use as the Label's Header text.</param>
public class LabelPrintJob(InterfaceCapture Capture, string Header) {
    // private class properties to hold Label data, header, and generated Label Bitmap
    private InterfaceCapture _capture = Capture;
    private string _header = Header;
    private Bitmap? _label = null;

    /// <summary>
    /// Creates a Label object and Bitmap image from the Job's saved information. 
    /// Stores the generated Bitmap image in _label property.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="LabelBuildException">Thrown if the LabelGenerator failed to create a Label.</exception>
    private async Task GenerateLabelImage() {
        // generate a new Label image and store it in the _label property
        try {
            // full label
            if (_capture.BasketType!.Equals("Full")) {
                _label = await LabelGenerator.GenerateFullLabelAsync(_header, _capture);
            // partial label
            } else {
                _label = await LabelGenerator.GeneratePartialLabelAsync(_header, _capture);
            }
        // there was an unexpected error in the Label generation
        } catch (LabelBuildException _ex) {
            throw new LabelBuildException(_ex.Message);
        }
    }

    /// <summary>
    /// Performs Serial Number processing logic at the end of a Print Job. 
    /// Calculates whether to consume or cache the Serial Number. 
    /// </summary>
    /// <param name="PrintResult"></param>
    /// <returns></returns>
    private async Task ProcessSerialNumber(bool PrintResult) {
        // retrieve data from the Capture to improve processing time
        Process SelectedProcess = _capture.SelectedProcess!;
        string PartNumber = _capture.SelectedPart!.PartNumber;
        string Serialization = SelectedProcess.Serialization;
        // retrieve the Serial Number from the Capture data
        string SerialNumber;
        if (Serialization.Equals("JBK")) {
            SerialNumber = SelectedProcess.JBKNumber;
        } else {
            SerialNumber = SelectedProcess.LotNumber;
        }
        // prepare the serial cache for the end of the print job
        SerialCacheController SerialCache = new SerialCacheController();
        // the print was successful; remove the cached serial number here (if the label was full)
        if (PrintResult) {
            if (_capture.BasketType!.Equals("Full")) {
                await SerialCache.RemoveCachedSerialNumber(SerialNumber, PartNumber);
            }
        // the print failed; cache the serial number
        } else {
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
    public async Task<bool> Run() {
        // generate a Label from the saved Label information
        try {
            await GenerateLabelImage();
        // there was an unexpected error in the label build; pass the error on
        } catch (LabelBuildException _ex) {
            throw new LabelBuildException(_ex.Message);
        }
        // create a PrintHandler object for the new Label
        bool Printed = false;
        PrintHandler LabelPrinter = new PrintHandler(_label!);
        // try to print the Label
        try {
            await LabelPrinter.PrintLabelAsync();
            Printed = true;
        // handle errors thrown by the PrintLabelAsync() method
        } catch (PrintRequestException _ex){
            throw new PrintRequestException(_ex.Message);
        }
        // process the serial number attached to this Label
        await ProcessSerialNumber(Printed);
        // log successful print jobs
        if (Printed) {
            await PrintLogger.LogPrintEvent(_capture);
        }
        return Printed;
    }
}