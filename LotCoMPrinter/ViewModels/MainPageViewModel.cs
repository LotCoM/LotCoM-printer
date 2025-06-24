using System.Collections.ObjectModel;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using LotCom.Types;
using LotCom.Database;
using LotCom.Enums;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Datatypes;
using LotCoMPrinter.Models.Exceptions;

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
        public const bool IsTicketEditorMenuShown = false;
        public const bool IsTicketActionsPanelShown = false;
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

    private bool _isTicketEditorMenuShown = Defaults.IsTicketEditorMenuShown;
    /// <summary>
    /// Provides the Visibility state of the Ticket Editor Menu.
    /// </summary>
    public bool IsTicketEditorMenuShown
    {
        get { return _isTicketEditorMenuShown; }
        set
        {
            _isTicketEditorMenuShown = value;
            OnPropertyChanged(nameof(_isTicketEditorMenuShown));
            OnPropertyChanged(nameof(IsTicketEditorMenuShown));
        }
    }

    private bool _isTicketActionsPanelShown = Defaults.IsTicketActionsPanelShown;
    /// <summary>
    /// Provides the Visibility state of the Ticket Editor Menu.
    /// </summary>
    public bool IsTicketActionsPanelShown
    {
        get { return _isTicketActionsPanelShown; }
        set
        {
            _isTicketActionsPanelShown = value;
            OnPropertyChanged(nameof(_isTicketActionsPanelShown));
            OnPropertyChanged(nameof(IsTicketActionsPanelShown));
        }
    }

    private readonly List<Process> _allProcesses;
    /// <summary>
    /// The List of Processes in the Database, captured at the time of instantiation.
    /// </summary>
    public List<Process> AllProcesses
    {
        get { return _allProcesses; }
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
        set
        {
            _openPrintTickets = value;
            OnPropertyChanged(nameof(_openPrintTickets));
            OnPropertyChanged(nameof(OpenPrintTickets));
        }
    }

    private TrackedPrintTicket? _editorTicket = null;
    /// <summary>
    /// The PrintTicket currently being edited, with Tracked changes.
    /// </summary>
    public TrackedPrintTicket? EditorTicket
    {
        get { return _editorTicket; }
        set
        {
            _editorTicket = value;
            OnPropertyChanged(nameof(_editorTicket));
            OnPropertyChanged(nameof(EditorTicket));
        }
    }

    private PrintTicket? _selectedTicket = null;
    /// <summary>
    /// The currently selected PrintTicket object in the OpenPrintTickets CollectionView.
    /// </summary>
    public PrintTicket? SelectedTicket
    {
        get { return _selectedTicket; }
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
    /// <exception cref="SystemException"></exception>
    /// <exception cref="JsonException"></exception>
    public MainPageViewModel()
    {
        // load Process data
        try
        {
            _allProcesses = new ProcessData().GetAllProcesses();
        }
        catch (SystemException)
        {
            throw;
        }
        catch (JsonException)
        {
            throw;
        }
        // configure Welcome menu
        WelcomeMenuTitleLabelText = "Welcome, Operator!";
        WelcomeMenuSubTitleLabelText = "Click 'Start New Label' to create a new Label or open In-Progress Labels by clicking the arrow button below.";
        // read and set PrintTicket properties
        _openPrintTickets = new ObservableCollection<PrintTicket>(PrintTicketCache.GetAllPrintTickets());
        try
        {
            SelectedTicket = OpenPrintTickets[0];
        }
        catch
        {
            SelectedTicket = null;
        }
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
    /// Opens the Ticket Editor Menu to display the currently editing PrintTicket.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    public void OpenPrintTicketEditorMenu()
    {
        if (EditorTicket is null)
        {
            throw new NullReferenceException("Cannot display a 'null' EditorTicket.");
        }
        IsWelcomeMenuShown = false;
        IsTicketEditorMenuShown = true;
    }

    /// <summary>
    /// Closes the Ticket Editor Menu and shows the Welcome Menu.
    /// Resets the EditorTicket and SelectedTicket properties.
    /// </summary>
    public void CloseTicketEditorMenu()
    {
        EditorTicket = null;
        SelectedTicket = null;
        IsWelcomeMenuShown = true;
        IsTicketEditorMenuShown = false;
    }

    /// <summary>
    /// Attempts to create and run a Label Print Job from EditorTicket.Tracked.
    /// Removes EditorTicket from OpenPrintTickets.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="PrintRequestException"></exception>
    public async Task<bool> PrintEditorTicket()
    {
        // create a LabelPrintJob from the editing Print Ticket
        if (EditorTicket is null)
        {
            throw new NullReferenceException("Cannot print 'null' PrintTicket.");
        }
        // validate the PrintTicket
        try
        {
            EditorTicket.Tracked = EditorTicket.Tracked.SelfValidate();
        }
        catch (Exception _ex)
        {
            throw new ArgumentException(_ex.Message);
        }
        PrintJob Job = new PrintJob(EditorTicket.Tracked);
        bool Printed;
        // attempt to run the Print Job
        try
        {
            Printed = await Job.Run();
        }
        catch (LabelBuildException)
        {
            throw new PrintRequestException("Could not create a Label from the entered information.");
        }
        catch (PrintRequestException)
        {
            throw new PrintRequestException("Failed to execute the print job for the generated Label.");
        }
        // remove the Ticket if the print was successful
        if (Printed)
        {
            bool Removed = OpenPrintTickets.Remove(EditorTicket.Untracked);
            // confirm that the EditorTicket exists in OpenPrintTickets and remove it
            if (!Removed)
            {
                throw new NullReferenceException("The Untracked EditorTicket was not found in OpenPrintTickets.");
            }
            await SaveOpenPrintTickets();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Saves the current OpenPrintTickets List to the PrintTicketCache system.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task SaveOpenPrintTickets()
    {
        await PrintTicketCache.SaveAsync(OpenPrintTickets.ToList());
    }

    /// <summary>
    /// Adds a PrintTicket to OpenPrintTickets.
    /// </summary>
    /// <param name="Ticket"></param>
    public async Task AddNewOpenPrintTicket(PrintTicket Ticket)
    {
        OpenPrintTickets = new ObservableCollection<PrintTicket>(OpenPrintTickets.Append(Ticket));
        await SaveOpenPrintTickets();
    }

    /// <summary>
    /// Attempts to add a blank PartialDataSet object to the editing Print Ticket.
    /// </summary>
    public void AddPartialDataSet()
    {
        if (EditorTicket is not null && EditorTicket.Tracked.HasSpace)
        {
            // create and add a blank PartialDataSet object to the ticket
            EditorTicket.Tracked.AddPartialDataSet
            (
                new PartialDataSet
                (
                    new Quantity(0),
                    Shift.None,
                    new Operator("ABC")
                )
            );
        }
    }

    /// <summary>
    /// Attempts to create and run a Partial Tag Print Job from EditorTicket.Tracked.
    /// </summary>
    /// <param name="PartialSetNumber">The PartialDataSet to use as the source of partial Production Data, either 1 or 2.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="PrintRequestException"></exception>
    public async Task<bool> PrintPartialTag(int PartialSetNumber)
    {
        // ensure the targeted PartialDataSet exists on the editing Print Ticket
        if (PartialSetNumber < 1 || PartialSetNumber > 2)
        {
            throw new ArgumentException
            (
                $"Cannot print PartialDataSet '{PartialSetNumber}' as it is outside the allowed set (1, 2).",
                nameof(PartialSetNumber)
            );
        }
        if (EditorTicket is null)
        {
            throw new NullReferenceException("Cannot print PartialDataSets from 'null' PrintTicket.");
        }
        // validate the target PartialDataSet
        try
        {
            if (PartialSetNumber == 1 && EditorTicket.Tracked.HasFirstPartialDataSet)
            {
                EditorTicket.Tracked.FirstPartialDataSet!.SelfValidate();
            }
            else if (PartialSetNumber == 2 && EditorTicket.Tracked.HasSecondPartialDataSet)
            {
                EditorTicket.Tracked.SecondPartialDataSet!.SelfValidate();
            }
            else
            {
                throw new NullReferenceException($"Cannot print 'null' PartialDataSet Number '{PartialSetNumber}'.");
            }
        }
        catch (NullReferenceException _nullEx)
        {
            throw new NullReferenceException(_nullEx.Message);
        }
        catch (ArgumentException _argEx)
        {
            throw new ArgumentException(_argEx.Message);
        }
        // create a new PrintJob from the PartialDataSet and attempt to run it
        PrintJob Job = new PrintJob(EditorTicket.Tracked, PartialSetNumber);
        bool Printed;
        try
        {
            Printed = await Job.Run();
        }
        catch (LabelBuildException)
        {
            throw new PrintRequestException("Could not create a Label from the entered information.");
        }
        catch (PrintRequestException)
        {
            throw new PrintRequestException("Failed to execute the print job for the generated Label.");
        }
        return Printed;
    }
}
# pragma warning restore CA1416 // Validate platform compatibility