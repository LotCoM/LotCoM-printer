using CommunityToolkit.Maui.Views;
using LotCom.Types.Enums;
using LotCom.Types;
using LotComPrinter.Models.Datatypes;
using LotComPrinter.Models.Exceptions;
using LotComPrinter.ViewModels;

namespace LotComPrinter.Views;

public partial class TicketEditorPage : ContentPage
{
    /// <summary>
    /// The ViewModel to bind the View to.
    /// </summary>
    private TicketEditorPageViewModel ViewModel;

    /// <summary>
    /// StaticResource Grey500 color.
    /// </summary>
    private static readonly Color Grey500 = new Color(110, 110, 110);

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
	private async void OnRemoveFirstPartialProductionDataSetButtonClicked(object sender, EventArgs e)
	{
		ConfirmPopup Confirm = new ConfirmPopup
		(
			"Remove Partial Basket Data?",
			"Are you sure you would like to remove the Partial Basket info from this Label? This is an irreversible action!",
			"Yes, remove",
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
			ViewModel.EditorTicket!.Tracked.RemoveFirstPartialDataSet();
		}
	}

	/// <summary>
	/// Event Handler for the Clicked Event from the AddPartialProductionDataset Button.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void OnRemoveSecondPartialProductionDataSetButtonClicked(object sender, EventArgs e)
	{
		ConfirmPopup Confirm = new ConfirmPopup
		(
			"Remove Partial Basket Data?",
			"Are you sure you would like to remove the Partial Basket info from this Label? This is an irreversible action!",
			"Yes, remove",
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
			ViewModel.EditorTicket!.Tracked.RemoveSecondPartialDataSet();
		}
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
	/// Handler for the Clicked event from the CloseTicketEditorButton control.
	/// </summary>
	private async void OnCloseTicketEditorButtonClicked(object sender, EventArgs e)
	{
		ConfirmPopup Confirm = new ConfirmPopup
		(
			"Save and Close?",
			"Are you sure you would like to save your changes and close this Label editor?",
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
			// remove the old version of the Ticket
			bool Removed;
			try
			{
				Removed = await ViewModel.DeleteTicket(Navigation.NavigationStack[^2]);
			}
			catch (SystemException)
			{
				this.ShowPopup
				(
					new BasicPopup
					(
						"Unexpected Error",
						"We encountered an unexpected error. Please see Management to resolve this issue." +
						"\n\nReference message: " +
						"Could not remove Editor Ticket from the cache."
					)
				);
				return;
			}
			// save changes to the Ticket
			if (!Removed)
			{
				this.ShowPopup
				(
					new BasicPopup
					(
						"Unexpected Error",
						"We encountered an unexpected error. Please see Management to resolve this issue." +
						"\n\nReference message: " +
						"Could not remove Editor Ticket from the cache."
					)
				);
				return;
			}
			ViewModel.EditorTicket!.MergeChanges();
			// ensure that Pass-through tickets have a Serial Number in their title
			if (ViewModel.EditorTicket.Untracked.Process.PassThroughType == PassThroughType.JBK)
			{
				if (ViewModel.EditorTicket.Untracked.VariableFields.JBKNumber is not null)
				{
					ViewModel.EditorTicket.Untracked.UpdateTitleSerialNumber(ViewModel.EditorTicket.Untracked.VariableFields.JBKNumber!.Formatted);
				}
			}
			else if (ViewModel.EditorTicket.Untracked.Process.PassThroughType == PassThroughType.Lot)
			{
				if (ViewModel.EditorTicket.Untracked.VariableFields.LotNumber is not null)
				{
					ViewModel.EditorTicket.Untracked.UpdateTitleSerialNumber(ViewModel.EditorTicket.Untracked.VariableFields.LotNumber!.Formatted);
				}
			}
			// add the modified ticket to the Open Tickets list
			try
			{
				await ViewModel.AddTicket(Navigation.NavigationStack[^2]);
			}
			catch (SystemException)
			{
				this.ShowPopup
				(
					new BasicPopup
					(
						"Unexpected Error",
						"We encountered an unexpected error. Please see Management to resolve this issue." +
						"\n\nReference message: " +
						"Could not save current OpenPrintTickets to the cache."
					)
				);
				return;
			}
		}
		// return to previous view (presenter)
		await Navigation.PopAsync();
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
		// confirm the delete action
		object? Confirmation = await this.ShowPopupAsync(Confirm, CancellationToken.None);
		// delete was cancelled
		if (Confirmation is null || !(bool)Confirmation)
		{
			return;
		}
		// delete was confirmed
		else
		{
			// confirm that the EditorTicket exists in OpenPrintTickets and remove it
			bool Removed = false;
			try
			{
				Removed = await ViewModel.DeleteTicket(Navigation.NavigationStack[^2]);
			}
			catch (SystemException)
			{
				this.ShowPopup
				(
					new BasicPopup
					(
						"Unexpected Error",
						"We encountered an unexpected error. Please see Management to resolve this issue." +
						"\n\nReference message: " +
						"The PrintTicket was not found or could not be removed from OpenPrintTickets."
					)
				);
			}
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
		}
		// return to previous view (presenter)
		await Navigation.PopAsync();
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
			bool Printed;
			try
			{
				Printed = await ViewModel.PrintTicket(Navigation.NavigationStack[^2]);
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
			}
		}
		// return to previous view (presenter)
		await Navigation.PopAsync();
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
				ViewModel.EditorTicket.Tracked.VariableFields.DieNumber = new DieNumber(e.NewTextValue);
				DieNumberControl.Stroke = Grey500;
			}
			catch
			{
				DieNumberControl.Stroke = Colors.Red;
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
				ViewModel.EditorTicket.Tracked.PrimaryData.Quantity = new Quantity(int.Parse(e.NewTextValue));
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
			&& ViewModel.EditorTicket.Tracked.HasSecondaryData
		)
		{
			try
			{
				ViewModel.EditorTicket.Tracked.SecondaryData!.Quantity = new Quantity(int.Parse(e.NewTextValue));
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
			&& ViewModel.EditorTicket.Tracked.HasTertiaryData
		)
		{
			try
			{
				ViewModel.EditorTicket.Tracked.TertiaryData!.Quantity = new Quantity(int.Parse(e.NewTextValue));
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
				Ticket.PrimaryData.Operator = new Operator(e.NewTextValue);
				OperatorControl.Stroke = Grey500;
			}
			catch
			{
				OperatorControl.Stroke = Colors.Red;
			}
		}
		// First Partial Operator was updated
		else if (sender.Equals(FirstPartialDataSetOperatorEntry) && Ticket.HasSecondaryData)
		{
			try
			{
				Ticket.SecondaryData!.Operator = new Operator(e.NewTextValue);
				FirstPartialDataSetOperatorControl.Stroke = Grey500;
			}
			catch
			{
				FirstPartialDataSetOperatorControl.Stroke = Colors.Red;
			}
		}
		// Second Partial Operator was updated
		else if (sender.Equals(SecondPartialDataSetOperatorEntry) && Ticket.HasTertiaryData)
		{
			try
			{
				Ticket.TertiaryData!.Operator = new Operator(e.NewTextValue);
				SecondPartialDataSetOperatorControl.Stroke = Grey500;
			}
			catch
			{
				SecondPartialDataSetOperatorControl.Stroke = Colors.Red;
			}
		}
	}

    /// <summary>
    /// Creates a Ticket Editor page for EditorTicket.
    /// </summary>
    /// <param name="EditorTicket"></param>
    public TicketEditorPage(PrintTicket EditorTicket)
    {
        ViewModel = new TicketEditorPageViewModel(EditorTicket);
        BindingContext = ViewModel;
        InitializeComponent();
    }
}