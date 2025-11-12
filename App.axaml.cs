using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Allva.Desktop.Services;
using Allva.Desktop.ViewModels;
using Allva.Desktop.Views;
using Velopack;

namespace Allva.Desktop
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        private static Window? _mainWindow;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            VelopackApp.Build().Run();
            ConfigureServices();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var licenseService = new LicenseService();

                if (!licenseService.EstaActivada())
                {
                    var licenseView = new LicenseActivationView
                    {
                        DataContext = new LicenseActivationViewModel(),
                        WindowState = WindowState.Maximized
                    };

                    desktop.MainWindow = licenseView;
                    _mainWindow = licenseView;
                }
                else
                {
                    var loginView = new LoginView
                    {
                        DataContext = new LoginViewModel()
                    };

                    var mainWindow = new Window
                    {
                        Title = "Allva System - Login",
                        WindowState = WindowState.Maximized,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        CanResize = true,
                        Content = loginView,
                        Icon = CargarIcono()
                    };

                    desktop.MainWindow = mainWindow;
                    _mainWindow = mainWindow;
                    
                    Task.Run(CheckForUpdatesInBackground);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<LocalizationService>();
            services.AddSingleton<NavigationService>();
            services.AddSingleton<UpdateService>();
            services.AddSingleton<LicenseService>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<LicenseActivationViewModel>();

            Services = services.BuildServiceProvider();
        }

        private async Task CheckForUpdatesInBackground()
        {
            await Task.Delay(5000);

            try
            {
                var updateService = new UpdateService();
                var updateInfo = await updateService.CheckForUpdatesAsync();

                if (updateInfo != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔄 Actualización disponible: {updateInfo.TargetFullRelease.Version}");
                    
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        if (_mainWindow != null)
                        {
                            var viewModel = new UpdateNotificationViewModel(updateInfo, updateService);
                            var dialog = new UpdateNotificationView(viewModel);
                            await dialog.ShowDialog(_mainWindow);
                        }
                    });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✓ La aplicación está actualizada (v{updateService.CurrentVersion})");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error verificando actualizaciones: {ex.Message}");
            }
        }

        private WindowIcon? CargarIcono()
        {
            try
            {
                var uri = new Uri("avares://Allva.Desktop/Assets/allva-icon.ico");
                return new WindowIcon(AssetLoader.Open(uri));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se pudo cargar el icono: {ex.Message}");
                return null;
            }
        }
    }
}