using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides controlled access to Process data sources.
/// </summary>
public static class ProcessData {
    /// <summary>
    /// Formats the Part passed into a two-line, newline-split Part String, which can be displayed.
    /// </summary>
    /// <param name="PartInformation">A key/value pair pulled from a part number dictionary.</param>
    public static string GetPartAsDisplayable(KeyValuePair<string, string> PartInformation) {
        // format the passed part as a displayable string
        string PartString = PartInformation.Key + "\n" + PartInformation.Value;
        return PartString;
    }

    /// <summary>
    /// Checks if a Process is an originator (a process that creates new parts and requires serialization).
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to check.</param>
    /// <returns>bool</returns>
    public static async Task<bool> IsOriginator(string ProcessFullName) {
        // load the Process' data
        JToken ProcessData = await ProcessMasterlist.GetProcessData(ProcessFullName);
        // check whether the process is an originator or not
        return ProcessData["Type"]!.Equals("Originator");
    }

    /// <summary>
    /// Retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns></returns>
    /// <exception cref="FileLoadException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<Dictionary<string, string>> GetProcessPartData(string ProcessFullName) {
        // load the Process' data
        JToken ProcessData = await ProcessMasterlist.GetProcessData(ProcessFullName);
        // pull the Part data
        JToken PartData = ProcessData["Parts"]!;
        Dictionary<string, string> PartDictionary = [];
        // no Part data was read
        if (PartData.Equals(null)) {
            throw new ArgumentException($"No Part data found for the Process '{ProcessFullName}'.");
        // convert the Part data into a dictionary of "Number: Name" format
        } else {
            // run conversion on a new CPU thread
            PartDictionary = await Task.Run(() => {
                Dictionary<string, string> Dict = [];
                // pull the number and name and add as a new KeyValuePair
                foreach (JToken _part in PartData) {
                    Dict.Add(_part["Number"]!.ToString(), _part["Name"]!.ToString());
                }
                return Dict;
            });
        }
        // return the Part data
        return PartDictionary;
    }
}