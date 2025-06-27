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
    /// Asynchronously opens and reads the Print History for process.
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    /// <exception cref="ProcessNameException"></exception>
    /// <exception cref="DatabaseException"></exception>
    private static  async Task<IEnumerable<string>> ReadAsync(Process process)
    {
        // create Table path and Database set variables
        string TablePath = $"{PrintFolder}\\{process.FullName}";
        IEnumerable<string> DatabaseSet;
        // attempt to read the Process' Print History datatable
        try
        {
            DatabaseSet = await File.ReadAllLinesAsync(TablePath);
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
    /// <exception cref="SystemException"></exception>
    private static PrintTicket ParsePrintTicket(string PrintLog, Process process)
    {
        // split the PrintLog string into seperate CSV segments
        List<string> SplitPrintLog = PrintLog.Split(",").ToList();
        // attempt to parse a Part using the Part Number
        string PrintLogPartNumber = SplitPrintLog[1];
        ProcessData processData = new ProcessData();
        Part part;
        try
        {
            part = processData.GetProcessPartData(process.FullName, PrintLogPartNumber);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException($"There was no Part '{PrintLogPartNumber}' defined on '{process.FullName}'.");
        }
        catch (SystemException)
        {
            throw;
        }
        // retrieve the Process' SerializationMode and use it to create a SerialNumber from the Log
        SerializationMode Mode = process.Serialization;
        SerialNumber serialNumber = new SerialNumber(Mode, part, int.Parse(SplitPrintLog[4]));
        // parse the production date
        DateTime PrintLogProductionDate;
        try
        {
            PrintLogProductionDate = DateTime.ParseExact(SplitPrintLog[^3], "MM/DD/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            throw new ArgumentException($"Could not parse a date from '{SplitPrintLog[^3]}'.");
        }
        // parse initial quantity, shift, and operator values and possible PartialDataSets
        Quantity PrintLogQuantity;
        Shift PrintLogShift;
        Operator PrintLogOperator;
        PartialDataSet? FirstPartialData = null;
        Quantity FirstPartialQuantity;
        Shift FirstPartialShift;
        Operator FirstPartialOperator;
        PartialDataSet? SecondPartialData = null;
        Quantity SecondPartialQuantity;
        Shift SecondPartialShift;
        Operator SecondPartialOperator;
        // colon indicates the existence of a split basket in the PrintLog
        if (SplitPrintLog[3].Contains(':'))
        {
            // split quantity field and save the first, second values
            List<string> SplitQuantity = SplitPrintLog[3].Split(":").ToList();
            try
            {
                PrintLogQuantity = new Quantity(int.Parse(SplitQuantity[0]));
                FirstPartialQuantity = new Quantity(int.Parse(SplitQuantity[1]));
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Could not parse integer Quantities from '{SplitPrintLog[3]}'.");
            }
            // split shift field and save the first, second values
            List<string> SplitShift = SplitPrintLog[^2].Split(":").ToList();
            try
            {
                PrintLogShift = ShiftExtensions.FromString(SplitShift[0]);
                FirstPartialShift = ShiftExtensions.FromString(SplitShift[1]);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Could not parse Shifts from '{SplitPrintLog[^2]}'.");
            }
            // split operator field and save the first, second values
            List<string> SplitOperator = SplitPrintLog[^1].Split(":").ToList();
            try
            {
                PrintLogOperator = new Operator(SplitOperator[0]);
                FirstPartialOperator = new Operator(SplitOperator[1]);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Could not parse string Operators from '{SplitPrintLog[^1]}'.");
            }
            // construct and save the First PartialDataSet object
            FirstPartialData = new PartialDataSet(FirstPartialQuantity, FirstPartialShift, FirstPartialOperator);
            // length of three in the split quantity field indicates two PartialDataSets
            if (SplitQuantity.Count == 3)
            {
                // retrieve the third set of values and construct the Second PartialDataSet object
                try
                {
                    SecondPartialQuantity = new Quantity(int.Parse(SplitQuantity[2]));
                }
                catch (FormatException)
                {
                    throw new ArgumentException($"Could not parse int Quantity from '{SplitQuantity[2]}'.");
                }
                try
                {
                    SecondPartialShift = ShiftExtensions.FromString(SplitShift[2]);
                }
                catch (FormatException)
                {
                    throw new ArgumentException($"Could not parse Shift from '{SplitShift[2]}'.");
                }
                try
                {
                    SecondPartialOperator = new Operator(SplitOperator[2]);
                }
                catch (FormatException)
                {
                    throw new ArgumentException($"Could not parse string Operator from '{SplitOperator[2]}'.");
                }
                SecondPartialData = new PartialDataSet(SecondPartialQuantity, SecondPartialShift, SecondPartialOperator);
            }
        }
        // there is no PartialDataSet information
        else
        {
            // retrieve values directly from the CSV fields
            try
            {
                PrintLogQuantity = new Quantity(int.Parse(SplitPrintLog[3]));
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Could not parse int Quantity from '{SplitPrintLog[3]}'.");
            }
            try
            {
                PrintLogShift = ShiftExtensions.FromString(SplitPrintLog[^2]);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Could not parse Shift from '{SplitPrintLog[^2]}'.");
            }
            try
            {
                PrintLogOperator = new Operator(SplitPrintLog[^1]);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Could not parse string Operator from '{SplitPrintLog[^1]}'.");
            }
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
            try
            {
                printTickets.Add(ParsePrintTicket(PrintLog, process));
            }
            catch
            {
                continue;
            }
        }
        return printTickets;
    }

    /// <summary>
    /// Asynchronously parses the Print History for process and returns a List of PrintTicket objects.
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    public static async Task<List<PrintTicket>> GetPrintTicketsAsync(Process process)
    {
        // read the datatable
        IEnumerable<string> DatabaseSet = await ReadAsync(process);
        // parse all of the logs to PrintTickets and return them
        List<PrintTicket?> printTickets = [];
        printTickets = DatabaseSet
            .Select(x =>
            {
                try
                {
                    return ParsePrintTicket(x, process);
                }
                // sets faulting PrintTicket parses to null
                catch
                {
                    return null;
                }
            })
            .ToList();
        // remove nulls
        printTickets = printTickets
            .Where(x => x is not null)
            .ToList();
        // return empty list if null; else return list with all nulls removed
        if (printTickets is null)
        {
            return [];
        }
        else
        {
            return printTickets!;
        }
    }
}   











