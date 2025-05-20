using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An objective version of a Process loaded from the Process Masterlist data source.
/// </summary>
/// <param name="LineCode">The four-digit Process Code assigned to the Process.</param>
/// <param name="Line">The Process' parent Line (i.e. AP5, CRV).</param>
/// <param name="Title">The linguistic title (descriptor) assigned to the Process.</param>
/// <param name="Type">The Process' serialization type (Originator || Pass-through).</param>
/// <param name="SerializationMode">The Process' serialization mode.</param>
/// <param name="Parts">The Parts assigned to the Process.</param>
/// <param name="RequiredFields">The data fields required by this Process.</param>
/// <param name="PassThroughType">
/// For Processes with OriginationTypes.PassThrough: 
/// The type of Serialization Header to apply to the Label.
/// For Processes with OriginationTypes.Originator:
/// None.
/// </param>
public partial class Process(int LineCode, string Line, string Title, OriginationType Type, SerializationMode SerializationMode, List<Part> Parts, RequiredFields RequiredFields, PassThroughType PassThroughType): ObservableObject() 
{
    /// <summary>
    /// [Observable] The four-digit Process Code assigned to the Process.
    /// </summary>
    [ObservableProperty]
    public partial int LineCode {get; set;} = LineCode;

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
    public partial OriginationType Type {get; set;} = Type;

    /// <summary>
    /// The Process' serialization mode (JBK || Lot).
    /// </summary>
    [ObservableProperty]
    public partial SerializationMode Serialization {get; set;} = SerializationMode;

    /// <summary>
    /// [Observable] The Parts assigned to the Process.
    /// </summary>
    [ObservableProperty]
    public partial List<Part> Parts {get; set;} = Parts;

    /// <summary>
    /// [Observable] The Production Data fields required at this Process.
    /// </summary>
    [ObservableProperty]
    public partial RequiredFields RequiredFields {get; set;} = RequiredFields;

    /// <summary>
    /// [Observable] For Processes with Type = "Pass-through", the type of Serialization Header to apply to the Label; "JBK" or "Lot".
    /// For Processes with Type = "Origination", null.
    /// </summary>
    [ObservableProperty]
    public partial PassThroughType PassThroughType {get; set;} = PassThroughType;
}