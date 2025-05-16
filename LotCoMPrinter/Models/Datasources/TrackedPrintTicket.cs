using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datasources;

public partial class TrackedPrintTicket(PrintTicket Ticket) : ObservableObject()
{
    /// <summary>
    /// The PrintTicket that this class was made to track.
    /// Does not receive any edits.
    /// </summary>
    [ObservableProperty]
    public partial PrintTicket Untracked {get; private set;} = Ticket;

    /// <summary>
    /// A copy of the UntrackedTicket that tracks edits.
    /// </summary>
    [ObservableProperty]
    public partial PrintTicket Tracked {get; set;} = Ticket;
}