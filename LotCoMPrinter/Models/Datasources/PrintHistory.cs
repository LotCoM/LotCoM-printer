using System.IO;
using System.Collections.Generic;

namespace LotCoMPrinter.Models.Datasources;

public static class PrintHistory
{

    // I want to direct and read correct print process file and save it to a list, ienumberable, or array
    private static readonly string PrintFolder = "\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints";

    public static bool PrintHistoryExists(Process process)
    {
        string TablePath = "";
        List<string> DatabaseSet;

        TablePath = $"{PrintFolder}\\{Process.FullName}";
        DatabaseSet = File.ReadAllLines(TablePath);

    }
    // I want to parse the log in the list and save them as objects to pass to the print job

}   











