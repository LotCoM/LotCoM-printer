using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Options;

namespace LotCoMPrinter.ViewModels;

public partial class NewPrintTicketFormViewModel : ObservableObject
{
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
            OnPropertyChanged(nameof(_process));
            OnPropertyChanged(nameof(Process));
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
            OnPropertyChanged(nameof(_part));
            OnPropertyChanged(nameof(Part));
        }
    }

    private SerialNumber? _serialNumber = null;
    /// <summary>
    /// The Serial Number (JBK or Lot Number) to assign to a PrintTicket object instantiated by this Form.
    /// </summary>
    public SerialNumber? SerialNumber
    {
        get {return _serialNumber;}
        set 
        {
            _serialNumber = value;
            OnPropertyChanged(nameof(_serialNumber));
            OnPropertyChanged(nameof(SerialNumber));
        }
    }

    private DateTime _productionDate = DateTime.Today;
    /// <summary>
    /// The Date and Time at which this Form was initiated.
    /// </summary>    
    public DateTime ProductionDate
    {
        get {return _productionDate;}
        set 
        {
            _productionDate = value;
            OnPropertyChanged(nameof(_productionDate));
            OnPropertyChanged(nameof(ProductionDate));
        }
    }

    private Shift? _productionShift = null;
    /// <summary>
    /// The Shift on which this Form was initiated.
    /// </summary>
    public Shift? ProductionShift
    {
        get {return _productionShift;}
        set 
        {
            _productionShift = value;
            OnPropertyChanged(nameof(_productionShift));
            OnPropertyChanged(nameof(ProductionShift));
        }
    }

    private int? _productionQuantity = null;
    /// <summary>
    /// The Quantity produced by the Shift that this Form was initiated on.
    /// </summary>
    public int? ProductionQuantity
    {
        get {return _productionQuantity;}
        set 
        {
            _productionQuantity = value;
            OnPropertyChanged(nameof(_productionQuantity));
            OnPropertyChanged(nameof(ProductionQuantity));
        }
    }

    private string? _productionOperator = null;
    /// <summary>
    /// The Operator by which this Form was initiated.
    /// </summary>
    public string? ProductionOperator
    {
        get {return _productionOperator;}
        set 
        {
            _productionOperator = value;
            OnPropertyChanged(nameof(_productionOperator));
            OnPropertyChanged(nameof(ProductionOperator));
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
            OnPropertyChanged(nameof(_selectedProcessIndex));
            OnPropertyChanged(nameof(SelectedProcessIndex));
        }
    }

    public List<Part>? _selectedProcessParts = [];
    /// <summary>
    /// Provides the list of Parts assigned to the currently selected Process for the Form.
    /// </summary>
    public List<Part>? SelectedProcessParts
    {
        get {return _selectedProcessParts;}
        set 
        {
            _selectedProcessParts = value;
            OnPropertyChanged(nameof(_selectedProcessParts));
            OnPropertyChanged(nameof(SelectedProcessParts));
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
            OnPropertyChanged(nameof(_selectedPartIndex));
            OnPropertyChanged(nameof(SelectedPartIndex));
        }
    }
    private List<Process> _allProcesses = new ProcessData().GetAllProcesses();
    /// <summary>
    /// The List of Processes in the Database, captured at the time of instantiation.
    /// </summary>
    public List<Process> AllProcesses
    {
        get {return _allProcesses;}
        set 
        {
            _allProcesses = value;
            OnPropertyChanged(nameof(_allProcesses));
            OnPropertyChanged(nameof(_allProcesses));
        }
    }

    /// <summary>
    /// Create a ViewModel to control the logic of a NewPrintTicketForm.
    /// </summary>
    public NewPrintTicketFormViewModel()
    {

    }

    /// <summary>
    /// Retrieves a Serial Number for a Process/Part pair and returns a new Print Ticket with the form's inputs.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SerializationException"></exception>
    public async Task<PrintTicket> OpenNewPrintTicket()
    {
        // confirm there are selections for all fields
        if (Process is null 
            || Part is null 
            || ProductionShift is null 
            || ProductionOperator is null)
        {
            throw new ArgumentException("Cannot create a PrintTicket without a full Form.");
        }
        // retrieve a Serial Number for this new Print Ticket
        SerialNumber? TicketNumber = await Serializer.Serialize(Process, Part);
        if (TicketNumber is null)
        {
            throw new SerializationException("Failed to retrieve a Serial Number for the new Print Ticket.");
        }
        PrintTicket NewTicket = new PrintTicket(Process, Part, Process.Serialization, TicketNumber, ProductionDate, (Shift)ProductionShift, 0, ProductionOperator, new VariableFieldSet());
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