using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Options;

public partial class MainPageOptions(): ObservableObject()
{
    /// <summary>
    /// Provides default widths for the Open Print Tickets Panel.
    /// </summary>
    public enum OpenPrintTicketsPanelWidths
    {
        Open = 350,
        Closed = 0
    }

    /// <summary>
    /// Provides constant Defaults for the MainPage Options.
    /// </summary>
    private static class DefaultOptions
    {
        public const OpenPrintTicketsPanelWidths OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Closed;
    }

    /// <summary>
    /// Provides the width Option of the Open Print Tickets Panel.
    /// </summary>
    [ObservableProperty]
    public partial OpenPrintTicketsPanelWidths OpenPrintTicketsPanelWidth {get; set;} = DefaultOptions.OpenPrintTicketsPanelWidth;

    /// <summary>
    /// Opens the Open Print Tickets Panel.
    /// </summary>
    public void RaiseOpenPrintTicketsPanel()
    {
        OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Open;
    }

    /// <summary>
    /// Closes the Open Print Tickets Panel.
    /// </summary>
    public void CollapseOpenPrintTicketsPanel()
    {
        OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Closed;
    }
}