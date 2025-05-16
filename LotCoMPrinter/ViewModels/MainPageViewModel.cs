using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Options;

namespace LotCoMPrinter.ViewModels;

# pragma warning disable CA1416 // Validate platform compatibility

/// <summary>
/// Constructs a ViewModel for the MainPage class.
/// </summary>
public partial class MainPageViewModel : ObservableObject 
{
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

    private MainPageOptions _options = new MainPageOptions();
    /// <summary>
    /// The MainPageOptions structure that controls the UI state options of the MainPage.
    /// </summary>
    public MainPageOptions Options
    {
        get {return _options;}
        set
        {
            _options = value;
            OnPropertyChanged(nameof(_options));
            OnPropertyChanged(nameof(Options));
        }
    }

    private bool _printing = false;
    /// <summary>
    /// Serves the current status of the application (true if a LabelPrintJob is running; false if not).
    /// </summary>
    public bool Printing 
    {
        get {return _printing;}
        set 
        {
            _printing = value;
            OnPropertyChanged(nameof(_printing));
            OnPropertyChanged(nameof(Printing));
        }
    }

    /// <summary>
    /// Checks if the Process requires Serialization (is an origination process).
    /// If so, elicits the Serialization Mode, checks for Cached Serial Numbers, and assigns a Serial Number to the Label.
    /// </summary>
    /// <param name="Capture"></param>
    /// <returns>An updated InterfaceCapture object.</returns>
    /// <exception cref="LabelBuildException"></exception>
    private async Task<InterfaceCapture> SerializeLabel(InterfaceCapture Capture) 
    {
        // retrieve values to save processing time (will not be null here; post-validation)
        Process SelectedProcess = Capture.Process;
        SerializationModes Serialization = SelectedProcess.Serialization;
        // check if the SelectedProcess is an Originator; if not, just return the passed Capture
        if (SelectedProcess.Type != OriginationTypes.Originator) 
        {
            return Capture;
        }
        // serialize the Label using the Process' Serialization Mode
        SerialNumber? SerialNumber = await Serializer.Serialize(Capture.Process, Capture.Part);
        // no serial number was assigned; this is fatal
        if (SerialNumber is null) 
        {
            throw new LabelBuildException("Failed to assign a Serial Number to the Label");
        }
        // update the Serialized Number in the Capture object
        if (Serialization == SerializationModes.JBK) 
        {
            Capture.VariableFields.JBKNumber = SerialNumber.Value;
        } 
        else 
        {
            Capture.VariableFields.LotNumber = SerialNumber.GetFormattedValue();
        }
        // return the updated Capture object
        return Capture;
    }

    /// <summary>
    /// Decides how to Head the Label, formats that field as a Header, and returns that string.
    /// </summary>
    /// <param name="Capture"></param>
    /// <returns>A string to use as the Label Header text.</returns>
    private static async Task<string> FormatLabelHeader(InterfaceCapture Capture) 
    {
        string LabelHeader = await Task.Run(() => 
        {
            // retrieve values to improve processing time
            Process SelectedProcess = Capture.Process;
            // decide to use the JBK or Date as the header
            string Header;
            if (SelectedProcess.PassThroughType == PassThroughTypes.JBK 
                || SelectedProcess.Serialization == SerializationModes.JBK) 
            {
                // header is the JBK #
                Header = Capture.VariableFields.JBKNumber.ToString()!;
            } 
            else if (SelectedProcess.PassThroughType == PassThroughTypes.Lot 
                || SelectedProcess.Serialization == SerializationModes.Lot) 
            {
                // header is the MM/DD of the Production Date; retrieve the Date from the UI Capture
                DateTime Date = Capture.ProductionDate;
                Header = $"{Date.Month}/{Date.Day}";
            } 
            else 
            {
                throw new LabelBuildException("There was no Header type assigned to this Process.");
            }
            return Header;
        });
        return LabelHeader;
    }

    /// <summary>
    /// Creates, validates, and formats an InterfaceCapture object from the current UI status.
    /// </summary>
    /// <param name="Process"></param>
    /// <param name="Part"></param>
    /// <param name="Quantity"></param>
    /// <param name="JBKNumber"></param>
    /// <param name="LotNumber"></param>
    /// <param name="DeburrJBKNumber"></param>
    /// <param name="DieNumber"></param>
    /// <param name="ModelNumber"></param>
    /// <param name="ProductionDate"></param>
    /// <param name="ProductionShift"></param>
    /// <param name="OperatorID"></param>
    /// <returns>An InterfaceCapture object.</returns>
    /// <exception cref="NullProcessException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    private static InterfaceCapture CreateCapture(Process Process, Part Part, int Quantity, int JBKNumber, string LotNumber, int DeburrJBKNumber, int DieNumber, string ModelNumber, string HeatNumber, DateTime ProductionDate, int ProductionShift, string OperatorID) 
    {
        // create an interface capture for this UI state
        InterfaceCapture Capture = new InterfaceCapture(Process, Part, Quantity, JBKNumber, LotNumber, DeburrJBKNumber, DieNumber, ModelNumber, HeatNumber, ProductionDate, ProductionShift, OperatorID);
        // validate the Capture
        try 
        {
            Capture = InterfaceCaptureValidator.Validate(Capture);
        // there was no process selected
		} 
        catch (NullProcessException) 
        {
            throw new NullProcessException();
        // there was a problem retrieving the process data
        } 
        catch (ArgumentException) 
        {
            throw new ArgumentException();
        // there was some invalid UI entry
        } 
        catch (FormatException _ex) 
        {
            throw new FormatException(_ex.Message);
        }
        // the Capture is valid and processed; return it
        return Capture;
    }
    
