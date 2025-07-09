using LotCoMPrinter.Models.Enums;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Services;

namespace LotCoMPrinter.Models.Datatypes;

public class PrintJob
{
    /// <summary>
    /// The PartialDataSet Number of Source to use for PartialTag Label PrintJobs.
    /// </summary>
    private int PartialSetNumber;

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
            if (Type == PrintJobType.Full)
            {
                Label = await LabelGenerator.GenerateLabelAsync(Source);
            }
            else if (Type == PrintJobType.Partial)
            {
                Label = await PartialTagGenerator.GenerateTagAsync(Source, PartialSetNumber);
            }
        }
        catch (LabelBuildException _ex)
        {
            throw new LabelBuildException(_ex.Message);
        }
    }

    /// <summary>
    /// Creates a Print Job that can generate a BasketLabel and spool a print job to the printing system.
    /// </summary>
    /// <param name="Ticket">A PrintTicket object to use as the source of data for this Job.</param>
    public PrintJob(PrintTicket Ticket)
    {
        Type = PrintJobType.Full;
        Source = Ticket;
    }

    /// <summary>
    /// Creates a Print Job that can generate a PartialTag Label and spool a print job to the printing system.
    /// </summary>
    /// <param name="Ticket">A PrintTicket object to use as the source of data for this Job.</param>
    /// <param name="PartialSetNumber">The PartialDataSet Number to use for a PartialTag PrintJob (1 or 2).</param>
    public PrintJob(PrintTicket Ticket, int PartialSetNumber)
    {
        Type = PrintJobType.Partial;
        Source = Ticket;
        this.PartialSetNumber = PartialSetNumber;
    }

    /// <summary>
    /// Creates a Print Job that can generate a Reprint Label from a PrintTicket.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <param name="Type">PrintJobType.Full or PrintJobType.Reprint.</param>
    public static PrintJob CreateReprintJob(PrintTicket Ticket)
    {
        /**
        Jared: this method is flawed. 

        Right now, it runs just like the basic constructor.
        The default is to create a `PrintJob` to print a `Full` Label.
        This will treat the Label as a 'new' one, which will prompt logging.
        
        Two questions:
        - Is this what we want?
        - If not, how can we make a `PrintJob` that won't log?

        That's your job!

        Look at the `PrintLogger` class to see how it Logs.
        Understanding that class will help you a lot.

        Good luck!
        **/
        PrintJob ReprintJob = new PrintJob(Ticket);
        ReprintJob.Type = PrintJobType.Full; // Modify this so it doesn't Log
        return ReprintJob;
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
            Printed = await PrintHandler.PrintLabelAsync(Label!);
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