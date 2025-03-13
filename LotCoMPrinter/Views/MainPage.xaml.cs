using LotCoMPrinter.Models.Datasources;
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
                {"ProcessPicker", new List<View> {ProcessControl, ProcessPicker, ProcessLabel}},
                {"JBKNumberEntry", new List<View> {JBKNumberControl, JBKNumberEntry, JBKNumberLabel}},
                {"LotNumberEntry", new List<View> {LotNumberControl, LotNumberEntry, LotNumberLabel}},
                {"DeburrJBKNumberEntry", new List<View> {DeburrJBKNumberControl, DeburrJBKNumberEntry, DeburrJBKNumberLabel}},
                {"DieNumberEntry", new List<View> {DieNumberControl, DieNumberEntry, DieNumberLabel}},
                {"PartPicker", new List<View> {PartControl, PartPicker, PartLabel}},
                {"ModelNumberEntry", new List<View> {ModelNumberControl, ModelNumberEntry, ModelNumberLabel}},
                {"QuantityEntry", new List<View> {QuantityControl, QuantityEntry, QuantityLabel}},
                {"ProductionDatePicker", new List<View> {ProductionDateControl, ProductionDatePicker, ProductionDateLabel}},
                {"ProductionShiftPicker", new List<View> {ProductionShiftControl, ProductionShiftPicker, ProductionShiftLabel}},
				{"OperatorIDEntry", new List<View> {OperatorIDControl, OperatorIDEntry, OperatorIDLabel}}
            };
			// get the process requirements for the currently selected Process
			List<string> Requirements = [];
			try {
				Requirements = ProcessRequirements.GetProcessRequirements(_viewModel.SelectedProcess);
			// the selected Process was invalid (uncommon)
			} catch (ArgumentException) {
				// show a warning
				App.AlertSvc!.ShowAlert("Unexpected Error", "The selected Process' requirements could not be retrieved. Please see management to resolve this issue.");
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
			if (_viewModel.IsOriginator) {
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
		// retrieve the picked process
		Picker ProcessPicker = (Picker)Sender;
		string? PickedProcess = (string?)ProcessPicker.ItemsSource[ProcessPicker.SelectedIndex];
		// reset the Page
		Reset();
		// update the SelectedProcess and change the visible UI elements
		if (PickedProcess != null) {
			await _viewModel.UpdateSelectedProcess(PickedProcess);
			ChangeDisplayedInputs();
		}
	}

	/// <summary>
	/// Handler for the ItemSelected event from PartPicker.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public async void OnPartSelection(object Sender, EventArgs e) {
		// update the SelectedPart, DisplayedModel, and DisplayedJBKNumber properties
		Picker PartPicker = (Picker)Sender;
		bool PartSelection = false;
		try {
			PartSelection = await _viewModel.UpdateSelectedPart(PartPicker);
		// the Model Number was either unimplied or the JBK # Queue could not be accessed
		} catch (Exception _ex) {
			// show a warning
			App.AlertSvc!.ShowAlert("Unexpected Error", "The selected Part/Model # could not be retrieved. Please see management to resolve this issue."
									+ $"\n\nError: {_ex.Message}");
		}
		// disable the Model Number control if the implication was successful
		if (PartSelection) {
			ModelNumberEntry.IsEnabled = false;
		}
	}

	/// <summary>
	/// Handler for the Pressed event from the PrintButton.
	/// Starts the print action using the information entered in the entries on the UI.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public async void OnPrintButtonPressed(object Sender, EventArgs e) {
		bool Printed = false;
		// call the ViewModel's Print Request method
		try {
			Printed = await _viewModel.PrintRequest(PartPicker, QuantityEntry, JBKNumberEntry, LotNumberEntry, DeburrJBKNumberEntry, DieNumberEntry, ModelNumberEntry, ProductionDatePicker, ProductionShiftPicker, OperatorIDEntry);
		// serialization failed; this is fatal; show a warning
		} catch (Exception _ex) {
			App.AlertSvc!.ShowAlert("Unexpected Error", $"Failed to Serialize the Label. Please see management to resolve this issue.\n\nError: {_ex.Message}");
		}
		// reset UI, show a confirmation if print was successful
		if (Printed) {
			Reset();
			App.AlertSvc!.ShowAlert("Label Printed", "The Label was printed successfully.");
		// the print failed for some reason; show a warning
		} else {
			App.AlertSvc!.ShowAlert("Failed to Print", "The system failed to print this Label. Please try again or see management to resolve this issue.");
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
		if (!_viewModel.IsOriginator) {
			// JBK Input reset
			JBKNumberEntry.Text = "";
			JBKNumberEntry.IsEnabled = true;
			// Lot Input reset
			LotNumberEntry.Text = "";
			LotNumberEntry.IsEnabled = true;
		}
	}
}

