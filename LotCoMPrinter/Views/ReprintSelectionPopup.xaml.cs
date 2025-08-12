using CommunityToolkit.Maui.Views;
using LotCom.DataAccess.Services;
using LotCom.Types;
using LotCom.UI;
using LotComPrinter.Models.Datatypes;

namespace LotComPrinter.Views;

public partial class ReprintSelectionPopup : Popup 
{
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

    private PageLoadingFlags _flags = new PageLoadingFlags();
    /// <summary>
    /// Indicates different loading related properties for the Page's Print History data.
    /// </summary>
    public PageLoadingFlags Flags
    {
        get { return _flags; }
        set
        {
            _flags = value;
            OnPropertyChanged();
        }
    }

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
            OnPropertyChanged();
        }
    }

    private IEnumerable<PrintTicket> _list = [];
    /// <summary>
    /// Serves a List of Print History Tickets for the Popup.
    /// </summary>
    public IEnumerable<PrintTicket> List
    {
        get {return _list;}
        set
        {
            _list = value;
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Handler for the Clicked event from the ConfirmButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnConfirmButtonClicked(object sender, EventArgs e)
    {
        if (SelectedTicket is null)
        {
            LabelReprintCollectionControl.StrokeThickness = ErrorStroke;
            LabelReprintCollectionControl.Stroke = Danger0;
            await Task.Delay(3000);
            LabelReprintCollectionControl.StrokeThickness = DefaultStroke;
            LabelReprintCollectionControl.Stroke = Neutral20;
            return;
        }
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
    private async void OnReprintDatePickerDateSelected(object sender, EventArgs e)
    {
        SelectedDate = ((DatePicker)sender).Date;
        await LoadHistory(SelectedDate);
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
    public ReprintSelectionPopup(Process Process)
    {
        BindingContext = this;
        InitializeComponent();
        // assign properties
        SelectedProcess = Process;
    }

    /// <summary>
    /// Populates the Popup with SelectedProcess' Print History for a specific Date.
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public async Task LoadHistory(DateTime Date)
    {
        Flags.Start();
        // retrieve Prints from database
        IEnumerable<Print>? PrintsFromDatabase = await PrintService.GetOnDateByProcess
        (
            Date,
            SelectedProcess.Id,
            App.UserAgent
        );
        if (PrintsFromDatabase is null)
        {
            Flags.Failure();
            return;
        }
        // convert Prints to PrintTickets
        IEnumerable<PrintTicket> PrintTickets = PrintsFromDatabase
            .Select(PrintTicket.FromPrint);
        // reverse and return the list of PrintTickets
        List = PrintTickets.Reverse();
        Flags.Success();
    }
}