using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Options;

/// <summary>
/// Provides a structure to capture Data related to a Partially-completed Basket.
/// </summary>
/// <param name="Quantity">The number of Parts produced during the Shift captured by the PartialDataSet.</param>
/// <param name="Shift">The Shift Number captured by the PartialDataSet.</param>
/// <param name="Operator">The Operator who created the PartialDataSet.</param>
public partial class PartialDataSet(int? Quantity = null, int? Shift = null, string? Operator = null) : ObservableObject()
{
    /// <summary>
    /// The number of Parts produced during the Shift captured by the PartialDataSet.
    /// </summary>
    [ObservableProperty]
    public partial int? Quantity {get; set;} = Quantity;

    /// <summary>
    /// The Shift Number captured by the PartialDataSet.
    /// </summary>
    [ObservableProperty]
    public partial int? Shift {get; set;} = Shift;

    /// <summary>
    /// The Operator who created the PartialDataSet.
    /// </summary>
    [ObservableProperty]
    public partial string? Operator {get; set;} = Operator;

    /// <summary>
    /// Attempts to parse and construct a PartialDataSet object from a JSON stream.
    /// </summary>
    /// <param name="JSON"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    public static PartialDataSet ParseJSON(JToken JSON)
    {
        // attempt to parse a Quantity, Shift Number, and Operator from the passed JSON stream
        int Quantity;
        int Shift;
        string Operator;
        try
        {
            Quantity = int.Parse(JSON["Quantity"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Quantity value from {JSON["FirstPartialDataSet"]!["Quantity"]!}.");
        }
        try
        {
            Shift = int.Parse(JSON!["Shift"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Quantity value from {JSON["FirstPartialDataSet"]!["Shift"]!}.");
        }
        try
        {
            Operator = JSON["Operator"]!.ToString();
        }
        catch
        {
            throw new JsonException($"Could not parse a Quantity value from {JSON["FirstPartialDataSet"]!["Operator"]!}.");
        }
        return new PartialDataSet(Quantity, Shift, Operator);
    }
}