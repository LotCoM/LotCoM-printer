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
	/// Animates the collapsing of the Open Print Tickets Panel across Duration milliseconds.
	/// </summary>
	/// <param name="Duration"></param>
	/// <returns></returns>
	private async Task AnimatedCollapseOpenPrintTicketsPanel()
	{
		ViewModel.Options.CollapseOpenPrintTicketsPanel();
		await OpenPrintTicketsCollapseButton.RotateTo(0);
	}

	/// <summary>
	/// Animates the raising of the Open Print Tickets Panel across Duration milliseconds.
	/// </summary>
	/// <param name="Duration"></param>
	/// <returns></returns>
	private async Task AnimatedRaiseOpenPrintTicketsPanel()
	{
		ViewModel.Options.RaiseOpenPrintTicketsPanel();
		await OpenPrintTicketsCollapseButton.RotateTo(180);
	}

	/// <summary>
	/// Fades the ActivePrintTicketLayout out over Duration milliseconds, then closes the display.
	/// </summary>
	/// <returns></returns>
	/// <param name="Duration"></param>
	private async Task AnimatedCloseActivePrintTicket(uint Duration = 100, bool Swapping = false)
	{
		await ActivePrintTicketLayout.FadeTo(0, Duration);
		ViewModel.CloseActivePrintTicket();
		ActivePrintTicketLayout.Opacity = 1;
		// show the welcome menu if not swapping ticket displays
		if (!Swapping)
		{
			ViewModel.Options.IsWindowHeaderShown = true;
		}
		await AnimatedCollapseOpenPrintTicketsPanel();
	}

	/// <summary>
	/// Sets the ActivePrintTicket to Index, then opens and fades the ActivePrintTicketLayout in over Duration milliseconds.
	/// </summary>
	/// <param name="Index"></param>
	/// <param name="Duration"></param>
	/// <returns></returns>
	private async Task AnimatedOpenActivePrintTicket(int Index = -1, uint Duration = 100)
	{
		ViewModel.Options.IsWindowHeaderShown = false;
		ActivePrintTicketLayout.Opacity = 0;
		await ActivePrintTicketLayout.FadeTo(1, Duration);
		ViewModel.SetActivePrintTicket(Index);
		ViewModel.OpenActivePrintTicket();
		await AnimatedCollapseOpenPrintTicketsPanel();
	}

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
    private async void OnOpenPrintTicketsCollapseButtonClicked(object sender, EventArgs e) 
    {
        if (ViewModel.Options.IsOpenPrintTicketsPanelShown) 
        {
			await AnimatedCollapseOpenPrintTicketsPanel();
        } 
        else 
        {
			await AnimatedRaiseOpenPrintTicketsPanel();
        }
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
		await AnimatedOpenActivePrintTicket();
    }

	/// <summary>
	/// Handler for the Clicked event from the CloseActivePrintTicketButton control.
	/// </summary>
	private async void OnCloseActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		await AnimatedCloseActivePrintTicket(Swapping: false);
	}

	/// <summary>
	/// Handler for the ItemSelected event from the OOpenPrintTicketsListView control.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnOpenPrintTicketsListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		ViewModel.Options.SelectedPrintTicketIndex = e.SelectedItemIndex;
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

	/// <summary>
	/// Handler for the Clicked event from the OpenPrintTicketFromListViewButton control.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void OnOpenPrintTicketFromListViewButtonClicked(object sender, EventArgs e)
	{
		// configure a Swapping flag and perform animations
		bool Swapping = false;
		if (ViewModel.ActivePrintTicket is not null)
		{
			Swapping = true;
		}
		// only collapse open print tickets panel if the same item selected; else swap to new item
		if (ViewModel.ActivePrintTicket == ViewModel.OpenPrintTickets[ViewModel.Options.SelectedPrintTicketIndex])
		{
			await AnimatedCollapseOpenPrintTicketsPanel();
		}
		else
		{
			await AnimatedCloseActivePrintTicket(Swapping: Swapping);
			await AnimatedOpenActivePrintTicket(ViewModel.Options.SelectedPrintTicketIndex); 
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

