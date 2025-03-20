using CommunityToolkit.Maui.Views;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.ViewModels;

namespace LotCoMPrinter.Views;

public partial class MainPage : ContentPage {
	// private property to store the ViewModel
	private readonly MainPageViewModel _viewModel;

	// full constructor
	public MainPage() {
		// instantiate the ViewModel and bind the Page to it
		_viewModel = new MainPageViewModel();
		BindingContext = _viewModel;

		// show the window from XAML
		InitializeComponent();

		// force the basket type to be full on start-up
		BasketTypePicker.SelectedIndex = 0;

		// hide the process type card on start-up
		ProcessTypeCard.IsVisible = false;
		ProcessTypeLabel.IsVisible = false;
	}

	/// <summary>
	/// Changes the visibility of Input Elements based on the Process selection.
	/// </summary>
	/// <returns></returns>
	public void ChangeDisplayedInputs() {
        // confirm there was a valid selection made in the Picker
        if (_viewModel.SelectedProcess != null) {
            // create a conversion dictionary for string names to control objects
            Dictionary<string, List<View>> Conversions = new Dictionary<string, List<View>> {
                {"SelectedProcess", new List<View> {ProcessControl, ProcessPicker, ProcessLabel}},
                {"JBKNumber", new List<View> {JBKNumberControl, JBKNumberEntry, JBKNumberLabel}},
                {"LotNumber", new List<View> {LotNumberControl, LotNumberEntry, LotNumberLabel}},
                {"DeburrJBKNumber", new List<View> {DeburrJBKNumberControl, DeburrJBKNumberEntry, DeburrJBKNumberLabel}},
                {"DieNumber", new List<View> {DieNumberControl, DieNumberEntry, DieNumberLabel}},
                {"SelectedPart", new List<View> {PartControl, PartPicker, PartLabel}},
                {"ModelNumber", new List<View> {ModelNumberControl, ModelNumberEntry, ModelNumberLabel}},
                {"Quantity", new List<View> {QuantityControl, QuantityEntry, QuantityLabel}},
                {"ProductionDate", new List<View> {ProductionDateControl, ProductionDatePicker, ProductionDateLabel}},
                {"ProductionShift", new List<View> {ProductionShiftControl, ProductionShiftPicker, ProductionShiftLabel}},
				{"OperatorID", new List<View> {OperatorIDControl, OperatorIDEntry, OperatorIDLabel}}
            };
			// get the process requirements for the currently selected Process
			List<string> Requirements = [];
			try {
				Requirements = ProcessRequirements.GetProcessRequirements(_viewModel.SelectedProcess.FullName);
			// the selected Process was invalid (uncommon)
			} catch (ArgumentException) {
				// show a warning
				BasicPopup Popup = new("Unexpected Error", "The selected Process' requirements could not be retrieved. Please see management to resolve this issue.");
				this.ShowPopup(Popup);
			}
			// show all necessary UI input elements
			foreach (KeyValuePair<string, List<View>> _pair in Conversions) {
				if (Requirements.Contains(_pair.Key)) {
					_pair.Value[0].IsVisible = true;
					_pair.Value[1].IsVisible = true;
					_pair.Value[2].IsVisible = true;
				} else {
					_pair.Value[0].IsVisible = false;
					_pair.Value[1].IsVisible = false;
					_pair.Value[2].IsVisible = false;
				}
			}
        	// confirm whether this label needs to be serialized or considered "pass-through"
			if (_viewModel.SelectedProcess.Type.Equals("Originator")) {
				// disable serial number inputs
				JBKNumberEntry.IsEnabled = false;
				LotNumberEntry.IsEnabled = false;
			} else {
				// enable serial number inputs
				JBKNumberEntry.IsEnabled = true;
				LotNumberEntry.IsEnabled = true;
			}
		}
    }

