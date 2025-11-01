using Microsoft.Extensions.DependencyInjection;
using PermitManagement.Presentation;
using System.Net.Http;
using System.Windows;

namespace PermitManagement.Desktop;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7158")
        });

        services.AddSingleton<PermitApiClient>();
        services.AddSingleton<PermitViewModel>();

        _services = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var window = new MainWindow
        {
            DataContext = _services.GetRequiredService<PermitViewModel>()
        };
        window.Show();
        base.OnStartup(e);
    }
}