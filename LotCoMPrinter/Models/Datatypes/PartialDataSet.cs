using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Enums;
using LotCoMPrinter.Models.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datatypes;

/// <summary>
/// Provides a structure to capture Data related to a Partially-completed Basket.
/// </summary>
/// <param name="Quantity">The number of Parts produced during the Shift captured by the PartialDataSet.</param>
/// <param name="Shift">The Shift Number captured by the PartialDataSet.</param>
/// <param name="Operator">The Operator who created the PartialDataSet.</param>
public partial class PartialDataSet(Quantity? Quantity = null, Shift? Shift = null, OperatorInitials? Operator = null) : ObservableObject()
{
    /// <summary>
    /// The number of Parts produced during the Shift captured by the PartialDataSet.
    /// </summary>
    [ObservableProperty]
    public partial Quantity? Quantity { get; set; } = Quantity;

    /// <summary>
    /// The Shift Number captured by the PartialDataSet.
    /// </summary>
    [ObservableProperty]
    public partial Shift? Shift { get; set; } = Shift;

    /// <summary>
    /// The Operator who created the PartialDataSet.
    /// </summary>
    [ObservableProperty]
    public partial OperatorInitials? Operator { get; set; } = Operator;

    /// <summary>
    /// Attempts to parse and construct a PartialDataSet object from a JSON stream.
    /// </summary>
    /// <param name="JSON"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    public static PartialDataSet ParseJSON(JToken JSON)
    {
        // attempt to parse a Quantity, Shift Number, and Operator from the passed JSON stream
        Quantity Quantity;
        Shift Shift;
        OperatorInitials Operator;
        try
        {
            Quantity = new Quantity(int.Parse(JSON["Quantity"]!.ToString()));
        }
        catch
        {
            throw new JsonException($"Could not parse a Quantity value from {JSON["FirstPartialDataSet"]!["Quantity"]!}.");
        }
        try
        {
            Shift = ShiftExtensions.FromString(JSON!["Shift"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Quantity value from {JSON["FirstPartialDataSet"]!["Shift"]!}.");
        }
        try
        {
            Operator = new OperatorInitials(JSON["Operator"]!.ToString());
        }
        catch
        {
            throw new JsonException($"Could not parse a Quantity value from {JSON["FirstPartialDataSet"]!["Operator"]!}.");
        }
        return new PartialDataSet(Quantity, Shift, Operator);
    }
}