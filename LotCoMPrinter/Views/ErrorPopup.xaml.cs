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

    
    public ErrorPopup(string ErrorTitle, string ErrorMessage) {
        // create the popup
        InitializeComponent();

        // assign properties
        Title = ErrorTitle;
        Message = ErrorMessage;
        PopupTitleLabel.Text = ErrorTitle;
        PopupMessageLabel.Text = ErrorMessage;
    }
}