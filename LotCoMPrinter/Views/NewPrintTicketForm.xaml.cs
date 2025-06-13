using CommunityToolkit.Maui.Views;
using LotCom.Enums;
using LotCom.Types;
using LotCoMPrinter.Models.Datatypes;
using LotCoMPrinter.ViewModels;

namespace LotCoMPrinter.Views;

public partial class NewPrintTicketForm : Popup
{
    /// <summary>
    /// The ViewModel object controlling this NewPrintTicketForm.
    /// </summary>
    private NewPrintTicketFormViewModel ViewModel;


    private const int DefaultStroke = 1;


    private const int ErrorStroke = 2;

    /// <summary>
    /// StaticResource Neutral-20 color.
    /// </summary>
    private static readonly Color Neutral20 = new Color(145, 145, 145);

    /// <summary>
    /// StaticResource Danger-20 color.
    /// </summary>
    private static readonly Color Danger0 = new Color(180, 28, 43);

    /// <summary>
    /// Highlights input borders that have null values with Red.
    /// </summary>
    private async Task ShowMissingInputs()
    {
        // turn missing inputs red
        // missing Process input
        if (ViewModel.Process is null)
        {
            ProcessControl.Stroke = Danger0;
            ProcessControl.StrokeThickness = ErrorStroke;
        }
        // missing Part input
        if (ViewModel.Part is null)
        {
            PartControl.Stroke = Danger0;
            PartControl.StrokeThickness = ErrorStroke;
        }
        // missing Shift input
        if (ViewModel.ProductionShift is Shift.None)
        {
            ShiftControl.Stroke = Danger0;
            ShiftControl.StrokeThickness = ErrorStroke;
        }
        // missing Operator input
        if (!ViewModel.ProductionOperator.ConfirmProperInitials())
        {
            OperatorControl.Stroke = Danger0;
            OperatorControl.StrokeThickness = ErrorStroke;
        }
        // reset borders after 3 seconds
        await Task.Delay(3000);
        ProcessControl.Stroke = Neutral20;
        ProcessControl.StrokeThickness = DefaultStroke;
        PartControl.Stroke = Neutral20;
        PartControl.StrokeThickness = DefaultStroke;
        ShiftControl.Stroke = Neutral20;
        ShiftControl.StrokeThickness = DefaultStroke;
        OperatorControl.Stroke = Neutral20;
        OperatorControl.StrokeThickness = DefaultStroke;
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
                OperatorControl.Stroke = Neutral20;
                OperatorControl.StrokeThickness = DefaultStroke;
            }
            catch
            {
                OperatorControl.Stroke = Danger0;
                OperatorControl.StrokeThickness = ErrorStroke;
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