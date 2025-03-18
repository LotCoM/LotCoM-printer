using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides controlled access to Process data sources.
/// </summary>
public static class ProcessData {
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
    /// Retrieves ProcessFullName's data utilizing the Process Masterlist data source.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve data for.</param>
    /// <returns></returns>
    public static async Task<JToken> GetProcessData(string ProcessFullName) {
        // invoke the Masterlist method to retrieve the Process' data
        return await ProcessMasterlist.GetProcessData(ProcessFullName);
    }

    /// <summary>
    /// Retrieves a list of Process Names utilizing the Process Masterlist data source.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetProcessNames() {
        // invoke the Masterlist method to retrieve the Process list
        return ProcessMasterlist.GetProcessNames();
    }

    /// <summary>
    /// Provides ProcessData controlled access to the Process Masterlist data source.
    /// </summary>
    private static class ProcessMasterlist {
        private const string _path = "\\\\144.133.122.1\\Lot Control Management\\Database\\process_control\\_process_masterlist.json";

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
        /// Synchronously loads the data from the Process Masterlist data source.
        /// </summary>
        /// <returns>A JSON dictionary containing the Process Masterlist data.</returns>
        /// <exception cref="JsonException"></exception>
        private static JObject LoadData() {
            // read the masterlist file
            JObject Masterlist = JObject.Parse(File.ReadAllText(_path));
            return Masterlist;
        }

        /// <summary>
        /// Synchronously retrieves a list of Process Full Names ("Code-Title").
        /// </summary>
        /// <returns></returns>
        public static List<string> GetProcessNames() {
            // load the data from the Masterlist
            JObject FullData = LoadData();
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
            JToken SelectedData;
            try {
                SelectedData = FullData["Processes"]!.Where(x => x["FullName"]!.ToString() == ProcessFullName).First();
            // no processes matched the name
            } catch {
                throw new ArgumentException($"Could not resolve process '{ProcessFullName}'.");
            }
            return SelectedData;
        } 
    }
}