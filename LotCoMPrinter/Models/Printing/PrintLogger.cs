namespace LotCoMPrinter.Models.Printing;

public static class PrintLogger {
    // print log path
    private const string _logPath = "\\\\144.133.122.1\\Lot Control Management\\database\\logs\\print_history.log";

    /// <summary>
    /// Converts LabelInformation into a Print Event string, including the print timestamp.
    /// </summary>
    /// <param name="LabelInformation">LabelInformation from the LabelPrintJob.</param>
    /// <returns></returns>
    private static async Task<string> CreateEventString(List<string> LabelInformation) {
        // use multi-threading to avoid blocking the UI while print logging occurs
        string PrintEvent = await Task.Run(() => {
            // get the timestamp of the print job
            string Formatted = LabelInformation[^3].Replace("Production Date: ", "");
            // format the Label Information as a readable string
            foreach(string _field in LabelInformation) {
                // remove any commas, replace the newline in Part with a comma, then remove all spaces
                string _formattedField = _field.Replace(",", "").Replace(" ", "").Replace("\n", ",");
                // remove the field prefixes and add the field value to the formatted string
                _formattedField = _formattedField.Split(":")[1].Replace(" ", "");
                Formatted += $",{_formattedField}";
            }
            return Formatted;
        });
        return $"{PrintEvent}\n";
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
        await File.AppendAllTextAsync(_logPath, PrintEvent);
    }
}