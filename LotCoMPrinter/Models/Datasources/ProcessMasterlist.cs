using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides controlled access to the Process Masterlist data source.
/// </summary>
public static class ProcessMasterlist {
    private const string _path = "\\\\144.133.122.1\\Lot Control Management\\Database\\part_control\\_process_masterlist";

    /// <summary>
    /// Asynchronously loads the data from the Process Masterlist data source.
    /// </summary>
    /// <returns>A JSON dictionary containing the Process Masterlist data.</returns>
    /// <exception cref="JsonException"></exception>
    private static async Task<JObject> LoadDataAsync() {
        // read the masterlist file
        JObject Masterlist = JObject.Parse(await File.ReadAllTextAsync(_path));
        return Masterlist;
    }

    /// <summary>
    /// Synchronously retrieves a list of Process Full Names ("Code-Title").
    /// </summary>
    /// <returns></returns>
    public static List<string> GetProcessNames() {
        // load the data from the Masterlist
        JObject FullData = LoadDataAsync().Result;
        // create a List of all Process Names
        List<string> Processes = [];
        foreach(JToken _process in FullData["Processes"]!) {
            Processes.Add(_process["FullName"]!.ToString());
        }
        return Processes;
    }

    /// <summary>
    /// Loads and queries the Process Masterlist data for data connected to ProcessFullName.
    /// </summary>
    /// <param name="ProcessFullName">The FULL name of a Process ("Code-Title") to query for.</param>
    /// <returns>A JToken object containing the Process' data.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<JToken> GetProcessData(string ProcessFullName) {
        // load the data from the Masterlist
        JObject FullData = await LoadDataAsync();
        // attempt to access the data for the passed Process
        JToken? SelectedData = FullData["Processes"]!.Where(x => x["FullName"]!.Equals(ProcessFullName)).First();
        // check for a result and return
        if (SelectedData.Equals(null)) {
            throw new ArgumentException($"Could not retrieve data for process '{ProcessFullName}'.");
        } else {
            return SelectedData;
        }
    } 
}