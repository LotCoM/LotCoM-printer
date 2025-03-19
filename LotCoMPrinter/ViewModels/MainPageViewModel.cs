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
    private List<Process> _processes = ProcessData.GetProcesses();
    /// <summary>
    /// Serves the Process masterlist to the ProcessPicker Control.
    /// </summary>
    public List<Process> Processes {
        get {return _processes;}
    }
    private Process? _selectedProcess = null;
    /// <summary>
    /// Serves ProcessPicker's selected value as a Process object. 
    /// </summary>
    public Process? SelectedProcess {
        get {return _selectedProcess;}
        set {
            OnPropertyChanged(nameof(_selectedProcess));
            OnPropertyChanged(nameof(SelectedProcess));
            _selectedProcess = value;
        }
    }
    private List<Part>? _selectedProcessParts = null;
    /// <summary>
    /// Serves the Part Data associated with the Process in SelectedProcess.
    /// </summary>
    public List<Part>? SelectedProcessParts {
        get {return _selectedProcessParts;}
        set {
            _selectedProcessParts = value;
            OnPropertyChanged(nameof(_selectedProcessParts));
            OnPropertyChanged(nameof(SelectedProcessParts));
        }
    }
    private Part? _selectedPart = null;
    /// <summary>
    /// Serves PartPicker's selected value as a Part object.
    /// </summary>
    public Part? SelectedPart {
        get {return _selectedPart;}
        set {
            _selectedPart = value;
            OnPropertyChanged(nameof(_selectedPart));
            OnPropertyChanged(nameof(SelectedPart));
        }
    }
    private string _displayedJBKNumber = "";
    /// <summary>
    /// Serves the JBK Number currently displayed (when programmatically assigned).
    /// </summary>
    public string DisplayedJBKNumber {
        get {return _displayedJBKNumber;}
        set {
            _displayedJBKNumber = value;
            OnPropertyChanged(nameof(_displayedJBKNumber));
            OnPropertyChanged(nameof(DisplayedJBKNumber));
        }
    }
    private string _displayedLotNumber = "";
    /// <summary>
    /// Serves the Lot Number currently displayed (when programmatically assigned).
    /// </summary>
    public string DisplayedLotNumber {
        get {return _displayedLotNumber;}
        set {
            _displayedLotNumber = value;
            OnPropertyChanged(nameof(_displayedLotNumber));
            OnPropertyChanged(nameof(DisplayedLotNumber));
        }
    }
    private string _basketType = "Full";
    /// <summary>
    /// Serves BasketTypePicker's selected value.
    /// </summary>
    public string BasketType {
        get {return _basketType;}
        set {
            _basketType = value;
            OnPropertyChanged(nameof(_basketType));
            OnPropertyChanged(nameof(BasketType));
        }
    }
    private bool _printing = false;
    /// <summary>
    /// Serves the current status of the application (true if a LabelPrintJob is running; false if not).
    /// </summary>
    public bool Printing {
        get {return _printing;}
        set {
            _printing = value;
            OnPropertyChanged(nameof(_printing));
            OnPropertyChanged(nameof(Printing));
        }
    }
    
    // full constructor
    public MainPageViewModel() {}

    /// <summary>
    /// Configures the PartPicker control and the SelectedPart ViewModel property.
    /// </summary>
    /// <param name="PartPicker">The UI's Picker control for the Part.</param>
    /// <returns>true if the selected part is valid; false if not.</returns>
    private async Task<bool> ConfigureSelectedPart(Picker PartPicker) {
        bool Result = await Task.Run(async () => {
            // ensure the Picker has a selection beyond -1 (default)
            if (PartPicker.SelectedIndex <= 0) {
                SelectedPart = null;
                return false;
            }
            // get the PartPicker's selected item
            Part? PickedPart = (Part?)PartPicker.ItemsSource[PartPicker.SelectedIndex];
            // ensure the picked Part index is not a null object
            if (PickedPart!.Equals(null)) {
                SelectedPart = null;
                return false;
            }
            // update the SelectedPart property and return successful
            SelectedPart = await PartData.GetPartData(SelectedProcess!.FullName, PickedPart.PartNumber);
            return true;
        });
        // return the validity of the Part selection
        return Result;
    }

    /// <summary>
    /// Checks if the Process requires Serialization (is an origination process).
    /// If so, elicits the SerializeMode, checks for Cached Serial Numbers, and assigns a Serial Number to the Label.
    /// </summary>
    /// <param name="JBKNumberEntry"></param>
    /// <param name="UICapture"></param>
    /// <returns>Tuple holding the SerializeMode (null if not serialized) and an updated UICapture List.</returns>
    /// <exception cref="LabelBuildException"></exception>
    private async Task<Tuple<string, List<string>>> SerializeLabel(Entry JBKNumberEntry, List<string> UICapture) {
        // get the serialize mode for this Label
        string SerializeMode = "Lot";
        if (JBKNumberEntry.IsVisible) {
            SerializeMode = "JBK";
        }
        // serialize the label if needed
        if (SelectedProcess!.Type.Equals("Originator")) {
            string? SerialNumber;
            // assign/retrieve a cached serial number for this label
            SerialNumber = await Serializer.Serialize(UICapture[1].Replace("Part: ", ""), SelectedPart!.ModelNumber, SerializeMode, BasketType);
            // no serial number was assigned; this is fatal
            if (SerialNumber == null) {
                throw new LabelBuildException("Failed to assign a Serial Number to the Label");
            }
            // assign the Serial Number to the JBK/Lot # field
            UICapture[3] = $"{SerializeMode}: {SerialNumber}";
        }
        // return the SerializeMode and the updated UICapture
        return new Tuple<string, List<string>>(SerializeMode, UICapture);
    }

    /// <summary>
    /// Decides how to Head the Label, formats that field as a Header, and returns that string.
    /// </summary>
    /// <param name="SerializeMode"></param>
    /// <param name="UICapture"></param>
    /// <returns>string</returns>
    private static async Task<string> FormatLabelHeader(string SerializeMode, List<string> UICapture) {
        string LabelHeader = await Task.Run(() => {
            // decide to use the JBK or Date as the header
            string Header;
            if (SerializeMode == "JBK") {
                // header is the JBK # (remove "JBK #: ")
                Header = UICapture[3].Split(":")[1].Replace(" ", "");
            } else {
                // header is the MM/DD of the Production Date 
                // retrieve the Date from the UI Capture
                string Date = UICapture[^3];
                // remove the "Production Date: " field tag
                Date = Date.Split(":")[1].Replace(" ", "");
                // split the date into its MM/DD/YY-HH:MM:SS segments
                string[] SplitDate = Date.Split("/");
                // set the header to use the MM and DD fields
                Header = $"{SplitDate[0]}/{SplitDate[1]}";
            }
            return Header;
        });
        return LabelHeader;
    }

    /// <summary>
    /// Updates the Page's Selected Process and its Part Data.
    /// </summary>
    /// <param name="Process">The Process name to configure the UI for.</param>
    /// <returns></returns>
    public async Task UpdateSelectedProcess(string Process) {
        await Task.Run(async () => {
            // update the SelectedProcess properties
            SelectedProcess = await ProcessData.GetIndividualProcessData(Process);
            // get the Process Parts for the Picked Process and convert those parts to strings
            List<string> ProcessPartStrings = await PartData.GetDisplayableProcessParts(SelectedProcess.FullName);
            // assign the new list of Part (as objects) to the SelectedProcessParts list
            SelectedProcessParts = SelectedProcess.Parts;
        });
    }

    /// <summary>
    /// Updates the Page's Selected Part and Model Number.
    /// </summary>
    /// <param name="PartPicker">The Picker UI Control that allows the selection of a Part.</param>
    /// <returns>Returns a boolean that indicates whether to disable the Model Picker (if the Model assignment was failed).</returns>
    public async Task<bool> UpdateSelectedPart(Picker PartPicker) {
        // configure the SelectedPart
        bool PartResult = await ConfigureSelectedPart(PartPicker);
        if (PartResult) {
            // the part number was valid and all of its data was retrieved
            return true;
        // the selected part number was somehow invalid
        } else {
            return false;
        }
    }

    /// <summary>
    /// Processes a Print Request from the user. 
    /// Captures the interface and validates it, then creates a new Label object from that captured data.
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
    /// <param name="OperatorIDEntry"></param>
    /// <returns></returns>
    /// <exception cref="NullProcessException">Thrown if there was no selection in the ProcessPicker Control.</exception>
    /// <exception cref="ArgumentException">Thrown if the Process Data could not be retrieved.</exception>
    /// <exception cref="FormatException">Thrown if there was a failed validation.</exception>
    /// <exception cref="LabelBuildException">Thrown if there was an error creating, formatting, serializing, or printing the Label.</exception>
    /// <exception cref="PrintRequestException">Thrown if there was an error communicating with the Printer or the Printing System.</exception>
    public async Task<bool> PrintRequest(Picker PartPicker, Entry QuantityEntry, Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry, Entry ModelNumberEntry, DatePicker ProductionDatePicker, Picker ProductionShiftPicker, Entry OperatorIDEntry) {
        // attempt to validate the current UI status
        List<string> UICapture;
        try {
			UICapture = InterfaceCaptureValidator.Validate(SelectedProcess!, 
				PartPicker, QuantityEntry, JBKNumberEntry, LotNumberEntry, DeburrJBKNumberEntry, 
                DieNumberEntry, ModelNumberEntry, ProductionDatePicker, ProductionShiftPicker, OperatorIDEntry);
        // there was no process selected
		} catch (NullProcessException) {
            throw new NullProcessException();
        // there was a problem retrieving the process data
        } catch (ArgumentException) {
            throw new ArgumentException();
        // there was some invalid UI entry
        } catch (FormatException _ex) {
            throw new FormatException(_ex.Message);
        }
        // check for serialization
        Tuple<string, List<string>> SerializationResults;
        try {
            SerializationResults = await SerializeLabel(JBKNumberEntry, UICapture);
        // failed to cache a new serial number or assign a serial number at all
        } catch (Exception _ex) {
            throw new LabelBuildException($"Failed to Serialize the Label due to the following exception:\n {_ex}: {_ex.Message}.");
        }
        // UI state is valid and can be used to produce a label for the selected process
        // get the Serialization mode and the modified UICapture
        string SerializeMode = SerializationResults.Item1;
        UICapture = SerializationResults.Item2;
        // format the Label's header
        string Header = await FormatLabelHeader(SerializeMode, UICapture);
        // create and run a Label print job
        bool Printed = false;
        LabelPrintJob Job = new LabelPrintJob(UICapture, BasketType, Header);
        try { 
            Printed = await Job.Run();
        // the print job failed
        } catch (Exception _ex) {
            if (_ex is LabelBuildException) {
                // there was an error while constructing the Label to print
                throw new LabelBuildException($"There was an error creating this Label:\n {_ex}: {_ex.Message}.");
            } else if (_ex is PrintRequestException) {
                // there was an error while communicating with the Printer or Printing System
                throw new PrintRequestException($"There was an error communicating with the Printer:\n {_ex}: {_ex.Message}.");
            }
        }
        // return the print success state
        return Printed;
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
        SelectedPart = null;
        DisplayedJBKNumber = "";
        DisplayedLotNumber = "";
        BasketType = "Full";
    }
}
# pragma warning restore CA1416 // Validate platform compatibility