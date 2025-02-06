namespace LCMSWipPrinter.Models.Datasources;

/// <summary>
/// Provides a static source for Required Process Data when verifying Label printability.
/// Requirements are stored as the name of the Input Element on the Main Page.
/// </summary>
public static class ProcessData {
    // requirements for each process
    // Diecast
    private static readonly List<string> _diecastRequirements = [
        "ProcessPicker", "JBKNumberInput", "DieNumberInput", "PartPicker", 
        "QuantityInput", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> DiecastRequirements {
        get {return _diecastRequirements;}
    }
    // Deburr
    private static readonly List<string> _deburrRequirements = [
        "ProcessPicker", "JBKNumberInput", "PartPicker", 
        "QuantityInput", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> DeburrRequirements {
        get {return _deburrRequirements;}
    }
    // Pivot Housing MC
    private static readonly List<string> _pivotHousingMCRequirements = [
        "ProcessPicker", "JBKNumberInput", "DeburrJBKNumberInput", "PartPicker", 
        "QuantityInput", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> PivotHousingMCRequirements {
        get {return _pivotHousingMCRequirements;}
    }
    // Tilt Bracket Weld
    private static readonly List<string> _tiltBracketWeldRequirements = [
        "ProcessPicker", "LotNumberInput", "PartPicker", 
        "QuantityInput", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> TiltBracketWeldRequirements {
        get {return _tiltBracketWeldRequirements;}
    }
    // Pipe Weld
    private static readonly List<string> _pipeWeldRequirements = [
        "ProcessPicker", "LotNumberInput", "PartPicker", "ModelNumberPicker",
        "QuantityInput", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> PipeWeldRequirements {
        get {return _pipeWeldRequirements;}
    }
    // Shaft Clinch
    private static readonly List<string> _shaftClinchRequirements = [
        "ProcessPicker", "LotNumberInput", "PartPicker", "ModelNumberPicker",
        "QuantityInput", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> ShaftClinchRequirements {
        get {return _shaftClinchRequirements;}
    }
    // Uppershaft MC
    private static readonly List<string> _uppershaftMCRequirements = [
        "ProcessPicker", "LotNumberInput", "PartPicker", 
        "QuantityInput", "ProductionDatePicker", "ProductionShiftPicker"
    ];
    public static List<string> UppershaftMCRequirements {
        get {return _uppershaftMCRequirements;}
    }
}