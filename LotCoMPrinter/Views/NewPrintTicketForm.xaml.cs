using CommunityToolkit.Maui.Views;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.ViewModels;

namespace LotCoMPrinter.Views;

public partial class NewPrintTicketForm : Popup
{
    /// <summary>
    /// The ViewModel object controlling this NewPrintTicketForm.
    /// </summary>
    private readonly NewPrintTicketFormViewModel ViewModel;

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
        if (ViewModel.ProductionShift is null)
        {
            ShiftControl.Stroke = Colors.Red;
        }
        // missing Operator input
        if (ViewModel.ProductionOperator is null)
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
    /// Creates an Input Form that prompts the user to configure and create a new Print Ticket.
    /// </summary>
    public NewPrintTicketForm() 
    {
        ViewModel = new NewPrintTicketFormViewModel();
        InitializeComponent();
        BindingContext = ViewModel;
    }
}