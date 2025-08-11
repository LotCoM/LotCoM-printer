using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using LotCom.DataAccess;
using LotCom.DataAccess.Services;
using LotCom.Types.Enums;
using LotCom.Types.Extensions;
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
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    private PartialDataSet _primaryData;
    /// <summary>
    /// The primary, required set of Partial Production Data associated with this Print Ticket.
    /// </summary>
    public PartialDataSet PrimaryData
    {
        get { return _primaryData; }
        set
        {
            _primaryData = value;
            OnPropertyChanged();
        }
    }

    private PartialDataSet? _secondaryData;
    /// <summary>
    /// The first optional, additional Partial Production Data sets associated with this Print Ticket.
    /// </summary>
    public PartialDataSet? SecondaryData
    {
        get { return _secondaryData; }
        set
        {
            _secondaryData = value;
            OnPropertyChanged();
        }
    }

    private PartialDataSet? _tertiaryData;
    /// <summary>
    /// The second optional, additional Partial Production Data sets associated with this Print Ticket.
    /// </summary>
    public PartialDataSet? TertiaryData
    {
        get { return _tertiaryData; }
        set
        {
            _tertiaryData = value;
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Returns whether the Print Ticket has a second Partial Data Set associated with it.
    /// </summary>
    [ObservableProperty]
    public partial bool HasSecondaryData { get; private set; }

    /// <summary>
    /// Returns whether the Print Ticket has a third Partial Data Sets associated with it.
    /// </summary>
    [ObservableProperty]
    public partial bool HasTertiaryData { get; private set; }

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
    /// Shifts the PartialDataSet in the Second position to the First.
    /// </summary>
    private void ShiftPartialDataSet()
    {
        SecondaryData = TertiaryData;
        HasSecondaryData = true;
        TertiaryData = null;
        HasTertiaryData = false;
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
    /// <param name="VariableFields">A set of VariableField values to include at instantiation.</param>
    /// <param name="PrimaryData">The required DataSet to include at instantiation.</param>
    /// <param name="SecondaryData">An optional DataSet to include at instantiation.</param>
    /// <param name="TertiaryData">A second optional DataSet to include at instantiation.</param>
    public PrintTicket(Process TicketProcess, Part TicketPart, SerializationMode TicketSerializationMode, SerialNumber TicketSerialNumber, DateTime TicketProductionDate, VariableFieldSet VariableFields, PartialDataSet PrimaryData, PartialDataSet? SecondaryData = null, PartialDataSet? TertiaryData = null)
    {
        // set the basic info input in the NewPrintTicketForm
        _process = TicketProcess;
        _part = TicketPart;
        _serializationMode = TicketSerializationMode;
        _serialNumber = TicketSerialNumber;
        _productionDate = TicketProductionDate;
        // save the passed VariableSet
        _variableFields = VariableFields;
        // set Partial Datasets
        _primaryData = PrimaryData;
        _secondaryData = SecondaryData;
        _tertiaryData = TertiaryData;
        // calculate binding properties
        HasSecondaryData = SecondaryData is not null;
        HasTertiaryData = TertiaryData is not null;
        HasSpace = !(HasSecondaryData && HasTertiaryData);
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
                $"\"Process\":\"{Process.Id}\"" +
                $"\"Part\":\"{Part.Id}\"" +
                $"\"SerializationMode\":\"{SerializationModeExtensions.ToString(SerializationMode)}\"," +
                $"\"SerialNumber\":{SerialNumber.ToJSON()}," +
                $"\"ProductionDate\":\"{new Timestamp(ProductionDate).Stamp}\"," +
                $"\"VariableFieldSet\":{VariableFields.ToJSON()}";
        // add partial data sets
        JSON +=
            ",\"PrimaryData\":{" +
                $"\"Quantity\":\"{PrimaryData!.Quantity.Value}\"," +
                $"\"Shift\":\"{ShiftExtensions.ToString(PrimaryData.Shift!)}\"," +
                $"\"Operator\":\"{PrimaryData!.Operator.Initials}\"" +
            "}";
        if (HasSecondaryData)
        {
            JSON +=
                ",\"SecondaryData\":{" +
                    $"\"Quantity\":\"{SecondaryData!.Quantity.Value}\"," +
                    $"\"Shift\":\"{ShiftExtensions.ToString(SecondaryData.Shift!)}\"," +
                    $"\"Operator\":\"{SecondaryData!.Operator.Initials}\"" +
                "}";
        }
        if (HasTertiaryData)
        {
            JSON +=
                ",\"TertiaryData\":{" +
                    $"\"Quantity\":\"{TertiaryData!.Quantity.Value}\"," +
                    $"\"Shift\":\"{ShiftExtensions.ToString(TertiaryData.Shift!)}\"," +
                    $"\"Operator\":\"{TertiaryData!.Operator.Initials}\"" +
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
    public static async Task<PrintTicket> ParseJSON(string Line)
    {
        // parse Line into JTokens
        JObject JSON = JObject.Parse(Line);
        // attempt to retrieve Process from the Database
        Process? Process;
        Part? Part;
        try
        {
            Process = await ProcessService.Get(int.Parse(JSON["Process"]!.ToString()), App.UserAgent);
        }
        catch (SystemException)
        {
            throw new JsonException($"Could not find a Process with Id '{JSON["Process"]!}'.");
        }
        if (Process is null)
        {
            throw new FormatException($"Could not find a Process with Id '{JSON["Process"]!}'.");
        }
        // attempt to retrieve Part from the Database
        try
        {
            Part = await PartService.Get(int.Parse(JSON["Part"]!.ToString()), App.UserAgent);
        }
        catch (SystemException)
        {
            throw new JsonException($"Could not find a Part with Id '{JSON["Part"]!}'.");
        }
        if (Part is null)
        {
            throw new FormatException($"Could not find a Part with Id '{JSON["Part"]!}'.");
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
        // parse out a timestamp for ProductionDate
        DateTime ParsedDate;
        try
        {
            ParsedDate = DateTime.ParseExact(JSON["ProductionDate"]!.ToString(), "MM/dd/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);
        }
        catch
        {
            throw new JsonException($"Could not parse a Production Date from '{JSON["ProductionDate"]!}'.");
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
        PartialDataSet Primary;
        PartialDataSet? Secondary;
        PartialDataSet? Tertiary;
        try
        {
            Primary = PartialDataSet.ParseJSON(JSON["PrimaryData"]!);
        }
        catch (JsonException)
        {
            throw new FormatException("No Primary Production Data could be parsed from JSON.");
        }
        try
        {
            Secondary = PartialDataSet.ParseJSON(JSON["SecondaryData"]!);
        }
        catch (JsonException)
        {
            throw new FormatException("Secondary Production Data is in an invalid format.");
        }
        try
        {
            Tertiary = PartialDataSet.ParseJSON(JSON["TertiaryData"]!);
        }
        catch (JsonException)
        {
            throw new FormatException("Tertiary Production Data is in an invalid format.");
        }
        // construct and return the parsed PrintTicket
        PrintTicket NewTicket = new PrintTicket(Process, Part, Mode, Number, ParsedDate, VariableFields, Primary, Secondary, Tertiary);
        if (Secondary is not null)
        {
            NewTicket.HasSecondaryData = true;
        }
        if (Tertiary is not null)
        {
            NewTicket.HasTertiaryData = true;
            NewTicket.HasSpace = false;
        }
        return NewTicket;
    }

    /// <summary>
    /// Creates a deep copy of the Source, removing all referential equivalencies.
    /// </summary>
    /// <param name="Source"></param>
    /// <returns></returns>
    public static PrintTicket DeepCopy(PrintTicket Source)
    {
        PrintTicket Copy = new PrintTicket
        (
            Source.Process,
            Source.Part,
            Source.SerializationMode,
            new SerialNumber
            (
                Source.SerializationMode,
                Source.Part.Id,
                Source.SerialNumber.Value
            ),
            new DateTime(Source.ProductionDate.Ticks),
            new VariableFieldSet
            (

            ),
            new PartialDataSet
            (
                new Quantity
                (
                    Source.PrimaryData.Quantity.Value
                ),
                Source.PrimaryData.Shift,
                new Operator
                (
                    Source.PrimaryData.Operator.Initials
                )
            )
        );
        if (Source.VariableFields.JBKNumber is not null)
        {
            Copy.VariableFields.JBKNumber = new JBKNumber
            (
                Source.VariableFields.JBKNumber.Literal
            );
        }
        if (Source.VariableFields.LotNumber is not null)
        {
            Copy.VariableFields.LotNumber = new LotNumber
            (
                Source.VariableFields.LotNumber.Literal
            );
        }
        if (Source.VariableFields.DieNumber is not null)
        {
            Copy.VariableFields.DieNumber = new DieNumber
            (
                Source.VariableFields.DieNumber.Formatted
            );
        }
        if (Source.VariableFields.DeburrJBKNumber is not null)
        {
            Copy.VariableFields.DeburrJBKNumber = new JBKNumber
            (
                Source.VariableFields.DeburrJBKNumber.Literal
            );
        }
        if (Source.VariableFields.HeatNumber is not null)
        {
            Copy.VariableFields.HeatNumber = new HeatNumber
            (
                Source.VariableFields.HeatNumber.Literal
            );
        }
        if (Source.SecondaryData is not null)
        {
            Copy.SecondaryData = new PartialDataSet
            (
                new Quantity
                (
                    Source.SecondaryData.Quantity.Value
                ),
                Source.SecondaryData.Shift,
                new Operator
                (
                    Source.SecondaryData.Operator.Initials
                )
            );
        }
        if (Source.TertiaryData is not null)
        {
            Copy.TertiaryData = new PartialDataSet
            (
                new Quantity
                (
                    Source.TertiaryData.Quantity.Value
                ),
                Source.TertiaryData.Shift,
                new Operator
                (
                    Source.TertiaryData.Operator.Initials
                )
            );
        }
        return Copy;
    }

    /// <summary>
    /// Converts a Print Model from the Database into a PrintTicket object.
    /// </summary>
    /// <param name="Model"></param>
    /// <returns></returns>
    public static PrintTicket FromPrint(Print Model)
    {
        return new PrintTicket
        (
            Model.Process,
            Model.Part,
            Model.Process.Serialization,
            Model.GetSerialNumber(),
            Model.ProductionDate,
            Model.VariableFields,
            Model.PrimaryDataSet,
            Model.SecondaryDataSet,
            Model.TertiaryDataSet
        );
    }

    /// <summary>
    /// Adds a PartialDataSet to either the First or Second PartialDataSet slot.
    /// </summary>
    /// <param name="DataSet"></param>
    public void AddPartialDataSet(PartialDataSet DataSet)
    {
        if (SecondaryData is null)
        {
            SecondaryData = DataSet;
            HasSecondaryData = true;
        }
        else if (TertiaryData is null)
        {
            TertiaryData = DataSet;
            HasTertiaryData = true;
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
        if (!HasSecondaryData)
        {
            return;
        }
        else
        {
            // remove the PartialDataSet and shift the Second PartialDataSet to the First position
            SecondaryData = null;
            HasSecondaryData = false;
            if (HasTertiaryData)
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
        if (!HasTertiaryData)
        {
            return;
        }
        else
        {
            // remove the PartialDataSet
            TertiaryData = null;
            HasTertiaryData = false;
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
        if (Process.RequiredFields.HeatNumber && VariableFields.HeatNumber is null)
        {
            throw new ArgumentException("Please enter a valid Heat # before printing a Label.");
        }
        // validate partial data sets
        try
        {
            PrimaryData.SelfValidate();
        }
        catch (ArgumentException)
        {
            throw new ArgumentException("Please enter valid Production Data before printing a Label.");
        }
        if (HasSecondaryData)
        {
            try
            {
                SecondaryData!.SelfValidate();
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Please enter valid Production Data in Partial Production Data #1 before printing a Label.");
            }
        }
        if (HasTertiaryData)
        {
            try
            {
                TertiaryData!.SelfValidate();
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Please enter valid Production Data in Partial Production Data #2 before printing a Label.");
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
        int FullQuantity = PrimaryData.Quantity.Value;
        if (HasSecondaryData)
        {
            FullQuantity += SecondaryData!.Quantity.Value;
        }
        if (HasTertiaryData)
        {
            FullQuantity += TertiaryData!.Quantity.Value;
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
        string FullShift = $"{ShiftExtensions.ToString(PrimaryData.Shift)}";
        if (HasSecondaryData)
        {
            FullShift = $"{FullShift}:{ShiftExtensions.ToString(SecondaryData!.Shift!)}";
        }
        if (HasTertiaryData)
        {
            FullShift = $"{FullShift}:{ShiftExtensions.ToString(TertiaryData!.Shift!)}";
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
        string FullQuantity = $"{PrimaryData.Quantity.Value}";
        if (HasSecondaryData)
        {
            FullQuantity = $"{FullQuantity}:{SecondaryData!.Quantity.Value}";
        }
        if (HasTertiaryData)
        {
            FullQuantity = $"{FullQuantity}:{TertiaryData!.Quantity.Value}";
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
        string FullOperator = $"{PrimaryData.Operator.Initials}";
        if (HasSecondaryData)
        {
            FullOperator = $"{FullOperator}:{SecondaryData!.Operator.Initials}";
        }
        if (HasTertiaryData)
        {
            FullOperator = $"{FullOperator}:{TertiaryData!.Operator.Initials}";
        }
        return FullOperator;
    }

    /// <summary>
    /// Updates the Serial Number string attached to the end of a PrintTicket's title.
    /// </summary>
    /// <param name="NewNumber"></param>
    public void UpdateTitleSerialNumber(string NewNumber)
    {
        Title = $"{Part.ModelNumber.Code} {Part.PartName} - {NewNumber}";
    }
}