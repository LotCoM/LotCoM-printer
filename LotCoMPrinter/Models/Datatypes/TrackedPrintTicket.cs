using CommunityToolkit.Mvvm.ComponentModel;
using LotComPrinter.Models.Services;

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
        DebugLogger.LogMessage("Starting TrackedPrintTicket initialization...", this);
        DebugLogger.LogMessage
        (
            "Source for TrackedPrintTicket: \"" +
            $"{Ticket.ToJSON()}"
            + "\"",
            this
        );
        DebugLogger.LogMessage("Setting Untracked to Source...", this);
        Untracked = Ticket;
        DebugLogger.LogMessage("Untracked set.", this);
        DebugLogger.LogMessage("Using PrintTicket.DeepCopy() to create a Copy of Untracked for Tracked...", this);
        try
        {
            Tracked = PrintTicket.DeepCopy(Ticket);
        }
        catch (Exception _ex)
        {
			DebugLogger.LogError("Failed to create a Deep Copy of Untracked.", _ex, this);
			throw;
        }
        DebugLogger.LogMessage("Deep Copy made; Tracked set.", this);
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
}