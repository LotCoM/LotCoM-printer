using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An objective version of a Process loaded from the Process Masterlist data source.
/// </summary>
/// <param name="Code">The four-digit Process Code assigned to the Process.</param>
/// <param name="Title">The linguistic title (descriptor) assigned to the Process.</param>
/// <param name="Type">The Process' serialization type (Originator || Pass-through).</param>
/// <param name="Parts">The Parts assigned to the Process.</param>
public partial class Process(string Code, string Title, string Type, List<Part> Parts): ObservableObject {
    /// <summary>
    /// [Observable] The four-digit Process Code assigned to the Process.
    /// </summary>
    [ObservableProperty]
    public partial string Code {get; set;} = Code;

    /// <summary>
    /// [Observable] The linguistic title (descriptor) assigned to the Process.
    /// </summary>
    [ObservableProperty]
    public partial string Title {get; set;} = Title;

    /// <summary>
    /// [Observable] A reference-friendly Process name in the form of "Code-Title".
    /// </summary>
    [ObservableProperty]
    public partial string FullName {get; set;} = $"{Code}-{Title}";

    /// <summary>
    /// [Observable] The Process' serialization type (Originator || Pass-through).
    /// </summary>
    [ObservableProperty]
    public partial string Type {get; set;} = Type;

    /// <summary>
    /// [Observable] The Parts assigned to the Process.
    /// </summary>
    [ObservableProperty]
    public partial List<Part> Parts {get; set;} = Parts;
}