    /// <summary>
    /// Create a ViewModel to control the logic of a Main Page instance.
    /// </summary>
    public MainPageViewModel() 
    {
        Options.WelcomeMenuTitleLabelText = "Welcome";
        Options.WelcomeMenuSubTitleLabelText = "Click 'Start New Label' to create a new Label or open In-Progress Labels by clicking the arrow button below.";
        _openPrintTickets = new ObservableCollection<PrintTicket>(PrintTicketCache.GetAllPrintTickets());
    }

    /// <summary>
    /// Processes a Print Request from the user. 
    /// Captures the interface and validates it, then creates a new Label object from that captured data.
    /// </summary>
    /// <remarks>
    /// Throws NullProcessException if there was no selection in the ProcessPicker Control.
    /// Throws ArgumentException if the Process Data could not be retrieved.
    /// Throws FormatException if there was a failed validation.
    /// Throws LabelBuildException if there was an error creating, formatting, serializing, or printing the Label.
    /// Throws PrintRequestException if there was an error communicating with the Printer or the Printing System.
    /// </remarks>
    /// <param name="Process"></param>
    /// <param name="Part"></param>
    /// <param name="Quantity"></param>
    /// <param name="JBKNumber"></param>
    /// <param name="LotNumber"></param>
    /// <param name="DeburrJBKNumber"></param>
    /// <param name="DieNumber"></param>
    /// <param name="ModelNumber"></param>
    /// <param name="HeatNumber"></param>
    /// <param name="ProductionDate"></param>
    /// <param name="ProductionShift"></param>
    /// <param name="OperatorID"></param>
    /// <returns></returns>
    /// <exception cref="NullProcessException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="LabelBuildException"></exception>
    /// <exception cref="PrintRequestException"></exception>
    public async Task<bool> PrintRequest(Process Process, Part Part, int Quantity, int JBKNumber, string LotNumber, int DeburrJBKNumber, int DieNumber, string ModelNumber, string HeatNumber, DateTime ProductionDate, int ProductionShift, string OperatorID) 
    {
        // capture the interface
        InterfaceCapture Capture;
        try 
        {
            Capture = CreateCapture(Process, Part, Quantity, JBKNumber, LotNumber, DeburrJBKNumber, DieNumber, ModelNumber, HeatNumber, ProductionDate, ProductionShift, OperatorID);
        // there was no process selection made
        } 
        catch (NullProcessException) 
        {
            throw new NullProcessException();
        // there was a problem retrieving process data for the selected process
        } 
        catch (ArgumentException) 
        {
            throw new ArgumentException();
        // a validation failed
        } 
        catch (FormatException _ex) 
        {
            throw new FormatException(_ex.Message);
        }
        // serialize the label (if needed)
        try 
        {
            Capture = await SerializeLabel(Capture);
        // failed to cache a new serial number or assign a serial number at all
        } 
        catch (Exception _ex) 
        {
            throw new LabelBuildException($"Failed to Serialize the Label due to the following exception:\n {_ex}: {_ex.Message}.");
        }
        // UI state is valid; format the Label's header
        string Header; 
        try 
        {
            Header = await FormatLabelHeader(Capture);
        } 
        catch (LabelBuildException _ex) 
        {
            throw new LabelBuildException(_ex.Message);
        }
        // create and run a Label print job
        bool Printed = false;
        LabelPrintJob Job = new LabelPrintJob(Capture, Header);
        try 
        { 
            Printed = await Job.Run();
        // the print job failed
        } 
        catch (Exception _ex) 
        {
            if (_ex is LabelBuildException) 
            {
                // there was an error while constructing the Label to print
                throw new LabelBuildException($"There was an error creating this Label:\n {_ex}: {_ex.Message}.");
            } 
            else if (_ex is PrintRequestException) 
            {
                // there was an error while communicating with the Printer or Printing System
                throw new PrintRequestException($"There was an error communicating with the Printer:\n {_ex}: {_ex.Message}.");
            } 
            else if (_ex is PrintLogException) 
            {
                // the print logger failed to log to the specific process table and was forced to default
                throw new PrintLogException(_ex.Message);
            }
        }
        // return the print success state
        return Printed;
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