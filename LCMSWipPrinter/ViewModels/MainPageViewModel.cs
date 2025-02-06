using CommunityToolkit.Mvvm.ComponentModel;

namespace LCMSWipPrinter.ViewModels;

/// <summary>
/// Constructs a ViewModel for the MainPage class.
/// </summary>
public partial class MainPageViewModel : ObservableObject {
    // public class properties
    private string? _process;
    public string? Process {
        get {return _process;}
        set {_process = value;}
    }

    // full constructor
    public MainPageViewModel() {
        
    }
}