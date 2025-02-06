using LCMSWipPrinter.ViewModels;

namespace LCMSWipPrinter.Views;

public partial class MainPage : ContentPage {
	
	// private property to store the ViewModel
	private readonly MainPageViewModel _viewModel;

	public MainPage() {
		// instantiate the ViewModel and bind the Page to it
		_viewModel = new MainPageViewModel();
		BindingContext = _viewModel;

		// show the window from XAML
		InitializeComponent();
	}

	/// <summary>
	/// Handler for the ItemSelected event from ProcessPicker.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public void OnProcessChange(object Sender, EventArgs e) {
		// change visible UI elements
	}

	/// <summary>
	/// Handler for the Pressed event from the PrintButton.
	/// Starts the print action using the information entered in the entries on the UI.
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="e"></param>
	public async void OnPrintButtonPressed(object Sender, EventArgs e) {
		// do print activities
		await Task.Delay(0);
	}
}

