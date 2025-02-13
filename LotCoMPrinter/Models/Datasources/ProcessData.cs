namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a static source for Required Process Data when verifying Label printability.
/// Requirements are stored as the name of the Input Element on the Main Page.
/// </summary>
public static class ProcessData {
    // master list of processes
    // each key is a displayable string and the value is the programmatic name
    private static readonly List<string> _processMasterList = [
        "Diecast", "Deburr", "Pivot Housing MC", "Uppershaft MC", "Tilt Bracket Weld", "Pipe Weld", "Shaft Clinch"
    ];
    public static List<string> ProcessMasterList {
        get {return _processMasterList;}
    }

    // requirements for each process
    // Diecast
    private static readonly List<string> _diecastRequirements = [
        "ProcessPicker", "JBKNumberEntry", "DieNumberEntry", "PartPicker", 
        "QuantityEntry", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> DiecastRequirements {
        get {return _diecastRequirements;}
    }
    // Deburr
    private static readonly List<string> _deburrRequirements = [
        "ProcessPicker", "JBKNumberEntry", "PartPicker", 
        "QuantityEntry", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> DeburrRequirements {
        get {return _deburrRequirements;}
    }
    // Pivot Housing MC
    private static readonly List<string> _pivotHousingMCRequirements = [
        "ProcessPicker", "JBKNumberEntry", "DeburrJBKNumberEntry", "PartPicker", 
        "QuantityEntry", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> PivotHousingMCRequirements {
        get {return _pivotHousingMCRequirements;}
    }
    // Tilt Bracket Weld
    private static readonly List<string> _tiltBracketWeldRequirements = [
        "ProcessPicker", "LotNumberEntry", "PartPicker", 
        "QuantityEntry", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> TiltBracketWeldRequirements {
        get {return _tiltBracketWeldRequirements;}
    }
    // Pipe Weld
    private static readonly List<string> _pipeWeldRequirements = [
        "ProcessPicker", "LotNumberEntry", "PartPicker", "ModelNumberPicker",
        "QuantityEntry", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> PipeWeldRequirements {
        get {return _pipeWeldRequirements;}
    }
    // Shaft Clinch
    private static readonly List<string> _shaftClinchRequirements = [
        "ProcessPicker", "LotNumberEntry", "PartPicker", "ModelNumberPicker",
        "QuantityEntry", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> ShaftClinchRequirements {
        get {return _shaftClinchRequirements;}
    }
    // Uppershaft MC
    private static readonly List<string> _uppershaftMCRequirements = [
        "ProcessPicker", "LotNumberEntry", "PartPicker", 
        "QuantityEntry", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> UppershaftMCRequirements {
        get {return _uppershaftMCRequirements;}
    }

    
    /// <summary>
    /// Allows access to a Process' requirements from a string.
    /// </summary>
    /// <param name="Process">The Process selection to retrieve requirements for.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static List<string> GetProcessRequirements(string Process) {
        // ensure non-null Process value
        if (Process == null) {
            throw new ArgumentException("Process must not be null.");
        }
        // create a dictionary to convert from Process to Property
        Dictionary<string, List<string>> Conversions = new Dictionary<string, List<string>> {
            {"Diecast", DiecastRequirements},
            {"Deburr", DeburrRequirements},
            {"PivotHousingMC", PivotHousingMCRequirements},
            {"UppershaftMC", UppershaftMCRequirements},
            {"TiltBracketWeld", TiltBracketWeldRequirements},
            {"PipeWeld", PipeWeldRequirements},
            {"ShaftClinch", ShaftClinchRequirements}
        };
        // find the property in the ProcessData class
        List<string> Requirements = [];
        try {
            Requirements = Conversions[Process];
        // the non-null Process is not in the datasource
        } catch (KeyNotFoundException) {
            throw new ArgumentException($"{Process} is not recognized as a Process");
        }
        return Requirements;
    }
}