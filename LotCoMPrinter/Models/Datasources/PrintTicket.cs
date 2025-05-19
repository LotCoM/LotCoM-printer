using System.Globalization;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

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

    private SerializationModes _serializationMode;
    /// <summary>
    /// The type of Serial Number used to Serialize this Print Ticket.
    /// </summary>
    public SerializationModes SerializationMode
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

    private int _productionShift;
    /// <summary>
    /// The Shift that this Print Ticket was initiated on.
    /// </summary>
    public int ProductionShift
    {
        get { return _productionShift; }
        set
        {
            _productionShift = value;
            OnPropertyChanged(nameof(_productionShift));
            OnPropertyChanged(nameof(ProductionShift));
        }
    }

    private int _productionQuantity;

    public int ProductionQuantity
    {
        get { return _productionQuantity; }
        set
        {
            _productionQuantity = value;
            OnPropertyChanged(nameof(_productionQuantity));
            OnPropertyChanged(nameof(ProductionQuantity));
        }
    }

    private string _productionOperator;
    /// <summary>
    /// The Operator that this Print Ticket was initiated by.
    /// </summary>
    public string ProductionOperator
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
    /// Returns the Production Date without the Time segment.
    /// </summary>
    [ObservableProperty]
    public partial string ProductionDateNoTime { get; set; }

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
        SecondPartialDataSet = null;
    }

    /// <summary>
    /// Attempts to convert a string literal to a SerializationMode enum value.
    /// </summary>
    /// <param name="RawMode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static SerializationModes SerializationModeFromString(string RawMode)
    {
        if (RawMode.Equals("JBK"))
        {
            return SerializationModes.JBK;
        }
        else if (RawMode.Equals("Lot"))
        {
            return SerializationModes.Lot;
        }
        else if (RawMode.Equals("None"))
        {
            return SerializationModes.None;
        }
        else
        {
            throw new ArgumentException($"Cannot convert {RawMode} to a SerializationMode.");
        }
    }

    /// <summary>
    /// Validates an integer as non-null.
    /// </summary>
    /// <param name="Value"></param>
    /// <exception cref="FormatException"></exception>
    private static async Task ValidatePositiveInteger(int? Value)
    {
        // run a new thread to ensure that the Integer contains at least one positive digit
        await Task.Run(() =>
        {
            if (Value is null || Value < 1)
            {
                throw new FormatException();
            }
        });
    }

    /// <summary>
    /// Validates a string as non-null. Enforces two or three length, uppercase character format.
    /// </summary>
    /// <param name="String"></param>
    /// <returns>The string as an uppercase Operator Initial.</returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<string> ValidateOperatorInitials(string? String)
    {
        // run a new thread to validate the string
        return await Task.Run(() =>
        {
            // validate that the string is non-null
            if (String is null || !OperatorRegex().IsMatch(String))
            {
                throw new FormatException("Please enter Operator Intials (ie. AB, ABC) before printing Labels.");
            }
            // cast the string to Uppercase and return it
            return String.ToUpper();
        });
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
    public PrintTicket(Process TicketProcess, Part TicketPart, SerializationModes TicketSerializationMode, SerialNumber TicketSerialNumber, DateTime TicketProductionDate, int TicketProductionShift, int TicketProductionQuantity, string TicketProductionOperator, VariableFieldSet VariableFields, PartialDataSet? FirstPartialDataSet = null, PartialDataSet? SecondPartialDataSet = null)
    {
        _process = TicketProcess;
        _part = TicketPart;
        _serializationMode = TicketSerializationMode;
        _serialNumber = TicketSerialNumber;
        _productionDate = TicketProductionDate;
        _productionShift = TicketProductionShift;
        _productionQuantity = TicketProductionQuantity;
        _productionOperator = TicketProductionOperator;
        _variableFields = VariableFields;
        _firstPartialDataSet = FirstPartialDataSet;
        _secondPartialDataSet = SecondPartialDataSet;
        // calculate binding properties
        HasFirstPartialDataSet = FirstPartialDataSet is not null;
        HasSecondPartialDataSet = SecondPartialDataSet is not null;
        HasSpace = !(HasFirstPartialDataSet && HasSecondPartialDataSet);
        Title = $"{Part.ModelNumber} {Part.PartName} - {SerialNumber.GetFormattedValue()}";
        IsPassThrough = _process.Type == OriginationTypes.PassThrough;
        ProductionDateNoTime = $"{ProductionDate.Month}/{ProductionDate.Day}/{ProductionDate.Year}";
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
                "\"Process\":{" +
                    $"\"FullName\":\"{Process.FullName}\"" +
                "}," +
                "\"Part\":{" +
                    $"\"PartNumber\":\"{Part.PartNumber}\"" +
                "}," +
                $"\"SerializationMode\":\"{ModeString}\"," +
                $"\"SerialNumber\":{SerialNumber.ToJSON()}," +
                $"\"ProductionDate\":\"{new Timestamp(ProductionDate).Stamp}\"," +
                $"\"ProductionShift\":\"{ProductionShift}\"," +
                $"\"ProductionQuantity\":\"{ProductionQuantity}\"," +
                $"\"ProductionOperator\":\"{ProductionOperator}\"," +
                "\"VariableFieldSet\":{" +
                    $"\"JBKNumber\":\"{VariableFields.JBKNumber}\"," +
                    $"\"LotNumber\":\"{VariableFields.LotNumber}\"," +
                    $"\"DeburrJBKNumber\":\"{VariableFields.DeburrJBKNumber}\"," +
                    $"\"DieNumber\":\"{VariableFields.DieNumber}\"," +
                    $"\"ModelNumber\":\"{VariableFields.ModelNumber}\"," +
                    $"\"HeatNumber\":\"{VariableFields.HeatNumber}\"" +
                "}";
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
        catch
        {
            throw new JsonException($"Could not parse a Process from '{JSON["Process"]!}'.");
        }
        try
        {
            Part = Data.GetProcessPartData(Process.FullName, JSON["Part"]!["PartNumber"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Part from '{JSON["Part"]!}'.");
        }
        // convert the SerializationMode from string to actual enum value and parse the SerialNumber
        SerializationModes Mode;
        SerialNumber Number;
        try
        {
            Mode = SerializationModeFromString(JSON["SerializationMode"]!.ToString());
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
        int ParsedShift;
        int ParsedQuantity;
        string ParsedOperator;
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
            ParsedShift = int.Parse(JSON["ProductionShift"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Shift from '{JSON["ProductionShift"]!}'.");
        }
        try
        {
            ParsedQuantity = int.Parse(JSON["ProductionQuantity"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Quantity from '{JSON["ProductionQuantity"]!}'.");
        }
        try
        {
            ParsedOperator = JSON["ProductionOperator"]!.ToString();
        }
        catch
        {
            throw new JsonException($"Could not parse an Operator from '{JSON["ProductionOperator"]!}'.");
        }
        // parse the variable field set assigned to the Ticket
        VariableFieldSet VariableFields = new VariableFieldSet();
        try
        {
            VariableFields.JBKNumber = int.Parse(JSON["VariableFieldSet"]!["JBKNumber"]!.ToString());
        }
        catch
        {
            VariableFields.JBKNumber = null;
        }
        try
        {
            VariableFields.LotNumber = JSON["VariableFieldSet"]!["LotNumber"]!.ToString();
        }
        catch
        {
            VariableFields.LotNumber = null;
        }
        try
        {
            VariableFields.DeburrJBKNumber = int.Parse(JSON["VariableFieldSet"]!["DeburrJBKNumber"]!.ToString());
        }
        catch
        {
            VariableFields.DeburrJBKNumber = null;
        }
        try
        {
            VariableFields.DieNumber = int.Parse(JSON["VariableFieldSet"]!["DieNumber"]!.ToString());
        }
        catch
        {
            VariableFields.DieNumber = null;
        }
        try
        {
            VariableFields.ModelNumber = JSON["VariableFieldSet"]!["ModelNumber"]!.ToString();
        }
        catch
        {
            VariableFields.ModelNumber = null;
        }
        try
        {
            VariableFields.HeatNumber = JSON["VariableFieldSet"]!["HeatNumber"]!.ToString();
        }
        catch
        {
            VariableFields.HeatNumber = null;
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
        catch
        {
            throw new JsonException($"Could not parse a Process from '{JSON["Process"]!}'.");
        }
        try
        {
            Part = await Data.GetProcessPartDataAsync(Process.FullName, JSON["Part"]!["PartNumber"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Part from '{JSON["Part"]!}'.");
        }
        // convert the SerializationMode from string to actual enum value and parse the SerialNumber
        SerializationModes Mode;
        SerialNumber Number;
        try
        {
            Mode = SerializationModeFromString(JSON["SerializationMode"]!.ToString());
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
        int ParsedShift;
        int ParsedQuantity;
        string ParsedOperator;
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
            ParsedShift = int.Parse(JSON["ProductionShift"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Shift from '{JSON["ProductionShift"]!}'.");
        }
        try
        {
            ParsedQuantity = int.Parse(JSON["ProductionQuantity"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Quantity from '{JSON["ProductionQuantity"]!}'.");
        }
        try
        {
            ParsedOperator = JSON["ProductionOperator"]!.ToString();
        }
        catch
        {
            throw new JsonException($"Could not parse an Operator from '{JSON["ProductionOperator"]!}'.");
        }
        // parse the variable field set assigned to the Ticket
        VariableFieldSet VariableFields = new VariableFieldSet();
        try
        {
            VariableFields.JBKNumber = int.Parse(JSON["VariableFieldSet"]!["JBKNumber"]!.ToString());
        }
        catch
        {
            VariableFields.JBKNumber = null;
        }
        try
        {
            VariableFields.LotNumber = JSON["VariableFieldSet"]!["LotNumber"]!.ToString();
        }
        catch
        {
            VariableFields.LotNumber = null;
        }
        try
        {
            VariableFields.DeburrJBKNumber = int.Parse(JSON["VariableFieldSet"]!["DeburrJBKNumber"]!.ToString());
        }
        catch
        {
            VariableFields.DeburrJBKNumber = null;
        }
        try
        {
            VariableFields.DieNumber = int.Parse(JSON["VariableFieldSet"]!["DieNumber"]!.ToString());
        }
        catch
        {
            VariableFields.DieNumber = null;
        }
        try
        {
            VariableFields.ModelNumber = JSON["VariableFieldSet"]!["ModelNumber"]!.ToString();
        }
        catch
        {
            VariableFields.ModelNumber = null;
        }
        try
        {
            VariableFields.HeatNumber = JSON["VariableFieldSet"]!["HeatNumber"]!.ToString();
        }
        catch
        {
            VariableFields.HeatNumber = null;
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
    public async Task<PrintTicket> SelfValidate()
    {
        // validate quantity, operator, variable field, and partial dataset field values
        try
        {
            await ValidatePositiveInteger(ProductionQuantity);
        }
        catch
        {
            throw new ArgumentException("Please enter a valid Production Quantity before printing a Label.");
        }
        try
        {
            ProductionOperator = await ValidateOperatorInitials(ProductionOperator);
        }
        catch
        {
            throw new ArgumentException("Please enter valid Operator Initials (ex. ABC) before printing a Label.");
        }
        try
        {
            VariableFields = await VariableFields.SelfValidate(Process.Type);
        }
        catch (Exception _ex)
        {
            throw new ArgumentException($"Please enter a valid {_ex.Message} # before printing a Label.");
        }
        if (HasFirstPartialDataSet)
        {
            try
            {
                await ValidatePositiveInteger(FirstPartialDataSet!.Shift);
            }
            catch
            {
                throw new ArgumentException("Please enter a valid Production Shift in Partial Production Data #1 before printing a Label.");
            }
            try
            {
                await ValidatePositiveInteger(FirstPartialDataSet.Quantity);
            }
            catch
            {
                throw new ArgumentException("Please enter a valid Production Quantity in Partial Production Data #1 before printing a Label.");
            }
            try
            {
                FirstPartialDataSet.Operator = await ValidateOperatorInitials(FirstPartialDataSet.Operator);
            }
            catch
            {
                throw new ArgumentException("Please enter valid Operator Initials (ex. ABC) in Partial Production Data #1 before printing a Label.");
            }
        }
        if (HasSecondPartialDataSet)
        {
            try
            {
                await ValidatePositiveInteger(SecondPartialDataSet!.Shift);
            }
            catch
            {
                throw new ArgumentException("Please enter a valid Production Shift in Partial Production Data #2 before printing a Label.");
            }
            try
            {
                await ValidatePositiveInteger(SecondPartialDataSet.Quantity);
            }
            catch
            {
                throw new ArgumentException("Please enter a valid Production Quantity in Partial Production Data #2 before printing a Label.");
            }
            try
            {
                SecondPartialDataSet.Operator = await ValidateOperatorInitials(SecondPartialDataSet.Operator);
            }
            catch
            {
                throw new ArgumentException("Please enter valid Operator Initials (ex. ABC) in Partial Production Data #2 before printing a Label.");
            }
        }
        // validation is okay; return an updated version of self
        return this;
    }

    // COMPILED REGEX PATTERNS

    [GeneratedRegex(@"^[a-zA-Z][a-zA-Z][a-zA-Z]?$")]
    private static partial Regex OperatorRegex();
}