using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// A manufacturing tracing identifier for Basket Labels.
/// </summary>
public partial class HeatNumber : ObservableObject
{
    /// <summary>
    /// The absolute lowest digit literal that can be assigned to a Heat Number.
    /// </summary>
    private const int MinValue = 1;

    /// <summary>
    /// The absolute highest digit literal that can be assigned to a Heat Number.
    /// </summary>
    private const int MaxValue = 999999999;

    /// <summary>
    /// The raw literal value of the Heat Number.
    /// </summary>
    [ObservableProperty]
    public partial int Literal { get; set; }

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