using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.ViewModels;

namespace LotCoMPrinter.Views;

public partial class PrintTicketPage : ContentPage
{
    /// <summary>
    /// The PrintTicketViewModel object that controls this View.
    /// </summary>
    private readonly PrintTicketViewModel ViewModel;

    /// <summary>
    /// Create a new PrintTicketView Element.
    /// </summary>
    /// <param name="PrintTicket"></param>
    public PrintTicketPage(PrintTicket PrintTicket)
    {
        ViewModel = new PrintTicketViewModel(PrintTicket);
        BindingContext = ViewModel;

        // show the View from XAML
		InitializeComponent();
    }
}