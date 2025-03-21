using System.Reflection;
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
            // get the timestamp of the print job
            string Formatted = $"{new Timestamp(Capture.ProductionDate).Stamp}";
            // format the Label Information as a readable string
            foreach (PropertyInfo _property in Capture.GetType().GetProperties()) {
                // save the property name
                string _name = _property.Name;
                // retrieve the Process Name from the SelectedProcess
                if (_name.Equals("SelectedProcess")) {
                    Formatted += $",{Capture.SelectedProcess!.FullName}";
                // retrieve the Part Number from SelectedPart
                } else if (_name.Equals("SelectedPart")) {
                    Formatted += $",{Capture.SelectedPart!.PartNumber}";
                    Formatted += $",{Capture.SelectedPart!.PartName}";
                // just add the value of the property on the Capture object
                } else {
                    Formatted += $",{(string)_property.GetValue(Capture)!}";
                }
            }
            return Formatted;
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