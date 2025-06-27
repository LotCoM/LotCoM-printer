using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Datatypes;
using LotCom.Database;
using LotCom.Enums;
using LotCom.Extensions;
using LotCom.Types;
using System.Globalization;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides access to the Print History database.
/// </summary>
public static class PrintHistory
{
    /// <summary>
    /// Directory to the Print History database folder.
    /// </summary>
    private static readonly string PrintFolder = "\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints";

    /// <summary>
    /// Opens and reads the Print History for process.
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    /// <exception cref="ProcessNameException"></exception>
    /// <exception cref="DatabaseException"></exception>
    private static IEnumerable<string> Read(Process process)
    {
        // create Table path and Database set variables
        string TablePath = $"{PrintFolder}\\{process.FullName}";
        IEnumerable<string> DatabaseSet;
        // attempt to read the Process' Print History datatable
        try
        {
            DatabaseSet = File.ReadAllLines(TablePath);
            return DatabaseSet;
        }
        // the file was not found in the Database
        catch (FileNotFoundException)
        {
            throw new ProcessNameException($"Could not find a table for the Process '{process}'.");
        }
        // there was some unexpected exception in the reading process
        catch (SystemException _ex)
        {
            throw new DatabaseException
            (
                Message: $"Failed to open the file at '{TablePath}' due to the following exception:\n{_ex.Message}.",
                InnerException: _ex
            );
        }
    }

