using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using LotCom.Database;
using LotCom.Enums;
using LotCom.Extensions;
using LotCom.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotComPrinter.Models.Datatypes;

public partial class PrintTicket : ObservableObject
{
    private Process _process;
    /// <summary>
    /// The Process that initiated this Print Ticket.
    /// </summary>
    public Process Process
    {
        get { return _process; }
        set
        {
            _process = value;
            OnPropertyChanged(nameof(_process));
            OnPropertyChanged(nameof(Process));
        }
    }

    private Part _part;
    /// <summary>
    /// The Part that this Print Ticket is applied to.
    /// </summary>
    public Part Part
    {
        get { return _part; }
        set
        {
            _part = value;
            OnPropertyChanged(nameof(_part));
            OnPropertyChanged(nameof(Part));
        }
    }

    private SerializationMode _serializationMode;
    /// <summary>
    /// The type of Serial Number used to Serialize this Print Ticket.
    /// </summary>
    public SerializationMode SerializationMode
    {
        get { return _serializationMode; }
        set
        {
            _serializationMode = value;
            OnPropertyChanged(nameof(_serializationMode));
            OnPropertyChanged(nameof(SerializationMode));
        }
    }

    private SerialNumber _serialNumber;
    /// <summary>
    /// The Serial Number (JBK or Lot Number) applied to this Print Ticket.
    /// </summary>
    public SerialNumber SerialNumber
    {
        get { return _serialNumber; }
        set
        {
            _serialNumber = value;
            OnPropertyChanged(nameof(_serialNumber));
            OnPropertyChanged(nameof(SerialNumber));
        }
    }

    private DateTime _productionDate;
    /// <summary>
    /// The Date and Time at which this Print Ticket was initiated.
    /// </summary>
    public DateTime ProductionDate
    {
        get { return _productionDate; }
        set
        {
            _productionDate = value;
            OnPropertyChanged(nameof(_productionDate));
            OnPropertyChanged(nameof(ProductionDate));
        }
    }

    private Shift _productionShift;
    /// <summary>
    /// The Shift that this Print Ticket was initiated on.
    /// </summary>
    public Shift ProductionShift
    {
        get { return _productionShift; }
        set
        {
            _productionShift = value;
            OnPropertyChanged(nameof(_productionShift));
            OnPropertyChanged(nameof(ProductionShift));
        }
    }

    private Quantity _productionQuantity;

    public Quantity ProductionQuantity
    {
        get { return _productionQuantity; }
        set
        {
            _productionQuantity = value;
            OnPropertyChanged(nameof(_productionQuantity));
            OnPropertyChanged(nameof(ProductionQuantity));
        }
    }

    private Operator _productionOperator;
    /// <summary>
    /// The Operator that this Print Ticket was initiated by.
    /// </summary>
    public Operator ProductionOperator
    {
        get { return _productionOperator; }
        set
        {
            _productionOperator = value;
            OnPropertyChanged(nameof(_productionOperator));
            OnPropertyChanged(nameof(ProductionOperator));
        }
    }

    private VariableFieldSet _variableFields;
    /// <summary>
    /// The Variable Field Set assigned to hold the data entered for this Print Ticket.
    /// </summary>
    public VariableFieldSet VariableFields
    {
        get { return _variableFields; }
        set
        {
            _variableFields = value;
            OnPropertyChanged(nameof(_variableFields));
            OnPropertyChanged(nameof(VariableFields));
        }
    }

    private PartialDataSet? _firstPartialDataSet;
    /// <summary>
    /// The first of the Partial Production Data sets associated with this Print Ticket.
    /// </summary>
    public PartialDataSet? FirstPartialDataSet
    {
        get { return _firstPartialDataSet; }
        set
        {
            _firstPartialDataSet = value;
            OnPropertyChanged(nameof(_firstPartialDataSet));
            OnPropertyChanged(nameof(FirstPartialDataSet));
        }
    }

    private PartialDataSet? _secondPartialDataSet;
    /// <summary>
    /// The second of the Partial Production Data sets associated with this Print Ticket.
    /// </summary>
    public PartialDataSet? SecondPartialDataSet
    {
        get { return _secondPartialDataSet; }
        set
        {
            _secondPartialDataSet = value;
            OnPropertyChanged(nameof(_secondPartialDataSet));
            OnPropertyChanged(nameof(SecondPartialDataSet));
        }
    }

