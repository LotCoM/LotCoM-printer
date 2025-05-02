using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Options;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a structure for the creation and maintenance of a Printing Ticket.
/// </summary>
/// <param name="Department">The Department that initiated this Print Ticket.</param>
/// <param name="Process">The Process that initiated this Print Ticket.</param>
/// <param name="Part">The Part that this Print Ticket is applied to.</param>
/// <param name="SerializationMode">The type of Serial Number used to Serialize this Print Ticket.</param>
/// <param name="SerialNumber"></param>
/// <param name="ProductionDate"></param>
public partial class PrintTicket(Department Department, Process Process, Part Part, SerializationModes SerializationMode, string SerialNumber, Timestamp ProductionDate) : ObservableObject()
{
    /// <summary>
    /// The Department that initiated this Print Ticket.
    /// </summary>
    private Department Department = Department;

    /// <summary>
    /// The Process that initiated this Print Ticket.
    /// </summary>
    private Process Process = Process;

    /// <summary>
    /// The Part that this Print Ticket is applied to.
    /// </summary>
    private Part Part = Part;

    /// <summary>
    /// The type of Serial Number used to Serialize this Print Ticket.
    /// </summary>
    private SerializationModes SerializationMode = SerializationMode;

    /// <summary>
    /// The Serial Number (JBK or Lot Number) applied to this Print Ticket.
    /// </summary>
    private string SerialNumber = SerialNumber;

    /// <summary>
    /// The Date and Time at which this Print Ticket was initiated.
    /// </summary>
    private Timestamp ProductionDate = ProductionDate;

    private PartialDataSet? _firstPartialDataSet = null;
    /// <summary>
    /// The first of the Partial Production Data sets associated with this Print Ticket.
    /// </summary>
    public PartialDataSet? FirstPartialDataSet
    {
        get {return _firstPartialDataSet;}
        set
        {
            _firstPartialDataSet = value;
            OnPropertyChanged(nameof(_firstPartialDataSet));
            OnPropertyChanged(nameof(FirstPartialDataSet));
        }
    }

    private PartialDataSet? _secondPartialDataSet = null;
    /// <summary>
    /// The second of the Partial Production Data sets associated with this Print Ticket.
    /// </summary>
    public PartialDataSet? SecondPartialDataSet
    {
        get {return _secondPartialDataSet;}
        set
        {
            _secondPartialDataSet = value;
            OnPropertyChanged(nameof(_secondPartialDataSet));
            OnPropertyChanged(nameof(SecondPartialDataSet));
        }
    }

    /// <summary>
    /// Returns whether the Print Ticket has one Partial Data Set associated with it.
    /// </summary>
    public bool HasFirstPartialDataSet 
    {
        get
        {
            return FirstPartialDataSet is not null;
        }
    }

    /// <summary>
    /// Returns whether the Print Ticket has two Partial Data Sets associated with it.
    /// </summary>
    public bool HasSecondPartialDataSet 
    {
        get
        {
            return SecondPartialDataSet is not null;
        }
    }

    /// <summary>
    /// Returns whether the Print Ticket has space for another Partial Data Set.
    /// </summary>
    public bool HasSpace 
    {
        get
        {
            return !HasSecondPartialDataSet;
        }
    }

    /// <summary>
    /// Provides a Title for the Print Ticket that gives the crucial information of the Ticket.
    /// </summary>
    public string Title
    {
        get
        {
            return $"{Part.ModelNumber} {Part.PartName} - {SerialNumber}";
        }
    }

    /// <summary>
    /// Returns whether the Print Ticket is serialized using a JBK Number or not.
    /// </summary>
    public bool IsJBKSerialized
    {
        get
        {
            return SerializationMode == SerializationModes.JBK;
        }
        set
        {
            _ = value;
            OnPropertyChanged(nameof(IsJBKSerialized));
        }
    }

    /// <summary>
    /// Returns whether the Print Ticket is serialized using a Lot Number or not.
    /// </summary>
    public bool IsLotSerialized
    {
        get
        {
            return SerializationMode == SerializationModes.Lot;
        }
        set
        {
            _ = value;
            OnPropertyChanged(nameof(IsLotSerialized));
        }
    }
}