using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// A serial identifier for Basket Labels that follows a strictly incrementing three-digit format, using leading zeroes.
/// </summary>
public partial class JBKNumber : ObservableObject
{
    /// <summary>
    /// The absolute lowest digit literal that can be assigned to a JBK Number.
    /// </summary>
    private const int MinValue = 1;

    /// <summary>
    /// The absolute highest digit literal that can be assigned to a JBK Number.
    /// </summary>
    private const int MaxValue = 999;

    /// <summary>
    /// The raw literal value of the JBK Number. Does not follow the three-digit formatting requirements.
    /// </summary>
    [ObservableProperty]
    public partial int Literal { get; set; }

    /// <summary>
    /// A formatted version of the JBK Number's literal value. 
    /// Prepends '0' digit characters to the front of the string to enforce three-digit formatting requirements.
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
        while (FormattedLiteral.Length < 3)
        {
            FormattedLiteral = $"0{FormattedLiteral}";
        }
        return FormattedLiteral;
    }

    /// <summary>
    /// Creates a new JBKNumber from Value.
    /// </summary>
    /// <param name="Value"></param>
    public JBKNumber(int Value)
    {
        // confirm that Value falls within the allowed literal range
        if (!IsValidValue(Value))
        {
            throw new ArgumentException($"'{Value}' is outside the allowed range of the JBKNumber class.", nameof(Value));
        }
        Literal = Value;
        Formatted = FormatLiteral();
    }
}