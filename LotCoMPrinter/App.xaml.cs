using LotCoMPrinter.Models.Services;

namespace LotCoMPrinter;

public partial class App : Application {
	// alert service interfaces
	private static IServiceProvider? _services;
	public static IServiceProvider? Services {
		get {return _services;}
		set {_services = value;}
	}
    private static IAlertService? _alertSvc;
	public static IAlertService? AlertSvc {
		get {return _alertSvc;}
		set {_alertSvc = value;}
	}

	public App(IServiceProvider provider) {
		InitializeComponent();

		Services = provider;
        AlertSvc = Services.GetService<IAlertService>();

	}

	protected override Window CreateWindow(IActivationState? activationState) {
		return new Window(new AppShell());
	}
}