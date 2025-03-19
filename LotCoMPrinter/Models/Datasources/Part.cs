namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An objective version of a Part loaded from the Process Masterlist data source.
/// </summary>
/// <param name="ParentProcess">The Process that the Part is assigned to.</param>
/// <param name="PartNumber">The Part Number assigned to the Part.</param>
/// <param name="PartName">The Part Name assigned to the Part.</param>
/// <param name="ModelNumber">The Model Number the Part is associated with.</param>
public class Part(string ParentProcess, string PartNumber, string PartName, string ModelNumber) {
    /// <summary>
    /// The Process that the Part is assigned to.
    /// </summary>
    private readonly string _parentProcess = ParentProcess; 
    /// <summary>
    /// [Observable] The Process that the Part is assigned to.
    /// </summary>
    public string ParentProcess {
        get {return _parentProcess;}
    }

    /// <summary>
    /// The Part Number assigned to the Part.
    /// </summary>
    private readonly string _partNumber = PartNumber;
    /// <summary>
    /// [Observable] The Part Number assigned to the Part.
    /// </summary>
    public string PartNumber {
        get {return _partNumber;}
    }

    /// <summary>
    /// The Part Name assigned to the Part.
    /// </summary>
    private readonly string _partName = PartName;
    /// <summary>
    /// [Observable] The Part Name assigned to the Part.
    /// </summary>
    public string PartName {
        get {return _partName;}
    }

    /// <summary>
    /// The Model Number the Part is associated with.
    /// </summary>
    private readonly string _modelNumber = ModelNumber;
    /// <summary>
    /// [Observable] The Model Number the Part is associated with.
    /// </summary>
    public string ModelNumber {
        get {return _modelNumber;}
    }
}