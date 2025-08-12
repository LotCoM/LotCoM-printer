using CommunityToolkit.Maui.Views;
using LotCom.Types;
using LotComPrinter.Models.Datatypes;
using LotComPrinter.ViewModels;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace LotComPrinter.Views;

public partial class MainPage : ContentPage
{
	/// <summary>
	/// The ViewModel providing Binding Context and business logic of the MainPage.
	/// </summary>
	public MainPageViewModel ViewModel;

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
			await ViewModel.LoadOpenPrintTickets();
		}
		else
		{
			await AnimatedRaiseOpenPrintTicketsPanel();
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
		{
			if (_ticket.Equals(ViewModel.SelectedTicket))
			{
				_ticket.IsSelectedInList = true;
			}
			else
			{
				_ticket.IsSelectedInList = false;
			}
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
		TicketEditorPage Editor = new TicketEditorPage(ViewModel.SelectedTicket);
		await Navigation.PushAsync(Editor);
		// collapse the OpenPrintTickets panel
		await AnimatedCollapseOpenPrintTicketsPanel();
	}

	/// <summary>
	/// Handler for the Clicked event from the OnStartNewLabelButton control.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <exception cref="SystemException"></exception>
	private async void OnStartNewLabelButtonClicked(object sender, EventArgs e)
	{
		// attempt to create a NewPrintTicketForm
		NewPrintTicketForm Form;
		try
		{
			Form = new NewPrintTicketForm();
		}
		catch (SystemException)
		{
			this.ShowPopup
			(
				new BasicPopup
				(
					"Unexpected Error",
					"We encountered an unexpected error. Please see Management to resolve this issue."
				)
			);
			return;
		}
		// check if there was a PrintTicket created and returned by the Popup form, then add it to the ViewModel
		object? Result = await this.ShowPopupAsync(Form, CancellationToken.None);
		// collapse the OpenPrintTickets panel
		await AnimatedCollapseOpenPrintTicketsPanel();
		// nothing was done (window was just cancelled)
		if (Result is null)
		{
			return;
		}
		// handle database exceptions from Process/Part loading and Serialization
		if (Result.GetType().Equals(typeof(string)))
			{
				this.ShowPopup
				(
					new BasicPopup
					(
						"Unexpected Error",
						"We encountered an unexpected error. Please see Management to resolve this issue." +
						$"\n\nReference message: {Result}"
					)
				);
				return;
			}
		if (Result.GetType().Equals(typeof(PrintTicket)))
		{
			// add the new ticket to the Open list and show an editor for that ticket
			try
			{
				await ViewModel.AddNewOpenPrintTicket((PrintTicket)Result);
			}
			catch (SystemException _ex)
			{
				this.ShowPopup
				(
					new BasicPopup
					(
						"Unexpected Error",
						"We encountered an unexpected error. Please see Management to resolve this issue." +
						$"\n\nReference message: {_ex}"
					)
				);
				return;
			}
			try
			{
				TicketEditorPage Editor = new TicketEditorPage(ViewModel.OpenPrintTickets[^1]);
				await Navigation.PushAsync(Editor);
			}
			catch (Exception _ex)
			{
				this.ShowPopup
				(
					new BasicPopup
					(
						"Unexpected Error",
						"We encountered an unexpected error. Please see Management to resolve this issue." +
						$"\n\nReference message: {_ex}"
					)
				);
				return;
			}
		}
	}

	/// <summary>
	/// Handler for the Clicked event from the ReprintLabelButton Button.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void OnReprintLabelButtonClicked(object sender, EventArgs e)
	{
		// create a new ProcessSelection popup
		ProcessSelectionPopup ProcessSelection = new ProcessSelectionPopup();
		object? Result = await this.ShowPopupAsync
		(
			ProcessSelection
		);
		// collapse the OpenPrintTickets panel
		await AnimatedCollapseOpenPrintTicketsPanel();
		if (Result is null)
		{
			return;
		}
		// save the selected Process object
		Process SelectedProcess = (Process)Result;
		// create a new ReprintSelection popup for the Selected Process
		ReprintSelectionPopup ReprintSelection = new ReprintSelectionPopup(SelectedProcess);
		Result = await this.ShowPopupAsync
		(
			ReprintSelection
		);
		if (Result is null)
		{
			return;
		}
		// save the selected PrintTicket object
		PrintTicket SelectedReprintTicket = (PrintTicket)Result;
		// create a new TicketReprintEditor for the Selected Ticket
		TicketReprintEditorPage ReprintEditor = new TicketReprintEditorPage(SelectedReprintTicket);
		await Navigation.PushAsync(ReprintEditor);
	}

	/// <summary>
	/// Creates a new MainPage Window for the Application.
	/// </summary>
	public MainPage()
	{
		// instantiate the ViewModel and bind the Page to it
		ViewModel = new MainPageViewModel();
		BindingContext = ViewModel;
		// initialize the UI components from XAML
		InitializeComponent();
	}

	/// <summary>
	/// Animates the collapsing of the Open Print Tickets Panel across Duration milliseconds.
	/// </summary>
	/// <param name="Duration">The time, in milliseconds, that it takes for this animation to complete.</param>
	/// <returns></returns>
	public async Task AnimatedCollapseOpenPrintTicketsPanel(uint Duration = 200)
	{
		ViewModel.CollapseOpenPrintTicketsPanel();
		await OpenPrintTicketsCollapseButton.RotateTo(180, length: Duration);
	}

	/// <summary>
	/// Animates the raising of the Open Print Tickets Panel across Duration milliseconds.
	/// </summary>
	/// <param name="Duration">The time, in milliseconds, that it takes for this animation to complete.</param>
	/// <returns></returns>
	public async Task AnimatedRaiseOpenPrintTicketsPanel(uint Duration = 200)
	{
		await ViewModel.RaiseOpenPrintTicketsPanel();
		await OpenPrintTicketsCollapseButton.RotateTo(0, length: Duration);
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}

