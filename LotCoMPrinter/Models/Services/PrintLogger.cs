using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Datatypes;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Services;

public static class PrintLogger 
{
    private const string PrintDatabase = "\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints";

    /// <summary>
    /// Generates a Log message from Job.
    /// </summary>
    /// <param name="Job"></param>
    /// <returns></returns>
    private static string GenerateLog(LabelPrintJob Job)
    {
        // add initial universal requirements
        string Log = $"{Job.Ticket.Process.FullName},{Job.Ticket.Part.PartNumber},{Job.Ticket.Part.PartName},{Job.Ticket.GetCombinedQuantities()}";
        // add variable fields when required
        RequiredFields Requirements = Job.Ticket.Process.RequiredFields;
        if (Requirements.JBKNumber) 
        {
            Log = $"{Log},{Job.Ticket.VariableFields.JBKNumber!.Formatted}";
        }
        if (Requirements.LotNumber) 
        {
            Log = $"{Log},{Job.Ticket.VariableFields.LotNumber!.Formatted}";
        }
        if (Requirements.DeburrJBKNumber) 
        {
            Log = $"{Log},{Job.Ticket.VariableFields.DeburrJBKNumber!.Formatted}";
        }
        if (Requirements.DieNumber) 
        {
            Log = $"{Log},{Job.Ticket.VariableFields.DieNumber!.Literal}";
        }
        if (Requirements.ModelNumber) 
        {
            Log = $"{Log},{Job.Ticket.VariableFields.ModelNumber!.Code}";
        }
        if (Requirements.HeatNumber) 
        {
            Log = $"{Log},{Job.Ticket.VariableFields.HeatNumber!.Literal}";
        }
        // add remaining universal requirements
        Log = $"{Log},{new Timestamp(Job.Ticket.ProductionDate).Stamp},{Job.Ticket.GetCombinedShifts()},{Job.Ticket.GetCombinedOperators()}";
        return Log;
    }

    /// <summary>
    /// Logs a LabelPrintJob's information to the database.
    /// </summary>
    /// <param name="Job">An LabelPrintJob object.</param>
    /// <returns></returns>
    public static async Task LogPrintEvent(LabelPrintJob Job)
    {
        // create a Log string from the LabelPrintJob info and append the string to the appropriate Log file
        string PrintEvent = GenerateLog(Job);
        string DatatablePath = $"{PrintDatabase}\\{Job.Ticket.Process.FullName}.txt";
        try
        {
            await File.AppendAllTextAsync(DatatablePath, $"{PrintEvent}\n");
        }
        catch (Exception _ex)
        {
            // log the print to the dump database table (backup)
            await File.AppendAllTextAsync($"{PrintDatabase}\\_failed_logs.log", $"{PrintEvent}\n");
            throw new PrintLogException(_ex.Message);
        }
    }
}