using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Datasources;

public static class ProcessRequirements {
    // data requirements for each process
    // Universal requirements
    private static readonly List<string> _universalRequirements = [
        "ProcessPicker", "PartPicker", "QuantityEntry", "ProductionDatePicker", "ProductionShiftPicker", "OperatorIDEntry"
    ];
    // individual process requirements
    private static readonly List<string> _diecastRequirements = ["JBKNumberEntry", "DieNumberEntry"];
    private static readonly List<string> _deburrRequirements = ["JBKNumberEntry", "DieNumberEntry"];
    private static readonly List<string> _pivotHousingMCRequirements = ["JBKNumberEntry", "DeburrJBKNumberEntry"];
    private static readonly List<string> _tiltBracketWeldRequirements = ["LotNumberEntry"];
    private static readonly List<string> _pipeWeldRequirements = ["LotNumberEntry", "ModelNumberEntry"];
    private static readonly List<string> _shaftClinchRequirements = ["LotNumberEntry", "ModelNumberEntry"];
    private static readonly List<string> _uppershaftMCRequirements = ["LotNumberEntry"];

    /// <summary>
    /// Allows access to a Process' requirements from a string.
    /// </summary>
    /// <param name="Process">The Process selection to retrieve requirements for.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static List<string> GetProcessRequirements(string Process) {
        // ensure non-null Process value
        if (Process == null || Process.Equals("")) {
            throw new NullProcessException();
        }
        // create a dictionary to convert from Process to Property
        Dictionary<string, List<string>> Conversions = new Dictionary<string, List<string>> {
            {"4420-Diecast", _diecastRequirements},
            {"4470-Deburr", _deburrRequirements},
            {"4159-UppershaftMC", _uppershaftMCRequirements},
            {"4165-CivPHMC", _pivotHousingMCRequirements},
            {"4155-TiltBrktWeld", _tiltBracketWeldRequirements},
            {"4155-PipeWeld", _pipeWeldRequirements},
            {"4162-CivShaftClinch", _shaftClinchRequirements}
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