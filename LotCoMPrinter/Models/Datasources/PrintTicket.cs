using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
public partial class PrintTicket(Department Department, Process Process, Part Part, SerializationModes SerializationMode, SerialNumber SerialNumber, Timestamp ProductionDate, int ProductionShift, string Operator, PartialDataSet? FirstPartialDataSet = null, PartialDataSet? SecondPartialDataSet = null) : ObservableObject()
{
    /// <summary>
    /// The Department that initiated this Print Ticket.
    /// </summary>
    private readonly Department Department = Department;

    /// <summary>
    /// The Process that initiated this Print Ticket.
    /// </summary>
    private readonly Process Process = Process;

    /// <summary>
    /// The Part that this Print Ticket is applied to.
    /// </summary>
    private readonly Part Part = Part;

    /// <summary>
    /// The type of Serial Number used to Serialize this Print Ticket.
    /// </summary>
    private readonly SerializationModes SerializationMode = SerializationMode;

    /// <summary>
    /// The Serial Number (JBK or Lot Number) applied to this Print Ticket.
    /// </summary>
    private readonly SerialNumber SerialNumber = SerialNumber;

    /// <summary>
    /// The Date and Time at which this Print Ticket was initiated.
    /// </summary>
    private readonly Timestamp ProductionDate = ProductionDate;

    /// <summary>
    /// The Shift that this Print Ticket was initiated on.
    /// </summary>
    private readonly int ProductionShift = ProductionShift;

    /// <summary>
    /// The Operator that this Print Ticket was initiated by.
    /// </summary>
    private readonly string ProductionOperator = Operator;

    private PartialDataSet? _firstPartialDataSet = FirstPartialDataSet;
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

    private PartialDataSet? _secondPartialDataSet = SecondPartialDataSet;
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

    [ObservableProperty]
    /// <summary>
    /// Returns whether the Print Ticket has one Partial Data Set associated with it.
    /// </summary>
    public partial bool HasFirstPartialDataSet {get; set;} = false;

    [ObservableProperty]
    /// <summary>
    /// Returns whether the Print Ticket has two Partial Data Sets associated with it.
    /// </summary>
    public partial bool HasSecondPartialDataSet {get; set;} = false;

    [ObservableProperty]
    /// <summary>
    /// Returns whether the Print Ticket has space for another Partial Data Set.
    /// </summary>
    public partial bool HasSpace {get; set;} = true;

    /// <summary>
    /// Shifts the PartialDataSet from the Second position into the First.
    /// </summary>
    private void ShiftPartialDataSet()
    {
        FirstPartialDataSet = SecondPartialDataSet;
        SecondPartialDataSet = null;
        HasFirstPartialDataSet = true;
        HasSecondPartialDataSet = false;
        HasSpace = true;
    }

    /// <summary>
    /// Converts the PrintTicket object to a JSON stream.
    /// </summary>
    /// <returns></returns>
    public string ToJSON()
    {
        // convert non-string SerializationModes type to a string
        string ModeString;
        if (SerializationMode == SerializationModes.JBK)
        {
            ModeString = "JBK";
        } 
        else
        {
            ModeString = "Lot";
        }
        // build the JSON stream piece-by-piece
        // Department, Process, Part, SerializationMode, SerialNumber, and Production Date and Shift are all universal
        string JSON = 
            "{" +
                "\"Department\":{" +
                    $"\"Title\":\"{Department.Title}\"" +
                "}," + 
                "\"Process\":{" +
                    $"\"FullName\":\"{Process.FullName}\"" +
                "}," +
                "\"Part\":{" +
                    $"\"PartNumber\":\"{Part.PartNumber}\"" + 
                "}," +
                $"\"SerializationMode\":\"{ModeString}\"," +
                $"\"SerialNumber\":\"{SerialNumber.ToJSON()}\"," +
                $"\"ProductionDate\":\"{ProductionDate.Stamp}\"," +
                $"\"ProductionShift\":\"{ProductionShift}\"," +
                $"\"ProductionOperator\":\"{ProductionOperator}\"";
        // add partial data sets only if assigned
        if (HasFirstPartialDataSet)
        {
            JSON += 
                ",\"FirstPartialDataSet\":{" +
                    $"\"Quantity\":\"{FirstPartialDataSet!.Quantity}\"," +
                    $"\"Shift\":\"{FirstPartialDataSet!.Shift}\"," +
                    $"\"Operator\":\"{FirstPartialDataSet!.Operator}\"" +
                "}";
        }
        if (HasSecondPartialDataSet)
        {
            JSON += 
                ",\"SecondPartialDataSet\":{" +
                    $"\"Quantity\":\"{SecondPartialDataSet!.Quantity}\"," +
                    $"\"Shift\":\"{SecondPartialDataSet!.Shift}\"," +
                    $"\"Operator\":\"{SecondPartialDataSet!.Operator}\"" +
                "}";
        }
        // close the JSON stream
        JSON += 
            "}";
        return JSON;
    }

