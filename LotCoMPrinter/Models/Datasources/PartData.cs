using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides controlled access to Part Data from the Process Masterlist data source.
/// </summary>
public static class PartData {
    /// <summary>
    /// Retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns>A JToken object containing the Parts assigned to the Process.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<JToken> GetProcessParts(string ProcessFullName) {
        // load the Process' data
        JToken Data = await ProcessData.GetProcessData(ProcessFullName);
        // pull the Part data
        JToken PartData = Data["Parts"]!;
        // no Part data was read
        if (PartData.Equals(null)) {
            throw new ArgumentException($"No Part data found for the Process '{ProcessFullName}'.");
        } 
        // return the Part data
        return PartData;
    }

    /// <summary>
    /// Retrieves and formats ProcessFullName's Part list as a list of Displayable strings.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns>A List of strings.</returns>
    /// <exception cref="FormatException"></exception>
    public static async Task<List<string>> GetDisplayableProcessParts(string ProcessFullName) {
        // retrieve the Process' parts
        JToken ProcessParts = await GetProcessParts(ProcessFullName);
        // convert each Part Token into a Displayable string
        List<string> PartStrings = [];
        try {
            foreach (JToken _part in ProcessParts) {
                PartStrings.Add(GetPartAsDisplayable(_part));
            }
        // one of the Part Tokens failed to convert to a Displayable string
        } catch {
            throw new FormatException("Failed to convert Parts to strings.");
        }
        // return the converted list
        return PartStrings;
    }

    /// <summary>
    /// Queries for a Part matching PartNumber in ProcessFullName's Part data.
    /// </summary>
    /// <param name="ProcessFullName">The FULL Name ("Code-Title") of the Process to query from.</param>
    /// <param name="PartNumber">The Part Number to query for within ProcessFullName's data.</param>
    /// <returns>A JToken object containing the Part data for PartNumber.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    public static async Task<JToken> GetPartData(string ProcessFullName, string PartNumber) {
        // perform the query on a new CPU thread
        JToken PartData = await Task.Run(async () => {
            // retrieve the Process' Part list
            JToken ProcessParts = await GetProcessParts(ProcessFullName);
            // no Part data for this Process
            if (!ProcessParts.Any()) {
                throw new ArgumentException($"No Part data has been assigned to Process '{ProcessFullName}'.");
            }
            // attempt to access the specific Part
            JToken? Data;
            try {
                Data = ProcessParts.Where(x => x["Number"]!.ToString().Equals(PartNumber)).First();
            // Part was not found in the Process' Part list
            } catch {
                throw new ArgumentException($"Part '{PartNumber}' not found assigned to Process '{ProcessFullName}'.");
            }
            // verify the Part's data is not null and return it
            if (Data.Equals(null)) {
                throw new FormatException($"The Part '{PartNumber}' has no data assigned.");
            }
            return Data;
        });
        // return the queried Part data
        return PartData;
    }

    /// <summary>
    /// Formats PartToken into a two-line, newline-split Part String, which can be displayed.
    /// </summary>
    /// <param name="PartToken">A JToken object pulled from a Process' data in ProcessMasterlist.</param>
    public static string GetPartAsDisplayable(JToken PartToken) {
        // format the passed Part as a displayable string
        string PartString;
        try {
            PartString = $"{PartToken["Number"]}\n{PartToken["Name"]}";
        // there was some problem accessing the Number and Name for this Part
        } catch {
            throw new ArgumentException($"Could not access a Number and/or Name from Part '{PartToken}'.");
        }
        return PartString;
    }

    /// <summary>
    /// Retrieves the Model Number assigned to PartToken.
    /// </summary>
    /// <param name="PartToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetPartModel(JToken PartToken) {
        // access the Model Number assigned to the Part Token
        string? ModelNumber;
        try {
            ModelNumber = PartToken["Model"]!.ToString();
        // no Model field accessible
        } catch {
            ModelNumber = null;
        }
        // confirm a Model Number was found by one of the two methods
        if (ModelNumber!.Equals(null)) {
            throw new ArgumentException($"No Model Number could be retrieved from Part '{PartToken}'.");
        }
        return ModelNumber;
    }
}