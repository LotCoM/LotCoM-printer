using CommunityToolkit.Mvvm.ComponentModel;
using LCMSWipPrinter.Models.Datasources;

namespace LCMSWipPrinter.ViewModels;

/// <summary>
/// Constructs a ViewModel for the MainPage class.
/// </summary>
public partial class MainPageViewModel : ObservableObject {
    // public class properties
    private string? _selectedProcess;
    public string? SelectedProcess {
        get {return _selectedProcess;}
        set {_selectedProcess = value;}
    }

    // public process list
    private List<string> _processes = ProcessData.ProcessMasterList;
    public List<string> Processes {
        get {return _processes;}
    }
    
    // public part lists
    public List<string> DiecastParts = PartData.DiecastPartsAsString;
    public List<string> DeburrParts = PartData.DeburrPartsAsString;
    public List<string> PivotHousingMCParts = PartData.PivotHousingMCPartsAsString;
    
    // full constructor
    public MainPageViewModel() {

    }
}