using CommunityToolkit.Mvvm.ComponentModel;
using LotCom.Types;
using LotCom.Types.Enums;
using LotCom.Exceptions;
using LotComPrinter.Models.Datatypes;
using LotComPrinter.Models.Services;
using Newtonsoft.Json;
using LotCom.DataAccess.Services;
using System.Collections.ObjectModel;
using LotCom.UI;

namespace LotComPrinter.ViewModels;

public partial class NewPrintTicketFormViewModel : ObservableObject
{
    private CompoundPageLoadingFlags _flags = new CompoundPageLoadingFlags();
    /// <summary>
    /// Indicates different loading related properties for the Page's Process and Part data.
    /// </summary>
    public CompoundPageLoadingFlags Flags
    {
        get { return _flags; }
        set
        {
            _flags = value;
            OnPropertyChanged();
        }
    }

    private Process? _process = null;
    /// <summary>
    /// The Process to assign to a PrintTicket object instantiated by this Form.
    /// </summary>
    public Process? Process
    {
        get {return _process;}
        set 
        {
            _process = value;
            OnPropertyChanged();
        }
    }

    private Part? _part = null;
    /// <summary>
    /// The Part to assign to a PrintTicket object instantiated by this Form.
    /// </summary>
    public Part? Part
    {
        get {return _part;}
        set 
        {
            _part = value;
            OnPropertyChanged();
        }
    }

    private SerialNumber? _serialNumber = null;
    /// <summary>
    /// The Serial Number to assign to a PrintTicket object instantiated by this Form.
    /// </summary>
    public SerialNumber? SerialNumber
    {
        get {return _serialNumber;}
        set 
        {
            _serialNumber = value;
            OnPropertyChanged();
        }
    }

    private DateTime _productionDate = DateTime.Now;
    /// <summary>
    /// The Date and Time at which this Form was initiated.
    /// </summary>    
    public DateTime ProductionDate
    {
        get {return _productionDate;}
        set 
        {
            _productionDate = value;
            OnPropertyChanged();
        }
    }

    private Shift _productionShift = Shift.None;
    /// <summary>
    /// The Shift on which this Form was initiated.
    /// </summary>
    public Shift ProductionShift
    {
        get {return _productionShift;}
        set 
        {
            _productionShift = value;
            OnPropertyChanged();
        }
    }

    private Quantity _productionQuantity = new Quantity(0);
    /// <summary>
    /// The Quantity produced by the Shift that this Form was initiated on.
    /// </summary>
    public Quantity ProductionQuantity
    {
        get {return _productionQuantity;}
        set 
        {
            _productionQuantity = value;
            OnPropertyChanged();
        }
    }

    private Operator _productionOperator = new Operator("");
    /// <summary>
    /// The Operator by which this Form was initiated.
    /// </summary>
    public Operator ProductionOperator
    {
        get {return _productionOperator;}
        set 
        {
            _productionOperator = value;
            OnPropertyChanged();
        }
    }
    
