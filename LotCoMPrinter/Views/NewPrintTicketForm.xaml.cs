using CommunityToolkit.Maui.Views;
using LotCoMPrinter.Models.Datatypes;
using LotCoMPrinter.Models.Enums;
using LotCoMPrinter.ViewModels;

namespace LotCoMPrinter.Views;

public partial class NewPrintTicketForm : Popup
{
    /// <summary>
    /// The ViewModel object controlling this NewPrintTicketForm.
    /// </summary>
    private NewPrintTicketFormViewModel ViewModel;

    /// <summary>
    /// StaticResource Grey500 color.
    /// </summary>
    private static readonly Color Grey500 = new Color(110, 110, 110);

    /// <summary>
    /// Highlights input borders that have null values with Red.
    /// </summary>
    private async Task ShowMissingInputs()
    {
        // turn missing inputs red
        // missing Process input
        if (ViewModel.Process is null)
        {
            ProcessControl.Stroke = Colors.Red;
        }
        // missing Part input
        if (ViewModel.Part is null)
        {
            PartControl.Stroke = Colors.Red;
        }
        // missing Shift input
        if (ViewModel.ProductionShift is Shift.None)
        {
            ShiftControl.Stroke = Colors.Red;
        }
        // missing Operator input
        if (!ViewModel.ProductionOperator.ConfirmProperInitials())
        {
            OperatorControl.Stroke = Colors.Red;
        }
        // reset borders after 3 seconds
        await Task.Delay(3000);
        ProcessControl.Stroke = Grey500;
        PartControl.Stroke = Grey500;
        ShiftControl.Stroke = Grey500;
        OperatorControl.Stroke = Grey500;
    }

    /// <summary>
    /// Handler for the Clicked event from the ConfirmButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnConfirmButtonClicked(object sender, EventArgs e)
    {
        try
        {
            PrintTicket NewTicket = await ViewModel.OpenNewPrintTicket();
            CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await CloseAsync(NewTicket, TokenSource.Token);
        }
        catch
        {
            await ShowMissingInputs();
        }
    }

    /// <summary>
    /// Handler for the Clicked event from the CancelButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnCancelButtonClicked(object sender, EventArgs e)
    {
        CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(null, TokenSource.Token);
    }

    /// <summary>
    /// Handler for the SelectedIndexChanged event from the ProcessPicker control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnProcessPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        ViewModel.SelectedProcessIndex = ProcessPicker.SelectedIndex;
        ViewModel.Process = (Process)ProcessPicker.ItemsSource[ViewModel.SelectedProcessIndex]!;
        ViewModel.SelectedProcessParts = ViewModel.Process.Parts;
    }

    /// <summary>
    /// Handler for the SelectedIndexChanged event from the PartPicker control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPartPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        ViewModel.SelectedPartIndex = PartPicker.SelectedIndex;
        ViewModel.Part = (Part)PartPicker.ItemsSource[ViewModel.SelectedPartIndex]!;
    }

    /// <summary>
    /// Handler for the TextChanged event from the OperatorEntry control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnOperatorEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue is null || e.NewTextValue.Equals(""))
		{
			return;
		}
		// JBK was updated
		if (sender.Equals(OperatorEntry))
		{
			try
			{
				ViewModel.ProductionOperator = new Operator(e.NewTextValue);
				OperatorControl.Stroke = Grey500;
			}
			catch
			{
				OperatorControl.Stroke = Colors.Red;
			}
		}
    }

    /// <summary>
	/// Attempts to initialize a ViewModel for the Window to bind to.
	/// </summary>
	private bool InitializeViewModel()
    {
        // attempt to create a new ViewModel
        try
        {
            ViewModel = new NewPrintTicketFormViewModel();
            return true;
        }
        // there was an issue communicating with or processing data from the Database 
        // or there was a formatting error in the JSON stream from the Database
        catch (SystemException)
        {
            return false;
        }
    }

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Creates an Input Form that prompts the user to configure and create a new Print Ticket.
    /// </summary>
    /// <exception cref="SystemException"></exception>
    public NewPrintTicketForm()
    {
        bool Setup = InitializeViewModel();
        if (Setup)
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }
        else
        {
            throw new SystemException("Failed to initialize the ViewModel.");
        }
    }
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}