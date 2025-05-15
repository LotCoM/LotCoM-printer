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
        Closed = 90
    }

    /// <summary>
    /// Provides constant Defaults for the MainPage Options.
    /// </summary>
    private static class DefaultOptions
    {
        public const int SelectedPrintTicketIndex = -1;
        public const OpenPrintTicketsPanelWidths OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Closed;
        public const bool IsOpenPrintTicketsPanelShown = false;
        public const bool IsWelcomeMenuShown = true;
        public const string WelcomeMenuTitleLabelText = "";
        public const string WelcomeMenuSubTitleLabelText = "";
        public const bool IsActivePrintTicketMenuShown = false;
    }

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
    /// Provides the Visibility state of the Welcome Menu.
    /// </summary>
    [ObservableProperty]
    public partial bool IsWelcomeMenuShown {get; set;} = DefaultOptions.IsWelcomeMenuShown;
    
    /// <summary>
    /// Provides the text to display on the Window Title Label.
    /// </summary>
    [ObservableProperty]
    public partial string WelcomeMenuTitleLabelText {get; set;} = DefaultOptions.WelcomeMenuTitleLabelText;

    /// <summary>
    /// Provides the text to display on the Window Sub-Title Label.
    /// </summary>
    [ObservableProperty]
    public partial string WelcomeMenuSubTitleLabelText {get; set;} = DefaultOptions.WelcomeMenuSubTitleLabelText;

    /// <summary>
    /// Provides the Visibility state of the ActivePrintTicket Menu.
    /// </summary>
    [ObservableProperty]
    public partial bool IsActivePrintTicketMenuShown {get; set;} = DefaultOptions.IsActivePrintTicketMenuShown;

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
    /// Updates the SelectedPrintTicketIndex without opening or changing the Active Print Ticket display.
    /// </summary>
    /// <param name="Index"></param>
    public void SetSelectedPrintTicket(int Index)
    {
        SelectedPrintTicketIndex = Index;
    }

    /// <summary>
    /// Opens the UI View of SelectedPrintTicket.
    /// </summary>
    public void OpenActivePrintTicket()
    {
        IsWelcomeMenuShown = false;
        IsActivePrintTicketMenuShown = true;
    }

    /// <summary>
    /// Closes the UI View of SelectedPrintTicket.
    /// </summary>
    public void CloseActivePrintTicket()
    {
        IsWelcomeMenuShown = true;
        IsActivePrintTicketMenuShown = false;
        SelectedPrintTicketIndex = -1;
    }
}