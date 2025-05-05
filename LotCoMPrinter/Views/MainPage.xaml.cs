using CommunityToolkit.Maui.Views;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.ViewModels;

namespace LotCoMPrinter.Views;

public partial class MainPage : ContentPage 
{
	/// <summary>
	/// The ViewModel controlling the UI and logic of the MainPage.
	/// </summary>
	private readonly MainPageViewModel ViewModel;

	// full constructor
	public MainPage() 
	{
		// instantiate the ViewModel and bind the Page to it
		ViewModel = new MainPageViewModel();
		BindingContext = ViewModel;
		// show the window from XAML
		InitializeComponent();
		// hide the process type card on start-up
		ProcessTypeCard.IsVisible = false;
		ProcessTypeLabel.IsVisible = false;
	}

	/// <summary>
	/// Changes the visibility of Input Elements based on the Process selection.
	/// </summary>
	/// <returns></returns>
	public void ChangeDisplayedInputs() 
	{
        // confirm there was a valid selection made in the Picker
        if (ViewModel.Options.SelectedProcess is not null) 
		{
            // create a conversion dictionary for string names to control objects
            Dictionary<string, List<View>> Conversions = new Dictionary<string, List<View>> 
			{
                {"SelectedProcess", new List<View> {ProcessControl, ProcessPicker, ProcessLabel}},
                {"SelectedPart", new List<View> {PartControl, PartPicker, PartLabel}},
                {"Quantity", new List<View> {QuantityControl, QuantityEntry, QuantityLabel}},
                {"JBKNumber", new List<View> {JBKNumberControl, JBKNumberEntry, JBKNumberLabel}},
                {"LotNumber", new List<View> {LotNumberControl, LotNumberEntry, LotNumberLabel}},
                {"DeburrJBKNumber", new List<View> {DeburrJBKNumberControl, DeburrJBKNumberEntry, DeburrJBKNumberLabel}},
                {"DieNumber", new List<View> {DieNumberControl, DieNumberEntry, DieNumberLabel}},
				{"HeatNumber", new List<View> {HeatNumberControl, HeatNumberEntry, HeatNumberLabel}},
                {"ModelNumber", new List<View> {ModelNumberControl, ModelNumberEntry, ModelNumberLabel}},
                {"ProductionDate", new List<View> {ProductionDateControl, ProductionDatePicker, ProductionDateLabel}},
                {"ProductionShift", new List<View> {ShiftControl, ShiftPicker, ShiftLabel}},
				{"OperatorID", new List<View> {OperatorControl, OperatorEntry, OperatorLabel}}
            };
			// get the process requirements for the currently selected Process
			List<string> Requirements = ViewModel.Options.SelectedProcess.RequiredFields;
			// show all necessary UI input elements
			foreach (KeyValuePair<string, List<View>> _pair in Conversions) 
			{
				if (Requirements.Contains(_pair.Key)) 
				{
					_pair.Value[0].IsVisible = true;
					_pair.Value[1].IsVisible = true;
					_pair.Value[2].IsVisible = true;
				}
				else 
				{
					_pair.Value[0].IsVisible = false;
					_pair.Value[1].IsVisible = false;
					_pair.Value[2].IsVisible = false;
				}
			}
        	// confirm whether this label needs to be serialized or considered "pass-through"
			if (ViewModel.Options.SelectedProcess.Type == OriginationTypes.Originator) 
			{
				// disable serial number inputs
				JBKNumberEntry.IsEnabled = false;
				LotNumberEntry.IsEnabled = false;
			} 
			else 
			{
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
	public async void OnProcessSelection(object Sender, EventArgs e) 
	{
		// update the SelectedProcess by invoking the ViewModel method
		Picker ProcessPicker = (Picker)Sender;
		await ViewModel.UpdateSelectedProcess((Process)ProcessPicker.SelectedItem);
		// reset the Page
		Reset();
		// update the SelectedProcess and change the visible UI elements
		try 
		{
			// show the process type card
			ProcessTypeCard.IsVisible = true;
			ProcessTypeLabel.IsVisible = true;
		// there was some error involving the Process file
		} 
		catch (FileLoadException) 
		{
			BasicPopup Popup = new("Failed to Retrieve Data", "There was an error retrieving Part Data for this Process. Please see management to resolve this issue.");
			this.ShowPopup(Popup);
		// there are no Parts assigned to the Process
		} 
		catch (ArgumentException) 
		{
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
	public async void OnPartSelection(object Sender, EventArgs e) 
	{
		// update the SelectedPart, DisplayedModel, and DisplayedJBKNumber properties
		Picker PartPicker = (Picker)Sender;
		try 
		{
			await ViewModel.UpdateSelectedPart((Part)PartPicker.SelectedItem);
		// the Model Number was either unimplied or the JBK # Queue could not be accessed
		} 
		catch (Exception _ex) 
		{
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
	public async void OnPrintButtonPressed(object Sender, EventArgs e) 
	{
		// start the Printing Indicator
		ViewModel.Printing = true;
		bool Printed;
		// call the ViewModel's Print Request method
		try 
		{
			Printed = await ViewModel.PrintRequest
			(
				(Process)ProcessPicker.SelectedItem, 
				(Part)PartPicker.SelectedItem, 
				int.Parse(QuantityEntry.Text), 
				int.Parse(JBKNumberEntry.Text), 
				LotNumberEntry.Text, 
				int.Parse(DeburrJBKNumberEntry.Text), 
				int.Parse(DieNumberEntry.Text), 
				HeatNumberEntry.Text, 
				ModelNumberEntry.Text, 
				ProductionDatePicker.Date, 
				(int)ShiftPicker.SelectedItem, 
				OperatorEntry.Text);
		} 
		catch (Exception _ex) 
		{
			// stop the Printing Indicator
			ViewModel.Printing = false;
			// show a message based on the exception type
			if (_ex is NullProcessException) 
			{
				// there was no process selection made
				BasicPopup Popup = new("Failed to Print", "Please select a Process before printing Labels.");
				this.ShowPopup(Popup);
			} 
			else if (_ex is ArgumentException) 
			{
				// there was an error retrieving the process data
				BasicPopup Popup = new("Failed to Print", "The selected Process' requirements could not be retrieved. Please see management to resolve this issue.");
				this.ShowPopup(Popup);
			} 
			else if (_ex is FormatException) 
			{
				// there was a failed UI validation
				BasicPopup Popup = new("Invalid Production Data.", _ex.Message);
				this.ShowPopup(Popup);
			} 
			else if (_ex is LabelBuildException) 
			{
				// there was an error serializing the Label
				BasicPopup Popup = new("Failed to Print", "Could not apply a Serial Number to the Label. Please see management to resolve this issue.");
				this.ShowPopup(Popup);
			} 
			else if (_ex is PrintRequestException) 
			{
				// there was an error communicating with the Printer or Printing System
				BasicPopup Popup = new("Failed to Print", "Could not connect to the printer. Please see management to resolve this issue.");
				this.ShowPopup(Popup);
			} 
			else if (_ex is PrintLogException) 
			{
				// the print logger failed to log to the specific process table and was forced to default
				BasicPopup Popup = new("Label Printed but Not Logged", "The Label was printed successfully, but the system failed to record the printed Label. Please see management to resolve this issue.");
				this.ShowPopup(Popup);
			}
			// escape the handler
			return;
		}
		// reset UI, show a confirmation if print was successful
		ViewModel.Printing = false;
		if (Printed) 
		{
			Reset();
			BasicPopup Popup = new("Label Printed", "The Label was printed successfully.");
			this.ShowPopup(Popup);
		// the print failed for some reason; show a warning
		} 
		else 
		{
			BasicPopup Popup = new("Failed to Print", "The system failed to print this Label. Please try again or see management to resolve this issue.");
			this.ShowPopup(Popup);
		}
	}

	/// <summary>
	/// Clears and reactivates all UI Controls on the Page.
	/// </summary>
	public void Reset() 
	{
		// reset viewmodel properties
		ViewModel.Reset();
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
		// Heat Number Input reset
		HeatNumberEntry.Text = "";
		HeatNumberEntry.IsEnabled = true;
		// Model Number Picker reset
		ModelNumberEntry.Text = "";
		ModelNumberEntry.IsEnabled = true;
		// Production Date Picker reset
		ProductionDatePicker.Date = DateTime.Now;
		ProductionDatePicker.IsEnabled = true;
		// Production Shift Picker reset
		ShiftPicker.SelectedIndex = -1;
		ShiftPicker.IsEnabled = true;
		// Operator Initials Entry reset
		OperatorEntry.Text = "";
		OperatorEntry.IsEnabled = true;
		// re-enable serial number inputs if serialization is not needed
		if (ViewModel.Options.SelectedProcess is null) 
		{
			return;
		}
		if (ViewModel.Options.SelectedProcess.Type != OriginationTypes.Originator) 
		{
			// JBK Input reset
			JBKNumberEntry.Text = "";
			JBKNumberEntry.IsEnabled = true;
			// Lot Input reset
			LotNumberEntry.Text = "";
			LotNumberEntry.IsEnabled = true;
		}
	}
}

