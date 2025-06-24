using CommunityToolkit.Maui.Views;
using LotCom.Types;
using LotCoMPrinter.Models.Datatypes;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.ViewModels;

namespace LotCoMPrinter.Views;

public partial class MainPage : ContentPage
{
	/// <summary>
	/// The ViewModel controlling the UI and logic of the MainPage.
	/// </summary>
	private MainPageViewModel ViewModel;

	/// <summary>
	/// StaticResource Grey500 color.
	/// </summary>
	private static readonly Color Grey500 = new Color(110, 110, 110);

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
	/// Opens and fades the TicketEditor Menu in over Duration milliseconds.
	/// </summary>
	/// <param name="Duration">The time, in milliseconds, that it takes for this animation to complete.</param>
	/// <returns></returns>
	private async Task AnimatedDisplayPrintTicket(uint Duration = 200)
	{
		ViewModel.OpenPrintTicketEditorMenu();
		await TicketEditorMenu.FadeTo(1, Duration);
		if (ViewModel.IsOpenPrintTicketsPanelShown)
		{
			await AnimatedCollapseOpenPrintTicketsPanel();
		}
	}

	/// <summary>
	/// Fades the TicketEditor Menu out over Duration milliseconds, then closes the display.
	/// </summary>
	/// <returns></returns>
	/// <param name="Duration">The time, in milliseconds, that it takes for this animation to complete.</param>
	private async Task AnimatedClosePrintTicket(uint Duration = 200)
	{
		await TicketEditorMenu.FadeTo(0, Duration);
		ViewModel.CloseTicketEditorMenu();
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
		ViewModel.EditorTicket!.Tracked.RemoveFirstPartialDataSet();
	}

	/// <summary>
	/// Event Handler for the Clicked Event from the AddPartialProductionDataset Button.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnRemoveSecondPartialProductionDataSetButtonClicked(object sender, EventArgs e)
	{
		ViewModel.EditorTicket!.Tracked.RemoveSecondPartialDataSet();
	}

