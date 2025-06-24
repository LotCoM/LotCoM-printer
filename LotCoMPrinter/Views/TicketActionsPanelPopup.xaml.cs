using CommunityToolkit.Maui.Views;

namespace LotCoMPrinter.Views;

public partial class TicketActionsPanelPopup : Popup 
{
    /// <summary>
    /// Possible actions to take when the user interacts with this menu.
    /// </summary>
    private readonly string[] Actions = ["Cancel", "Close", "Save", "Delete", "Print"];

    private string _title = "";
    /// <summary>
    /// Serves the Title for the Popup.
    /// </summary>
    public string Title
    {
        get {return _title;}
        set
        {
            _title = value;
            OnPropertyChanged(_title);
            OnPropertyChanged(Title);
        }
    }

    /// <summary>
    /// Handler for the Clicked event from the CloseTicket Button.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnCloseTicketButtonClicked(object sender, EventArgs e)
    {
        CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(Actions[1], TokenSource.Token);
    }

    /// <summary>
    /// Handler for the Clicked event from the SaveTicket Button.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSaveTicketButtonClicked(object sender, EventArgs e)
    {
        CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(Actions[2], TokenSource.Token);
    }

    /// <summary>
    /// Handler for the Clicked event from the DeleteTicket Button.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnDeleteTicketButtonClicked(object sender, EventArgs e)
    {
        CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(Actions[3], TokenSource.Token);
    }

    /// <summary>
    /// Handler for the Clicked event from the PrintTicket Button.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPrintTicketButtonClicked(object sender, EventArgs e)
    {
        CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(Actions[4], TokenSource.Token);
    }

    /// <summary>
    /// Creates a Simple Popup that contains a Title, a Message, and a single "OK" Button.
    /// </summary>
    /// <param name="PopupTitle"></param>
    public TicketActionsPanelPopup(string PopupTitle)
    {
        InitializeComponent();
        // assign properties
        Title = PopupTitle;
        PopupTitleLabel.Text = PopupTitle;
    }
}