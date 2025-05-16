using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Options;

namespace LotCoMPrinter.ViewModels;

# pragma warning disable CA1416 // Validate platform compatibility

/// <summary>
/// Constructs a ViewModel for the MainPage class.
/// </summary>
public partial class MainPageViewModel : ObservableObject 
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
    /// Provides constant Defaults for the MainPage Visual State Options.
    /// </summary>
    private static class Defaults
    {
        public const OpenPrintTicketsPanelWidths OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Closed;
        public const bool IsOpenPrintTicketsPanelShown = false;
        public const bool IsWelcomeMenuShown = true;
        public const string WelcomeMenuTitleLabelText = "";
        public const string WelcomeMenuSubTitleLabelText = "";
        public const bool IsActivePrintTicketMenuShown = false;
    }

    private OpenPrintTicketsPanelWidths _openPrintTicketsPanelWidth = OpenPrintTicketsPanelWidths.Closed;
    /// <summary>
    /// Provides the width Option of the Open Print Tickets Panel.
    /// </summary>
    public OpenPrintTicketsPanelWidths OpenPrintTicketsPanelWidth
    {
        get { return _openPrintTicketsPanelWidth; }
        set
        {
            _openPrintTicketsPanelWidth = value;
            OnPropertyChanged(nameof(_openPrintTicketsPanelWidth));
            OnPropertyChanged(nameof(OpenPrintTicketsPanelWidth));
        }
    }

    private bool _isOpenPrintTicketsPanelShown = Defaults.IsOpenPrintTicketsPanelShown;
    /// <summary>
    /// Provides the Visibility state of the Open Print Tickets Panel.
    /// </summary>
    public bool IsOpenPrintTicketsPanelShown
    {
        get { return _isOpenPrintTicketsPanelShown; }
        set
        {
            _isOpenPrintTicketsPanelShown = value;
            OnPropertyChanged(nameof(_isOpenPrintTicketsPanelShown));
            OnPropertyChanged(nameof(IsOpenPrintTicketsPanelShown));
        }
    }

    private bool _isWelcomeMenuShown = Defaults.IsWelcomeMenuShown;
    /// <summary>
    /// Provides the Visibility state of the Welcome Menu.
    /// </summary>
    public bool IsWelcomeMenuShown
    {
        get { return _isWelcomeMenuShown; }
        set
        {
            _isWelcomeMenuShown = value;
            OnPropertyChanged(nameof(_isWelcomeMenuShown));
            OnPropertyChanged(nameof(IsWelcomeMenuShown));
        }
    }
    
    private string _welcomeMenuTitleLabelText = Defaults.WelcomeMenuTitleLabelText;
    /// <summary>
    /// Provides the text to display on the Window Title Label.
    /// </summary>
    public string WelcomeMenuTitleLabelText
    {
        get { return _welcomeMenuTitleLabelText; }
        set
        {
            _welcomeMenuTitleLabelText = value;
            OnPropertyChanged(nameof(_welcomeMenuTitleLabelText));
            OnPropertyChanged(nameof(WelcomeMenuTitleLabelText));
        }
    }

    private string _welcomeMenuSubTitleLabelText = Defaults.WelcomeMenuSubTitleLabelText;
    /// <summary>
    /// Provides the text to display on the Window Sub-Title Label.
    /// </summary>
    public string WelcomeMenuSubTitleLabelText
    {
        get { return _welcomeMenuSubTitleLabelText; }
        set
        {
            _welcomeMenuSubTitleLabelText = value;
            OnPropertyChanged(nameof(_welcomeMenuSubTitleLabelText));
            OnPropertyChanged(nameof(WelcomeMenuSubTitleLabelText));
        }
    }

    private bool _isActivePrintTicketMenuShown = Defaults.IsActivePrintTicketMenuShown;
    /// <summary>
    /// Provides the Visibility state of the ActivePrintTicket Menu.
    /// </summary>
    public bool IsActivePrintTicketMenuShown
    {
        get { return _isActivePrintTicketMenuShown; }
        set
        {
            _isActivePrintTicketMenuShown = value;
            OnPropertyChanged(nameof(_isActivePrintTicketMenuShown));
            OnPropertyChanged(nameof(IsActivePrintTicketMenuShown));
        }
    }

    private readonly List<Process> _allProcesses = new ProcessData().GetAllProcesses();
    /// <summary>
    /// The List of Processes in the Database, captured at the time of instantiation.
    /// </summary>
    public List<Process> AllProcesses
    {
        get {return _allProcesses;}
    }

    private ObservableCollection<PrintTicket> _openPrintTickets = [];
    /// <summary>
    /// The List of Open Print Tickets currently saved locally.
    /// </summary>
    public ObservableCollection<PrintTicket> OpenPrintTickets
    {
        get 
        {
            if (_openPrintTickets == null)
            {
                _openPrintTickets = new ObservableCollection<PrintTicket>();
            }
            return _openPrintTickets;
        }
    }

    private TrackedPrintTicket? _activeTicket = null;
    /// <summary>
    /// The currently active PrintTicket object with Tracked changes.
    /// </summary>
    public TrackedPrintTicket? ActiveTicket
    {
        get {return _activeTicket;}
        set
        {
            _activeTicket = value;
            OnPropertyChanged(nameof(_activeTicket));
            OnPropertyChanged(nameof(ActiveTicket));
        }
    }

    private PrintTicket? _selectedTicket = null;
    /// <summary>
    /// The currently selected PrintTicket object in the OpenPrintTickets CollectionView.
    /// </summary>
    public PrintTicket? SelectedTicket
    {
        get {return _selectedTicket;}
        set
        {
            _selectedTicket = value;
            OnPropertyChanged(nameof(_selectedTicket));
            OnPropertyChanged(nameof(SelectedTicket));
        }
    }

    /// <summary>
    /// Create a ViewModel to control the logic of a Main Page instance.
    /// </summary>
    public MainPageViewModel()
    {
        WelcomeMenuTitleLabelText = "Welcome";
        WelcomeMenuSubTitleLabelText = "Click 'Start New Label' to create a new Label or open In-Progress Labels by clicking the arrow button below.";
        _openPrintTickets = new ObservableCollection<PrintTicket>(PrintTicketCache.GetAllPrintTickets());
        SelectedTicket = OpenPrintTickets[0];
    }
    
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
    /// Opens the ActivePrintTicket Menu to display the current ActiveTicket.
    /// Additionally, collapses the OpenPrintTicketsPanel.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    public void OpenActivePrintTicketMenu()
    {
        if (ActiveTicket is null)
        {
            throw new NullReferenceException("Cannot display a 'null' ActivePrintTicket.");
        }
        IsWelcomeMenuShown = false;
        IsActivePrintTicketMenuShown = true;
        CollapseOpenPrintTicketsPanel();
    }

    /// <summary>
    /// Closes the ActivePrintTicket Menu and shows the Welcome Menu.
    /// Resets the ActiveTicket and SelectedTicket properties.
    /// Additionally, collapses the OpenPrintTicketsPanel.
    /// </summary>
    public void CloseActivePrintTicketMenu()
    {
        ActiveTicket = null;
        SelectedTicket = null;
        IsWelcomeMenuShown = true;
        IsActivePrintTicketMenuShown = false;
        CollapseOpenPrintTicketsPanel();
    }

    /// <summary>
    /// Attempts to add a blank PartialDataSet object to the Active Print Ticket.
    /// </summary>
    public void AddPartialDataSet()
    {
        if (ActiveTicket is not null && ActiveTicket.Tracked.HasSpace)
        {
            // create and add a blank PartialDataSet object to the ticket
            ActiveTicket.Tracked.AddPartialDataSet(new PartialDataSet());
        }
    }
}
# pragma warning restore CA1416 // Validate platform compatibility