namespace LotCoMPrinter;

public partial class AppShell : Shell {

	public AppShell() {
		// get the version of the assembly at entry (the app version number)
		string Version = System.Reflection.Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
		Title = $"LotCom WIP Labels {Version}";

		InitializeComponent();
	}
}
