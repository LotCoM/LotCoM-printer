using System.IO;
using System.Collections.Generic;
using LotCom.Types;
using LotCoMPrinter.models.Exceptions;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Datasources;

public static class PrintHistory
{

    // I want to direct and read correct print process file and save it to a list, ienumberable, or array
    private static readonly string PrintFolder = "\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints";

    public static IEnumerable<string> Read(Process process)
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
    // I want to parse the log in the list and save them as objects to pass to the print job

}   











