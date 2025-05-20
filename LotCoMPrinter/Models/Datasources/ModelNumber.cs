using System.Text.RegularExpressions;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An identifying code for Part Models that follows a strict three- or four-character, alphanumerical, and uppercase format.
/// </summary>
public partial class ModelNumber
{
    /// <summary>
    /// Sets the absolute minimum length of a Model Number's code.
    /// </summary>
    public const int MinLength = 3;

    /// <summary>
    /// Sets the absolute maximum length of a Model Number's code.
    /// </summary>
    public const int MaxLength = 4;

    private string _code;
    /// <summary>
    /// The code value of the Model Number.
    /// </summary>
    public string Code
    {
        get { return _code; }
        set
        {
            if (value.Length < MinLength || value.Length > MaxLength)
            {
                throw new ArgumentException($"{value} is outside the allowed length of codes for the ModelNumber class.");
            }
            // enforce formatting of the code as uppercase and alphanumerical
            if (ModelRegex().IsMatch(value))
            {
                throw new ArgumentException($"'{value}' does not follow formatting requirements of codes for the ModelNumber class.", nameof(value));
            }
            _code = value.ToUpper();
        }
    }

    /// <summary>
    /// Creates a new ModelNumber from Value.
    /// </summary>
    /// <param name="Value"></param>
    public ModelNumber(string Value)
    {
        // confirm that Value falls within the allowed code length
        if (Value.Length > MaxLength || Value.Length < MinLength)
        {
            throw new ArgumentException($"'{Value}' is outside the allowed length of codes for the ModelNumber class.", nameof(Value));
        }
        // enforce formatting of the code as uppercase and alphanumerical
        if (ModelRegex().IsMatch(Value))
        {
            throw new ArgumentException($"'{Value}' does not follow formatting requirements of codes for the ModelNumber class.", nameof(Value));
        }
        _code = Value.ToUpper();
    }

    // COMPILED REGEX PATTERNS 

    [GeneratedRegex(@"^[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]?$")]
    private static partial Regex ModelRegex();
}