    private bool _jbkManualEntry = false;
    /// <summary>
    /// Whether this PrintTicket can accept a manually-entered JBK Number or not.
    /// </summary>
    public bool JBKManualEntry
    {
        get { return _jbkManualEntry; }
        set
        {
            _jbkManualEntry = value;
            OnPropertyChanged(nameof(_jbkManualEntry));
            OnPropertyChanged(nameof(JBKManualEntry));
        }
    }

    private bool _lotManualEntry = false;
    /// <summary>
    /// Whether this PrintTicket can accept a manually-entered Lot Number or not.
    /// </summary>
    public bool LotManualEntry
    {
        get { return _lotManualEntry; }
        set
        {
            _lotManualEntry = value;
            OnPropertyChanged(nameof(_lotManualEntry));
            OnPropertyChanged(nameof(LotManualEntry));
        }
    }

    /// <summary>
    /// Returns whether the Print Ticket has one Partial Data Set associated with it.
    /// </summary>
    [ObservableProperty]
    public partial bool HasFirstPartialDataSet { get; private set; }

    /// <summary>
    /// Returns whether the Print Ticket has two Partial Data Sets associated with it.
    /// </summary>
    [ObservableProperty]
    public partial bool HasSecondPartialDataSet { get; private set; }

    /// <summary>
    /// Returns whether the Print Ticket has space for another Partial Data Set.
    /// </summary>
    [ObservableProperty]
    public partial bool HasSpace { get; private set; }

    /// <summary>
    /// Provides a Title for the Print Ticket that gives the crucial information of the Ticket.
    /// </summary>
    [ObservableProperty]
    public partial string Title { get; set; }

    /// <summary>
    /// Returns whether the Print Ticket's Process is a Pass-through Process or not.
    /// </summary>
    [ObservableProperty]
    public partial bool IsPassThrough { get; set; }

    /// <summary>
    /// Returns the Production Date without the Year or Time segment.
    /// </summary>
    [ObservableProperty]
    public partial string ProductionDateShort { get; set; }

    /// <summary>
    /// Controls whether the Print Ticket is selected in the Open Print Ticket ListView.
    /// Purely a template-binding property.
    /// </summary>
    [ObservableProperty]
    public partial bool IsSelectedInList { get; set; } = false;

    /// <summary>
    /// Shifts the PartialDataSet from the Second position into the First.
    /// </summary>
    private void ShiftPartialDataSet()
    {
        FirstPartialDataSet = SecondPartialDataSet;
        HasFirstPartialDataSet = true;
        SecondPartialDataSet = null;
        HasSecondPartialDataSet = false;
        HasSpace = true;
    }

    /// <summary>
    /// Provides a structure for the creation and maintenance of a Printing Ticket.
    /// </summary>
    /// <param name="TicketProcess">The Process that initiated this Print Ticket.</param>
    /// <param name="TicketPart">The Part that this Print Ticket is applied to.</param>
    /// <param name="TicketSerializationMode">The type of Serial Number used to Serialize this Print Ticket.</param>
    /// <param name="TicketSerialNumber">The Serial Number to apply to this Print Ticket.</param>
    /// <param name="TicketProductionDate">The Date on which this Print Ticket was initiated.</param>
    /// <param name="TicketProductionShift">The Shift that this Print Ticket was initiated on.</param>
    /// <param name="TicketProductionQuantity">The Quantity produced on the Shift that this Print Ticket was initiated on.</param>
    /// <param name="TicketProductionOperator">The Operator that initiated this Print Ticket.</param>
    /// <param name="VariableFields">A set of VariableField values to include at instantiation.</param>
    /// <param name="FirstPartialDataSet">An optional DataSet to include at instantiation.</param>
    /// <param name="SecondPartialDataSet">A second optional DataSet to include at instantiation.</param>
    public PrintTicket(Process TicketProcess, Part TicketPart, SerializationMode TicketSerializationMode, SerialNumber TicketSerialNumber, DateTime TicketProductionDate, Shift TicketProductionShift, Quantity TicketProductionQuantity, Operator TicketProductionOperator, VariableFieldSet VariableFields, PartialDataSet? FirstPartialDataSet = null, PartialDataSet? SecondPartialDataSet = null)
    {
        // set the basic info input in the NewPrintTicketForm
        _process = TicketProcess;
        _part = TicketPart;
        _serializationMode = TicketSerializationMode;
        _serialNumber = TicketSerialNumber;
        _productionDate = TicketProductionDate;
        _productionShift = TicketProductionShift;
        _productionQuantity = TicketProductionQuantity;
        _productionOperator = TicketProductionOperator;
        // save the passed VariableSet and set its Model Number using the Part selected
        _variableFields = VariableFields;
        VariableFields.ModelNumber = Part.ModelNumber;
        // set Partial Datasets if passed
        _firstPartialDataSet = FirstPartialDataSet;
        _secondPartialDataSet = SecondPartialDataSet;
        // calculate binding properties
        HasFirstPartialDataSet = FirstPartialDataSet is not null;
        HasSecondPartialDataSet = SecondPartialDataSet is not null;
        HasSpace = !(HasFirstPartialDataSet && HasSecondPartialDataSet);
        Title = $"{Part.ModelNumber.Code} {Part.PartName} - {SerialNumber.GetFormattedValue()}";
        IsPassThrough = _process.Origination == OriginationType.PassThrough;
        ProductionDateShort = $"{ProductionDate.Month}/{ProductionDate.Day}";
        if (Process.RequiredFields.JBKNumber && SerializationMode != SerializationMode.JBK)
        {
            JBKManualEntry = true;
        }
        if (Process.RequiredFields.LotNumber && SerializationMode != SerializationMode.Lot)
        {
            LotManualEntry = true;
        }
    }

