using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datatypes;

/// <summary>
/// A serial identifier for Basket Labels that follows a strictly incrementing nine-digit format, using leading zeroes.
/// </summary>
public partial class LotNumber : ObservableObject
{
    /// <summary>
    /// The absolute lowest digit literal that can be assigned to a Lot Number.
    /// </summary>
    private const int MinValue = 1;

    /// <summary>
    /// The absolute highest digit literal that can be assigned to a Lot Number.
    /// </summary>
    private const int MaxValue = 999999999;

    /// <summary>
    /// The raw literal value of the Lot Number.
    /// </summary>
    [ObservableProperty]
    public partial int Literal { get; set; }

    /// <summary>
    /// A formatted version of the Lot Number's literal value. 
    /// Prepends '0' digit characters to the front of the string to enforce nine-digit formatting requirements.
    /// </summary>
    [ObservableProperty]
    public partial string Formatted { get; set; }

    /// <summary>
    /// Confirms that Value is a valid value for this datatype.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    private static bool IsValidValue(int Value)
    {
        return Value <= MaxValue && Value >= MinValue;
    }

    /// <summary>
    /// Formats the current Literal value according to the Datatype's formatting requirements.
    /// </summary>
    /// <returns>The Literal value as a Formatted string.</returns>
    private string FormatLiteral()
    {
        string FormattedLiteral = Literal.ToString();
        while (FormattedLiteral.Length < 9)
        {
            FormattedLiteral = $"0{FormattedLiteral}";
        }
        return FormattedLiteral;
    }

    /// <summary>
    /// Creates a new LotNumber from Value.
    /// </summary>
    /// <param name="Value"></param>
    public LotNumber(int Value)
    {
        // confirm that Value falls within the allowed literal range
        if (!IsValidValue(Value))
        {
            throw new ArgumentException($"'{Value}' is outside the allowed range of the LotNumber class.", nameof(Value));
        }
        Literal = Value;
        Formatted = FormatLiteral();
    }
}