	/// <summary>
	/// Handler for the ItemSelected event from ProcessPicker.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public async void OnProcessSelection(object Sender, EventArgs e) {
		// update the SelectedProcess by invoking the ViewModel method
		Picker ProcessPicker = (Picker)Sender;
		await _viewModel.UpdateSelectedProcess(ProcessPicker);
		// reset the Page
		Reset();
		// update the SelectedProcess and change the visible UI elements
		try {
			// show the process type card
			ProcessTypeCard.IsVisible = true;
			ProcessTypeLabel.IsVisible = true;
		// there was some error involving the Process file
		} catch (FileLoadException) {
			BasicPopup Popup = new("Failed to Retrieve Data", "There was an error retrieving Part Data for this Process. Please see management to resolve this issue.");
			this.ShowPopup(Popup);
		// there are no Parts assigned to the Process
		} catch (ArgumentException) {
			BasicPopup Popup = new("Failed to Retrieve Data", "There are no Parts assigned to this Process.");
			this.ShowPopup(Popup);
		}
		// update the inputs either way
		ChangeDisplayedInputs();
	}

	/// <summary>
	/// Handler for the ItemSelected event from PartPicker.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public async void OnPartSelection(object Sender, EventArgs e) {
		// update the SelectedPart, DisplayedModel, and DisplayedJBKNumber properties
		Picker PartPicker = (Picker)Sender;
		try {
			await _viewModel.UpdateSelectedPart(PartPicker);
		// the Model Number was either unimplied or the JBK # Queue could not be accessed
		} catch (Exception _ex) {
			// show a warning
			BasicPopup Popup = new("Unexpected Error", $"The selected Part/Model # could not be retrieved. Please see management to resolve this issue.\n\nError: {_ex.Message}");
			this.ShowPopup(Popup);
		}
		// disable the Model Number control 
		ModelNumberEntry.IsEnabled = false;
	}

	/// <summary>
	/// Handler for the Pressed event from the PrintButton.
	/// Starts the print action using the information entered in the entries on the UI.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public async void OnPrintButtonPressed(object Sender, EventArgs e) {
		// start the Printing Indicator
		_viewModel.Printing = true;
		bool Printed;
		// call the ViewModel's Print Request method
		try {
			Printed = await _viewModel.PrintRequest(ProcessPicker, PartPicker, QuantityEntry, JBKNumberEntry, LotNumberEntry, DeburrJBKNumberEntry, DieNumberEntry, ModelNumberEntry, BasketTypePicker, ProductionDatePicker, ProductionShiftPicker, OperatorIDEntry);
		} catch (Exception _ex) {
			// stop the Printing Indicator
			_viewModel.Printing = false;
			// show a message based on the exception type
			if (_ex is NullProcessException) {
				// there was no process selection made
				BasicPopup Popup = new("Failed to Print", "Please select a Process before printing Labels.");
				this.ShowPopup(Popup);
			} else if (_ex is ArgumentException) {
				// there was an error retrieving the process data
				BasicPopup Popup = new("Failed to Print", "The selected Process' requirements could not be retrieved. Please see management to resolve this issue.");
				this.ShowPopup(Popup);
			} else if (_ex is FormatException) {
				// there was a failed UI validation
				BasicPopup Popup = new("Invalid Production Data.", _ex.Message);
				this.ShowPopup(Popup);
			} else if (_ex is LabelBuildException) {
				// there was an error serializing the Label
				BasicPopup Popup = new("Failed to Print", "Could not apply a Serial Number to the Label. Please see management to resolve this issue.");
				this.ShowPopup(Popup);
			} else if (_ex is PrintRequestException) {
				// there was an error communicating with the Printer or Printing System
				BasicPopup Popup = new("Failed to Print", "Could not connect to the printer. Please see management to resolve this issue.");
				this.ShowPopup(Popup);
			}
			// escape the handler
			return;
		}
		// reset UI, show a confirmation if print was successful
		if (Printed) {
			Reset();
			// stop printing indicator
			_viewModel.Printing = false;
			BasicPopup Popup = new("Label Printed", "The Label was printed successfully.");
			this.ShowPopup(Popup);
		// the print failed for some reason; show a warning
		} else {
			// stop printing indicator
			_viewModel.Printing = false;
			BasicPopup Popup = new("Failed to Print", "The system failed to print this Label. Please try again or see management to resolve this issue.");
			this.ShowPopup(Popup);
		}
	}

	/// <summary>
	/// Handler for the Item Selected event from the BasketTypePicker.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public async void OnBasketTypeSelection(object Sender, EventArgs e) {
		// update the BasketType ViewModel property
		Picker BasketTypePicker = (Picker)Sender;
		string? BasketType = (string?)BasketTypePicker.ItemsSource[BasketTypePicker.SelectedIndex];
		if (BasketType != null) {
			await _viewModel.UpdateBasketType(BasketType);
		}
	}

	/// <summary>
	/// Clears and reactivates all UI Controls on the Page.
	/// </summary>
	public void Reset() {
		// reset viewmodel properties
		_viewModel.Reset();
		// Basket Type Picker reset
		BasketTypePicker.SelectedIndex = 0;
		BasketTypePicker.IsEnabled = true;
		// Part Picker reset
		PartPicker.SelectedIndex = -1;
		PartPicker.IsEnabled = true;
		// Quantity Input reset
		QuantityEntry.Text = "";
		QuantityEntry.IsEnabled = true;
		// Deburr JBK Input reset
		DeburrJBKNumberEntry.Text = "";
		DeburrJBKNumberEntry.IsEnabled = true;
		// Die Number Input reset
		DieNumberEntry.Text = "";
		DieNumberEntry.IsEnabled = true;
		// Model Number Picker reset
		ModelNumberEntry.Text = "";
		ModelNumberEntry.IsEnabled = true;
		// Production Date Picker reset
		ProductionDatePicker.Date = DateTime.Now;
		ProductionDatePicker.IsEnabled = true;
		// Production Shift Picker reset
		ProductionShiftPicker.SelectedIndex = -1;
		ProductionShiftPicker.IsEnabled = true;
		// Operator Initials Entry reset
		OperatorIDEntry.Text = "";
		OperatorIDEntry.IsEnabled = true;
		// re-enable serial number inputs if serialization is not needed
		if (_viewModel.SelectedProcess == null) {
			return;
		}
		if (!_viewModel.SelectedProcess.Type.Equals("Originator")) {
			// JBK Input reset
			JBKNumberEntry.Text = "";
			JBKNumberEntry.IsEnabled = true;
			// Lot Input reset
			LotNumberEntry.Text = "";
			LotNumberEntry.IsEnabled = true;
		}
	}
}

