using CommunityToolkit.Maui.Views;

namespace LotCoMPrinter.Views;

public partial class FailedStartupPopup : Popup 
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
            OnPropertyChanged(nameof(_title));
            OnPropertyChanged(nameof(Title));
        }
    }

    private string _message = "";
    /// <summary>
    /// Serves the Message Body for the Popup.
    /// </summary>
    public string Message
    {
        get {return _message;}
        set
        {
            _message = value;
            OnPropertyChanged(nameof(_message));
            OnPropertyChanged(nameof(Message));
        }
    }

    /// <summary>
    /// Handler for the Clicked event from the ConfirmationButton.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnConfirmation(object sender, EventArgs e)
    {
        Close();
        Application.Current?.Quit();
    }

    /// <summary>
    /// Creates a Simple Popup that explains a failed startup to the user, with an "Exit" button.
    /// </summary>
    /// <param name="PopupTitle"></param>
    /// <param name="PopupMessage"></param>
    public FailedStartupPopup(string PopupTitle, string PopupMessage)
    {
        InitializeComponent();
        // configure the Popup window
        Title = PopupTitle;
        Message = PopupMessage;
        PopupTitleLabel.Text = PopupTitle;
        PopupMessageLabel.Text = PopupMessage;
    }
}