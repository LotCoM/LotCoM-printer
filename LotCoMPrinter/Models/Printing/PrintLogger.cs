using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Validators;

namespace LotCoMPrinter.Models.Printing;

public static class PrintLogger {
    // print log path
    private const string _printDatabase = "\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints";

    /// <summary>
    /// Logs a LabelPrintJob's information to the database.
    /// </summary>
    /// <param name="Capture">An InterfaceCapture object.</param>
    /// <returns></returns>
    public static async Task LogPrintEvent(InterfaceCapture Capture) {
        // create a print event string from the Label Information
        string PrintEvent = Capture.FormatAsCSV();
        // try to open and append the print event to the print datatable for the Selected Process
        string DatatablePath = $"{_printDatabase}\\{Capture.SelectedProcess.FullName}.txt";
        try {
            await File.AppendAllTextAsync(DatatablePath, $"{PrintEvent}\n");
        // there was an error opening and writing the print event to the appropriate table
        } catch (Exception _ex) {
            // log the print to the bulk dump database table
            await File.AppendAllTextAsync($"{_printDatabase}\\_failed_logs.log", $"{PrintEvent}\n");
            throw new PrintLogException(_ex.Message);
        }
        
    }
}