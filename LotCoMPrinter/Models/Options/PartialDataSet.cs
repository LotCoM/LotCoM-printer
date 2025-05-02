using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Options;

/// <summary>
/// Provides a structure to capture Data related to a Partially-completed Basket.
/// </summary>
/// <param name="Quantity">The number of Parts produced during the Shift captured by the PartialDataSet.</param>
/// <param name="Shift">The Shift Number captured by the PartialDataSet.</param>
/// <param name="Operator">The Operator who created the PartialDataSet.</param>
public partial class PartialDataSet(int Quantity, int Shift, string Operator) : ObservableObject()
{
    /// <summary>
    /// The number of Parts produced during the Shift captured by the PartialDataSet.
    /// </summary>
    public int Quantity = Quantity;

    /// <summary>
    /// The Shift Number captured by the PartialDataSet.
    /// </summary>
    public int Shift = Shift;

    /// <summary>
    /// The Operator who created the PartialDataSet.
    /// </summary>
    public string Operator = Operator;
}