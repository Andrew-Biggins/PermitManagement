namespace PermitManagement.Maui;

public partial class App : Application
{
	public App(MainPage mainPage)
	{
		InitializeComponent();

		MainPage = mainPage;
	}

	protected override Window CreateWindow(Microsoft.Maui.IActivationState? activationState)
	{
		var window = base.CreateWindow(activationState);

#if WINDOWS
		if (window is not null)
		{
			window.Width = 600;
		}
#endif

		return window ?? throw new InvalidOperationException("Failed to create application window.");
	}
}
