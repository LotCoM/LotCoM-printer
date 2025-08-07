using CommunityToolkit.Maui.Views;
using LotCom.Enums;
using LotCom.Exceptions;
using LotCom.Types;
using LotComPrinter.Models.Services;
using LotComPrinter.ViewModels;

namespace LotComPrinter.Views;

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
    /// <exception cref="SystemException"></exception>
    private async void OnConfirmButtonClicked(object sender, EventArgs e)
    {
		DebugLogger.LogMessage("Handling ConfirmButton Clicked event...", this);
		DebugLogger.LogMessage("Configuring output...", this);
        object? Output;
        try
        {
		    DebugLogger.LogMessage("Calling ViewModel.OpenNewPrintTicket()...", this);
            Output = await ViewModel.OpenNewPrintTicket();
		    DebugLogger.LogMessage("Method did not throw exceptions.", this);
        }
        catch (ArgumentException _ex)
        {
            DebugLogger.LogError("A validation exception occurred.", _ex, this);
            DebugLogger.LogWarning("This exception is non-fatal.", this);
		    DebugLogger.LogMessage("Calling ShowMissingInputs()...", this);
            try
            {
                await ShowMissingInputs();
            }
            catch (Exception _innerEx)
            { 
                DebugLogger.LogError("Failed to show missing inputs.", _innerEx, this);
                throw;
            }
		    DebugLogger.LogMessage("Method did not throw exceptions.", this);
            Output = $"{_ex.Message}";
        }
        catch (SerializationException _ex)
        {
            DebugLogger.LogError("A serialization exception occurred.", _ex, this);
            Output = $"{_ex.Message}";
        }
        DebugLogger.LogMessage($"Output string: \"{Output}\".", this);
        DebugLogger.LogMessage("Closing and returning output...", this);
        try
        {
            CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await CloseAsync(Output, TokenSource.Token);
        }
        catch (Exception _ex)
        { 
            DebugLogger.LogError("Failed to close and return output.", _ex, this);
            throw;
        }
        DebugLogger.LogMessage("Closed and returned output.", this);
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
        ViewModel.SelectedProcessParts = ViewModel.Process.PrintParts;
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
		DebugLogger.LogMessage("Initializing ViewModel...", this);
        // attempt to create a new ViewModel
        try
        {
		    DebugLogger.LogMessage("Attempting to create new NewPrintTicketFormViewModel...", this);
            ViewModel = new NewPrintTicketFormViewModel();
		    DebugLogger.LogMessage("Created new NewPrintTicketFormViewModel.", this);
            return true;
        }
        // there was an issue communicating with or processing data from the Database 
        // or there was a formatting error in the JSON stream from the Database
        catch (SystemException _ex)
        {
		    DebugLogger.LogError("Failed to create new NewPrintTicketFormViewModel.", _ex, this);
            throw;
        }
    }

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Creates an Input Form that prompts the user to configure and create a new Print Ticket.
    /// </summary>
    /// <exception cref="SystemException"></exception>
    public NewPrintTicketForm()
    {
		DebugLogger.LogMessage("Starting NewPrintTicketForm initialization...", this);
		DebugLogger.LogMessage("Creating new NewPrintTicketFormViewModel to bind to...", this);
        bool Setup;
        try
        {
            Setup = InitializeViewModel();
        }
        catch (Exception _ex)
        {
            DebugLogger.LogError("Failed to create a NewPrintTicketFormViewModel to bind to.", _ex, this);
            throw;
        }
        if (Setup)
        {
            DebugLogger.LogMessage("Created new NewPrintTicketFormViewModel.", this);
            DebugLogger.LogMessage("Initializing UI Components...", this);
            try
            {
                InitializeComponent();
            }
            catch (Exception _ex)
            {
                DebugLogger.LogError("Failed to initialize UI Components.", _ex, this);
                throw;
            }
            DebugLogger.LogMessage("Initialized UI Components.", this);
            DebugLogger.LogMessage("Binding to new TicketEditorPageViewModel...", this);
            try
            {
                BindingContext = ViewModel;
            }
            catch (Exception _ex)
            {
                DebugLogger.LogError("Failed to bind to TicketEditorPageViewModel.", _ex, this);
                throw;
            }
            DebugLogger.LogMessage("Bound to TicketEditorPageViewModel.", this);
        }
        else
        {
            DebugLogger.LogError("Failed to create a NewPrintTicketFormViewModel to bind to.", new Exception(), this);
            throw new SystemException("Failed to initialize the ViewModel.");
        }
		DebugLogger.LogMessage("UI Components initialized.", this);
		DebugLogger.LogMessage("Page setup complete.", this);
    }
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}