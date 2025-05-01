using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Options;

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