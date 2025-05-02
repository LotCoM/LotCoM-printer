using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;

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
        Closed = 0
    }

    /// <summary>
    /// Provides constant Defaults for the MainPage Options.
    /// </summary>
    private static class DefaultOptions
    {
        public const PrintTicket? SelectedPrintTicket = null;
        public const int SelectedPrintTicketIndex = -1;
        public const bool HasActivePrintTicket = false;
        public const Department? SelectedDepartment = null;
        public const int SelectedDepartmentIndex = -1;
        public const Process? SelectedProcess = null;
        public const int SelectedProcessIndex = -1;
        public static readonly List<Part>? SelectedProcessParts = null;
        public const Part? SelectedPart = null;
        public const int SelectedPartIndex = -1;
        public const VariableFieldSet DisplayedVariableFields = null;
        public const OpenPrintTicketsPanelWidths OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Closed;
        public const bool IsOpenPrintTicketsPanelShown = false;
    }

    /// <summary>
    /// Provides whether there is an active Print Ticket to display or not.
    /// </summary>
    [ObservableProperty]
    public partial bool HasActivePrintTicket {get; set;} = DefaultOptions.HasActivePrintTicket;

    /// <summary>
    /// Provides the currently selected PrintTicket in the Open Print Tickets Panel.
    /// This is NOT equivalent to the ActivePrintTicket.
    /// </summary>
    [ObservableProperty]
    public partial PrintTicket? SelectedPrintTicket {get; set;} = DefaultOptions.SelectedPrintTicket;

    /// <summary>
    /// Provides the index of the currently selected PrintTicket in the Open Print Tickets Panel ListView.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedPrintTicketIndex {get; set;} = DefaultOptions.SelectedPrintTicketIndex;

    /// <summary>
    /// Provides the currently selected Department for the active PrintTicket.
    /// </summary>
    [ObservableProperty]
    public partial Department? SelectedDepartment {get; set;} = DefaultOptions.SelectedDepartment;

    /// <summary>
    /// Provides the index of the currently selected Department for the active PrintTicket.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedDepartmentIndex {get; set;} = DefaultOptions.SelectedDepartmentIndex;

    /// <summary>
    /// Provides the currently selected Process for the active PrintTicket.
    /// </summary>
    [ObservableProperty]
    public partial Process? SelectedProcess {get; set;} = DefaultOptions.SelectedProcess;

    /// <summary>
    /// Provides the index of the currently selected Process for the active PrintTicket.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedProcessIndex {get; set;} = DefaultOptions.SelectedProcessIndex;

    /// <summary>
    /// Provides the list of Parts assigned to the currently selected Process for the active PrintTicket.
    /// </summary>
    [ObservableProperty]
    public partial List<Part>? SelectedProcessParts {get; set;} = DefaultOptions.SelectedProcessParts;

    /// <summary>
    /// Provides the currently selected Part for the active PrintTicket.
    /// </summary>
    [ObservableProperty]
    public partial Part? SelectedPart {get; set;} = DefaultOptions.SelectedPart;

    /// <summary>
    /// Provides the index of the currently selected Part for the active PrintTicket.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedPartIndex {get; set;} = DefaultOptions.SelectedPartIndex;

    /// <summary>
    /// Provides the displayed VariableFieldSet for the active PrintTicket.
    /// Allows the UI to display dummy values without interfering with back-end logic.
    /// </summary>
    [ObservableProperty]
    public partial VariableFieldSet? DisplayedVariableFields {get; set;} = DefaultOptions.DisplayedVariableFields;

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
}