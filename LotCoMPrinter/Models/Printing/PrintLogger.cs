namespace LotCoMPrinter.Models.Printing;

public static class PrintLogger {
    // print log path
    private const string _logPath = "\\\\Lot Control Management\\database\\logs\\print_history.log";

    /// <summary>
    /// Converts LabelInformation into a Print Event string, including the print timestamp.
    /// </summary>
    /// <param name="LabelInformation">LabelInformation from the LabelPrintJob.</param>
    /// <returns></returns>
    private static async Task<string> CreateEventString(List<string> LabelInformation) {
        // use multi-threading to avoid blocking the UI while print logging occurs
        string PrintEvent = await Task.Run(() => {
            // get the timestamp of the print event
            string Timestamp = DateTime.Now.ToLongDateString();
            string Formatted = Timestamp;
            // format the Label Information as a readable string
            foreach(string _field in LabelInformation) {
                Formatted += $", {_field}";
            }
            return Formatted;
        });
        return PrintEvent;
    }

    /// <summary>
    /// Logs a LabelPrintJob's information to the database.
    /// </summary>
    /// <param name="LabelInformation">LabelInformation from the LabelPrintJob.</param>
    /// <returns></returns>
    public static async Task LogPrintEvent(List<string> LabelInformation) {
        // create a print event from the Label Information
        string PrintEvent = await CreateEventString(LabelInformation);
        // open the print history .log file and append the new event to the file
        await File.WriteAllTextAsync(_logPath, PrintEvent);
    }
}