    private int _selectedProcessIndex = -1;
    /// <summary>
    /// Provides the index of the currently selected Process for the Form.
    /// </summary>
    public int SelectedProcessIndex
    {
        get {return _selectedProcessIndex;}
        set 
        {
            _selectedProcessIndex = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Part>? _selectedProcessParts = [];
    /// <summary>
    /// Provides the list of Printable Parts assigned to the currently selected Process for the Form.
    /// </summary>
    public ObservableCollection<Part>? SelectedProcessParts
    {
        get {return _selectedProcessParts;}
        set 
        {
            _selectedProcessParts = value;
            OnPropertyChanged();
        }
    }

    public int _selectedPartIndex = -1;
    /// <summary>
    /// Provides the index of the currently selected Part for the Form.
    /// </summary>
    public int SelectedPartIndex
    {
        get {return _selectedPartIndex;}
        set 
        {
            _selectedPartIndex = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Process> _allProcesses = [];
    /// <summary>
    /// The List of Processes in the Database, captured at the time of instantiation.
    /// </summary>
    public ObservableCollection<Process> AllProcesses
    {
        get {return _allProcesses;}
        set 
        {
            _allProcesses = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Create a ViewModel to control the logic of a NewPrintTicketForm.
    /// </summary>
    public NewPrintTicketFormViewModel()
    {

    }

    /// <summary>
    /// Loads the Processes used to populate the Process Selection Dropdown.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DatabaseException"></exception>
    public async Task LoadProcesses()
    {
        if (Flags.IsProcessComplete)
        {
            return;
        }
        // load Process data
        Flags.StartProcesses();
        IEnumerable<Process>? ProcessesFromDatabase;
        try
        {
            ProcessesFromDatabase = await ProcessService.GetAll(App.UserAgent);
        }
        // some database-generated issue
        catch (HttpRequestException _ex)
        {
            Flags.FailureProcesses();
            throw new DatabaseException("Could not retreive Processes from the Database.", _ex);
        }
        // some formatting issue
        catch (JsonException _ex)
        {
            Flags.FailureProcesses();
            throw new DatabaseException("Could not process JSON response.", _ex);
        }
        // the response was nothing but there was no error (no contents returned)
        if (ProcessesFromDatabase is null)
        {
            AllProcesses = [];
        }
        else
        {
            AllProcesses = new ObservableCollection<Process>(ProcessesFromDatabase);
        }
        Flags.SuccessProcesses();
        return;
    }

    /// <summary>
    /// Loads the Parts used to populate the Part Selection Dropdown.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DatabaseException"></exception>
    public async Task LoadParts()
    {
        if (Flags.IsPartComplete || Process is null)
        {
            return;
        }
        // load Part data
        Flags.StartParts();
        IEnumerable<Part>? PartsFromDatabase;
        try
            {
                PartsFromDatabase = await PartService.GetPrintedByProcess(Process.Id, App.UserAgent);
            }
            // some database-generated issue
            catch (HttpRequestException _ex)
            {
                Flags.FailureParts();
                throw new DatabaseException("Could not retreive Parts from the Database.", _ex);
            }
            // some formatting issue
            catch (JsonException _ex)
            {
                Flags.FailureParts();
                throw new DatabaseException("Could not process JSON response.", _ex);
            }
        // the response was nothing but there was no error (no contents returned)
        if (PartsFromDatabase is null)
        {
            SelectedProcessParts = [];
        }
        else
        {
            SelectedProcessParts = new ObservableCollection<Part>(PartsFromDatabase);
        }
        Flags.SuccessParts();
        return;
    }

    /// <summary>
    /// Retrieves a Serial Number for a Process/Part pair and returns a new Print Ticket with the form's inputs.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SerializationException"></exception>
    public async Task<PrintTicket> OpenNewPrintTicket()
    {
        // confirm there are selections for all fields
        if
        (
            Process is null
            || Part is null
            || ProductionShift == Shift.None
            || !ProductionOperator.ConfirmProperInitials()
        )
        {
            throw new ArgumentException("Cannot create a PrintTicket without a full Form.");
        }
        // retrieve a Serial Number for this new Print Ticket
        SerialNumber? TicketNumber = await SerializationService.Serialize(Process, Part);
        if (TicketNumber is null)
        {
            // a serial number is needed
            if (Process.Serialization != SerializationMode.None)
            {
                throw new SerializationException("Failed to retrieve a Serial Number for the new Print Ticket.");
            }
            // pass-through process
            else
            {
                TicketNumber = new SerialNumber(SerializationMode.None, Part.Id, 0);
            }
        }
        PrintTicket NewTicket = new PrintTicket(Process, Part, Process.Serialization, TicketNumber, DateTime.Now, new VariableFieldSet(), new PartialDataSet(ProductionQuantity, ProductionShift, ProductionOperator));
        // apply the SerialNumber to the appropriate field and return the new Ticket
        if (NewTicket.SerializationMode == SerializationMode.JBK)
        {
            NewTicket.VariableFields.JBKNumber = new JBKNumber(NewTicket.SerialNumber.Value);
        }
        else if (NewTicket.SerializationMode == SerializationMode.Lot)
        {
            NewTicket.VariableFields.LotNumber = new LotNumber(NewTicket.SerialNumber.Value);
        }
        return NewTicket;
    }
}