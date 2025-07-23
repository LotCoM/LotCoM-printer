using CommunityToolkit.Maui.Views;

namespace LotComPrinter.Views;

public partial class BasicPopup : Popup 
{
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
            OnPropertyChanged(_message);
            OnPropertyChanged(Message);
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
    }

    /// <summary>
    /// Creates a Simple Popup that contains a Title, a Message, and a single "OK" Button.
    /// </summary>
    /// <param name="PopupTitle"></param>
    /// <param name="PopupMessage"></param>
    public BasicPopup(string PopupTitle, string PopupMessage)
    {
        InitializeComponent();
        // assign properties
        Title = PopupTitle;
        Message = PopupMessage;
        PopupTitleLabel.Text = PopupTitle;
        PopupMessageLabel.Text = PopupMessage;
    }
}