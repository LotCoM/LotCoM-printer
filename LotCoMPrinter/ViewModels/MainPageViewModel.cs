using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Printing;
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
            if (PartPicker.SelectedIndex != -1) {
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
    /// Configures the JBKNumberEntry control and the DisplayedJBKNumber ViewModel property using the DisplayedModel property.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task ConfigureJBKNumber() {
        // retrieve the Queued JBK number for the Displayed Model
        try {
            int _awaited = await JBKQueue.QueuedAsync(DisplayedModel);
            string Queued = _awaited.ToString();
            // add leading zeroes to enforce 3-digit format
            while (Queued.Length < 3) {
                Queued = "0" + Queued;
            }
            // update the Displayed JBK
            DisplayedJBKNumber = Queued;
        // there was no JBK queue for the passed Model Number
        } catch (ArgumentException) {
            throw new ArgumentException($"Could not find a JBK # Queue for the Model # {DisplayedModel}.");
        }
    }

    /// <summary>
    /// Configures the LotNumberEntry control and the DisplayedLotNumber ViewModel property using the DisplayedModel property.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task ConfigureLotNumber() {
        // retrieve the Queued Lot number for the Displayed Model
        try {
            int _awaited = await LotQueue.QueuedAsync(DisplayedModel);
            string Queued = _awaited.ToString();
            // add leading zeroes to enforce 9-digit format
            while (Queued.Length < 9) {
                Queued = "0" + Queued;
            }
            // update the Displayed Lot Number
            DisplayedLotNumber = Queued;
        // there was no Lot queue for the passed Model Number
        } catch (ArgumentException) {
            throw new ArgumentException($"Could not find a Lot # Queue for the Model # {DisplayedModel}.");
        }
    }

    /// <summary>
    /// Attempts to assign a serialized identifier (JBK or Lot #) to the Label in the UI.
    /// </summary>
    /// <param name="SerializeMode">The data field that serializes this Process' labels (either "JBK" or "Lot").</param>
    /// <returns></returns>
    /// <exception cref="SystemException"></exception>
    private async Task AssignSerializer(string SerializeMode) {
        await Task.Run(async () => {
            // attempt to assign a JBK number
            if (SerializeMode == "JBK") {
                try {
                    await ConfigureJBKNumber();
                } catch (Exception _ex) {
                    DisplayedJBKNumber = "";
                    throw new SystemException("Failed to assign a JBK # to this Label. Please see management to resolve this issue."
                                              + $"\n\nException Message(s): {_ex.Message}");
                }
            // attempt to assign a Lot number
            } else {
                try {
                    await ConfigureLotNumber();
                // the JBK AND Lot number queue reads failed (fatal)
                } catch (ArgumentException _ex) {
                    DisplayedLotNumber = "";
                    throw new SystemException("Failed to assign a Lot # to this Label. Please see management to resolve this issue."
                                              + $"\n\nException Message(s): {_ex.Message}");
                }
            }
        });
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
    /// Updates the Page's Selected Part, Model Number, and queued JBK Number.
    /// Attempts to automatically assign a Model Number to the Model Picker UI control.
    /// Attempts to automatically assign the queued JBK Number for that Model to the JBK Entry UI control.
    /// </summary>
    /// <param name="PartPicker">The Picker UI Control that allows the selection of a Part.</param>
    /// <param name="SerializeMode">The data field that serializes this Process' labels (either "JBK" or "Lot").</param>
    /// <returns>Returns a boolean that indicates whether to disable the Model Picker (if the Model assignment was failed).</returns>
    public async Task<bool> UpdateSelectedPart(Picker PartPicker, string SerializeMode) {
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
            // attempt to apply a serializer to the Label being generated (JBK/Lot #)
            try {
                await AssignSerializer(SerializeMode);
            } catch (Exception _ex) {
                throw new SystemException($"Failed to assign a {SerializeMode} # to this Label. Please see management to resolve this issue."
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
    public async Task PrintRequest(Picker PartPicker, Entry QuantityEntry, Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry, Entry ModelNumberEntry, DatePicker ProductionDatePicker, Picker ProductionShiftPicker, Entry OperatorIDEntry) {
        // attempt to validate the current UI status
        List<string> UICapture;
        try {
			UICapture = InterfaceCaptureValidator.Validate(SelectedProcess.Replace(" ", ""), 
				PartPicker, QuantityEntry, JBKNumberEntry, LotNumberEntry, DeburrJBKNumberEntry, 
				DieNumberEntry, ModelNumberEntry, ProductionDatePicker, ProductionShiftPicker, OperatorIDEntry);
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
        // create and run a Label print job
        LabelPrintJob Job = new LabelPrintJob(UICapture, SerializeMode, DisplayedModel);
        await Job.Run();
    }

    /// <summary>
    /// Resets the ViewModel's public properties.
    /// </summary>
    public void Reset() {
        SelectedProcess = "";
        SelectedProcessParts = [];
        SelectedPart = "";
        DisplayedModel = "";
    }
}
# pragma warning restore CA1416 // Validate platform compatibility