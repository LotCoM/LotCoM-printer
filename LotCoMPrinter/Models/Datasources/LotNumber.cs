namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// A serial identifier for Basket Labels that follows a strictly incrementing nine-digit format, using leading zeroes.
/// </summary>
public class LotNumber
{
    /// <summary>
    /// The absolute lowest digit literal that can be assigned to a Lot Number.
    /// </summary>
    private const int MinValue = 1;

    /// <summary>
    /// The absolute highest digit literal that can be assigned to a Lot Number.
    /// </summary>
    private const int MaxValue = 999999999;

    private int _literal;
    /// <summary>
    /// The raw literal value of the Lot Number.
    /// </summary>
    public int Literal
    {
        get { return _literal; }
        set
        {
            if (value >= MinValue && value <= MaxValue)
            {
                _literal = value;
                Formatted = "";
                while (Formatted.Length < 3)
                {
                    Formatted = $"0{Formatted}";
                }
            }
            else
            {
                throw new ArgumentException($"{value} is outside the allowed range of the LotNumber class.");
            }
        }
    }

    /// <summary>
    /// A formatted version of the Lot Number's literal value. 
    /// Prepends '0' digit characters to the front of the string to enforce nine-digit formatting requirements.
    /// </summary>
    public string Formatted { get; private set; }

    /// <summary>
    /// Creates a new LotNumber from Value.
    /// </summary>
    /// <param name="Value"></param>
    public LotNumber(int Value)
    {
        // confirm that Value falls within the allowed literal range
        if (Value > MaxValue || Value < MinValue)
        {
            throw new ArgumentException($"'{Value}' is outside the allowed range of the LotNumber class.", nameof(Value));
        }
        Literal = Value;
        Formatted = Literal.ToString();
        // configure the Formatted property on instantiation
        while (Formatted.Length < 9)
        {
            Formatted = $"0{Formatted}";
        }
    }
}