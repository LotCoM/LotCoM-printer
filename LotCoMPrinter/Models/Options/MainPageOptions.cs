using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Options;

/// <summary>
/// Provides a control structure for the UI Options of the MainPage.
/// </summary>
public partial class MainPageOptions(): ObservableObject()
{
    /// <summary>
    /// Provides default widths for the Open Print Tickets Panel.
    /// </summary>
    public enum OpenPrintTicketsPanelWidths
    {
        Open = 350,
        Closed = 50
    }

    /// <summary>
    /// Provides constant Defaults for the MainPage Options.
    /// </summary>
    private static class DefaultOptions
    {
        public const int SelectedPrintTicketIndex = -1;
        public const bool HasActivePrintTicket = false;
        public const VariableFieldSet DisplayedVariableFields = null;
        public const OpenPrintTicketsPanelWidths OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Closed;
        public const bool IsOpenPrintTicketsPanelShown = false;
        public const bool IsWindowHeaderShown = true;
        public const string WindowHeaderLabelText = "";
    }

    /// <summary>
    /// Controls whether there is an active Print Ticket to display or not.
    /// </summary>
    [ObservableProperty]
    public partial bool HasActivePrintTicket {get; set;} = DefaultOptions.HasActivePrintTicket;

    /// <summary>
    /// Controls the index of the currently selected PrintTicket in the Open Print Tickets Panel ListView.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedPrintTicketIndex {get; set;} = DefaultOptions.SelectedPrintTicketIndex;

    /// <summary>
    /// Provides the width Option of the Open Print Tickets Panel.
    /// </summary>
    [ObservableProperty]
    public partial OpenPrintTicketsPanelWidths OpenPrintTicketsPanelWidth {get; set;} = DefaultOptions.OpenPrintTicketsPanelWidth;

    /// <summary>
    /// Provides the Visibility state of the Open Print Tickets Panel.
    /// </summary>
    [ObservableProperty]
    public partial bool IsOpenPrintTicketsPanelShown {get; set;} = DefaultOptions.IsOpenPrintTicketsPanelShown;

    /// <summary>
    /// Provides the Visibility state of the Window Header.
    /// </summary>
    [ObservableProperty]
    public partial bool IsWindowHeaderShown {get; set;} = DefaultOptions.IsWindowHeaderShown;

    /// <summary>
    /// Provides the text to display on the Window Header Label.
    /// </summary>
    [ObservableProperty]
    public partial string WindowHeaderLabelText {get; set;} = DefaultOptions.WindowHeaderLabelText;

    /// <summary>
    /// Opens the Open Print Tickets Panel.
    /// </summary>
    public void RaiseOpenPrintTicketsPanel()
    {
        OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Open;
        IsOpenPrintTicketsPanelShown = true;
    }

    /// <summary>
    /// Closes the Open Print Tickets Panel.
    /// </summary>
    public void CollapseOpenPrintTicketsPanel()
    {
        OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Closed;
        IsOpenPrintTicketsPanelShown = false;
    }

    /// <summary>
    /// Opens the UI View of SelectedPrintTicket.
    /// </summary>
    /// <param name="Index"></param>
    public void OpenActivePrintTicket(int Index)
    {
        HasActivePrintTicket = true;
        SelectedPrintTicketIndex = Index;
        IsWindowHeaderShown = false;
    }

    /// <summary>
    /// Closes the UI View of SelectedPrintTicket.
    /// </summary>
    public void CloseActivePrintTicket()
    {
        HasActivePrintTicket = false;
        SelectedPrintTicketIndex = -1;
        IsWindowHeaderShown = true;
    }
}