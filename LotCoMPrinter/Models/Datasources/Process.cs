using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An objective version of a Process loaded from the Process Masterlist data source.
/// </summary>
/// <param name="LineCode">The four-digit Process Code assigned to the Process.</param>
/// <param name="Line">The Process' parent Line (i.e. AP5, CRV).</param>
/// <param name="Title">The linguistic title (descriptor) assigned to the Process.</param>
/// <param name="Type">The Process' serialization type (Originator || Pass-through).</param>
/// <param name="Serialization">The Process' serialization mode (JBK || Lot || null [pass-through only])</param>
/// <param name="Parts">The Parts assigned to the Process.</param>
public partial class Process(string LineCode, string Line, string Title, string Type, string Serialization, List<Part> Parts, List<string> RequiredFields): ObservableObject {
    /// <summary>
    /// [Observable] The four-digit Process Code assigned to the Process.
    /// </summary>
    [ObservableProperty]
    public partial string LineCode {get; set;} = LineCode;

    /// <summary>
    /// [Observable] The Process' parent Line.
    /// </summary>
    [ObservableProperty]
    public partial string Line {get; set;} = Line;

    /// <summary>
    /// [Observable] The linguistic title (descriptor) assigned to the Process.
    /// </summary>
    [ObservableProperty]
    public partial string Title {get; set;} = Title;

    /// <summary>
    /// [Observable] A reference-friendly Process name in the form of "Code-Title".
    /// </summary>
    [ObservableProperty]
    public partial string FullName {get; set;} = $"{LineCode}-{Line}-{Title}";

    /// <summary>
    /// [Observable] The Process' serialization type (Originator || Pass-through).
    /// </summary>
    [ObservableProperty]
    public partial string Type {get; set;} = Type;

    /// <summary>
    /// The Process' serialization mode (JBK || Lot).
    /// </summary>
    [ObservableProperty]
    public partial string Serialization {get; set;} = Serialization;

    /// <summary>
    /// [Observable] The Parts assigned to the Process.
    /// </summary>
    [ObservableProperty]
    public partial List<Part> Parts {get; set;} = Parts;

    /// <summary>
    /// [Observable] The Production Data fields required at this Process.
    /// </summary>
    [ObservableProperty]
    public partial List<string> RequiredFields {get; set;} = RequiredFields;
}