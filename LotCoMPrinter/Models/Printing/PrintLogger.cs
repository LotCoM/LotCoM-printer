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
        try {
            string DatatablePath = $"{_printDatabase}\\{Capture.SelectedProcess.FullName}.txt";
            await File.AppendAllTextAsync(DatatablePath, $"{PrintEvent}\n");
        } catch {

        }
        
    }
}