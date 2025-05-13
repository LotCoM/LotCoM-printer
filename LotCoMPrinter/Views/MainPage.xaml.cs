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

	/// <summary>
	/// Handler for the Pressed event from the PrintButton.
	/// Starts the print action using the information entered in the entries on the UI.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	private async void OnPrintButtonPressed(object Sender, EventArgs e) 
	{
		// start the Printing Indicator
		ViewModel.Printing = true;
		bool Printed;
		// call the ViewModel's Print Request method
		try 
		{
			Printed = await ViewModel.PrintRequest
			(
				ViewModel.ActivePrintTicket!.Process, 
				ViewModel.ActivePrintTicket.Part, 
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
	/// Event Handler for the Clicked Event from the AddPartialProductionDataSet Button.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    private void OnAddPartialProductionDataSetButtonClicked(object sender, EventArgs e)
    {
		ViewModel.AddPartialDataSet();
    }

	/// <summary>
	/// Event Handler for the Clicked Event from the AddPartialProductionDataset Button.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnRemoveFirstPartialProductionDataSetButtonClicked(object sender, EventArgs e)
	{
		ViewModel.ActivePrintTicket!.RemoveFirstPartialDataSet();
	}

	/// <summary>
	/// Event Handler for the Clicked Event from the AddPartialProductionDataset Button.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnRemoveSecondPartialProductionDataSetButtonClicked(object sender, EventArgs e)
	{
		ViewModel.ActivePrintTicket!.RemoveSecondPartialDataSet();
	}

	/// <summary>
    /// Handler for the Clicked event from the OpenPrintTicketsCollapseButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnOpenPrintTicketsCollapseButtonClicked(object sender, EventArgs e) 
    {
        if (ViewModel.Options.IsOpenPrintTicketsPanelShown) 
        {
            ViewModel.Options.CollapseOpenPrintTicketsPanel();
        } 
        else 
        {
            ViewModel.Options.RaiseOpenPrintTicketsPanel();
        }
        OpenPrintTicketsCollapseButton.Rotation += 180;
    }

	/// <summary>
    /// Handler for the Clicked event from the OnStartNewLabelButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnStartNewLabelButtonClicked(object sender, EventArgs e)
    {
		NewPrintTicketForm Form = new NewPrintTicketForm();
		object? Result = await this.ShowPopupAsync(Form, CancellationToken.None);
		// check if there was a PrintTicket created and returned by the Popup form, then add it to the ViewModel
		if (Result is null)
		{
			return;
		}
		if (Result.GetType().Equals(typeof(PrintTicket)))
		{
			ViewModel.AddOpenPrintTicket((PrintTicket)Result);
		}
		// change the ViewModel's ActivePrintTicket
		ViewModel.SetActivePrintTicket();
    }

	/// <summary>
	/// Handler for the Clicked event from the CloseActivePrintTicketButton control.
	/// </summary>
	private void OnCloseActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		ViewModel.CloseActivePrintTicket();
	}

	/// <summary>
	/// Handler for the ItemSelected event from the OOpenPrintTicketsListView control.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void OnOpenPrintTicketsListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		// find the selected PrintTicket in the Open Print Tickets list and set its IsSelectedInList property
		foreach (PrintTicket _ticket in ViewModel.OpenPrintTickets)
		if (_ticket.Equals(e.SelectedItem))
		{
			_ticket.IsSelectedInList = true;
		}
		else
		{
			_ticket.IsSelectedInList = false;
		}
	}

	// full constructor
	public MainPage() 
	{
		// instantiate the ViewModel and bind the Page to it
		ViewModel = new MainPageViewModel();
		BindingContext = ViewModel;
		// show the window from XAML
		InitializeComponent();
	}
}

