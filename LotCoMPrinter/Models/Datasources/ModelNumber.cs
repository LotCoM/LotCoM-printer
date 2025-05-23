using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An identifying code for Part Models that follows a strict three- or four-character, alphanumerical, and uppercase format.
/// </summary>
public partial class ModelNumber : ObservableObject
{
    /// <summary>
    /// The code value of the Model Number.
    /// </summary>
    [ObservableProperty]
    public partial string Code { get; set; }

    /// <summary>
    /// Confirms that Value is a valid value for this datatype.
    /// </summary>
    /// <param name="Value"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <returns></returns>
    private static bool IsValidValue(string Value)
    {
        return ModelRegex().IsMatch(Value);
    }

    /// <summary>
    /// Creates a new ModelNumber from Value.
    /// </summary>
    /// <param name="Value"></param>
    public ModelNumber(string Value)
    {
        // confirm that Value falls within the allowed code length
        if (!IsValidValue(Value))
        {
            throw new ArgumentException($"'{Value}' is outside the allowed length of codes for the ModelNumber class.", nameof(Value));
        }
        Code = Value.ToUpper();
    }

    // COMPILED REGEX PATTERNS 

    [GeneratedRegex(@"^[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]?$")]
    private static partial Regex ModelRegex();
}