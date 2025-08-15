using CommunityToolkit.Mvvm.ComponentModel;

namespace LotComPrinter.Models.Datatypes;

public partial class TrackedPrintTicket : ObservableObject
{
    /// <summary>
    /// The PrintTicket that the object was made to track.
    /// Does not receive any edits.
    /// </summary>
    [ObservableProperty]
    public partial PrintTicket Untracked { get; private set; }

    /// <summary>
    /// A copy of the UntrackedTicket that tracks edits.
    /// </summary>
    [ObservableProperty]
    public partial PrintTicket Tracked { get; set; }

    /// <summary>
    /// Creates a new TrackedPrintTicket that will track changes made to a copy of the passed Ticket.
    /// </summary>
    /// <param name="Ticket"></param>
    public TrackedPrintTicket(PrintTicket Ticket)
    {
        Untracked = Ticket;
        Tracked = PrintTicket.DeepCopy(Ticket);
    }

    /// <summary>
    /// Overwrites changes made to Tracked by assigning it the value of Untracked.
    /// </summary>
    public void DiscardChanges()
    {
        Tracked = PrintTicket.DeepCopy(Untracked);
    }

    /// <summary>
    /// Merges (saves) changes made to Tracked by overwriting Untracked with its value.
    /// </summary>
    /// <returns></returns>
    public void MergeChanges()
    {
        Untracked = PrintTicket.DeepCopy(Tracked);
    }

    /// <summary>
    /// Checks if the TrackedPrintTicket has unmerged changes (Tracked is not the same as Untracked).
    /// </summary>
    /// <returns></returns>
    public bool HasUnmergedChanges()
    {
        return Untracked.ToJSON().Equals(Tracked.ToJSON());
    }
}