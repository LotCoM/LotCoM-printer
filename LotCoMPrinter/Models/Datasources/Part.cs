using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An objective version of a Part loaded from the Process Masterlist data source.
/// </summary>
/// <param name="ParentProcess">The Process that the Part is assigned to.</param>
/// <param name="PartNumber">The Part Number assigned to the Part.</param>
/// <param name="PartName">The Part Name assigned to the Part.</param>
/// <param name="ModelNumber">The Model Number the Part is associated with.</param>
public partial class Part(string ParentProcess, string PartNumber, string PartName, string ModelNumber): ObservableObject {
    /// <summary>
    /// [Observable] The Process that the Part is assigned to.
    /// </summary>
    [ObservableProperty]
    public partial string ParentProcess {get; set;} = ParentProcess;

    /// <summary>
    /// [Observable] The Part Number assigned to the Part.
    /// </summary>
    [ObservableProperty]
    public partial string PartNumber {get; set;} = PartNumber;

    /// <summary>
    /// [Observable] The Part Name assigned to the Part.
    /// </summary>
    [ObservableProperty]
    public partial string PartName {get; set;} = PartName;

    /// <summary>
    /// [Observable] The Model Number the Part is associated with.
    /// </summary>
    [ObservableProperty]
    public partial string ModelNumber {get; set;} = ModelNumber;
}