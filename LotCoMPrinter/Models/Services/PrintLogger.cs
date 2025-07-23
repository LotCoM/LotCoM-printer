using LotCom.Types;
using LotComPrinter.Models.Datatypes;
using LotComPrinter.Models.Exceptions;

namespace LotComPrinter.Models.Services;

public static class PrintLogger 
{
    /// <summary>
    /// Directory that contains the Print Log Tables for each Process. 
    /// </summary>
    private const string PrintDatabase = "\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints";

    /// <summary>
    /// Generates a Log message from Job that has generated a full BasketLabel.
    /// </summary>
    /// <param name="Job"></param>
    /// <returns></returns>
    private static string GenerateLog(PrintJob Job)
    {
        // add initial universal requirements
        PrintTicket JobSource = (PrintTicket)Job.Source;
        string Log = $"{JobSource.Process.FullName},{JobSource.Part.PartNumber},{JobSource.Part.PartName},{JobSource.GetCombinedQuantities()}";
        // add variable fields when required
        RequiredFields Requirements = JobSource.Process.RequiredFields;
        if (Requirements.JBKNumber)
        {
            Log = $"{Log},{JobSource.VariableFields.JBKNumber!.Formatted}";
        }
        if (Requirements.LotNumber)
        {
            Log = $"{Log},{JobSource.VariableFields.LotNumber!.Formatted}";
        }
        if (Requirements.DeburrJBKNumber)
        {
            Log = $"{Log},{JobSource.VariableFields.DeburrJBKNumber!.Formatted}";
        }
        if (Requirements.DieNumber)
        {
            Log = $"{Log},{JobSource.VariableFields.DieNumber!.Literal}";
        }
        if (Requirements.ModelNumber)
        {
            Log = $"{Log},{JobSource.VariableFields.ModelNumber!.Code}";
        }
        if (Requirements.HeatNumber)
        {
            Log = $"{Log},{JobSource.VariableFields.HeatNumber!.Literal}";
        }
        // add remaining universal requirements
        Log = $"{Log},{new Timestamp(JobSource.ProductionDate).Stamp},{JobSource.GetCombinedShifts()},{JobSource.GetCombinedOperators()}";
        return Log;
    }

    /// <summary>
    /// Logs a LabelPrintJob's information to the database.
    /// </summary>
    /// <param name="Job">An LabelPrintJob object.</param>
    /// <returns></returns>
    public static async Task LogPrintEvent(PrintJob Job)
    {
        // create a Log string from the PrintJob info and append the string to the appropriate Log file
        string PrintEvent = GenerateLog(Job);
        PrintTicket JobSource = (PrintTicket)Job.Source;
        string DatatablePath = $"{PrintDatabase}\\{JobSource.Process.FullName}.txt";
        try
        {
            await File.AppendAllTextAsync(DatatablePath, $"{PrintEvent}\n");
        }
        catch (Exception _ex)
        {
            // log the print to the dump database table (backup)
            await File.AppendAllTextAsync($"{PrintDatabase}\\failed_prints.log", $"{PrintEvent}\n");
            throw new PrintLogException(_ex.Message);
        }
    }
}