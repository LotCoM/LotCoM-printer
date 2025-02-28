using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Printing;
using LotCoMPrinter.Models.Serialization;
using LotCoMPrinter.Models.Validators;

namespace LotCoMPrinter.ViewModels;

# pragma warning disable CA1416 // Validate platform compatibility

/// <summary>
/// Constructs a ViewModel for the MainPage class.
/// </summary>
public partial class MainPageViewModel : ObservableObject {
    // public class properties
    private List<string> _processes = ProcessData.ProcessMasterList;
    public List<string> Processes {
        get {return _processes;}
    }
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
    private string _displayedModel = "";
    public string DisplayedModel {
        get {return _displayedModel;}
        set {
            _displayedModel = value;
            OnPropertyChanged(nameof(_displayedModel));
            OnPropertyChanged(nameof(DisplayedModel));
        }
    }
    private string _displayedJBKNumber = "";
    public string DisplayedJBKNumber {
        get {return _displayedJBKNumber;}
        set {
            _displayedJBKNumber = value;
            OnPropertyChanged(nameof(_displayedJBKNumber));
            OnPropertyChanged(nameof(DisplayedJBKNumber));
        }
    }
    private string _displayedLotNumber = "";
    public string DisplayedLotNumber {
        get {return _displayedLotNumber;}
        set {
            _displayedLotNumber = value;
            OnPropertyChanged(nameof(_displayedLotNumber));
            OnPropertyChanged(nameof(DisplayedLotNumber));
        }
    }
    private string _basketType = "Full";
    public string BasketType {
        get {return _basketType;}
        set {
            _basketType = value;
            OnPropertyChanged(nameof(_basketType));
            OnPropertyChanged(nameof(BasketType));
        }
    }
    
    
    // full constructor
    public MainPageViewModel() {}

    /// <summary>
    /// Uses the SelectedPart property to attempt to imply the Model number.
    /// </summary>
    /// <returns>Model # if implication is successful; raises ArgumentException if not.</returns>
    /// <exception cref="ArgumentException"></exception>"
    private async Task<string> AttemptModelNumberImplication() {
        // get the Part's Model number and update the SelectedModel property
        string Model;
        try {
            Model = await ModelData.AttemptModelFromPart(SelectedPart);
        } catch (AggregateException _ex) {
            // could not automatically determine Model number from Part selection
            throw new ArgumentException($"Could not imply Model # from {SelectedPart} due to the following exception(s):"
                                        + $"\n{_ex.InnerExceptions}");
        }
        // Model implication did not fail; return
        return Model;
    }

    /// <summary>
    /// Configures the PartPicker control and the SelectedPart ViewModel property.
    /// </summary>
    /// <param name="PartPicker">The UI's Picker control for the Part.</param>
    /// <returns>true if the selected part is valid; false if not.</returns>
    private async Task<bool> ConfigureSelectedPart(Picker PartPicker) {
        bool Result = await Task.Run(() => {
            // get the PartPicker's selected item
            if (PartPicker.SelectedIndex >= 0) {
                var PickedPart = (string?)PartPicker.ItemsSource[PartPicker.SelectedIndex];
                // update the SelectedPart properties
                if (PickedPart != null) {
                    SelectedPart = PickedPart;
                    return true;
                } else {
                    SelectedPart = "";
                    return false;
                }
            } else {
                SelectedPart = "";
                return false;
            }
        });
        return Result;
    }