	/// <summary>
	/// Event Handler for the Clicked Event from the PrintFirst or PrintSecondPartialProductionDataSet Buttons.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void OnPrintPartialProductionDataSetButtonClicked(object sender, EventArgs e)
	{
		// set the targeted PartialDataSet
		int DataSet = 2;
		if (sender.Equals(PrintFirstPartialProductionDataSetButton))
		{
			DataSet = 1;
		}
		// set a printed flag and attempt to print a Partial Data Set Tag
		bool Printed = false;
		try
		{
			Printed = await ViewModel.PrintPartialTag(DataSet);
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
					"Partial Tag Printed",
					$"Partial Tag printing was successful. Retrieve your new Tag from the Printer!"
				)
			);
		}
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
		if (Result is null)
		{
			return;
		}
		if (Result.GetType().Equals(typeof(PrintTicket)))
		{
			await ViewModel.AddNewOpenPrintTicket((PrintTicket)Result);
			ViewModel.EditorTicket = new TrackedPrintTicket(ViewModel.OpenPrintTickets[^1]);
			await AnimatedDisplayPrintTicket();
		}
	}

	/// <summary>
	/// Handler for the Clicked event from the CloseTicketEditorButton control.
	/// </summary>
	private async void OnCloseTicketEditorButtonClicked(object sender, EventArgs e)
	{
		ConfirmPopup Confirm = new ConfirmPopup
		(
			"Close without saving?",
			"Are you sure you would like to close this Label editor and discard your changes?",
			"Yes, close",
			"No, go back"
		);
		// confirm the close action
		object? Confirmation = await this.ShowPopupAsync(Confirm, CancellationToken.None);
		// close was cancelled
		if (Confirmation is null || !(bool)Confirmation)
		{
			return;
		}
		// close was confirmed
		else
		{
			if (ViewModel.EditorTicket is null)
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
			else
			{
				// close the Ticket Editor Menu window
				ViewModel.EditorTicket = new TrackedPrintTicket(ViewModel.EditorTicket!.Untracked);
				await AnimatedClosePrintTicket();
			}
		}
	}

	/// <summary>
	/// Handler for the Clicked event from the SaveTicketEditorButton control.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="OperationCanceledException"></exception>
	private async void OnSaveTicketEditorButtonClicked(object sender, EventArgs e)
	{
		ConfirmPopup Confirm = new ConfirmPopup
		(
			"Close and save?",
			"Are you sure you would like to close this Label editor and save your changes?",
			"Yes, save",
			"No, go back"
		);
		// confirm the close action
		object? Confirmation = await this.ShowPopupAsync(Confirm, CancellationToken.None);
		// close was cancelled
		if (Confirmation is null || !(bool)Confirmation)
		{
			return;
		}
		// close was confirmed
		else
		{
			// confirm the index of the EditorTicket exists in OpenPrintTickets
			if (ViewModel.EditorTicket is null)
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
				return;
			}
			// confirm that the EditorTicket exists in OpenPrintTickets and replace it with the Tracked PrintTicket
			int Index = ViewModel.OpenPrintTickets.IndexOf(ViewModel.EditorTicket!.Untracked);
			if (Index == -1)
			{
				this.ShowPopup
				(
					new BasicPopup
					(
						"Unexpected Error",
						"We encountered an unexpected error. Please see Management to resolve this issue." +
						"\n\nReference message: " +
						"The Untracked PrintTicket was not found in OpenPrintTickets."
					)
				);
				return;
			}
			// close the Ticket Editor Menu window
			ViewModel.EditorTicket.MergeChanges();
			ViewModel.OpenPrintTickets[Index] = ViewModel.EditorTicket.Untracked;
			await ViewModel.SaveOpenPrintTickets();
			await AnimatedClosePrintTicket();
		}
	}

	/// <summary>
	/// Handler for the Clicked event from the DeleteTicketEditorButton control.
	/// </summary>
	private async void OnDeleteTicketEditorButtonClicked(object sender, EventArgs e)
	{
		ConfirmPopup Confirm = new ConfirmPopup
		(
			"Delete Label?",
			"Are you sure you would like to Delete this Label? This is an irreversible action!",
			"Yes, delete",
			"No, go back"
		);
		// confirm the close action
		object? Confirmation = await this.ShowPopupAsync(Confirm, CancellationToken.None);
		// close was cancelled
		if (Confirmation is null || !(bool)Confirmation)
		{
			return;
		}
		// close was confirmed
		else
		{
			// get the index of the EditorTicket in OpenPrintTickets
			if (ViewModel.EditorTicket is null)
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
				return;
			}
			// confirm that the EditorTicket exists in OpenPrintTickets and remove it
			bool Removed = ViewModel.OpenPrintTickets.Remove(ViewModel.EditorTicket!.Untracked);
			if (!Removed)
			{
				this.ShowPopup
				(
					new BasicPopup
					(
						"Unexpected Error",
						"We encountered an unexpected error. Please see Management to resolve this issue." +
						"\n\nReference message: " +
						"The Untracked PrintTicket was not found in OpenPrintTickets."
					)
				);
				return;
			}
			// close the Ticket Editor Menu window
			await ViewModel.SaveOpenPrintTickets();
			await AnimatedClosePrintTicket();
		}
	}

	/// <summary>
	/// Handler for the Clicked event from the PrintTicketEditorButton control.
	/// </summary>
	private async void OnPrintTicketEditorButtonClicked(object sender, EventArgs e)
	{
		ConfirmPopup Confirm = new ConfirmPopup
		(
			"Finalize Label and Print?",
			"Are you sure you would like to finish editing this Label and Print it?",
			"Yes, print",
			"No, go back"
		);
		// confirm the close action
		object? Confirmation = await this.ShowPopupAsync(Confirm, CancellationToken.None);
		// close was cancelled
		if (Confirmation is null || !(bool)Confirmation)
		{
			return;
		}
		// close was confirmed
		else
		{
			// attempt to print the Label
			bool Printed = false;
			try
			{
				Printed = await ViewModel.PrintEditorTicket();
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
				return;
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
				return;
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
				return;
			}
			// print was successful
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
		ViewModel.EditorTicket = new TrackedPrintTicket(ViewModel.SelectedTicket);
		await AnimatedDisplayPrintTicket();
	}

	/// <summary>
	/// Handler for the TextChanged event from the VariableFieldSet Entry controls.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnVariableFieldSetEntryTextChanged(object sender, TextChangedEventArgs e)
	{
		if (ViewModel.EditorTicket is null || e.NewTextValue is null || e.NewTextValue.Equals(""))
		{
			return;
		}
		// JBK was updated
		if (sender.Equals(JBKNumberEntry))
		{
			try
			{
				new JBKNumber(int.Parse(e.NewTextValue));
				JBKNumberControl.Stroke = Grey500;
			}
			catch
			{
				JBKNumberControl.Stroke = Colors.Red;
			}
		}
		// Lot was updated
		else if (sender.Equals(LotNumberEntry))
		{
			try
			{
				new LotNumber(int.Parse(e.NewTextValue));
				LotNumberControl.Stroke = Grey500;
			}
			catch
			{
				LotNumberControl.Stroke = Colors.Red;
			}
		}
		// Deburr JBK was updated
		else if (sender.Equals(DeburrJBKNumberEntry))
		{
			try
			{
				ViewModel.EditorTicket.Tracked.VariableFields.DeburrJBKNumber = new JBKNumber(int.Parse(e.NewTextValue));
				DeburrJBKNumberControl.Stroke = Grey500;
			}
			catch
			{
				DeburrJBKNumberControl.Stroke = Colors.Red;
			}
		}
		// Die was updated
		else if (sender.Equals(DieNumberEntry))
		{
			try
			{
				ViewModel.EditorTicket.Tracked.VariableFields.DieNumber = new DieNumber(int.Parse(e.NewTextValue));
				DieNumberControl.Stroke = Grey500;
			}
			catch
			{
				DieNumberControl.Stroke = Colors.Red;
			}
		}
		// Model was updated
		else if (sender.Equals(ModelNumberEntry))
		{
			try
			{
				ViewModel.EditorTicket.Tracked.VariableFields.ModelNumber = new ModelNumber(e.NewTextValue);
				ModelNumberControl.Stroke = Grey500;
			}
			catch
			{
				ModelNumberControl.Stroke = Colors.Red;
			}
		}
		// Heat was updated
		else if (sender.Equals(HeatNumberEntry))
		{
			try
			{
				ViewModel.EditorTicket.Tracked.VariableFields.HeatNumber = new HeatNumber(int.Parse(e.NewTextValue));
				HeatNumberControl.Stroke = Grey500;
			}
			catch
			{
				HeatNumberControl.Stroke = Colors.Red;
			}
		}
	}

	/// <summary>
	/// Handler for the Unfocused event from the JBKNumberEntry or LotNumberEntry controls.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnSerialNumberEntryUnfocused(object sender, EventArgs e)
	{
		if (ViewModel.EditorTicket is null)
		{
			return;
		}
		Entry Sender = (Entry)sender;
		// JBK Number was unfocused
		if (sender.Equals(JBKNumberEntry))
		{
			if (Sender.Text is null || Sender.Text.Equals(""))
			{
				JBKNumberControl.Stroke = Colors.Red;
			}
			try
			{
				ViewModel.EditorTicket!.Tracked.VariableFields.JBKNumber = new JBKNumber(int.Parse(Sender.Text!));
				JBKNumberControl.Stroke = Grey500;
			}
			catch
			{
				JBKNumberControl.Stroke = Colors.Red;
			}
		}
		// Lot Number was unfocused
		if (sender.Equals(LotNumberEntry))
		{
			if (Sender.Text is null || Sender.Text.Equals(""))
			{
				LotNumberControl.Stroke = Colors.Red;
			}
			try
			{
				ViewModel.EditorTicket!.Tracked.VariableFields.LotNumber = new LotNumber(int.Parse(Sender.Text!));
				LotNumberControl.Stroke = Grey500;
			}
			catch
			{
				LotNumberControl.Stroke = Colors.Red;
			}
		}
		// Deburr JBK Number was unfocused
		if (sender.Equals(DeburrJBKNumberEntry))
		{
			if (Sender.Text is null || Sender.Text.Equals(""))
			{
				DeburrJBKNumberControl.Stroke = Colors.Red;
			}
			try
			{
				ViewModel.EditorTicket!.Tracked.VariableFields.DeburrJBKNumber = new JBKNumber(int.Parse(Sender.Text!));
				DeburrJBKNumberControl.Stroke = Grey500;
			}
			catch
			{
				DeburrJBKNumberControl.Stroke = Colors.Red;
			}
		}
	}

	/// <summary>
	/// Handler for the TextChanged event from any of the Quantity Entry controls.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnQuantityEntryTextChanged(object sender, TextChangedEventArgs e)
	{
		if
		(
			ViewModel.EditorTicket is null
			|| e.NewTextValue is null
			|| e.NewTextValue.Equals("")
		)
		{
			return;
		}
		// Main Quantity was updated
		if (sender.Equals(QuantityEntry))
		{
			try
			{
				ViewModel.EditorTicket.Tracked.ProductionQuantity = new Quantity(int.Parse(e.NewTextValue));
				QuantityControl.Stroke = Grey500;
			}
			catch
			{
				QuantityControl.Stroke = Colors.Red;
			}
		}
		// First Partial Quantity was updated
		else if
		(
			sender.Equals(FirstPartialDataSetQuantityEntry)
			&& ViewModel.EditorTicket.Tracked.HasFirstPartialDataSet
		)
		{
			try
			{
				ViewModel.EditorTicket.Tracked.FirstPartialDataSet!.Quantity = new Quantity(int.Parse(e.NewTextValue));
				FirstPartialDataSetQuantityControl.Stroke = Grey500;
			}
			catch
			{
				FirstPartialDataSetQuantityControl.Stroke = Colors.Red;
			}
		}
		// Second Partial Quantity was updated
		else if
		(
			sender.Equals(SecondPartialDataSetQuantityEntry)
			&& ViewModel.EditorTicket.Tracked.HasSecondPartialDataSet
		)
		{
			try
			{
				ViewModel.EditorTicket.Tracked.SecondPartialDataSet!.Quantity = new Quantity(int.Parse(e.NewTextValue));
				SecondPartialDataSetQuantityControl.Stroke = Grey500;
			}
			catch
			{
				SecondPartialDataSetQuantityControl.Stroke = Colors.Red;
			}
		}
	}

	/// <summary>
	/// Handler for the TextChanged event from any of the Operator Initials Entry controls.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnOperatorEntryTextChanged(object sender, TextChangedEventArgs e)
	{
		if
		(
			ViewModel.EditorTicket is null
			|| e.NewTextValue is null
			|| e.NewTextValue.Equals("")
		)
		{
			return;
		}
		PrintTicket Ticket = ViewModel.EditorTicket.Tracked;
		// Main Operator was updated
		if (sender.Equals(OperatorEntry))
		{
			try
			{
				Ticket.ProductionOperator = new Operator(e.NewTextValue);
				OperatorControl.Stroke = Grey500;
			}
			catch
			{
				OperatorControl.Stroke = Colors.Red;
			}
		}
		// First Partial Operator was updated
		else if (sender.Equals(FirstPartialDataSetOperatorEntry) && Ticket.HasFirstPartialDataSet)
		{
			try
			{
				Ticket.FirstPartialDataSet!.Operator = new Operator(e.NewTextValue);
				FirstPartialDataSetOperatorControl.Stroke = Grey500;
			}
			catch
			{
				FirstPartialDataSetOperatorControl.Stroke = Colors.Red;
			}
		}
		// Second Partial Operator was updated
		else if (sender.Equals(SecondPartialDataSetOperatorEntry) && Ticket.HasSecondPartialDataSet)
		{
			try
			{
				Ticket.SecondPartialDataSet!.Operator = new Operator(e.NewTextValue);
				SecondPartialDataSetOperatorControl.Stroke = Grey500;
			}
			catch
			{
				SecondPartialDataSetOperatorControl.Stroke = Colors.Red;
			}
		}
	}

	/// <summary>
	/// Handler for the Clicked event from the TicketActions Button.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void OnTicketActionsButtonClicked(object sender, EventArgs e)
	{
		// create a new TicketActionsPanel popup and configure its anchor and layout
		TicketActionsPanelPopup Actions = new TicketActionsPanelPopup("Label Actions");
		Actions.Anchor = TicketActionsButton;
		Actions.HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.End;
		Actions.VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
		Actions.Size = new Size(175, 230);
		// show the popup and wait for a resulting action
		object? Result = await this.ShowPopupAsync
		(
			Actions
		);
		if (Result is null)
		{
			return;
		}
		// execute the selected action
		string Action = (string)Result;
		if (Action.Equals("Close"))
		{
			OnCloseTicketEditorButtonClicked(sender, e);
		}
		else if (Action.Equals("Save"))
		{
			OnSaveTicketEditorButtonClicked(sender, e);
		}
		else if (Action.Equals("Delete"))
		{
			OnDeleteTicketEditorButtonClicked(sender, e);
		}
		else if (Action.Equals("Print"))
		{
			OnPrintTicketEditorButtonClicked(sender, e);
		}
	}

	/// <summary>
	/// Attempts to initialize a ViewModel for the Window to bind to.
	/// If this method raises an exception, it will create a popup for the user and then Quit.
	/// </summary>
	private bool InitializeViewModel()
	{
		// attempt to create a new ViewModel
		try
		{
			ViewModel = new MainPageViewModel();
			return true;
		}
		// there was an issue communicating with or processing data from the Database 
		// or there was a formatting error in the JSON stream from the Database
		catch (SystemException)
		{
			this.ShowPopup
			(
				new FailedStartupPopup
				(
					PopupTitle: "Failed to Launch",
					PopupMessage: "We're sorry, we couldn't launch LotCom WIP Labels. Please see Management to resolve this issue."
				)
			);
			// return false so the application can begin exiting
			return false;
		}
	}

	#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	/// <summary>
	/// Creates a new MainPage Window for the Application.
	/// </summary>
	public MainPage()
	{
		// instantiate the ViewModel and bind the Page to it
		bool Launch = InitializeViewModel();
		if (Launch)
		{
			BindingContext = ViewModel;
			// show the window from XAML
			InitializeComponent();
		}
	}
	#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}

