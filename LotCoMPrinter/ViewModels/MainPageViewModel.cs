using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LotComPrinter.Models.Datasources;
using LotComPrinter.Models.Datatypes;
using LotComPrinter.Models.Exceptions;
using LotComPrinter.Models.Enums;

namespace LotComPrinter.ViewModels;

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
            OnPropertyChanged(nameof(_isOpenPrintTicketsPanelShown));
            OnPropertyChanged(nameof(IsOpenPrintTicketsPanelShown));
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
            OnPropertyChanged(nameof(_openPrintTickets));
            OnPropertyChanged(nameof(OpenPrintTickets));
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
    public MainPageViewModel()
    {
        try
        {
            // read and set PrintTicket properties
            _openPrintTickets = new ObservableCollection<PrintTicket>
            (
                PrintTicketCache.GetAllPrintTickets()
            );
            SelectedTicket = OpenPrintTickets[0];
        }
        catch
        {
            _openPrintTickets = [];
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
    /// Saves the current OpenPrintTickets List to the PrintTicketCache system.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SystemException"></exception>
    public async Task SaveOpenPrintTickets()
    {
        try
        {
            await PrintTicketCache.SaveAsync(OpenPrintTickets.ToList());
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