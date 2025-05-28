using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datatypes;

public partial class Operator : ObservableObject
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
    /// Creates a new Operator object to verify ownership of an item.
    /// </summary>
    /// <param name="Value">The string to use as the Initials of the OperatorInitials object.</param>
    /// <exception cref="ArgumentException"></exception>
    public Operator(string Value)
    {
        Initials = Value.ToUpper();
    }

    /// <summary>
    /// Confirms that the OperatorInitials object contains a valid 2-3 length string of alphabetical characters.
    /// </summary>
    /// <returns></returns>
    public bool ConfirmProperInitials()
    {
        return IsValidValue(Initials);
    }

    // COMPILED REGEX PATTERNS

    [GeneratedRegex(@"^[a-zA-Z][a-zA-Z][a-zA-Z]?$")]
    private static partial Regex OperatorRegex();
}