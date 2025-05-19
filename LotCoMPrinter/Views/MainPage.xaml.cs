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
	/// <param name="Duration">The time, in milliseconds, that it takes for this animation to complete.</param>
	/// <returns></returns>
	private async Task AnimatedCollapseOpenPrintTicketsPanel(uint Duration = 200)
	{
		ViewModel.CollapseOpenPrintTicketsPanel();
		await OpenPrintTicketsCollapseButton.RotateTo(180, length: Duration);
	}

	/// <summary>
	/// Animates the raising of the Open Print Tickets Panel across Duration milliseconds.
	/// </summary>
	/// <param name="Duration">The time, in milliseconds, that it takes for this animation to complete.</param>
	/// <returns></returns>
	private async Task AnimatedRaiseOpenPrintTicketsPanel(uint Duration = 200)
	{
		ViewModel.RaiseOpenPrintTicketsPanel();
		await OpenPrintTicketsCollapseButton.RotateTo(0, length: Duration);
	}

	/// <summary>
	/// Opens and fades the ActivePrintTicket Menu in over Duration milliseconds.
	/// </summary>
	/// <param name="Duration">The time, in milliseconds, that it takes for this animation to complete.</param>
	/// <returns></returns>
	private async Task AnimatedDisplayPrintTicket(uint Duration = 200)
	{
		ViewModel.OpenActivePrintTicketMenu();
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
	/// <param name="Duration">The time, in milliseconds, that it takes for this animation to complete.</param>
	private async Task AnimatedClosePrintTicket(uint Duration = 200)
	{
		await ActivePrintTicketLayout.FadeTo(0, Duration);
		ViewModel.CloseActivePrintTicketMenu();
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
			await ViewModel.AddNewOpenPrintTicket((PrintTicket)Result);
			ViewModel.ActiveTicket = new TrackedPrintTicket(ViewModel.OpenPrintTickets[^1]);
			await AnimatedDisplayPrintTicket();
		}
    }

	/// <summary>
	/// Handler for the Clicked event from the CloseActivePrintTicketButton control.
	/// </summary>
	private async void OnCloseActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		// get the index of the ActiveTicket in OpenPrintTickets
		if (ViewModel.ActiveTicket is null)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Unexpected Error",
					"We encountered an unexpected error. Please see Management to resolve this issue." +
					"\n\nReference message: " +
					"Cannot close 'null'."
					
				)
			);
		}
		ViewModel.ActiveTicket = new TrackedPrintTicket(ViewModel.ActiveTicket!.Untracked);
		// close the ActivePrintTicket window
		await AnimatedClosePrintTicket();
	}

	/// <summary>
	/// Handler for the Clicked event from the SaveActivePrintTicketButton control.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="OperationCanceledException"></exception>
	private async void OnSaveActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		// get the index of the ActiveTicket in OpenPrintTickets
		if (ViewModel.ActiveTicket is null)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Unexpected Error",
					"We encountered an unexpected error. Please see Management to resolve this issue." +
					"\n\nReference message: " +
					"Cannot save 'null' to OpenPrintTickets."
					
				)
			);
		}
		int Index = ViewModel.OpenPrintTickets.IndexOf(ViewModel.ActiveTicket!.Untracked);
		// confirm that the ActiveTicket exists in OpenPrintTickets and replace it with the Tracked PrintTicket
		if (Index == -1)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Unexpected Error",
					"We encountered an unexpected error. Please see Management to resolve this issue." +
					"\n\nReference message: " +
					"The Untracked ActivePrintTicket was not found in OpenPrintTickets."
					
				)
			);
		}
		ViewModel.ActiveTicket.MergeChanges();
		ViewModel.OpenPrintTickets[Index] = ViewModel.ActiveTicket.Untracked;
		await ViewModel.SaveOpenPrintTickets();
		// close the ActivePrintTicket window
		await AnimatedClosePrintTicket();
	}

	/// <summary>
	/// Handler for the Clicked event from the DeleteActivePrintTicketButton control.
	/// </summary>
	private async void OnDeleteActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		// get the index of the ActiveTicket in OpenPrintTickets
		if (ViewModel.ActiveTicket is null)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Unexpected Error",
					"We encountered an unexpected error. Please see Management to resolve this issue." +
					"\n\nReference message: " +
					"Cannot delete 'null' from OpenPrintTickets."
					
				)
			);
		}
		bool Removed = ViewModel.OpenPrintTickets.Remove(ViewModel.ActiveTicket!.Untracked);
		// confirm that the ActiveTicket exists in OpenPrintTickets and remove it
		if (!Removed)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Unexpected Error",
					"We encountered an unexpected error. Please see Management to resolve this issue." +
					"\n\nReference message: " +
					"The Untracked ActivePrintTicket was not found in OpenPrintTickets."
					
				)
			);
		}
		await ViewModel.SaveOpenPrintTickets();
		// close the ActivePrintTicket window
		await AnimatedClosePrintTicket();
	}

	/// <summary>
	/// Handler for the Clicked event from the PrintActivePrintTicketButton control.
	/// </summary>
	private async void OnPrintActivePrintTicketButtonClicked(object sender, EventArgs e)
	{
		bool Printed = false;
		try
		{
			Printed = await ViewModel.PrintActivePrintTicket();
		}
		// catch and handle validation messages
		catch (ArgumentException _ex)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Invalid Production Data",
					$"{_ex.Message}\n\nResolve this issue and try again."
				)
			);
		}
		// catch and handle print spooling messages
		catch (PrintRequestException _ex)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Print Failed",
					$"The Print request could not be completed." +
					" Please see Management to resolve this issue." +
					$"\n\nReference message: {_ex.Message}."
				)
			);
		}
		// catch other, unexpected issues
		catch (Exception _ex)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Print Failed",
					$"We encountered an unexpected error." +
					" Please see Management to resolve this issue." +
					$"\n\nReference message: {_ex.Message}."
				)
			);
		}
		if (Printed)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Label Printed",
					$"Label printing was successful. Retrieve your new Label from the Printer!"
				)
			);
			await AnimatedClosePrintTicket();
		}
	}

	/// <summary>
	/// Handler for the SelectionChanged event from the OpenPrintTicketsCollectionView control.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnOpenPrintTicketsCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		// find the selected PrintTicket in OpenPrintTickets and set its IsSelectedInList property
		CollectionView View;
		try
		{
			View = (CollectionView)sender;
		}
		catch
		{
			return;
		}
		ViewModel.SelectedTicket = (PrintTicket)View.SelectedItem;
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

