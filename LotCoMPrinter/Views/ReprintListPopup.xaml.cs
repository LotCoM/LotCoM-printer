using CommunityToolkit.Maui.Views;
using LotCom.Types;
using LotComPrinter.Models.Datasources;
using LotComPrinter.Models.Datatypes;

namespace LotComPrinter.Views;

public partial class ReprintListPopup : Popup 
{
    /// <summary>
    /// Contains the Process object passed to the Popup for History retrieval.
    /// </summary>
    private Process SelectedProcess;

    private DateTime _selectedDate = DateTime.Now;
    /// <summary>
    /// Contains the selected DateTime used to select print history for the popup.
    /// </summary>
    public DateTime SelectedDate
    {
        get { return _selectedDate; }
        set
        {
            _selectedDate = value;
            OnPropertyChanged(nameof(_selectedDate));
            OnPropertyChanged(nameof(SelectedDate));
        }
    }

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
    /// Retrieves SelectedProcess' Print History for a specific Date.
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    private List<PrintTicket> GetHistoryForDate(DateTime Date)
    {
        List = PrintHistory.GetPrintTicketsForDate(Date, SelectedProcess);
        List.Reverse();
        return List;
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
    /// Handler for the DateSelected event from the ReprintDatePicker control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnReprintDatePickerDateSelected(object sender, EventArgs e)
    {
        SelectedDate = ((DatePicker)sender).Date;
        LabelReprintCollectionView.ItemsSource = GetHistoryForDate(SelectedDate);
    }

    /// <summary>
    /// Handler for the DateSelected event from the LabelReprintCollectionView control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLabelReprintCollectionViewItemSelectionChanged(object sender, EventArgs e)
    {
        SelectedTicket = (PrintTicket)((CollectionView)sender).SelectedItem;
    }

    /// <summary>
    /// Creates a Simple Popup that contains a List of Print History from a Process.
    /// </summary>
    /// <param name="Process"></param>
    public ReprintListPopup(Process Process)
    {
        InitializeComponent();
        // assign properties
        SelectedProcess = Process;
        LabelReprintCollectionView.ItemsSource = GetHistoryForDate(DateTime.Now);
    }
}