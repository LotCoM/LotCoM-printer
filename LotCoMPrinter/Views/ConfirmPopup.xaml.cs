using CommunityToolkit.Maui.Views;

namespace LotComPrinter.Views;

public partial class ConfirmPopup : Popup
{
    private string _title = "";
    /// <summary>
    /// Serves the Title for the Popup.
    /// </summary>
    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged(_title);
            OnPropertyChanged(Title);
        }
    }

    private string _message = "";
    /// <summary>
    /// Serves the Message Body for the Popup.
    /// </summary>
    public string Message
    {
        get { return _message; }
        set
        {
            _message = value;
            OnPropertyChanged(_message);
            OnPropertyChanged(Message);
        }
    }
    private string _confirmText = "";
    /// <summary>
    /// Serves the Text to show on the Confirmation Button of the Popup.
    /// </summary>
    public string ConfirmText
    {
        get { return _confirmText; }
        set
        {
            _confirmText = value;
            OnPropertyChanged(_confirmText);
            OnPropertyChanged(ConfirmText);
        }
    }

    private string _cancelText = "";
    /// <summary>
    /// Serves the Text to show on the Cancellation Button of the Popup.
    /// </summary>
    public string CancelText
    {
        get { return _cancelText; }
        set
        {
            _cancelText = value;
            OnPropertyChanged(_cancelText);
            OnPropertyChanged(CancelText);
        }
    }

    /// <summary>
    /// Handler for the Clicked event from the ConfirmButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnConfirmationButtonClicked(object sender, EventArgs e)
    {
        CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(true, TokenSource.Token);
    }

    /// <summary>
    /// Handler for the Clicked event from the CancelButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnCancellationButtonClicked(object sender, EventArgs e)
    {
        CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(false, TokenSource.Token);
    }

    /// <summary>
    /// A BasicPopup extension that allows the addition of Cancellation and Confirmation logic.
    /// </summary>
    /// <param name="Title"></param>
    /// <param name="Message"></param>
    /// <param name="ConfirmText"></param>
    /// <param name="CancelText"></param>
    public ConfirmPopup(string Title, string Message, string ConfirmText = "Confirm", string CancelText = "Cancel")
    {
        InitializeComponent();
        this.Title = Title;
        this.Message = Message;
        this.ConfirmText = ConfirmText;
        this.CancelText = CancelText;
        PopupTitleLabel.Text = Title;
        PopupMessageLabel.Text = Message;
        ConfirmationButton.Text = ConfirmText;
        CancellationButton.Text = CancelText;
    }
}