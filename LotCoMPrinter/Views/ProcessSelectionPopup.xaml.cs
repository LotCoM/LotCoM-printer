using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using LotCom.DataAccess.Services;
using LotCom.Types;
using LotCom.UI;
using Newtonsoft.Json;

namespace LotComPrinter.Views;

public partial class ProcessSelectionPopup : Popup
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
    /// Indicates different loading related properties for the Page's Process data.
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

    private ObservableCollection<Process>? _processes;
    /// <summary>
    /// Controls the list of Processes in the ProcessCollectionView.
    /// </summary>
    public ObservableCollection<Process>? Processes
    {
        get { return _processes; }
        set
        {
            _processes = value;
            OnPropertyChanged();
        }
    }

    private Process? _selectedProcess;
    /// <summary>
    /// Controls the currently selected Process in the ProcessCollectionView.
    /// </summary>
    public Process? SelectedProcess
    {
        get { return _selectedProcess; }
        set
        {
            _selectedProcess = value;
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
        if (SelectedProcess is null)
        {
            ProcessCollectionControl.StrokeThickness = ErrorStroke;
            ProcessCollectionControl.Stroke = Danger0;
            await Task.Delay(3000);
            ProcessCollectionControl.StrokeThickness = DefaultStroke;
            ProcessCollectionControl.Stroke = Neutral20;
            return;
        }
        CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(SelectedProcess, TokenSource.Token);
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
    /// Create a new Process Selection Popup window.
    /// </summary>
    public ProcessSelectionPopup()
    {
        BindingContext = this;
        InitializeComponent();
    }

    /// <summary>
    /// Loads the Processes used to populate the popup.
    /// </summary>
    /// <returns></returns>
    public async Task LoadProcesses()
    {
        Flags.Start();
        IEnumerable<Process>? ProcessesFromDatabase;
        try
        {
            ProcessesFromDatabase = await ProcessService.GetAll(App.UserAgent);
        }
        // some database-generated issue
        catch (HttpRequestException)
        {
            Flags.Failure();
            return;
        }
        // some formatting issue
        catch (JsonException)
        {
            Flags.Failure();
            return;
        }
        // no error but no results from the Database
        if (ProcessesFromDatabase is null)
        {
            Processes = [];
            Flags.Success();
            return;
        }
        Processes = new ObservableCollection<Process> (ProcessesFromDatabase);
        Flags.Success();
    }
}