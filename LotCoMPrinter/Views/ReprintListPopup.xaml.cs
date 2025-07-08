using CommunityToolkit.Maui.Views;
using LotCom.Types;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Datatypes;

namespace LotCoMPrinter.Views;

public partial class ReprintListPopup : Popup 
{
    private List<PrintTicket> _list = [];
    /// <summary>
    /// Serves a List of Print History Tickets for the Popup.
    /// </summary>
    public List<PrintTicket> List
    {
        get {return _list;}
        set
        {
            _list = value;
            OnPropertyChanged(nameof(_list));
            OnPropertyChanged(nameof(List));
        }
    }

    private PrintTicket? _selectedTicket = null;
    /// <summary>
    /// Contains the selected PrintTicket object from the LabelReprintCollectionView.
    /// </summary>
    public PrintTicket? SelectedTicket
    {
        get { return _selectedTicket; }
        set
        {
            _selectedTicket = value;
            OnPropertyChanged(nameof(_selectedTicket));
            OnPropertyChanged(nameof(SelectedTicket));
        }
    }

    /// <summary>
    /// Handler for the Clicked event from the ConfirmButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnConfirmButtonClicked(object sender, EventArgs e)
    {
        CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(SelectedTicket, TokenSource.Token);
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
    /// Creates a Simple Popup that contains a List of Print History from a Process.
    /// </summary>
    /// <param name="Process"></param>
    public ReprintListPopup(Process Process)
    {
        InitializeComponent();
        // assign properties
        List = PrintHistory.GetPrintTickets(Process);
        List.Reverse();
        LabelReprintCollectionView.ItemsSource = List;
    }
}