    /// <summary>
    /// Attempts to parse a full PrintTicket object from a JSON formatted Line.
    /// </summary>
    /// <param name="Line"></param>
    /// <returns>A PrintTicket object.</returns>
    /// <exception cref="JsonException"></exception>
    public static async Task<PrintTicket> ParseJSON(string Line)
    {
        // parse Line into JTokens
        JObject JSON = JObject.Parse(Line);
        // attempt to find Department, Process, and Part in the Process Masterlist
        ProcessData Data = new ProcessData();
        Department Department;
        Process Process;
        Part Part;
        try
        {
            Department = await Data.GetIndividualDepartmentAsync(JSON["Department"]!["Title"]!.ToString());
            Process = await Data.GetIndividualProcessAsync(JSON["Process"]!["FullName"]!.ToString());
            Part = await Data.GetProcessPartDataAsync(Process.FullName, JSON["Part"]!["PartNumber"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Department, Process, and/or Part from '{Line}'.");
        }
        // convert the SerializationMode from string to actual enum value and parse the SerialNumber
        SerializationModes Mode;
        if (JSON["SerializationMode"]!.Equals("JBK"))
        {
            Mode = SerializationModes.JBK;
        }
        else
        {
            Mode = SerializationModes.Lot;
        }
        SerialNumber Number = await SerialNumber.ParseJSON(JSON["SerialNumber"]!.ToString());
        // parse out a timestamp for ProductionDate, Shift number, and Operator
        Timestamp ParsedDate;
        int ParsedShift;
        string ParsedOperator;
        if (DateTime.TryParse(JSON["ProductionDate"]!.ToString(), out DateTime ParsedStamp))
        {
            ParsedDate = new Timestamp(ParsedStamp);
        }
        else
        {
            throw new JsonException($"Could not parse a Production Date from '{Line}'.");
        }
        try
        {
            ParsedShift = int.Parse(JSON["ProductionShift"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Shift from '{Line}'.");
        }
        try
        {
            ParsedOperator = JSON["ProductionOperator"]!.ToString();
        }
        catch
        {
            throw new JsonException($"Could not parse an Operator from '{Line}'.");
        }
        // check for and parse partial data sets
        PartialDataSet? FirstPartialDataSet = null;
        PartialDataSet? SecondPartialDataSet = null;
        if (JSON.ContainsKey("FirstPartialDataSet"))
        {
            int Quantity = int.Parse(JSON["FirstPartialDataSet"]!["Quantity"]!.ToString());
            int Shift = int.Parse(JSON["FirstPartialDataSet"]!["Shift"]!.ToString());
            string Operator = JSON["FirstPartialDataSet"]!["Operator"]!.ToString();
            FirstPartialDataSet = new PartialDataSet(Quantity, Shift, Operator);
        }
        if (JSON.ContainsKey("SecondPartialDataSet"))
        {
            int Quantity = int.Parse(JSON["SecondPartialDataSet"]!["Quantity"]!.ToString());
            int Shift = int.Parse(JSON["SecondPartialDataSet"]!["Shift"]!.ToString());
            string Operator = JSON["SecondPartialDataSet"]!["Operator"]!.ToString();
            SecondPartialDataSet = new PartialDataSet(Quantity, Shift, Operator);
        }
        // construct the parsed PrintTicket
        return new PrintTicket(Department, Process, Part, Mode, Number, ParsedDate, ParsedShift, ParsedOperator, FirstPartialDataSet, SecondPartialDataSet);
    }

    /// <summary>
    /// Adds a PartialDataSet to either the First or Second PartialDataSet slot.
    /// </summary>
    /// <param name="DataSet"></param>
    public void AddPartialDataSet(PartialDataSet DataSet)
    {
        if (FirstPartialDataSet is null)
        {
            FirstPartialDataSet = DataSet;
            HasFirstPartialDataSet = true;
        }
        else if (SecondPartialDataSet is null)
        {
            SecondPartialDataSet = DataSet;
            HasSecondPartialDataSet = true;
            HasSpace = false;
        }
    }

    /// <summary>
    /// Removes the First PartialDataSet if it exists.
    /// Shifts the Second into its place, if there is a Second PartialDataSet.
    /// </summary>
    public void RemoveFirstPartialDataSet()
    {
        // quickly return if no DataSet is present in the First slot
        if (!HasFirstPartialDataSet)
        {
            return;
        }
        else
        {
            // remove the PartialDataSet and shift the Second PartialDataSet to the First position
            FirstPartialDataSet = null;
            HasFirstPartialDataSet = false;
            if (HasSecondPartialDataSet)
            {
                ShiftPartialDataSet();
            }
        }
    }

    /// <summary>
    /// Removes the Second PartialDataSet if it exists.
    /// </summary>
    public void RemoveSecondPartialDataSet()
    {
        // quickly return if no DataSet is present in the Second slot
        if (!HasSecondPartialDataSet)
        {
            return;
        }
        else
        {
            // remove the PartialDataSet
            SecondPartialDataSet = null;
            HasSecondPartialDataSet = false;
            HasSpace = true;
        }
    }
}