using LotComPrinter.Models.Datatypes;
using LotCom.Database;
using LotCom.Enums;
using LotCom.Exceptions;
using LotCom.Extensions;
using LotCom.Types;
using System.Globalization;

namespace LotComPrinter.Models.Datasources;

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
        string TablePath = $"{PrintFolder}\\{process.FullName}.txt";
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
    private static async Task<IEnumerable<string>> ReadAsync(Process process)
    {
        // create Table path and Database set variables
        string TablePath = $"{PrintFolder}\\{process.FullName}.txt";
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
            PrintLogProductionDate = DateTime.ParseExact(SplitPrintLog[^3], "MM/dd/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            throw new ArgumentException($"Could not parse a date from '{SplitPrintLog[^3]}'.");
        }
        // parse initial quantity, shift, and operator values and possible PartialDataSets
        List<PartialDataSet?> Partials;
        try
        {
            Partials = PartialDataSet.Parse(SplitPrintLog[3], SplitPrintLog[^2], SplitPrintLog[^1])!;
        }
        catch (ArgumentException)
        {
            throw;
        }
        while (Partials.Count < 3)
        {
            Partials.Add(null);
        }
        // enumerate over the required fields and parse them from the CSV fields 
            VariableFieldSet PrintLogVariableFieldSet;
        try
        {
            PrintLogVariableFieldSet = VariableFieldSet.ParseCSV(SplitPrintLog[4..^3].ToArray(), process.RequiredFields);
        }
        catch (ArgumentException)
        {
            throw;
        }
        // construct and return a new PrintTicket object
        if (Partials[0] is null)
        {
            throw new ArgumentException("No initial Quantity, Shift, and Operator information.");
        }
        return new PrintTicket(process, part, Mode, serialNumber, PrintLogProductionDate, Partials[0]!.Shift, Partials[0]!.Quantity, Partials[0]!.Operator, PrintLogVariableFieldSet, Partials[1], Partials[2]);
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

    /// <summary>
    /// Retrieves all PrintTickets printed by Process on Date.
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="Process"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static List<PrintTicket> GetPrintTicketsForDate(DateTime Date, Process Process)
    {
        // read the datatable
        IEnumerable<string> DatabaseSet = Read(Process);
        // parse all of the logs from the passed date and return them
        List<PrintTicket> PrintTickets = [];
        foreach (string PrintLog in DatabaseSet)
        {
            // parse the production date
            DateTime LogDate;
            try
            {
                LogDate = DateTime.ParseExact(PrintLog.Split(',')[^3], "MM/dd/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Could not parse a date from '{PrintLog.Split()[^3]}'.");
            }
            // confirm that the LogDate year hasn't already passed
            if (LogDate.Year > Date.Year)
            {
                // time is out of range by Year; stop process
                return PrintTickets;
            }
            // LogDate year is either equal to or below requested Date
            else if (LogDate.Year < Date.Year)
            {
                // skip Log; too early
                continue;
            }
            // LogDate year is equal to requested Date year; repeat for Month
            // confirm that the LogDate month hasn't already passed
            if (LogDate.Month > Date.Month)
            {
                // time is out of range by Month; stop process
                return PrintTickets;
            }
            // LogDate month is either equal to or below requested Date
            else if (LogDate.Month < Date.Month)
            {
                // skip Log; too early
                continue;
            }
            // LogDate month is equal to requested Date month; repeat for Day
            // confirm that the LogDate day hasn't already passed
            if (LogDate.Day > Date.Day)
            {
                // time is out of range by Day; stop process
                return PrintTickets;
            }
            // LogDate day is either equal to or below requested Date
            else if (LogDate.Day < Date.Day)
            {
                // skip Log; too early
                continue;
            }
            // Dates are the same
            try
            {
                PrintTickets.Add(ParsePrintTicket(PrintLog, Process));
            }
            catch
            {
                continue;
            }
        }
        return PrintTickets;
    }
}   











