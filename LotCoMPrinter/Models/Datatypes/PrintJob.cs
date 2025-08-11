using LotComPrinter.Models.Enums;
using LotComPrinter.Models.Exceptions;
using LotComPrinter.Models.Services;

namespace LotComPrinter.Models.Datatypes;

/// <summary>
/// A configured PrintJob that takes source information, generates a Label, and prints that Label.
/// </summary>
public class PrintJob
{
    /// <summary>
    /// The PartialDataSet Number of Source to use for PartialTag Label PrintJobs.
    /// </summary>
    private int? PartialSetNumber;

    /// <summary>
    /// Contains the type 
    /// </summary>
    public PrintJobType Type { get; private set; }

    /// <summary>
    /// The PrintTicket to use as the source of Data for this Print Job.
    /// </summary>
    public PrintTicket Source { get; private set; }

    /// <summary>
    /// A generated Label object.
    /// </summary>
    public Label? Label { get; private set; } = null;

    /// <summary>
    /// Creates a Label object from the Job's saved Source object. 
    /// Stores the generated Label in the Label property.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="LabelBuildException">Thrown if the LabelGenerator failed to create a Label.</exception>
    private async Task GenerateLabel()
    {
        // generate a new Label image and store it in the Label property
        try
        {
            if (Type == PrintJobType.Partial)
            {
                Label = await PartialTagService.GenerateTagAsync(Source, (int)PartialSetNumber!);
            }
            // both Full and Reprint Labels are identical
            else
            {
                Label = await LabelService.GenerateLabelAsync(Source);
            }
        }
        catch (LabelBuildException _ex)
        {
            throw new LabelBuildException(_ex.Message);
        }
    }

    /// <summary>
    /// Creates a new PrintJob of Type with Ticket as its source of data.
    /// Optionally accepts a PartialDataSetNumber for Reprint type PrintJobs.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <param name="Type"></param>
    /// <param name="PartialSetNumber"></param>
    /// <exception cref="ArgumentException"></exception>
    public PrintJob(PrintTicket Ticket, PrintJobType Type, int? PartialSetNumber = null)
    {
        Source = Ticket;
        this.Type = Type;
        // confirm that Partial Type Jobs contain an acceptable Set number
        if (Type == PrintJobType.Partial)
        {
            if (PartialSetNumber != 1 && PartialSetNumber != 2)
            {
                throw new ArgumentException("Partial print jobs must have a partial set number");
            }
            this.PartialSetNumber = PartialSetNumber;
        }
        // nullify PartialSetNumber if it is a reprint Job
        else
        {
            this.PartialSetNumber = null;
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
        // set the final ProductionDate and generate a Label from the saved Source
        try
        {
            await GenerateLabel();
        }
        catch (LabelBuildException _ex)
        {
            throw new LabelBuildException(_ex.Message);
        }
        // create a PrintHandler object for the new Label and attempt to print it
        bool Printed = false;
        try
        {
            Printed = await PrintingService.PrintLabelAsync(Label!);
        }
        catch (PrintRequestException _ex)
        {
            throw new PrintRequestException(_ex.Message);
        }
        // log successful print jobs
        if (Printed && Type == PrintJobType.Full)
        {
            try
            {
                await LoggingService.LogPrintEvent(this);
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