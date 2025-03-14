using CommunityToolkit.Maui.Views;

namespace LotCoMPrinter.Views;

public partial class BasicPopup : Popup {
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
    /// <param name="PopupTitle"></param>
    /// <param name="PopupMessage"></param>
    public BasicPopup(string PopupTitle, string PopupMessage) {
        // create the popup
        InitializeComponent();

        // assign properties
        Title = PopupTitle;
        Message = PopupMessage;
        PopupTitleLabel.Text = PopupTitle;
        PopupMessageLabel.Text = PopupMessage;
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