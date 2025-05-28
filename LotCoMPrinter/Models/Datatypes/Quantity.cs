using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datatypes;

public partial class Quantity : ObservableObject
{
    /// <summary>
    /// The number of items represented by this Quantity object.
    /// </summary>
    [ObservableProperty]
    public partial int Value { get; set; }

    /// <summary>
    /// Creates a new Quantity to represent a number of items.
    /// </summary>
    /// <param name="Value">The count of items this Quantity should represent.</param>
    /// <exception cref="ArgumentException"></exception>
    public Quantity(int Value)
    {
        // ensure that the Quantity will represent at least 1 item
        if (Value < 1)
        {
            throw new ArgumentException("Cannot instantiate a Quantity that represents less than 1 item.", nameof(Value));
        }
    }
}