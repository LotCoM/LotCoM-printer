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
    public readonly string ParentProcess = ParentProcess;
    /// <summary>
    /// The Part Number assigned to the Part.
    /// </summary>
    public readonly string PartNumber = PartNumber;
    /// <summary>
    /// The Part Name assigned to the Part.
    /// </summary>
    public readonly string PartName = PartName;
    /// <summary>
    /// The Model Number the Part is associated with.
    /// </summary>
    public readonly string ModelNumber = ModelNumber;
}