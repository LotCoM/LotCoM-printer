using CommunityToolkit.Mvvm.ComponentModel;
using LotCom.Types;

namespace LotComPrinter.Models.Datatypes;

public partial class TrackedPrintTicket : ObservableObject
{
    /// <summary>
    /// A Copy of the PrintTicket that the object was made to track.
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
        Untracked = PrintTicket.DeepCopy(Ticket);
        Tracked = PrintTicket.DeepCopy(Ticket);
    }

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