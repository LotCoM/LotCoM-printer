using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides controlled access to Process data sources.
/// </summary>
public static class ProcessData {
    /// <summary>
    /// Retrieves ProcessFullName's serialization status.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to check.</param>
    /// <returns>"Originator" || "Pass-through".</returns>
    public static async Task<string> IsOriginator(string ProcessFullName) {
        // load the Process' data
        Process Data = await ProcessMasterlist.GetIndividualProcess(ProcessFullName);
        // check whether the process is an originator or not
        return Data.Type;
    }

    /// <summary>
    /// Retrieves ProcessFullName's data utilizing the Process Masterlist data source.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve data for.</param>
    /// <returns>A Process object.</returns>
    public static async Task<Process> GetIndividualProcessData(string ProcessFullName) {
        // invoke the Masterlist method to retrieve the Process' data
        return await ProcessMasterlist.GetIndividualProcess(ProcessFullName);
    }

    /// <summary>
    /// Retrieves the full list of Processes utilizing the Process Masterlist data source.
    /// </summary>
    /// <returns>A JToken object containing the full list of Processes.</returns>
    public static List<Process> GetProcesses() {
        // invoke the Masterlist method to retrieve the Processes
        return ProcessMasterlist.GetAllProcesses();
    }

    /// <summary>
    /// Retrieves a list of Process Names utilizing the Process Masterlist data source.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetProcessNames() {
        // invoke the Masterlist method to retrieve the Process list
        return ProcessMasterlist.GetAllProcessNames();
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
        /// Attempts to resolve a Part object from the data in Token.
        /// </summary>
        /// <param name="Token">A JToken object containing Part data.</param>
        /// <param name="ParentProcess">The known Process that the Part should belong to.</param>
        /// <returns>A Part object with data resolved from the JToken.</returns>
        /// <exception cref="FormatException"></exception>
        private static Part ResolvePartFromToken(JToken Token, string ParentProcess) {
            // hold variables for each Part object property
            string Number;
            string Name;
            string Model;
            // attempt to pull the needed fields from the passed JToken
            try {
                Number = Token["Number"]!.ToString();
                Name = Token["Name"]!.ToString();
                Model = Token["Model"]!.ToString();
            // one of the needed fields was not accessible
            } catch {
                throw new FormatException($"Could not resolve '{Token}' to a Part object.");
            }
            // attempt to construct the Part object from the resolved data
            Part ResolvedPart;
            try {
                ResolvedPart = new Part(ParentProcess, Number, Name, Model);
            } catch {
                throw new FormatException($"Could not resolve '{Token}' to a Part object.");
            }
            // return the resolved Part object
            return ResolvedPart;
        }

        /// <summary>
        /// Attempts to resolve a Process object from the data in Token.
        /// </summary>
        /// <param name="Token">A JToken object containing Part data.</param>
        /// <returns>A Process object with data resolved from the JToken.</returns>
        /// <exception cref="FormatException"></exception>
        private static Process ResolveProcessFromToken(JToken Token) {
            // hold variables for each Process object property
            string Code;
            string Title;
            string Type;
            string Serialization;
            JToken Parts;
            // attempt to access each field of Data from the Process Token
            try {
                Code = Token["Code"]!.ToString();
                Title = Token["Title"]!.ToString();
                Type = Token["Type"]!.ToString();
                Serialization = Token["Serialization"]!.ToString();
                Parts = Token["Parts"]!;
            // one of the needed fields was not accessible
            } catch {
                throw new FormatException($"Could not resolve '{Token}' to a Process object.");
            }
            // process and add each part to the parts list individually
            List<Part> PartObjects = [];
            try {
                // resolve a Part object from each Token
                foreach (JToken _part in Parts) {
                    PartObjects.Add(ResolvePartFromToken(_part, $"{Code}-{Title}"));
                }
            // one of the Tokens could not be resolved to a Part
            } catch (Exception _ex) {
                throw new FormatException($"Could not resolve '{Token}' to a Process object, due to the following Part resolution failure: {_ex.Message}");
            }
            // attempt to construct the Part object from the resolved data
            Process ResolvedProcess;
            try {
                ResolvedProcess = new Process(Code, Title, Type, Serialization, PartObjects);
            } catch {
                throw new FormatException($"Could not resolve '{Token}' to a Process object.");
            }
            // return the resolved Process object
            return ResolvedProcess;
        }

        /// <summary>
        /// Retrieves the list of Processes, as Process objects, from the Process Masterlist.
        /// </summary>
        /// <returns>A list of Process objects.</returns>
        /// <exception cref="FileLoadException"></exception>
        public static List<Process> GetAllProcesses() {
            // load the data from the Masterlist
            JObject FullData = LoadData();
            // return the list of Processes in the Masterlist
            if (FullData["Processes"]!.Equals(null)) {
                throw new FileLoadException("Failed to load the Processes from the Process Masterlist data source.");
            }
            // convert the Process tokens into Process objects
            List<Process> ProcessObjects = [];
            foreach (JToken _process in FullData["Processes"]!) {
                // resolve the Token to a Process object
                try {
                    ProcessObjects.Add(ResolveProcessFromToken(_process));
                // there was a problem resolving the Process
                } catch (Exception _ex) {
                    throw new FormatException($"Failed to load Processes due to the following exception: {_ex.Message}.");
                }
            }
            return ProcessObjects;
        }

        /// <summary>
        /// Synchronously retrieves a list of Process Full Names ("Code-Title").
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllProcessNames() {
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
        /// Returns a Process object constructed from the first found match.
        /// </summary>
        /// <param name="ProcessFullName">The FULL name of a Process ("Code-Title") to query for.</param>
        /// <returns>A Process object.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<Process> GetIndividualProcess(string ProcessFullName) {
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
            // resolve the Token to a Process
            Process ResolvedProcess;
            try {
                ResolvedProcess = ResolveProcessFromToken(SelectedData);
            // the Token could not be resolved to a Process
            } catch {
                throw new FormatException($"Could not resolve '{SelectedData}' to a Process object.");
            }
            // return the resolved Process object
            return ResolvedProcess;
        } 
    }
}