    /// <summary>
    /// Configures the ModelNumberEntry control and the DisplayedModel ViewModel property using the SelectedPart property.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task ConfigureSelectedModelNumber() {
        // get the Part's Model number and update the DisplayedModel property to only include the implied Model
        try {
            string ModelNumber = await AttemptModelNumberImplication();
            DisplayedModel = ModelNumber;
        } catch {
            throw new ArgumentException($"Could not configure the Model # for the Part {SelectedPart}.");
        }
    }

    /// <summary>
    /// Updates the Page's Selected Process and its Part Data.
    /// </summary>
    /// <param name="Process">The Process name to configure the UI for.</param>
    public async Task UpdateSelectedProcess(string Process) {
        // update the SelectedProcess properties
        await Task.Run(async () => {
            SelectedProcess = Process;
            // get the Process Parts for the Picked Process and convert those parts to strings
            Dictionary<string, string> ProcessParts = new Dictionary<string, string> {};
            try {
                ProcessParts = await ProcessData.GetProcessPartData(SelectedProcess);
            } catch (AggregateException _ex) {
                App.AlertSvc!.ShowAlert(
                    "Unexpected Error", "There was an error retrieving Part Data for this Process. Please see management to resolve this issue."
                    + $"\n\nException Message(s): {_ex.InnerExceptions}"
                );
            }
            List<string> DisplayableParts = [];
            foreach (KeyValuePair<string, string> _pair in ProcessParts) {
                DisplayableParts = DisplayableParts.Append(ProcessData.GetPartAsDisplayable(_pair)).ToList();
            }
            // assign the new list of string parts to the SelectedProcessParts list
            SelectedProcessParts = DisplayableParts;
        });
    }

    /// <summary>
    /// Updates the Page's Selected Part and Model Number.
    /// Attempts to automatically assign a Model Number to the Model Picker UI control.
    /// </summary>
    /// <param name="PartPicker">The Picker UI Control that allows the selection of a Part.</param>
    /// <returns>Returns a boolean that indicates whether to disable the Model Picker (if the Model assignment was failed).</returns>
    public async Task<bool> UpdateSelectedPart(Picker PartPicker) {
        // configure the SelectedPart
        bool PartResult = await ConfigureSelectedPart(PartPicker);
        if (PartResult) {
            // attempt to imply and configure the Model Number of the Part
            try {
                await ConfigureSelectedModelNumber();
            // the Model Number implication failed (some fatal issue with part selection)
            } catch (ArgumentException _ex) {
                throw new SystemException("There was an unexpected error while retreiving the Model #. Please see management to resolve this issue."
                                          + $"\n\nException Message(s): {_ex.Message}");
            }
            // the part number was valid and all of its data was retrieved
            return true;
        // the selected part number was somehow invalid
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
    /// <param name="ModelNumberEntry"></param>
    /// <param name="ProductionDatePicker"></param>
    /// <param name="ProductionShiftPicker"></param>
    /// <returns></returns>
    public async Task PrintRequest(Picker PartPicker, Entry QuantityEntry, Entry JBKNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry, Entry ModelNumberEntry, DatePicker ProductionDatePicker, Picker ProductionShiftPicker, Entry OperatorIDEntry) {
        // attempt to validate the current UI status
        List<string> UICapture;
        try {
			UICapture = InterfaceCaptureValidator.Validate(SelectedProcess.Replace(" ", ""), 
				PartPicker, QuantityEntry, DeburrJBKNumberEntry, DieNumberEntry, ModelNumberEntry, 
                ProductionDatePicker, ProductionShiftPicker, OperatorIDEntry);
        // something was not valid in the UI
		} catch (FormatException) {
            // warnings are handled by the CaptureValidator; escape method as the print request cannot continue
            return;
        }
        // get the serialize mode for this Label
        string SerializeMode;
        if (JBKNumberEntry.IsVisible) {
			SerializeMode = "JBK";
		} else {
			SerializeMode = "Lot";
		}
        // assign/retrieve a cached serial number for this label
        Serializer LabelSerializer = new Serializer();
        string? SerialNumber = await LabelSerializer.Serialize(UICapture[1].Replace("Part: ", ""), DisplayedModel, SerializeMode, BasketType);
        if (SerialNumber == null) {
            throw new LabelBuildException("Failed to assign a Serial Number to the Label");
        }
        // assign the Serial Number to the JBK/Lot # field
        UICapture[3] = $"{SerializeMode}: {SerialNumber}";
        // create and run a Label print job
        LabelPrintJob Job = new LabelPrintJob(UICapture, BasketType);
        await Job.Run();
    }

    /// <summary>
    /// Updates the Page's Selected Basket Type (Full/Partial).
    /// </summary>
    /// <param name="BasketType">'Full' or 'Partial', as selected in the UI.</param>
    /// <returns></returns>
    public async Task UpdateBasketType(string BasketType) {
        // update the BasketType property
        await Task.Run(() => {
            this.BasketType = BasketType;
        });
    }

    /// <summary>
    /// Resets the ViewModel's public properties.
    /// </summary>
    public void Reset() {
        SelectedPart = "";
        DisplayedModel = "";
        DisplayedJBKNumber = "";
        DisplayedLotNumber = "";
        BasketType = "Full";
    }
}
# pragma warning restore CA1416 // Validate platform compatibility