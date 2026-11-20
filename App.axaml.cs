
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

using MusicPlayer.Models;
using MusicPlayer.Services;
using MusicPlayer.ViewModels;
using MusicPlayer.Views;

namespace MusicPlayer;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

            _ = InitializeApplicationAsync(desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task InitializeApplicationAsync(
        IClassicDesktopStyleApplicationLifetime desktop)
    {
        try
        {
            // 1. Create AppDataService.
            var appDataService = new AppDataService();

            // 2. Load AppData.
            AppData appData = await appDataService.LoadAppDataAsync();

            // 3. Create ArchiveManager.
            var archiveManager = new ArchiveManager(appData);

            // 4. Find new playlists.
            archiveManager.GetNewPlaylists();

            // 5. Add new playlists.
            archiveManager.AddNewPlaylists();

            // 6. Save updated AppData.
            await appDataService.SaveAppDataAsync(appData);

            // 7. Register dependencies.
            var services = new ServiceCollection();

            services.AddSingleton(appData);
            services.AddSingleton(appDataService);
            services.AddSingleton(archiveManager);

            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<PlayListsViewModel>();
            services.AddTransient<SettingsViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            // 8. Update the existing MainWindow's DataContext.
            if (desktop.MainWindow is MainWindow mainWindow)
            {
                mainWindow.DataContext = serviceProvider
                    .GetRequiredService<MainWindowViewModel>();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Startup failed: {ex}");

            // Keep the window open so the user can see an error
            // or you can display an error dialog here.
        }
    }
}
