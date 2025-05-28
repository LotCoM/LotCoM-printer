using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datatypes;

public partial class OperatorInitials : ObservableObject
{
    /// <summary>
    /// The 2-3 character string assigned to this OperatorInitials object.
    /// </summary>
    [ObservableProperty]
    public partial string Initials { get; set; }

    /// <summary>
    /// Confirms that Value is a valid value for this datatype.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    private static bool IsValidValue(string Value)
    {
        if (Value is null || !OperatorRegex().IsMatch(Value))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Creates a new OperatorInitials object to verify ownership of an item.
    /// </summary>
    /// <param name="Value">The string to use as the Initials of the OperatorInitials object.</param>
    /// <exception cref="ArgumentException"></exception>
    public OperatorInitials(string Value)
    {
        // ensure that the Value is in valid format and cast the initials to uppercase
        if (!IsValidValue(Value))
        {
            throw new ArgumentException($"Cannot instantiate an OperatorInitials object with initials '{Initials}'.", nameof(Value));
        }
        Initials = Value.ToUpper();
    }

    // COMPILED REGEX PATTERNS

    [GeneratedRegex(@"^[a-zA-Z][a-zA-Z][a-zA-Z]?$")]
    private static partial Regex OperatorRegex();
}