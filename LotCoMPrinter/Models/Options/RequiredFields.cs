using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides structured control over the requirement of data fields in the variable set.
/// </summary>
/// <param name="JBKNumber"></param>
/// <param name="LotNumber"></param>
/// <param name="DeburrJBKNumber"></param>
/// <param name="DieNumber"></param>
/// <param name="ModelNumber"></param>
/// <param name="HeatNumber"></param>
public partial class RequiredFields(bool JBKNumber, bool LotNumber, bool DeburrJBKNumber, bool DieNumber, bool ModelNumber, bool HeatNumber) : ObservableObject()
{
    [ObservableProperty]
    public partial bool JBKNumber {get; set;} = JBKNumber;

    [ObservableProperty]
    public partial bool LotNumber {get; set;} = LotNumber;
    
    [ObservableProperty]
    public partial bool DeburrJBKNumber {get; set;} = DeburrJBKNumber;
    
    [ObservableProperty]
    public partial bool DieNumber {get; set;} = DieNumber;
    
    [ObservableProperty]
    public partial bool ModelNumber {get; set;} = ModelNumber;
    
    [ObservableProperty]
    public partial bool HeatNumber {get; set;} = HeatNumber;
}