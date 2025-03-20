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
    private string _displayedModelNumber = "";
    /// <summary>
    /// Serves the Model Number that the current SelectedPart is associated with.
    /// </summary>
    public string DisplayedModelNumber {
        get {return _displayedModelNumber;}
        set {
            _displayedModelNumber = value;
            OnPropertyChanged(nameof(_displayedModelNumber));
            OnPropertyChanged(nameof(DisplayedModelNumber));
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
    /// Checks if the Process requires Serialization (is an origination process).
    /// If so, elicits the Serialization Mode, checks for Cached Serial Numbers, and assigns a Serial Number to the Label.
    /// </summary>
    /// <param name="Capture"></param>
    /// <returns>An updated InterfaceCapture object.</returns>
    /// <exception cref="LabelBuildException"></exception>
    private async Task<InterfaceCapture> SerializeLabel(InterfaceCapture Capture) {
        // retrieve values to save processing time (will not be null here; post-validation)
        Process SelectedProcess = Capture.SelectedProcess!;
        string Serialization = SelectedProcess.Serialization;
        // check if the SelectedProcess is an Originator; if not, just return the passed Capture
        if (!SelectedProcess.Type.Equals("Originator")) {
            return Capture;
        }
        // serialize the Label using the Process' Serialization Mode
        string? SerialNumber = await Serializer.Serialize(Capture);
        // no serial number was assigned; this is fatal
        if (SerialNumber == null) {
            throw new LabelBuildException("Failed to assign a Serial Number to the Label");
        }
        // update the Serialized Number in the Capture object
        if (Serialization == "JBK") {
            Capture.JBKNumber = SerialNumber;
        } else {
            Capture.LotNumber = SerialNumber;
        }
        // return the updated Capture object
        return Capture;
    }

    /// <summary>
    /// Decides how to Head the Label, formats that field as a Header, and returns that string.
    /// </summary>
    /// <param name="Capture"></param>
    /// <returns>A string to use as the Label Header text.</returns>
    private static async Task<string> FormatLabelHeader(InterfaceCapture Capture) {
        string LabelHeader = await Task.Run(() => {
            // retrieve values to improve processing time
            Process SelectedProcess = Capture.SelectedProcess!;
            string Serialization = SelectedProcess.Serialization;
            // decide to use the JBK or Date as the header
            string Header;
            if (Serialization.Equals("JBK")) {
                // header is the JBK # (remove "JBK #: ")
                Header = Capture.JBKNumber!;
            } else {
                // header is the MM/DD of the Production Date; retrieve the Date from the UI Capture
                DateTime Date = Capture.ProductionDate;
                Header = $"{Date.Month}/{Date.Day}";
            }
            return Header;
        });
        return LabelHeader;
    }

    /// <summary>
    /// Creates, validates, and formats an InterfaceCapture object from the current UI status.
    /// </summary>
    /// <param name="ProcessPicker"></param>
    /// <param name="PartPicker"></param>
    /// <param name="QuantityEntry"></param>
    /// <param name="JBKNumberEntry"></param>
    /// <param name="LotNumberEntry"></param>
    /// <param name="DeburrJBKNumberEntry"></param>
    /// <param name="DieNumberEntry"></param>
    /// <param name="ModelNumberEntry"></param>
    /// <param name="BasketTypePicker"></param>
    /// <param name="ProductionDatePicker"></param>
    /// <param name="ProductionShiftPicker"></param>
    /// <param name="OperatorIDEntry"></param>
    /// <returns>An InterfaceCapture object.</returns>
    /// <exception cref="NullProcessException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    private static async Task<InterfaceCapture> CreateCapture(Picker ProcessPicker, Picker PartPicker, Entry QuantityEntry, Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry, Entry ModelNumberEntry, Picker BasketTypePicker, DatePicker ProductionDatePicker, Picker ProductionShiftPicker, Entry OperatorIDEntry) {
        // create an interface capture for this UI state
        InterfaceCapture Capture = new InterfaceCapture();
        Capture.Capture(ProcessPicker, PartPicker, QuantityEntry, JBKNumberEntry, LotNumberEntry, DeburrJBKNumberEntry, DieNumberEntry, ModelNumberEntry, BasketTypePicker, ProductionDatePicker, ProductionShiftPicker, OperatorIDEntry);
        // validate the Capture
        try {
            Capture = await InterfaceCaptureValidator.Validate(Capture);
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
        // the Capture is valid and processed; return it
        return Capture;
    }

    /// <summary>
    /// Updates the Page's Selected Process and its Part Data.
    /// </summary>
    /// <param name="ProcessPicker"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>"
    public async Task UpdateSelectedProcess(Picker ProcessPicker) {
        await Task.Run(() => {
            // retrieve the selected Process
		    Process? PickedProcess = (Process?)Processes[ProcessPicker.SelectedIndex];
            // update the SelectedProcess properties
            if (ProcessPicker.SelectedIndex == -1) {
                return;
            }
            if (PickedProcess == null) {
                throw new ArgumentException("Process was not found in Process masterlist.");
            }
            // update SelectedProcess and assign the new list of Parts (as objects) to the SelectedProcessParts list
            SelectedProcess = PickedProcess;
            SelectedProcessParts = SelectedProcess.Parts;
        });
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
    /// Updates the Page's Selected Part and Model Number.
    /// </summary>
    /// <param name="PartPicker">The Picker UI Control that allows the selection of a Part.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>"
    public async Task UpdateSelectedPart(Picker PartPicker) {
        await Task.Run(() => {
            // configure the SelectedPart property
            try {
                // get the PartPicker's selected item
                if (PartPicker.SelectedIndex == -1) {
                    return;
                }
                Part? PickedPart = (Part?)PartPicker.ItemsSource[PartPicker.SelectedIndex];
                // update the SelectedPart property and the DisplayedModelNumber property
                if (PickedPart == null) {
                    throw new ArgumentException("Part was not found in Process Part list.");
                }
                SelectedPart = PickedPart;
                DisplayedModelNumber = SelectedPart.ModelNumber;
            // the selected part number was somehow invalid
            } catch (ArgumentException) {
                throw new ArgumentException("Part was not found in Process Part list.");
            }
        });
    }

    /// <summary>
    /// Processes a Print Request from the user. 
    /// Captures the interface and validates it, then creates a new Label object from that captured data.
    /// </summary>
    /// <remarks>
    /// Throws NullProcessException if there was no selection in the ProcessPicker Control.
    /// Throws ArgumentException if the Process Data could not be retrieved.
    /// Throws FormatException if there was a failed validation.
    /// Throws LabelBuildException if there was an error creating, formatting, serializing, or printing the Label.
    /// Throws PrintRequestException if there was an error communicating with the Printer or the Printing System.
    /// </remarks>
    /// <param name="ProcessPicker"></param>
    /// <param name="PartPicker"></param>
    /// <param name="QuantityEntry"></param>
    /// <param name="JBKNumberEntry"></param>
    /// <param name="LotNumberEntry"></param>
    /// <param name="DeburrJBKNumberEntry"></param>
    /// <param name="DieNumberEntry"></param>
    /// <param name="ModelNumberEntry"></param>
    /// <param name="BasketTypePicker"></param>
    /// <param name="ProductionDatePicker"></param>
    /// <param name="ProductionShiftPicker"></param>
    /// <param name="OperatorIDEntry"></param>
    /// <returns></returns>
    /// <exception cref="NullProcessException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="LabelBuildException"></exception>
    /// <exception cref="PrintRequestException"></exception>
    public async Task<bool> PrintRequest(Picker ProcessPicker, Picker PartPicker, Entry QuantityEntry, Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry, Entry ModelNumberEntry, Picker BasketTypePicker, DatePicker ProductionDatePicker, Picker ProductionShiftPicker, Entry OperatorIDEntry) {
        // capture the interface
        InterfaceCapture Capture;
        try {
            Capture = await CreateCapture(ProcessPicker, PartPicker, QuantityEntry, JBKNumberEntry, LotNumberEntry, DeburrJBKNumberEntry, DieNumberEntry, ModelNumberEntry, BasketTypePicker, ProductionDatePicker, ProductionShiftPicker, OperatorIDEntry);
        // there was no process selection made
        } catch (NullProcessException) {
            throw new NullProcessException();
        // there was a problem retrieving process data for the selected process
        } catch (ArgumentException) {
            throw new ArgumentException();
        // a validation failed
        } catch (FormatException _ex) {
            throw new FormatException(_ex.Message);
        }
        // serialize the label (if needed)
        try {
            Capture = await SerializeLabel(Capture);
        // failed to cache a new serial number or assign a serial number at all
        } catch (Exception _ex) {
            throw new LabelBuildException($"Failed to Serialize the Label due to the following exception:\n {_ex}: {_ex.Message}.");
        }
        // UI state is valid; format the Label's header
        string Header = await FormatLabelHeader(Capture);
        // create and run a Label print job
        bool Printed = false;
        LabelPrintJob Job = new LabelPrintJob(Capture, Header);
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