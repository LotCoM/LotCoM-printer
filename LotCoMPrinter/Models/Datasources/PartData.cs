namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a static source for Part Data throughout the LotCoM WIP Printer application.
/// </summary>
public static class PartData {
    // Part Data masterlist
    // formatted as {Part Number: PartName}
    private static readonly Dictionary<string, string> _partsMasterList = new Dictionary<string, string> {
        {"00-T20-532AP-A000-YB1", "HOUSING, PIVOT*"}
    };
    public static Dictionary<string, string> PartsMasterList {
        get {return _partsMasterList;}
    }

    // sub-lists for each process
    // Diecast
    private static readonly Dictionary<string, string> _diecastPartsList = new Dictionary<string, string> {
        {"00-T20-532AP-A000-YB1", "HOUSING, PIVOT*"}
    };
    public static Dictionary<string, string> DiecastPartsList {
        get {return _diecastPartsList;}
    }
    // Deburr
    private static readonly Dictionary<string, string> _deburrPartsList = new Dictionary<string, string> {
        {"00-T20-532AP-A000-YB1", "HOUSING, PIVOT*"}
    };
    public static Dictionary<string, string> DeburrPartsList {
        get {return _deburrPartsList;}
    }
    // Pivot Housing Machining
    private static readonly Dictionary<string, string> _pivotHousingMCPartsList = new Dictionary<string, string> {
        {"00-T20-532AP-A000-YB1", "HOUSING, PIVOT*"}
    };
    public static Dictionary<string, string> PivotHousingMCPartsList {
        get {return _pivotHousingMCPartsList;}
    }
    // Uppershaft Machining
    private static readonly Dictionary<string, string> _uppershaftMCPartsList = new Dictionary<string, string> {
        {"00-T20-532AP-A000-YB1", "HOUSING, PIVOT*"}
    };
    public static Dictionary<string, string> UppershaftMCPartsList {
        get {return _uppershaftMCPartsList;}
    }
    // Tilt Bracket Weld
    private static readonly Dictionary<string, string> _tiltBracketWeldPartsList = new Dictionary<string, string> {
        {"00-T20-532AP-A000-YB1", "HOUSING, PIVOT*"}
    };
    public static Dictionary<string, string> TiltBracketWeldPartsList {
        get {return _tiltBracketWeldPartsList;}
    }
    // Pipe Weld
    private static readonly Dictionary<string, string> _pipeWeldPartsList = new Dictionary<string, string> {
        {"00-T20-532AP-A000-YB1", "HOUSING, PIVOT*"}
    };
    public static Dictionary<string, string> PipeWeldPartsList {
        get {return _pipeWeldPartsList;}
    }
    // Shaft Clinch
    private static readonly Dictionary<string, string> _shaftClinchPartsList = new Dictionary<string, string> {
        {"00-T20-532AP-A000-YB1", "HOUSING, PIVOT*"}
    };
    public static Dictionary<string, string> ShaftClinchPartsList {
        get {return _shaftClinchPartsList;}
    }

    /// <summary>
    /// Attempts to access the Part Information stored under PartNumber.
    /// </summary>
    /// <param name="PartNumber"></param>
    /// <returns>String formatted as "PartNumber\nPartName" if the PartNumber exists as a key; null if not.</returns>
    public static string GetPartAsString(string PartNumber) {
        // access the full part data from the internal static source
        try {
            var PartData = PartsMasterList[PartNumber];
            string PartString = PartNumber + "\n" + PartData;
            return PartString;
        } catch {
            throw new ArgumentException($"Part Number {PartNumber} was not found in the Parts masterlist.");
        }
    }

    /// <summary>
    /// Allows access to a Process' Part Data from a string.
    /// </summary>
    /// <param name="Process">The Process selection to retrieve Part Data for.</param>
    /// <returns></returns>
    public static Dictionary<string, string> GetProcessParts(string Process) {
        // create a dictionary to convert from Process to Property
        Dictionary<string, Dictionary<string, string>> Conversions = new Dictionary<string, Dictionary<string, string>> {
            {"Diecast", DiecastPartsList},
            {"Deburr", DeburrPartsList},
            {"PivotHousingMC", PivotHousingMCPartsList},
            {"UppershaftMC", UppershaftMCPartsList},
            {"TiltBracketWeld", TiltBracketWeldPartsList},
            {"PipeWeld", PipeWeldPartsList},
            {"ShaftClinch", ShaftClinchPartsList}
        };
        // find the property in the ProcessData class
        Dictionary<string, string> PartData = Conversions[Process.Replace(" ", "")];
        return PartData;
    }
}