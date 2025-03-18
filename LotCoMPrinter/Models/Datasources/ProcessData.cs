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
    /// Retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns></returns>
    /// <exception cref="FileLoadException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<JToken> GetProcessParts(string ProcessFullName) {
        // load the Process' data
        JToken ProcessData = await ProcessMasterlist.GetProcessData(ProcessFullName);
        // pull the Part data
        JToken PartData = ProcessData["Parts"]!;
        // no Part data was read
        if (PartData.Equals(null)) {
            throw new ArgumentException($"No Part data found for the Process '{ProcessFullName}'.");
        } 
        // return the Part data
        return PartData;
    }
}