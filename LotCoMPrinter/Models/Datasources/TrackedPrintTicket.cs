namespace LotCoMPrinter.Models.Datasources;

public class TrackedPrintTicket(PrintTicket Ticket)
{
    /// <summary>
    /// The PrintTicket that this class was made to track.
    /// Does not receive any edits.
    /// </summary>
    public readonly PrintTicket Untracked = Ticket;

    /// <summary>
    /// A copy of the UntrackedTicket that tracks edits.
    /// </summary>
    public PrintTicket Tracked = Ticket;
}