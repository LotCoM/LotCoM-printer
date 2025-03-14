using CommunityToolkit.Maui.Views;

namespace LotCoMPrinter.Views;

public partial class ErrorPopup : Popup {
    // binding properties
    private string _title = "";
    /// <summary>
    /// Serves the Title for the Popup.
    /// </summary>
    public string Title {
        get {return _title;}
        set {
            OnPropertyChanged(_title);
            OnPropertyChanged(Title);
            _title = value;
        }
    }
    private string _message = "";
    /// <summary>
    /// Serves the Message Body for the Popup.
    /// </summary>
    public string Message {
        get {return _message;}
        set {
            OnPropertyChanged(_message);
            OnPropertyChanged(Message);
            _message = value;
        }
    }

    /// <summary>
    /// Creates a Simple Popup that contains a Title, a Message, and a single "OK" Button.
    /// </summary>
    /// <param name="ErrorTitle"></param>
    /// <param name="ErrorMessage"></param>
    public ErrorPopup(string ErrorTitle, string ErrorMessage) {
        // create the popup
        InitializeComponent();

        // assign properties
        Title = ErrorTitle;
        Message = ErrorMessage;
        PopupTitleLabel.Text = ErrorTitle;
        PopupMessageLabel.Text = ErrorMessage;
    }

    /// <summary>
    /// Handler for the Clicked event from the ConfirmationButton.
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void OnConfirmation(object sender, EventArgs e) {
        Close();
    }
}