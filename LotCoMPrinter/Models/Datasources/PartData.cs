namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides controlled access to Part Data from the Process Masterlist data source.
/// </summary>
public static class PartData {
    /// <summary>
    /// Retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns>A list of Part objects assigned to the Process.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<List<Part>> GetProcessParts(string ProcessFullName) {
        // load the Process' data
        Process Data = await ProcessData.GetIndividualProcessData(ProcessFullName);
        // pull the Part list
        List<Part> Parts = Data.Parts;
        // no Part data was read
        if (Parts.Count < 1) {
            throw new ArgumentException($"No Part data found for the Process '{ProcessFullName}'.");
        } 
        // return the Part list
        return Parts;
    }

    /// <summary>
    /// Retrieves and formats ProcessFullName's Part list as a list of Displayable strings.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns>A List of strings.</returns>
    public static async Task<List<string>> GetDisplayableProcessParts(string ProcessFullName) {
        // retrieve the Process' parts
        List<Part> ProcessParts = await GetProcessParts(ProcessFullName);
        // convert each Part Token into a Displayable string
        List<string> PartStrings = [];
        foreach (Part _part in ProcessParts) {
            PartStrings.Add($"{_part.PartNumber}\n{_part.PartName}");
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
    public static async Task<Part> GetPartData(string ProcessFullName, string PartNumber) {
        // perform the query on a new CPU thread
        Part PartData = await Task.Run(async () => {
            // retrieve the Process' Part list
            List<Part> ProcessParts = await GetProcessParts(ProcessFullName);
            // no Part data for this Process
            if (!ProcessParts.Any()) {
                throw new ArgumentException($"No Part data has been assigned to Process '{ProcessFullName}'.");
            }
            // attempt to access the specific Part
            Part? SelectedPart;
            try {
                SelectedPart = ProcessParts.Where(x => x.PartNumber.Equals(PartNumber)).First();
            // Part was not found in the Process' Part list
            } catch {
                throw new ArgumentException($"Part '{PartNumber}' not found assigned to Process '{ProcessFullName}'.");
            }
            return SelectedPart;
        });
        // return the queried Part data
        return PartData;
    }
}