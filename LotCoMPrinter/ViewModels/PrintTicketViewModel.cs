using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;

namespace LotCoMPrinter.ViewModels;

public partial class PrintTicketViewModel : ObservableObject
{
    /// <summary>
    /// The PrintTicket object to reference for Data.
    /// </summary>
    public PrintTicket Model;

    public PrintTicketViewModel(PrintTicket PrintTicket)
    {
        Model = PrintTicket;
    }
}