using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Datatypes;
using LotCom.Database;
using LotCom.Enums;
using LotCom.Extensions;
using LotCom.Types;
using System.Globalization;

namespace LotCoMPrinter.Models.Datasources;

public static class PrintHistory
{

    // I want to direct and read correct print process file and save it to a list, ienumberable, or array
    private static readonly string PrintFolder = "\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints";

    private static IEnumerable<string> Read(Process process)
    {
        string TablePath = "";
        IEnumerable<string> DatabaseSet;
        try
        {
            TablePath = $"{PrintFolder}\\{process.FullName}";
            DatabaseSet = File.ReadAllLines(TablePath);

            return DatabaseSet;
        }
        catch (FileNotFoundException)
        {
            throw new ProcessNameException($"Could not find a table for the Process '{process}'.");
        }
        catch (SystemException _ex)
        {
            throw new DatabaseException
            (
                Message: $"Failed to open the file at '{TablePath}' due to the following exception:\n{_ex.Message}.",
                InnerException: _ex
            );
        }
    }

    private static PrintTicket ParsePrintTicket(string PrintLog, Process process)
    {
        List<string> SplitPrintLog = PrintLog.Split(",").ToList();
        string PrintLogPartNumber = SplitPrintLog[1];
        ProcessData processData = new ProcessData();
        Part part = processData.GetProcessPartData(process.FullName, PrintLogPartNumber);

        SerializationMode Mode = process.Serialization;

        SerialNumber serialNumber = new SerialNumber(Mode, part, int.Parse(SplitPrintLog[4]));

        DateTime PrintLogProductionDate = DateTime.ParseExact(SplitPrintLog[^3], "MM/DD/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);

        Quantity PrintLogQuanity;

        Shift PrintLogShift;

        Operator PrintLogOperator;

        PartialDataSet? FirstPartialData = null;
        PartialDataSet? SecondPartialData = null;
        if (SplitPrintLog[3].Contains(":"))
        {
            List<string> SplitQuantity = SplitPrintLog[3].Split(":").ToList();
            PrintLogQuanity = new Quantity(int.Parse(SplitQuantity[0]));
            Quantity FirstPartialQuantity = new Quantity(int.Parse(SplitQuantity[1]));
            List<string> SplitShift = SplitPrintLog[^2].Split(":").ToList();
            PrintLogShift = ShiftExtensions.FromString(SplitShift[0]);
            Shift FirstPartialShift = ShiftExtensions.FromString(SplitShift[1]);
            List<string> SplitOperator = SplitPrintLog[^1].Split(":").ToList();
            PrintLogOperator = new Operator(SplitOperator[0]);
            Operator FirstPartialOperator = new Operator(SplitOperator[1]);

            FirstPartialData = new PartialDataSet(FirstPartialQuantity, FirstPartialShift, FirstPartialOperator);

            if (SplitQuantity.Count == 3)
            {
                Quantity SecondPartialQuantity = new Quantity(int.Parse(SplitQuantity[2]));
                Shift SecondPartialShift = ShiftExtensions.FromString(SplitShift[2]);
                Operator SecondPartialOperator = new Operator(SplitOperator[2]);

                SecondPartialData = new PartialDataSet(SecondPartialQuantity, SecondPartialShift, SecondPartialOperator);
            }

        }
        else
        {
            PrintLogQuanity = new Quantity(int.Parse(SplitPrintLog[3]));
            PrintLogShift = ShiftExtensions.FromString(SplitPrintLog[^2]);
            PrintLogOperator = new Operator(SplitPrintLog[^1]);
        }

        // Enumerationg over the required fields 
            VariableFieldSet PrintLogVariableFieldSet = new VariableFieldSet();
        RequiredFields PrintLogRequiredFields = process.RequiredFields;
        int NextValue = 0;
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

        return new PrintTicket(process, part, Mode, serialNumber, PrintLogProductionDate, PrintLogShift, PrintLogQuanity, PrintLogOperator, PrintLogVariableFieldSet, FirstPartialData, SecondPartialData);
    }

    // I want to parse the log in the list and save them as objects to pass to the print job
    // 4165-CIV-Pivot-Housing-MC,00-T20-532AP-A000-YB1,HOUSING PIVOT*,100:164,008,444,03/10/2025-10:07:43,1:2,ZZZ:XXX
    public static List<PrintTicket> GetPrintTickets(Process process)
    {
        List<PrintTicket> printTickets = [];
        IEnumerable<string> DatabaseSet = Read(process);
        foreach (string PrintLog in DatabaseSet)
        {
            printTickets.Add(ParsePrintTicket(PrintLog, process));
        }
        return printTickets;
    }
}   











