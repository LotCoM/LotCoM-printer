namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An objective version of a Department loaded from the Process Masterlist data source.
/// </summary>
/// <param name="Title">The Title of the Department.</param>
/// <param name="Code">The two-character abbreviation code assigned to the Department.</param>
/// <param name="Lines">A List of Lines assigned to the Department.</param>
public class Department(string Title, string Code, List<string> Lines) 
{
    /// <summary>
    /// The Title of the Department.
    /// </summary>
    public string Title = Title;

    /// <summary>
    /// The two-character abbreviation code assigned to the Department.
    /// </summary>
    public string Code = Code;

    /// <summary>
    /// A List of Lines assigned to the Department.
    /// </summary>
    public List<string> Lines = Lines;
}