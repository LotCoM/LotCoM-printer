using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Services;

namespace LotCoMPrinter.Models.Datatypes;

/// <summary>
/// Creates a Print Job that can generate a Bitmap image Label and spool a print job to the printing system.
/// </summary>
/// <param name="Ticket">A PrintTicket object.</param>
public class LabelPrintJob(PrintTicket Ticket) 
{
    /// <summary>
    /// The PrintTicket object to use as the source of Data for this Label Print Job.
    /// </summary>
    public readonly PrintTicket Ticket = Ticket;

    /// <summary>
    /// A generated Label object.
    /// </summary>
    public Label? Label = null;

    /// <summary>
    /// Creates a BasketLabel object from the Job's saved PrintTicket object. 
    /// Stores the generated BasketLabel in the Label property.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="LabelBuildException">Thrown if the LabelGenerator failed to create a Label.</exception>
    private async Task GenerateLabelImage() 
    {
        // generate a new Label image and store it in the Label property
        try 
        {
            Label = await LabelGenerator.GenerateLabelAsync(Ticket);
        } 
        catch (LabelBuildException _ex) 
        {
            throw new LabelBuildException(_ex.Message);
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
        // set the final ProductionDate and generate a Label from the saved PrintTicket
        Ticket.ProductionDate = DateTime.Now;
        try
        {
            await GenerateLabelImage();
        }
        catch (LabelBuildException _ex)
        {
            throw new LabelBuildException(_ex.Message);
        }
        // create a PrintHandler object for the new Label and attempt to print it
        bool Printed = false;
        try
        {
            Printed = await PrintHandler.PrintLabelAsync(Label!);
        }
        catch (PrintRequestException _ex)
        {
            throw new PrintRequestException(_ex.Message);
        }
        // log successful print jobs
        if (Printed) 
        {
            try 
            {
                await PrintLogger.LogPrintEvent(this);
            // the print logging was forced to default on its backup logging; report this to user
            } 
            catch (Exception _ex) 
            {
                throw new PrintLogException(_ex.Message);
            }
        }
        return Printed;
    }
}