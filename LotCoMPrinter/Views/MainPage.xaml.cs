using CommunityToolkit.Maui.Views;
using LotCoMPrinter.Models.Datasources;
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
	/// <returns></returns>
	private async Task AnimatedCollapseOpenPrintTicketsPanel()
	{
		ViewModel.CollapseOpenPrintTicketsPanel();
		await OpenPrintTicketsCollapseButton.RotateTo(180);
	}

	/// <summary>
	/// Animates the raising of the Open Print Tickets Panel across Duration milliseconds.
	/// </summary>
	/// <returns></returns>
	private async Task AnimatedRaiseOpenPrintTicketsPanel()
	{
		ViewModel.RaiseOpenPrintTicketsPanel();
		await OpenPrintTicketsCollapseButton.RotateTo(0);
	}

	/// <summary>
	/// Opens and fades the ActivePrintTicket Menu in over Duration milliseconds.
	/// </summary>
	/// <param name="Duration"></param>
	/// <returns></returns>
	private async Task AnimatedDisplayPrintTicket(uint Duration = 100)
	{
		await ActivePrintTicketLayout.FadeTo(1, Duration);
		if (ViewModel.IsOpenPrintTicketsPanelShown)
		{
			await AnimatedCollapseOpenPrintTicketsPanel();
		}
	}

	/// <summary>
	/// Fades the ActivePrintTicket Menu out over Duration milliseconds, then closes the display.
	/// </summary>
	/// <returns></returns>
	/// <param name="Duration"></param>
	private async Task AnimatedClosePrintTicket(uint Duration = 100)
	{
		await ActivePrintTicketLayout.FadeTo(0, Duration);
		if (ViewModel.IsOpenPrintTicketsPanelShown)
		{
			await AnimatedCollapseOpenPrintTicketsPanel();
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
		ViewModel.ActiveTicket!.Tracked.RemoveFirstPartialDataSet();
	}

	/// <summary>
	/// Event Handler for the Clicked Event from the AddPartialProductionDataset Button.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnRemoveSecondPartialProductionDataSetButtonClicked(object sender, EventArgs e)
	{
		ViewModel.ActiveTicket!.Tracked.RemoveSecondPartialDataSet();
	}

	/// <summary>
    /// Handler for the Clicked event from the OpenPrintTicketsCollapseButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnOpenPrintTicketsCollapseButtonClicked(object sender, EventArgs e) 
    {
        if (ViewModel.IsOpenPrintTicketsPanelShown) 
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
		object? Result = await this.ShowPopupAsync(new NewPrintTicketForm(), CancellationToken.None);
		// check if there was a PrintTicket created and returned by the Popup form, then add it to the ViewModel
		if (Result is null)
		{
			return;
		}
		if (Result.GetType().Equals(typeof(PrintTicket)))
		{
			await AnimatedDisplayPrintTicket();
		}
    }

	/// <summary>
	/// Handler for the Clicked event from the CloseActivePrintTicketButton control.
	/// </summary>
	private async void OnCloseActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		await AnimatedClosePrintTicket();
	}
	
	/// <summary>
	/// Handler for the Clicked event from the SaveActivePrintTicketButton control.
	/// </summary>
	private async void OnSaveActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		await AnimatedClosePrintTicket();
	}

	/// <summary>
	/// Handler for the Clicked event from the DeleteActivePrintTicketButton control.
	/// </summary>
	private async void OnDeleteActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		await AnimatedClosePrintTicket();
	}

	/// <summary>
	/// Handler for the Clicked event from the PrintActivePrintTicketButton control.
	/// </summary>
	private async void OnPrintActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		await AnimatedClosePrintTicket();
	}

	/// <summary>
	/// Handler for the SelectionChanged event from the OpenPrintTicketsCollectionView control.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnOpenPrintTicketsCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		// find the selected PrintTicket in the Open Print Tickets list and set its IsSelectedInList property
		ViewModel.SelectedTicket = (PrintTicket)OpenPrintTicketsCollectionView.SelectedItem;
		foreach (PrintTicket _ticket in ViewModel.OpenPrintTickets)
			if (_ticket.Equals(ViewModel.SelectedTicket))
			{
				_ticket.IsSelectedInList = true;
			}
			else
			{
				_ticket.IsSelectedInList = false;
			}
	}

	/// <summary>
	/// Handler for the Clicked event from the OpenPrintTicketFromCollectionViewButton control.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void OnOpenPrintTicketFromCollectionViewButtonClicked(object sender, EventArgs e)
	{
		if (ViewModel.SelectedTicket is null)
		{
			return;
		}
		ViewModel.ActiveTicket = new TrackedPrintTicket(ViewModel.SelectedTicket);
		await AnimatedDisplayPrintTicket();
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