    /// <summary>
    /// Converts the PrintTicket object to a JSON stream.
    /// </summary>
    /// <returns></returns>
    public string ToJSON()
    {
        // build the JSON stream piece-by-piece
        // Department, Process, Part, SerializationMode, SerialNumber, and Production Date and Shift are all universal
        string JSON =
            "{" +
                "\"Process\":{" +
                    $"\"FullName\":\"{Process.FullName}\"" +
                "}," +
                "\"Part\":{" +
                    $"\"PartNumber\":\"{Part.PartNumber}\"" +
                "}," +
                $"\"SerializationMode\":\"{SerializationModeExtensions.ToString(SerializationMode)}\"," +
                $"\"SerialNumber\":{SerialNumber.ToJSON()}," +
                $"\"ProductionDate\":\"{new Timestamp(ProductionDate).Stamp}\"," +
                $"\"ProductionShift\":\"{ShiftExtensions.ToString(ProductionShift)}\"," +
                $"\"ProductionQuantity\":\"{ProductionQuantity.Value}\"," +
                $"\"ProductionOperator\":\"{ProductionOperator.Initials}\"," +
                $"\"VariableFieldSet\":{VariableFields.ToJSON()}";
        // add partial data sets only if assigned
        if (HasFirstPartialDataSet)
        {
            JSON +=
                ",\"FirstPartialDataSet\":{" +
                    $"\"Quantity\":\"{FirstPartialDataSet!.Quantity.Value}\"," +
                    $"\"Shift\":\"{ShiftExtensions.ToString(FirstPartialDataSet.Shift!)}\"," +
                    $"\"Operator\":\"{FirstPartialDataSet!.Operator.Initials}\"" +
                "}";
        }
        if (HasSecondPartialDataSet)
        {
            JSON +=
                ",\"SecondPartialDataSet\":{" +
                    $"\"Quantity\":\"{SecondPartialDataSet!.Quantity.Value}\"," +
                    $"\"Shift\":\"{ShiftExtensions.ToString(SecondPartialDataSet.Shift!)}\"," +
                    $"\"Operator\":\"{SecondPartialDataSet!.Operator.Initials}\"" +
                "}";
        }
        // close the JSON stream
        JSON = $"{JSON}" + "}";
        return JSON;
    }

