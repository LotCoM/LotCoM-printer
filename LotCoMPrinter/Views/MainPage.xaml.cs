using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.ViewModels;

namespace LotCoMPrinter.Views;

public partial class MainPage : ContentPage {
	
	// private property to store the ViewModel
	private readonly MainPageViewModel _viewModel;

	public MainPage() {
		// instantiate the ViewModel and bind the Page to it
		_viewModel = new MainPageViewModel();
		BindingContext = _viewModel;

		// show the window from XAML
		InitializeComponent();
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
                {"ProcessPicker", new List<View> {ProcessPicker, ProcessLabel}},
                {"JBKNumberEntry", new List<View> {JBKNumberEntry, JBKNumberLabel}},
                {"LotNumberEntry", new List<View> {LotNumberEntry, LotNumberLabel}},
                {"DeburrJBKNumberEntry", new List<View> {DeburrJBKNumberEntry, DeburrJBKNumberLabel}},
                {"DieNumberEntry", new List<View> {DieNumberEntry, DieNumberLabel}},
                {"PartPicker", new List<View> {PartPicker, PartLabel}},
                {"ModelNumberPicker", new List<View> {ModelNumberPicker, ModelNumberLabel}},
                {"QuantityEntry", new List<View> {QuantityEntry, QuantityLabel}},
                {"ProductionDatePicker", new List<View> {ProductionDatePicker, ProductionDateLabel}},
                {"ProductionShiftPicker", new List<View> {ProductionShiftPicker, ProductionShiftLabel}}
            };
			// get the process requirements for the currently selected Process
			List<string> ProcessRequirements = ProcessData.GetProcessRequirements(_viewModel.SelectedProcess.Replace(" ", ""));
			// show all necessary UI input elements
			foreach (KeyValuePair<string, List<View>> _pair in Conversions) {
				if (ProcessRequirements.Contains(_pair.Key)) {
					_pair.Value[0].IsVisible = true;
					_pair.Value[1].IsVisible = true;
				} else {
					_pair.Value[0].IsVisible = false;
					_pair.Value[1].IsVisible = false;
				}
			}
		}
    }

	/// <summary>
	/// Handler for the ItemSelected event from ProcessPicker.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public async void OnProcessSelection(object Sender, EventArgs e) {
		// update the SelectedProcess property
		Picker picker = (Picker)Sender;
		_viewModel.SelectedProcess = (string?)picker.ItemsSource[picker.SelectedIndex];
		// change visible UI elements
		ChangeDisplayedInputs();
		await Task.Delay(0);
	}

	/// <summary>
	/// Handler for the Pressed event from the PrintButton.
	/// Starts the print action using the information entered in the entries on the UI.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public async void OnPrintButtonPressed(object Sender, EventArgs e) {
		// do print activities
		await Task.Delay(0);
	}
}

