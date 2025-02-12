using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Labels;
using LotCoMPrinter.Models.Validators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace LotCoMPrinter.ViewModels;

/// <summary>
/// Constructs a ViewModel for the MainPage class.
/// </summary>
public partial class MainPageViewModel : ObservableObject {
    // public class properties
    private string _selectedProcess = "";
    public string SelectedProcess {
        get {return _selectedProcess;}
        set {
            OnPropertyChanged(_selectedProcess);
            OnPropertyChanged(SelectedProcess);
            _selectedProcess = value;
        }
    }

    private List<string> _selectedProcessParts = [];
    public List<string> SelectedProcessParts {
        get {return _selectedProcessParts;}
        set {
            _selectedProcessParts = value;
            OnPropertyChanged(nameof(_selectedProcessParts));
            OnPropertyChanged(nameof(SelectedProcessParts));
        }
    }

    private string _selectedPart = "";
    public string SelectedPart {
        get {return _selectedPart;}
        set {
            _selectedPart = value;
            OnPropertyChanged(nameof(_selectedPart));
            OnPropertyChanged(nameof(SelectedPart));
        }
    }

    private List<string> _displayedModels = [];
    public List<string> DisplayedModels {
        get {return _displayedModels;}
        set {
            _displayedModels = value;
            OnPropertyChanged(nameof(_displayedModels));
            OnPropertyChanged(nameof(DisplayedModels));
        }
    }
    
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
    public async Task UpdateSelectedProcess(Picker ProcessPicker) {
        // get the ProcessPicker's selected item
        if (ProcessPicker.SelectedIndex != -1) {
            var PickedProcess = (string?)ProcessPicker.ItemsSource[ProcessPicker.SelectedIndex];
            // update the SelectedProcess properties
            await Task.Run(() => {
                if (PickedProcess != null) {
                    SelectedProcess = PickedProcess;
                    // get the Process Parts for the Picked Process and convert those parts to strings
                    var ProcessParts = PartData.GetProcessParts(SelectedProcess);
                    List<string> DisplayableParts = [];
                    foreach (KeyValuePair<string, string> _pair in ProcessParts) {
                        try {
                            DisplayableParts = DisplayableParts.Append(PartData.GetPartAsString(_pair.Key)).ToList();
                        // part number was not found in the Parts masterlist; skip it
                        } catch {continue;}
                    }
                    // assign the new list of string parts to the SelectedProcessParts list
                    SelectedProcessParts = DisplayableParts;
                }
            });
        }
    }

    /// <summary>
    /// Uses the SelectedPart property to attempt to imply the Model number.
    /// </summary>
    /// <returns>Model # if implication is successful; raises ArgumentException if not.</returns>
    private async Task<string> AttemptModelNumberImplication() {
        // get the Part's Model number and update the SelectedModel property
        string Model;
        try {
            Model = await ModelData.AttemptModelFromPart(SelectedPart);
        } catch {
            // could not automatically determine Model number from Part selection
            throw new ArgumentException();
        }
        // Model implication did not fail; return
        return Model;
    }

    /// <summary>
    /// Updates the Page's Selected Part and Model Number.
    /// Attempts to automatically assign a Model Number to the Model Picker UI control.
    /// </summary>
    /// <param name="PartPicker">The Picker UI Control that allows the selection of a Part.</param>
    /// <returns>Returns a boolean that indicates whether to disable the Model Picker (if the Model assignment was failed).</returns>
    public async Task<bool> UpdateSelectedPart(Picker PartPicker) {
        // get the PartPicker's selected item
        if (PartPicker.SelectedIndex != -1) {
            var PickedPart = (string?)PartPicker.ItemsSource[PartPicker.SelectedIndex];
            // update the SelectedPart properties
            if (PickedPart != null) {
                SelectedPart = PickedPart;
                // get the Part's Model number and update the DisplayedModel property to only include the implied Model
                try {
                    string ModelNumber = await AttemptModelNumberImplication();
                    DisplayedModels = [ModelNumber];
                    return true;
                } catch (ArgumentException) {
                    return false;
                }
            // the PickedPart was null
            } else {
                return false;
            }
        // the Selection index was -1 (invalid; no selection)
        } else {
            return false;
        }
    }

    /// <summary>
    /// Processes a Print Request from the user 
    /// (captures the interface and validates it, then creates a new Label object from that captured data).
    /// </summary>
    /// <param name="PartPicker"></param>
    /// <param name="QuantityEntry"></param>
    /// <param name="JBKNumberEntry"></param>
    /// <param name="LotNumberEntry"></param>
    /// <param name="DeburrJBKNumberEntry"></param>
    /// <param name="DieNumberEntry"></param>
    /// <param name="ModelNumberPicker"></param>
    /// <param name="ProductionDatePicker"></param>
    /// <param name="ProductionShiftPicker"></param>
    /// <returns></returns>
    public async Task PrintRequest(Picker PartPicker, Entry QuantityEntry, Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry, Picker ModelNumberPicker, DatePicker ProductionDatePicker, Picker ProductionShiftPicker) {
        // attempt to validate the current UI status
        List<string> UICapture = [];
        try {
			UICapture = InterfaceCaptureValidator.Validate(SelectedProcess.Replace(" ", ""), 
				PartPicker, QuantityEntry, JBKNumberEntry, LotNumberEntry, DeburrJBKNumberEntry, 
				DieNumberEntry, ModelNumberPicker, ProductionDatePicker, ProductionShiftPicker);
        // something was not valid in the UI
		} catch (FormatException) {
            // warnings are handled by the CaptureValidator
        }
		// generate a Label from the captured data
		Image<Rgba32> NewLabel = await LabelGenerator.GenerateLabel(JBKNumberEntry.Text, UICapture);
        // do print activities
        await Task.Delay(0);
    }

    /// <summary>
    /// Resets the ViewModel's public properties.
    /// </summary>
    public void Reset() {
        SelectedProcess = "";
        SelectedProcessParts = [];
        SelectedPart = "";
        DisplayedModels = [];
    }
}