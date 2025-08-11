using LotCom.DataAccess;
using LotComPrinter.Views;

namespace LotComPrinter;

public partial class App : Application 
{
	/// <summary>
	/// The current application version.
	/// </summary>
	public static readonly string AppVersion = System.Reflection.Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
	
	/// <summary>
	/// A global UserAgent object to use as an API interaction authorizer for this app instance.
	/// </summary>
	public static readonly UserAgent UserAgent = UserAgentFactory.CreatePrinterAgent(AppVersion);

	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState) 
	{
		// create the Main Window
		Window MainWindow = new Window(new NavigationPage(new MainPage()));
		// create a TitleBar for the Window
		TitleBar MainWindowTitleBar = new TitleBar 
		{
			Icon = "lotcom_logo.png",
			Title = "LotCom Printer",
			Subtitle = $"v{AppVersion}"      
		};
		// add the TitleBar to the Main Window
		MainWindow.TitleBar = MainWindowTitleBar;
		return MainWindow;
	}
}