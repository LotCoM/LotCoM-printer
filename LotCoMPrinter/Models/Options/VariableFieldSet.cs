using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Options;

/// <summary>
/// Provides structured control over the values of each of the data fields in the variable set.
/// </summary>
/// <param name="JBKNumber"></param>
/// <param name="LotNumber"></param>
/// <param name="DeburrJBKNumber"></param>
/// <param name="DieNumber"></param>
/// <param name="ModelNumber"></param>
/// <param name="HeatNumber"></param>
public partial class VariableFieldSet(int? JBKNumber = null, string? LotNumber = null, int? DeburrJBKNumber = null, int? DieNumber = null, string? ModelNumber = null, string? HeatNumber = null) : ObservableObject ()
{
    [ObservableProperty]
    public partial int? JBKNumber {get; set;} = JBKNumber;

    [ObservableProperty]
    public partial string? LotNumber {get; set;} = LotNumber;

    [ObservableProperty]
    public partial int? DeburrJBKNumber {get; set;} = DeburrJBKNumber;

    [ObservableProperty]
    public partial int? DieNumber {get; set;} = DieNumber;

    [ObservableProperty]
    public partial string? ModelNumber {get; set;} = ModelNumber;

    [ObservableProperty]
    public partial string? HeatNumber {get; set;} = HeatNumber;
}