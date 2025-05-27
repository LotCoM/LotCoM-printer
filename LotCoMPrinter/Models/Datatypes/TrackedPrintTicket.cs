using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datatypes;

public partial class TrackedPrintTicket(PrintTicket Ticket) : ObservableObject()
{
    /// <summary>
    /// The PrintTicket that this class was made to track.
    /// Does not receive any edits.
    /// </summary>
    [ObservableProperty]
    public partial PrintTicket Untracked { get; private set; } = Ticket;

    /// <summary>
    /// A copy of the UntrackedTicket that tracks edits.
    /// </summary>
    [ObservableProperty]
    public partial PrintTicket Tracked { get; set; } = new PrintTicket
    (
        Ticket.Process,
        Ticket.Part,
        Ticket.SerializationMode,
        Ticket.SerialNumber,
        Ticket.ProductionDate,
        Ticket.ProductionShift,
        Ticket.ProductionQuantity,
        Ticket.ProductionOperator,
        Ticket.VariableFields,
        Ticket.FirstPartialDataSet,
        Ticket.SecondPartialDataSet
    );

    /// <summary>
    /// Overwrites changes made to Tracked by assigning it the value of Untracked.
    /// </summary>
    public void DiscardChanges()
    {
        Tracked = Untracked;
    }

    /// <summary>
    /// Merges (saves) changes made to Tracked by overwriting Untracked with its value.
    /// </summary>
    /// <returns></returns>
    public void MergeChanges()
    {
        Untracked = Tracked;
    }
}