    /// <summary>
    /// Attempts to parse a PrintTicket object from a PrintLog string.
    /// Uses process as the Process object to reference for PrintTicket information.
    /// </summary>
    /// <param name="PrintLog"></param>
    /// <param name="process"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static PrintTicket ParsePrintTicket(string PrintLog, Process process)
    {
        // split the PrintLog string into seperate CSV segments
        List<string> SplitPrintLog = PrintLog.Split(",").ToList();
        // attempt to parse a Part using the Part Number
        string PrintLogPartNumber = SplitPrintLog[1];
        ProcessData processData = new ProcessData();
        Part part = processData.GetProcessPartData(process.FullName, PrintLogPartNumber);
        // retrieve the Process' SerializationMode and use it to create a SerialNumber from the Log
        SerializationMode Mode = process.Serialization;
        SerialNumber serialNumber = new SerialNumber(Mode, part, int.Parse(SplitPrintLog[4]));
        // parse the production date
        DateTime PrintLogProductionDate = DateTime.ParseExact(SplitPrintLog[^3], "MM/DD/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);
        // parse initial quantity, shift, and operator values and possible PartialDataSets
        Quantity PrintLogQuantity;
        Shift PrintLogShift;
        Operator PrintLogOperator;
        PartialDataSet? FirstPartialData = null;
        PartialDataSet? SecondPartialData = null;
        // colon indicates the existence of a split basket in the PrintLog
        if (SplitPrintLog[3].Contains(':'))
        {
            // split quantity field and save the first, second values
            List<string> SplitQuantity = SplitPrintLog[3].Split(":").ToList();
            PrintLogQuantity = new Quantity(int.Parse(SplitQuantity[0]));
            Quantity FirstPartialQuantity = new Quantity(int.Parse(SplitQuantity[1]));
            // split shift field and save the first, second values
            List<string> SplitShift = SplitPrintLog[^2].Split(":").ToList();
            PrintLogShift = ShiftExtensions.FromString(SplitShift[0]);
            Shift FirstPartialShift = ShiftExtensions.FromString(SplitShift[1]);
            // split operator field and save the first, second values
            List<string> SplitOperator = SplitPrintLog[^1].Split(":").ToList();
            PrintLogOperator = new Operator(SplitOperator[0]);
            Operator FirstPartialOperator = new Operator(SplitOperator[1]);
            // construct and save the First PartialDataSet object
            FirstPartialData = new PartialDataSet(FirstPartialQuantity, FirstPartialShift, FirstPartialOperator);
            // length of three in the split quantity field indicates two PartialDataSets
            if (SplitQuantity.Count == 3)
            {
                // retrieve the third set of values and construct the Second PartialDataSet object
                Quantity SecondPartialQuantity = new Quantity(int.Parse(SplitQuantity[2]));
                Shift SecondPartialShift = ShiftExtensions.FromString(SplitShift[2]);
                Operator SecondPartialOperator = new Operator(SplitOperator[2]);
                SecondPartialData = new PartialDataSet(SecondPartialQuantity, SecondPartialShift, SecondPartialOperator);
            }
        }
        // there is no PartialDataSet information
        else
        {
            // retrieve values directly from the CSV fields
            PrintLogQuantity = new Quantity(int.Parse(SplitPrintLog[3]));
            PrintLogShift = ShiftExtensions.FromString(SplitPrintLog[^2]);
            PrintLogOperator = new Operator(SplitPrintLog[^1]);
        }
        // enumerate over the required fields and parse them from the CSV fields 
        VariableFieldSet PrintLogVariableFieldSet = new VariableFieldSet();
        RequiredFields PrintLogRequiredFields = process.RequiredFields;
        // creates an offset that indicates which variable field to look in
        int NextValue = 0;
        // parse a JBK # if required
        if (PrintLogRequiredFields.JBKNumber)
        {
            try
            {
                PrintLogVariableFieldSet.JBKNumber = new JBKNumber(int.Parse(SplitPrintLog[4 + NextValue]));
                NextValue += 1;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Failed to create a JBK Number from the required value {SplitPrintLog[4 + NextValue]}.");
            }
        }
        // parse a Lot # if required
        if (PrintLogRequiredFields.LotNumber)
        {
            try
            {
                PrintLogVariableFieldSet.LotNumber = new LotNumber(int.Parse(SplitPrintLog[4 + NextValue]));
                NextValue += 1;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Failed to create a Lot Number from the required value {SplitPrintLog[4 + NextValue]}.");
            }
        }
        // parse a Deburr JBK # if required
        if (PrintLogRequiredFields.DeburrJBKNumber)
        {
            try
            {
                PrintLogVariableFieldSet.DeburrJBKNumber = new JBKNumber(int.Parse(SplitPrintLog[4 + NextValue]));
                NextValue += 1;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Failed to create a JBK Number from the required value {SplitPrintLog:[4 + NextValue]}.");
            }
        }
        // parse a Die # if required
        if (PrintLogRequiredFields.DieNumber)
        {
            try
            {
                PrintLogVariableFieldSet.DieNumber = new DieNumber(int.Parse(SplitPrintLog[4 + NextValue]));
                NextValue += 1;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Failed to create a Die Number from the required value {SplitPrintLog[4 + NextValue]}.");
            }
        }
        // parse a Model # if required
        if (PrintLogRequiredFields.ModelNumber)
        {
            try
            {
                PrintLogVariableFieldSet.ModelNumber = new ModelNumber(SplitPrintLog[4 + NextValue]);
                NextValue += 1;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Failed to create a Model Number from the required value {SplitPrintLog[4 + NextValue]}.");
            }
        }
        // parse a Heat # if required
        if (PrintLogRequiredFields.HeatNumber)
        {
            try
            {
                PrintLogVariableFieldSet.HeatNumber = new HeatNumber(int.Parse(SplitPrintLog[4 + NextValue]));

                NextValue += 1;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Failed to create a Heat Number from the required value {SplitPrintLog[4 + NextValue]}.");
            }
        }
        // construct and return a new PrintTicket object
        return new PrintTicket(process, part, Mode, serialNumber, PrintLogProductionDate, PrintLogShift, PrintLogQuantity, PrintLogOperator, PrintLogVariableFieldSet, FirstPartialData, SecondPartialData);
    }

    /// <summary>
    /// Parses the Print History for process and returns a List of PrintTicket objects.
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    public static List<PrintTicket> GetPrintTickets(Process process)
    {
        // read the datatable
        IEnumerable<string> DatabaseSet = Read(process);
        // parse all of the logs to PrintTickets and return them
        List<PrintTicket> printTickets = [];
        foreach (string PrintLog in DatabaseSet)
        {
            printTickets.Add(ParsePrintTicket(PrintLog, process));
        }
        return printTickets;
    }
}   











