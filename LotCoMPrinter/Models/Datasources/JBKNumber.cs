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
    /// Creates a new JBKNumber from Value.
    /// </summary>
    /// <param name="Value"></param>
    public JBKNumber(int Value)
    {
        // confirm that Value falls within the allowed literal range
        if (Value > MaxValue || Value < MinValue)
        {
            throw new ArgumentException($"'{Value}' is outside the allowed range of the JBKNumber class.", nameof(Value));
        }
        Literal = Value;
        Formatted = Literal.ToString();
        // configure the Formatted property on instantiation
        while (Formatted.Length < 3)
        {
            Formatted = $"0{Formatted}";
        }
    }
}