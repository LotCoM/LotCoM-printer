namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a static source for Required Process Data when verifying Label printability.
/// Requirements are stored as the name of the Input Element on the Main Page.
/// </summary>
public static class ProcessData {
    // master list of processes
    private static readonly List<string> _processMasterList = [
        "Diecast", "Deburr", "Pivot Housing MC", "Uppershaft MC", "Tilt Bracket Weld", "Pipe Weld", "Shaft Clinch"
    ];
    public static List<string> ProcessMasterList {
        get {return _processMasterList;}
    }

    /// <summary>
    /// Formats the Part passed into a two-line, newline-split Part String, which can be displayed.
    /// </summary>
    /// <param name="PartNumber"></param>
    public static string GetPartAsDisplayable(string PartNumber) {
        // format the passed part as a displayable string
        string[] SplitPart = PartNumber.Split(",");
        string PartString = SplitPart[0] + "\n" + SplitPart[1];
        return PartString;
    }

    /// <summary>
    /// Retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="Process"></param>
    /// <returns></returns>
    /// <exception cref="FileLoadException"></exception>
    public static async Task<Dictionary<string, string>> GetProcessPartData(string Process) {
        // create a new CPU thread to read the file on
        Dictionary<string, string> ProcessParts = await Task.Run(() => {
            // create a ProcessPartTable for the Process
            ProcessPartTable Source = new ProcessPartTable(Process);
            string[] Parts = [];
            // read the Table
            Parts = Source.Read();
            // convert the Part List into a dictionary of Part # key : Part Name value format
            Dictionary<string, string> PartDictionary = new Dictionary<string, string> {};
            foreach (string _part in Parts) {
                // split the part at the comma and add a new KeyValuePair in the dict
                string[] _splitPart = _part.Split(",");
                PartDictionary.Add(_splitPart[0], _splitPart[1]);
            }
            // return the constructed dictionary
            return PartDictionary;
        });
        return ProcessParts;
    }
}