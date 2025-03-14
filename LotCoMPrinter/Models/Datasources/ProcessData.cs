namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a static source for Required Process Data when verifying Label printability.
/// Requirements are stored as the name of the Input Element on the Main Page.
/// </summary>
public static class ProcessData {
    // master list of processes
    private static readonly List<string> _processMasterList = [
        // Raw Material/In-House Mfg.
        "4420 - Diecast", "4470 - Deburr",
        "4159 - Uppershaft MC",
        // Civic Steering Column
        "4165 - CIV PH MC", "4155 - CIV T. Brkt. Weld", "4155 - CIV Pipe Weld", "4162 - CIV Shaft Clinch"
    ];
    public static List<string> ProcessMasterList {
        get {return _processMasterList;}
    }
    // list of origination processes (processes that produce serialization)
    private static List<string> _originationProcesses = [
        // Raw Material/In-House Mfg.
        "4420 - Diecast",
        "4159 - Uppershaft MC",
        // Civic Steering Column
        "4165 - CIV PH MC", "4155 - CIV T. Brkt. Weld", "4155 - CIV Pipe Weld", "4162 - CIV Shaft Clinch"
    ];

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
    /// Retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="Process"></param>
    /// <returns></returns>
    /// <exception cref="FileLoadException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<Dictionary<string, string>> GetProcessPartData(string Process) {
        // create a new CPU thread to read the file on
        Dictionary<string, string> ProcessParts = await Task.Run(() => {
            // create a ProcessPartTable for the Process
            ProcessPartTable Source = new ProcessPartTable(Process);
            string[] Parts = [];
            // read the Table
            try {
                Parts = Source.Read();
            // the Table file could not be read
            } catch (FileLoadException) {
                throw new FileLoadException($"The Part data for {Process} could not be found.");
            }
            // confirm that there were parts in the file
            if (Parts.Length < 1) {
                throw new ArgumentException($"There are no Parts assigned to {Process}.");
            }
            // convert the Part List into a dictionary of Part # key : Part Name value format
            Dictionary<string, string> PartDictionary = new Dictionary<string, string> {};
            foreach (string _part in Parts) {
                // split the part at the comma and add a new KeyValuePair in the dict
                string[] _splitPart = _part.Split(":");
                PartDictionary.Add(_splitPart[0], _splitPart[1]);
            }
            // return the constructed dictionary
            return PartDictionary;
        });
        return ProcessParts;
    }

    /// <summary>
    /// Checks if a Process is an originator (a process that creates new parts and requires serialization).
    /// </summary>
    /// <param name="Process">Process Title to check (including code prefix).</param>
    /// <returns>bool</returns>
    public static bool IsOriginator(string Process) {
        // check whether the process is an originator or not
        return _originationProcesses.Contains(Process);
    }
}