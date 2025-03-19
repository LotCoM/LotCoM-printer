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
    private readonly string _code = Code;
    /// <summary>
    /// [Observable] The four-digit Process Code assigned to the Process.
    /// </summary>
    public string Code {
        get {return _code;}
    }

    /// <summary>
    /// The linguistic title (descriptor) assigned to the Process.
    /// </summary>
    private readonly string _title = Title;
    /// <summary>
    /// [Observable] The linguistic title (descriptor) assigned to the Process.
    /// </summary>
    public string Title {
        get {return _title;}
    }

    /// <summary>
    /// A reference-friendly Process name in the form of "Code-Title".
    /// </summary>
    private readonly string _fullName = $"{Code}-{Title}";
    /// <summary>
    /// [Observable] A reference-friendly Process name in the form of "Code-Title".
    /// </summary>
    public string FullName {
        get {return _fullName;}
    }

    /// <summary>
    /// The Process' serialization type (Originator || Pass-through).
    /// </summary>
    private readonly string _type = Type;
    /// <summary>
    /// [Observable] The Process' serialization type (Originator || Pass-through).
    /// </summary>
    public string Type {
        get {return _type;}
    }

    /// <summary>
    /// The Parts assigned to the Process.
    /// </summary>
    private readonly List<Part> _parts = Parts;
    /// <summary>
    /// [Observable] The Parts assigned to the Process.
    /// </summary>
    public List<Part> Parts {
        get {return _parts;}
    }
}