namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a static source for Model Data throughout the LotCoM WIP Printer application.
/// </summary>
public static class ModelData {
    // Model Data masterlist
    private static readonly List<string> _modelsMasterList = [
        "T20", "TBA", "TLA"
    ];
    public static List<string> ModelsMasterList {
        get {return _modelsMasterList;}
    }

    /// <summary>
    /// Attempts to find the Model Number attached to PartNumber. 
    /// Only returns a number if the Model is in the Models masterlist.
    /// </summary>
    /// <param name="PartNumber">The Part Number to imply the Model Number from.</param>
    /// <returns>string Model Number (if found).</returns>
    /// <exception cref="ArgumentException">Thrown if the Model Number is not found.</exception>
    public static async Task<string> AttemptModelFromPart(string PartNumber) {
        string ModelNumber = await Task.Run(() => {
            // split the Model Number out of the Part Number (xx-[MODELNUMBER]-xxx-xxx...)
            string ModelNumber = PartNumber.Split("-")[1];
            // try to find that Model in the Models masterlist
            if (ModelsMasterList.Contains(ModelNumber)) {
                return ModelNumber;
            // Model is not in the Keys list; doesn't exist
            } else {
                return "";
            }
        });
        // if a Model Number was found, return it, otherwise throw an exception
        if (ModelNumber != "") {
            return ModelNumber;
        } else {
            throw new ArgumentException($"Part Number '{PartNumber}' containing Model Number '{ModelNumber}' was not in the Model masterlist.");
        }
    }
}

