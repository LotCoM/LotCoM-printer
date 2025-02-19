namespace LotCoMPrinter.Models.Datasources;

public static class ProcessRequirements {
    // data requirements for each process
    // Universal requirements
    private static readonly List<string> _universalRequirements = [
        "ProcessPicker", "PartPicker", "QuantityEntry", "ProductionDatePicker", "ProductionShiftPicker", "OperatorIDEntry"
    ];
    // individual process requirements
    private static readonly List<string> _diecastRequirements = ["JBKNumberEntry", "DieNumberEntry"];
    private static readonly List<string> _deburrRequirements = ["JBKNumberEntry"];
    private static readonly List<string> _pivotHousingMCRequirements = ["JBKNumberEntry", "DeburrJBKNumberEntry"];
    private static readonly List<string> _tiltBracketWeldRequirements = ["LotNumberEntry"];
    private static readonly List<string> _pipeWeldRequirements = ["LotNumberEntry", "ModelNumberPicker"];
    private static readonly List<string> _shaftClinchRequirements = ["LotNumberEntry", "ModelNumberPicker"];
    private static readonly List<string> _uppershaftMCRequirements = ["LotNumberEntry"];

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
            {"Diecast", _diecastRequirements},
            {"Deburr", _deburrRequirements},
            {"PivotHousingMC", _pivotHousingMCRequirements},
            {"UppershaftMC", _uppershaftMCRequirements},
            {"TiltBracketWeld", _tiltBracketWeldRequirements},
            {"PipeWeld", _pipeWeldRequirements},
            {"ShaftClinch", _shaftClinchRequirements}
        };
        // start with the universal requirement set
        List<string> Requirements = _universalRequirements.ToList();
        try {
            // try to convert the string name to a set of Process Requirements
            Requirements.AddRange(Conversions[Process]);
        // the non-null Process is not in the datasource
        } catch (KeyNotFoundException) {
            throw new ArgumentException($"{Process} is not recognized as a Process");
        }
        return Requirements;
    }
}