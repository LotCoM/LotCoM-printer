using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Options;

/// <summary>
/// Provides a structure to capture Data related to a Partially-completed Basket.
/// </summary>
public partial class PartialDataSet(int Quantity, VariableFieldSet VariableFields, int Shift, string Operator) : ObservableObject()
{
    private int Quantity = Quantity;

    private VariableFieldSet VariableFields = VariableFields;

    private int Shift = Shift;

    private string Operator = Operator;
}