    /// <summary>
    /// Attempts to parse a full PrintTicket object from a JSON formatted Line.
    /// </summary>
    /// <param name="Line"></param>
    /// <returns>A PrintTicket object.</returns>
    /// <exception cref="JsonException"></exception>
    public static PrintTicket ParseJSON(string Line)
    {
        // parse Line into JTokens
        JObject JSON = JObject.Parse(Line);
        // attempt to find Department, Process, and Part in the Process Masterlist
        ProcessData Data = new ProcessData();
        Process Process;
        Part Part;
        try
        {
            Process = Data.GetIndividualProcess(JSON["Process"]!["FullName"]!.ToString());
        }
        catch (SystemException)
        {
            throw new JsonException($"Could not parse a Process from '{JSON["Process"]!}'.");
        }
        try
        {
            Part = Data.GetProcessPartData(Process.FullName, JSON["Part"]!["PartNumber"]!.ToString());
        }
        catch (SystemException)
        {
            throw new JsonException($"Could not parse a Part from '{JSON["Part"]!}'.");
        }
        // convert the SerializationMode from string to actual enum value and parse the SerialNumber
        SerializationMode Mode;
        SerialNumber Number;
        try
        {
            Mode = SerializationModeExtensions.FromString(JSON["SerializationMode"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a SerializationMode from '{JSON["SerializationMode"]!}'.");
        }
        try
        {
            Number = SerialNumber.ParseJSON(JSON["SerialNumber"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a SerialNumber from '{JSON["SerialNumber"]!}'.");
        }
        // parse out a timestamp for ProductionDate, Shift number, and Operator
        DateTime ParsedDate;
        Shift ParsedShift;
        Quantity ParsedQuantity;
        Operator ParsedOperator;
        try
        {
            ParsedDate = DateTime.ParseExact(JSON["ProductionDate"]!.ToString(), "MM/dd/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Date from '{JSON["ProductionDate"]!}'.");
        }
        try
        {
            ParsedShift = ShiftExtensions.FromString(JSON["ProductionShift"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Shift from '{JSON["ProductionShift"]!}'.");
        }
        try
        {
            ParsedQuantity = new Quantity(int.Parse(JSON["ProductionQuantity"]!.ToString()));
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Quantity from '{JSON["ProductionQuantity"]!}'.");
        }
        try
        {
            ParsedOperator = new Operator(JSON["ProductionOperator"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse an Operator from '{JSON["ProductionOperator"]!}'.");
        }
        // parse the variable field set assigned to the Ticket
        VariableFieldSet VariableFields;
        try
        {
            VariableFields = VariableFieldSet.ParseJSON(JSON["VariableFieldSet"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a VariableFieldSet from '{JSON["VariableFieldSet"]!}'.");
        }
        // check for and parse partial data sets
        PartialDataSet? FirstPartialDataSet = null;
        PartialDataSet? SecondPartialDataSet = null;
        if (JSON.ContainsKey("FirstPartialDataSet"))
        {
            try
            {
                FirstPartialDataSet = PartialDataSet.ParseJSON(JSON["FirstPartialDataSet"]!);
            }
            catch
            {
                throw new JsonException($"Could not parse the First Partial Data Set from {JSON["FirstPartialDataSet"]!}.");
            }
        }
        if (JSON.ContainsKey("SecondPartialDataSet"))
        {
            try
            {
                SecondPartialDataSet = PartialDataSet.ParseJSON(JSON["SecondPartialDataSet"]!);
            }
            catch
            {
                throw new JsonException($"Could not parse the Second Partial Data Set from {JSON["SecondPartialDataSet"]!}.");
            }
        }
        // construct and return the parsed PrintTicket
        PrintTicket NewTicket = new PrintTicket(Process, Part, Mode, Number, ParsedDate, ParsedShift, ParsedQuantity, ParsedOperator, VariableFields, FirstPartialDataSet, SecondPartialDataSet);
        if (FirstPartialDataSet is not null)
        {
            NewTicket.HasFirstPartialDataSet = true;
        }
        if (SecondPartialDataSet is not null)
        {
            NewTicket.HasSecondPartialDataSet = true;
            NewTicket.HasSpace = false;
        }
        return NewTicket;
    }

    /// <summary>
    /// Attempts to asynchronously parse a full PrintTicket object from a JSON formatted Line.
    /// </summary>
    /// <param name="Line"></param>
    /// <returns>A PrintTicket object.</returns>
    /// <exception cref="JsonException"></exception>
    public static async Task<PrintTicket> ParseJSONAsync(string Line)
    {
        // parse Line into JTokens
        JObject JSON = JObject.Parse(Line);
        // attempt to find Department, Process, and Part in the Process Masterlist
        ProcessData Data = new ProcessData();
        Process Process;
        Part Part;
        try
        {
            Process = await Data.GetIndividualProcessAsync(JSON["Process"]!["FullName"]!.ToString());
        }
        catch (SystemException)
        {
            throw new JsonException($"Could not parse a Process from '{JSON["Process"]!}'.");
        }
        try
        {
            Part = await Data.GetProcessPartDataAsync(Process.FullName, JSON["Part"]!["PartNumber"]!.ToString());
        }
        catch (SystemException)
        {
            throw new JsonException($"Could not parse a Part from '{JSON["Part"]!}'.");
        }
        // convert the SerializationMode from string to actual enum value and parse the SerialNumber
        SerializationMode Mode;
        SerialNumber Number;
        try
        {
            Mode = SerializationModeExtensions.FromString(JSON["SerializationMode"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a SerializationMode from '{JSON["SerializationMode"]!}'.");
        }
        try
        {
            Number = await SerialNumber.ParseJSONAsync(JSON["SerialNumber"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a SerialNumber from '{JSON["SerialNumber"]!}'.");
        }
        // parse out a timestamp for ProductionDate, Shift number, and Operator
        DateTime ParsedDate;
        Shift ParsedShift;
        Quantity ParsedQuantity;
        Operator ParsedOperator;
        try
        {
            ParsedDate = DateTime.ParseExact(JSON["ProductionDate"]!.ToString(), "MM/dd/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Date from '{JSON["ProductionDate"]!}'.");
        }
        try
        {
            ParsedShift = ShiftExtensions.FromString(JSON["ProductionShift"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Shift from '{JSON["ProductionShift"]!}'.");
        }
        try
        {
            ParsedQuantity = new Quantity(int.Parse(JSON["ProductionQuantity"]!.ToString()));
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Quantity from '{JSON["ProductionQuantity"]!}'.");
        }
        try
        {
            ParsedOperator = new Operator(JSON["ProductionOperator"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse an Operator from '{JSON["ProductionOperator"]!}'.");
        }
        // parse the variable field set assigned to the Ticket
        VariableFieldSet VariableFields;
        try
        {
            VariableFields = VariableFieldSet.ParseJSON(JSON["VariableFieldSet"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a VariableFieldSet from '{JSON["VariableFieldSet"]!}'.");
        }
        // check for and parse partial data sets
        PartialDataSet? FirstPartialDataSet = null;
        PartialDataSet? SecondPartialDataSet = null;
        if (JSON.ContainsKey("FirstPartialDataSet"))
        {
            try
            {
                FirstPartialDataSet = PartialDataSet.ParseJSON(JSON["FirstPartialDataSet"]!);
            }
            catch
            {
                throw new JsonException($"Could not parse the First Partial Data Set from {JSON["FirstPartialDataSet"]!}.");
            }
        }
        if (JSON.ContainsKey("SecondPartialDataSet"))
        {
            try
            {
                SecondPartialDataSet = PartialDataSet.ParseJSON(JSON["SecondPartialDataSet"]!);
            }
            catch
            {
                throw new JsonException($"Could not parse the Second Partial Data Set from {JSON["SecondPartialDataSet"]!}.");
            }
        }
        // construct and return the parsed PrintTicket
        PrintTicket NewTicket = new PrintTicket(Process, Part, Mode, Number, ParsedDate, ParsedShift, ParsedQuantity, ParsedOperator, VariableFields, FirstPartialDataSet, SecondPartialDataSet);
        if (FirstPartialDataSet is not null)
        {
            NewTicket.HasFirstPartialDataSet = true;
        }
        if (SecondPartialDataSet is not null)
        {
            NewTicket.HasSecondPartialDataSet = true;
            NewTicket.HasSpace = false;
        }
        return NewTicket;
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

    /// <summary>
    /// Validates all PrintTicket values to ensure proper formatting and value types.
    /// </summary>
    /// <returns>A modified (formatted) version of the object calling this method.</returns>
    /// <exception cref="ArgumentException"></exception>
    public PrintTicket SelfValidate()
    {
        // validate quantity, operator
        if (!ProductionQuantity.ConfirmPositiveCount())
        {
            throw new ArgumentException("Please enter a valid Production Quantity before printing a Label.");
        }
        if (!ProductionOperator.ConfirmProperInitials())
        {
            throw new ArgumentException("Please enter valid Operator Initials before printing a Label.");
        }
        // validate the variable fields
        if (Process.RequiredFields.JBKNumber && VariableFields.JBKNumber is null)
        {
            throw new ArgumentException("Please enter a valid JBK # before printing a Label.");
        }
        if (Process.RequiredFields.LotNumber && VariableFields.LotNumber is null)
        {
            throw new ArgumentException("Please enter a valid Lot # before printing a Label.");
        }
        if (Process.RequiredFields.DeburrJBKNumber && VariableFields.DeburrJBKNumber is null)
        {
            throw new ArgumentException("Please enter a valid Deburr JBK # before printing a Label.");
        }
        if (Process.RequiredFields.DieNumber && VariableFields.DieNumber is null)
        {
            throw new ArgumentException("Please enter a valid Die # before printing a Label.");
        }
        if (Process.RequiredFields.ModelNumber && VariableFields.ModelNumber is null)
        {
            throw new ArgumentException("Please enter a valid Model # before printing a Label.");
        }
        if (Process.RequiredFields.HeatNumber && VariableFields.HeatNumber is null)
        {
            throw new ArgumentException("Please enter a valid Heat # before printing a Label.");
        }
        // validate partial data sets
        if (HasFirstPartialDataSet)
        {
            try
            {
                FirstPartialDataSet!.SelfValidate();
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Please enter a valid Production Quantity in Partial Production Data #1 before printing a Label.");
            }
        }
        if (HasSecondPartialDataSet)
        {
            try
            {
                SecondPartialDataSet!.SelfValidate();
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Please enter a valid Production Quantity in Partial Production Data #2 before printing a Label.");
            }
        }
        // validation is okay; return an updated version of self
        return this;
    }

    /// <summary>
    /// Combines the possible three partial quantity values into one total quantity.
    /// </summary>
    /// <returns></returns>
    public Quantity GetTotalQuantity()
    {
        // compile the full PrintTicket quantity
        int FullQuantity = ProductionQuantity.Value;
        if (HasFirstPartialDataSet)
        {
            FullQuantity += FirstPartialDataSet!.Quantity.Value;
        }
        if (HasSecondPartialDataSet)
        {
            FullQuantity += SecondPartialDataSet!.Quantity.Value;
        }
        return new Quantity(FullQuantity);
    }

    /// <summary>
    /// Combines the three possible Shift values into a string with format "3:1:2".
    /// </summary>
    /// <returns></returns>
    public string GetCombinedShifts()
    {
        // compile the Shift values into a single value
        string FullShift = $"{ShiftExtensions.ToString(ProductionShift)}";
        if (HasFirstPartialDataSet)
        {
            FullShift = $"{FullShift}:{ShiftExtensions.ToString(FirstPartialDataSet!.Shift!)}";
        }
        if (HasSecondPartialDataSet)
        {
            FullShift = $"{FullShift}:{ShiftExtensions.ToString(SecondPartialDataSet!.Shift!)}";
        }
        return FullShift;
    }

    /// <summary>
    /// Combines the three possible Quantity values into a string with format "100:100:100".
    /// </summary>
    /// <returns></returns>
    public string GetCombinedQuantities()
    {
        // compile the Quantity values into a single value
        string FullQuantity = $"{ProductionQuantity.Value}";
        if (HasFirstPartialDataSet)
        {
            FullQuantity = $"{FullQuantity}:{FirstPartialDataSet!.Quantity.Value}";
        }
        if (HasSecondPartialDataSet)
        {
            FullQuantity = $"{FullQuantity}:{SecondPartialDataSet!.Quantity.Value}";
        }
        return FullQuantity;
    }

    /// <summary>
    /// Combines the three possible Operator values into a string with format "OP1:OP2:OP3".
    /// </summary>
    /// <returns></returns>
    public string GetCombinedOperators()
    {
        // compile the Operator values into a single value
        string FullOperator = $"{ProductionOperator.Initials}";
        if (HasFirstPartialDataSet)
        {
            FullOperator = $"{FullOperator}:{FirstPartialDataSet!.Operator.Initials}";
        }
        if (HasSecondPartialDataSet)
        {
            FullOperator = $"{FullOperator}:{SecondPartialDataSet!.Operator.Initials}";
        }
        return FullOperator;
    }
}