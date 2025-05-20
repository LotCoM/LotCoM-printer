namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// An identifier for Dies used to cast Parts in a Basket. Follows a strict one- or two-digit format.
/// </summary>
public class DieNumber
{
    /// <summary>
    /// The absolute lowest digit literal that can be assigned to a Die Number.
    /// </summary>
    private const int MinValue = 1;

    /// <summary>
    /// The absolute highest digit literal that can be assigned to a Die Number.
    /// </summary>
    private const int MaxValue = 50;

    private int _literal;
    /// <summary>
    /// The raw literal value of the Die Number.
    /// </summary>
    public int Literal
    {
        get { return _literal; }
        set
        {
            if (value >= MinValue && value <= MaxValue)
            {
                _literal = value;
            }
            else
            {
                throw new ArgumentException($"{value} is outside the allowed range of the DieNumber class.");
            }
        }
    }

    /// <summary>
    /// Creates a new DieNumber from Value.
    /// </summary>
    /// <param name="Value"></param>
    public DieNumber(int Value)
    {
        // confirm that Value falls within the allowed literal range
        if (Value > MaxValue || Value < MinValue)
        {
            throw new ArgumentException($"'{Value}' is outside the allowed range of the DieNumber class.", nameof(Value));
        }
        Literal = Value;
    }
}