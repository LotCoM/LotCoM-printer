using CommunityToolkit.Maui.Views;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.ViewModels;

namespace LotCoMPrinter.Views;

public partial class NewPrintTicketForm : Popup
{
    private readonly NewPrintTicketFormViewModel ViewModel;

    /// <summary>
    /// Handler for the Clicked event from the ConfirmButton control.
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void OnConfirmButtonClicked(object sender, EventArgs e) 
    {
        
    }

    /// <summary>
    /// Handler for the Clicked event from the CancelButton control.
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void OnCancelButtonClicked(object sender, EventArgs e) 
    {
        Close();
    }

    /// <summary>
    /// Handler for the SelectedIndexChanged event from the ProcessPicker control.
    /// </summary>
    /// <param name="Sender"></param>
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
    /// <param name="Sender"></param>
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