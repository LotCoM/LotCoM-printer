using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;

namespace LotCoMPrinter.ViewModels;

/// <summary>
/// Constructs a ViewModel for the MainPage class.
/// </summary>
public partial class MainPageViewModel : ObservableObject {
    // public class properties
    private string? _selectedProcess;
    public string? SelectedProcess {
        get {return _selectedProcess;}
        set {
            OnPropertyChanged(_selectedProcess);
            OnPropertyChanged(SelectedProcess);
            _selectedProcess = value;
        }
    }

    private List<string?> _selectedProcessParts = [];
    public List<string?> SelectedProcessParts {
        get {return _selectedProcessParts;}
        set {
            _selectedProcessParts = value;
            OnPropertyChanged(nameof(_selectedProcessParts));
            OnPropertyChanged(nameof(SelectedProcessParts));
        }
    }

    // public process list
    private List<string> _processes = ProcessData.ProcessMasterList;
    public List<string> Processes {
        get {return _processes;}
    }
        
    // full constructor
    public MainPageViewModel() {}

    /// <summary>
    /// Updates the Page's Selected Process and its Part Data.
    /// </summary>
    /// <param name="ProcessPicker">The Picker UI Control that allows the selection of a Process.</param>
    public void UpdateSelectedProcess(Picker ProcessPicker) {
        // get the ProcessPicker's selected item
        var PickedProcess = (string?)ProcessPicker.ItemsSource[ProcessPicker.SelectedIndex];
        // update the SelectedProcess properties
        Task.Run(() => {
            if (PickedProcess != null) {
                SelectedProcess = PickedProcess;
                // get the Process Parts for the Picked Process and convert those parts to strings
                var ProcessParts = PartData.GetProcessParts(SelectedProcess);
                List<string?> DisplayableParts = [];
                foreach (KeyValuePair<string, string> _pair in ProcessParts) {
                    DisplayableParts = DisplayableParts.Append(PartData.GetPartAsString(_pair.Key)).ToList();
                }
                // assign the new list of string parts to the SelectedProcessParts list
                SelectedProcessParts = DisplayableParts;
            }
        });
    }
}