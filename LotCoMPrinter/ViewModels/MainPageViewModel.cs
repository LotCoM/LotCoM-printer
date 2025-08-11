using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LotComPrinter.Models.Datasources;
using LotComPrinter.Models.Datatypes;
using LotComPrinter.Models.Exceptions;
using LotComPrinter.Models.Enums;
using LotCom.UI;

namespace LotComPrinter.ViewModels;

# pragma warning disable CA1416 // Validate platform compatibility

/// <summary>
/// Binding context (ViewModel) for the MainPage UI component.
/// </summary>
public partial class MainPageViewModel : ObservableObject
{
    private const int OpenPrintTicketsPanelWidthOpen = 350;
    private const int OpenPrintTicketsPanelWidthClosed = 90;

    private PageLoadingFlags _flags = new PageLoadingFlags();
    /// <summary>
    /// Indicates different data loading related properties.
    /// </summary>
    public PageLoadingFlags Flags
    {
        get { return _flags; }
        set
        {
            _flags = value;
            OnPropertyChanged();
        }
    }

    private int _openPrintTicketsPanelWidth = OpenPrintTicketsPanelWidthClosed;
    /// <summary>
    /// Provides the width Option of the Open Print Tickets Panel.
    /// </summary>
    public int OpenPrintTicketsPanelWidth
    {
        get { return _openPrintTicketsPanelWidth; }
        set
        {
            _openPrintTicketsPanelWidth = value;
            OnPropertyChanged();
        }
    }

    private bool _isOpenPrintTicketsPanelShown = false;
    /// <summary>
    /// Provides the Visibility state of the Open Print Tickets Panel.
    /// </summary>
    public bool IsOpenPrintTicketsPanelShown
    {
        get { return _isOpenPrintTicketsPanelShown; }
        set
        {
            _isOpenPrintTicketsPanelShown = value;
            OnPropertyChanged();
        }
    }

    private string _openPrintTicketsPanelLoadMessage = "Failed to load Open Tickets";
    /// <summary>
    /// Provides the message to show as the Load Message of the Open Print Tickets Panel.
    /// </summary>
    public string OpenPrintTicketsPanelLoadMessage
    {
        get { return _openPrintTicketsPanelLoadMessage; }
        set
        {
            _openPrintTicketsPanelLoadMessage = value;
            OnPropertyChanged();
        }
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
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Create a ViewModel for the MainPage to bind to.
    /// </summary>
    public MainPageViewModel()
    {
        _openPrintTickets = [];
        SelectedTicket = null;
        OpenPrintTicketsPanelLoadMessage = "";
        Flags.Reset();
    }

    /// <summary>
    /// Attempts to Load the PrintTicketCache and update OpenPrintTickets.
    /// </summary>
    /// <returns></returns>
    public async Task LoadOpenPrintTickets()
    {
        // don't load if tickets are already loaded
        if (Flags.IsComplete)
        {
            return;
        }
        // start the loading flags and attempt to load the PrintTicketCache
        Flags.Start();
        try
        {
            OpenPrintTickets = new ObservableCollection<PrintTicket>
            (
                await PrintTicketCache.GetAllPrintTickets()
            );
            OpenPrintTicketsPanelLoadMessage = "";
            Flags.Success();
            return;
        }
        catch
        {
            OpenPrintTicketsPanelLoadMessage = $"Sorry, we couldn't load Open Labels.\n\nPlease see management to resolve the issue.";
            OpenPrintTickets = [];
            Flags.Failure();
        }
    }

    /// <summary>
    /// Opens the Open Print Tickets Panel.
    /// </summary>
    public async Task RaiseOpenPrintTicketsPanel()
    {
        OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidthOpen;
        IsOpenPrintTicketsPanelShown = true;
        await LoadOpenPrintTickets();
    }

    /// <summary>
    /// Closes the Open Print Tickets Panel.
    /// </summary>
    public void CollapseOpenPrintTicketsPanel()
    {
        OpenPrintTicketsPanelWidth = OpenPrintTicketsPanelWidthClosed;
        IsOpenPrintTicketsPanelShown = false;
    }

    /// <summary>
    /// Saves the current OpenPrintTickets List to the PrintTicketCache system.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SystemException"></exception>
    public async Task SaveOpenPrintTickets()
    {
        try
        {
            await PrintTicketCache.Save(OpenPrintTickets.ToList());
        }
        catch (ArgumentNullException _ex)
        {
            throw new SystemException
            (
                "Could not save the Print Ticket Cache due to the following exception:"
                + $"\n\t{_ex.GetType()}"
                + $"\n\t{_ex.Message}"
            );
        }
        catch (OperationCanceledException _ex)
        {
            throw new SystemException
            (
                "Could not save the Print Ticket Cache due to the following operation cancellation:"
                + $"\n\t{_ex.GetType()}"
                + $"\n\t{_ex.Message}"
                + $"\n\t{_ex.InnerException}"
            );
        }
    }

    /// <summary>
    /// Adds a PrintTicket to OpenPrintTickets.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <exception cref="SystemException"></exception>
    public async Task AddNewOpenPrintTicket(PrintTicket Ticket)
    {
        OpenPrintTickets = new ObservableCollection<PrintTicket>(OpenPrintTickets.Append(Ticket));
        try
        {
            await SaveOpenPrintTickets();
        }
        catch (SystemException)
        {
            throw;
        }
    }

    /// <summary>
    /// Removes a PrintTicket from OpenPrintTickets.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    /// <exception cref="SystemException"></exception>
    public async Task<bool> RemoveOpenPrintTicket(PrintTicket Ticket)
    {
        bool Result = OpenPrintTickets.Remove(Ticket);
        if (!Result)
        {
            return false;
        }
        try
        {
            await SaveOpenPrintTickets();
        }
        catch (SystemException)
        {
            throw;
        }
        return Result;
    }

    /// <summary>
    /// Reprints a Label using the information from the selected PrintTicket.
    /// </summary>
    /// <param name="ReprintTicket"></param>
    /// <returns></returns>
    public async Task<bool> ReprintLabel(PrintTicket ReprintTicket)
    {
        // setup a reprint Job from the selected PrintTicket
        PrintJob Job = new PrintJob(ReprintTicket, PrintJobType.Reprint);
        bool Printed;
        // attempt to run the Print Job, handle exceptions, and return the result
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