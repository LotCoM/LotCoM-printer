using CommunityToolkit.Maui.Views;
using LotCom.Database;
using LotCom.Types;

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
            OnPropertyChanged(nameof(_selectedProcess));
            OnPropertyChanged(nameof(SelectedProcess));
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
        InitializeComponent();
        BindingContext = this;
        ProcessCollectionView.ItemsSource = new ProcessData().GetAllProcesses();
    }
}