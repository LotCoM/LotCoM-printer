namespace LCMSWipPrinter.Models.Datasources;

/// <summary>
/// Provides a static source for Part Data throughout the LCMS WIP Printer application.
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
}