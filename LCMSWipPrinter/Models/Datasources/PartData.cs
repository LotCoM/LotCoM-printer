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

    // public attribute that returns all parts as displayable strings
    public static List<string> AllPartsAsString {
        get {
            List<string> AllPartStrings = [];
            foreach (string _key in PartsMasterList.Keys) {
                var _part = GetPartAsString(_key);
                if (_part != null) {
                    AllPartStrings = AllPartStrings.Append(_part).ToList();
                }
            }
            return AllPartStrings;
        }
    }

    // public attribute that returns Diecast parts as displayable strings
    public static List<string> DiecastPartsAsString {
        get {
            List<string> AllPartStrings = [];
            foreach (string _key in DiecastPartsList.Keys) {
                var _part = GetPartAsString(_key);
                if (_part != null) {
                    AllPartStrings = AllPartStrings.Append(_part).ToList();
                }
            }
            return AllPartStrings;
        }
    }

    // public attribute that returns Deburr parts as displayable strings
    public static List<string> DeburrPartsAsString {
        get {
            List<string> AllPartStrings = [];
            foreach (string _key in DeburrPartsList.Keys) {
                var _part = GetPartAsString(_key);
                if (_part != null) {
                    AllPartStrings = AllPartStrings.Append(_part).ToList();
                }
            }
            return AllPartStrings;
        }
    }

    // public attribute that returns Pivot Housing MC parts as displayable strings
    public static List<string> PivotHousingMCPartsAsString {
        get {
            List<string> AllPartStrings = [];
            foreach (string _key in PivotHousingMCPartsList.Keys) {
                var _part = GetPartAsString(_key);
                if (_part != null) {
                    AllPartStrings = AllPartStrings.Append(_part).ToList();
                }
            }
            return AllPartStrings;
        }
    }

    /// <summary>
    /// Attempts to access the Part Information stored under PartNumber.
    /// </summary>
    /// <param name="PartNumber"></param>
    /// <returns>String formatted as "PartNumber\nPartName" if the PartNumber exists as a key; null if not.</returns>
    public static string? GetPartAsString(string PartNumber) {
        // access the full part data from the internal static source
        try {
            var FullData = PartsMasterList[PartNumber];
            string FullDataString = FullData[0] + "\n" + FullData[1];
            return FullDataString;
        } catch {
            return null;
        }
    }
}