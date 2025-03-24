using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Validators;

namespace LotCoMPrinter.Models.Printing;

public static class PrintLogger {
    // print log path
    private const string _logPath = "\\\\144.133.122.1\\Lot Control Management\\Database\\logs\\print_history.log";

    /// <summary>
    /// Converts LabelInformation into a Print Event string, including the print timestamp.
    /// </summary>
    /// <param name="Capture">An InterfaceCapture object.</param>
    /// <returns></returns>
    private static async Task<string> CreateEventString(InterfaceCapture Capture) {
        // use multi-threading to avoid blocking the UI while print logging occurs
        string PrintEvent = await Task.Run(() => {
            // retrieve the Process' requirements
            List<string> Requirements = Capture.SelectedProcess.RequiredFields;
            // add universal requirements (front)
            string Log = $"{Capture.SelectedProcess.FullName},{Capture.SelectedPart.PartNumber},{Capture.SelectedPart.PartName},{Capture.Quantity}";
            // add internal, variable fields
            if (Requirements.Contains("JBKNumber")) {
                Log = $"{Log},{Capture.JBKNumber}";
            }
            if (Requirements.Contains("LotNumber")) {
                Log = $"{Log},{Capture.LotNumber}";
            }
            if (Requirements.Contains("DeburrJBKNumber")) {
                Log = $"{Log},{Capture.DeburrJBKNumber}";
            }
            if (Requirements.Contains("DieNumber")) {
                Log = $"{Log},{Capture.DieNumber}";
            }
            if (Requirements.Contains("HeatNumber")) {
                Log = $"{Log},{Capture.HeatNumber}";
            }
            if (Requirements.Contains("ModelNumber")) {
                Log = $"{Log},{Capture.ModelNumber}";
            }
            // add universal requirements (back)
            Log = $"{Log},{new Timestamp(Capture.ProductionDate).Stamp},{Capture.ProductionShift},{Capture.OperatorID}";
            return Log;
        });
        return $"{PrintEvent}\n";
    }

    /// <summary>
    /// Logs a LabelPrintJob's information to the database.
    /// </summary>
    /// <param name="Capture">An InterfaceCapture object.</param>
    /// <returns></returns>
    public static async Task LogPrintEvent(InterfaceCapture Capture) {
        // create a print event from the Label Information
        string PrintEvent = await CreateEventString(Capture);
        // open the print history .log file and append the new event to the file
        await File.AppendAllTextAsync(_logPath, PrintEvent);
    }
}