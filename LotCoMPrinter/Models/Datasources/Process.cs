namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An objective version of a Process loaded from the Process Masterlist data source.
/// </summary>
/// <param name="Code">The four-digit Process Code assigned to the Process.</param>
/// <param name="Title">The linguistic title (descriptor) assigned to the Process.</param>
/// <param name="Type">The Process' serialization type (Originator || Pass-through).</param>
/// <param name="Parts">The Parts assigned to the Process.</param>
public class Process(string Code, string Title, string Type, List<Part> Parts) {
    /// <summary>
    /// The four-digit Process Code assigned to the Process.
    /// </summary>
    public readonly string Code = Code;
    /// <summary>
    /// The linguistic title (descriptor) assigned to the Process.
    /// </summary>
    public readonly string Title = Title;
    /// <summary>
    /// A reference-friendly Process name in the form of "Code-Title".
    /// </summary>
    public readonly string FullName = $"{Code}-{Title}";
    /// <summary>
    /// The Process' serialization type (Originator || Pass-through).
    /// </summary>
    public readonly string Type = Type;
    /// <summary>
    /// The Parts assigned to the Process.
    /// </summary>
    public readonly List<Dictionary<string, string>> Parts = Parts;
}