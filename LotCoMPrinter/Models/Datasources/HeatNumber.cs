namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// A manufacturing tracing identifier for Basket Labels.
/// </summary>
public class HeatNumber
{
    /// <summary>
    /// The absolute lowest digit literal that can be assigned to a Heat Number.
    /// </summary>
    private const int MinValue = 1;

    /// <summary>
    /// The absolute highest digit literal that can be assigned to a Heat Number.
    /// </summary>
    private const int MaxValue = 999999999;

    private int _literal;
    /// <summary>
    /// The raw literal value of the Heat Number.
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
                throw new ArgumentException($"{value} is outside the allowed range of the HeatNumber class.");
            }
        }
    }

    /// <summary>
    /// Creates a new HeatNumber from Value.
    /// </summary>
    /// <param name="Value"></param>
    public HeatNumber(int Value)
    {
        // confirm that Value falls within the allowed literal range
        if (Value > MaxValue || Value < MinValue)
        {
            throw new ArgumentException($"'{Value}' is outside the allowed range of the HeatNumber class.", nameof(Value));
        }
        Literal = Value;
    }
}