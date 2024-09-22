using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using PwdGen.Services;
using PwdGen.ViewModels;
using PwdGen.Views;

namespace PwdGen;

public partial class App : Application
{
    public static new App Current => (Application.Current as App)!;

    public PwdGenDbService DbService { get; } = new();

    public MainViewModel MainViewModel { get; private set; }

    public TopLevel? TopLevel { get; private set; }

    public IClipboard? Clipboard { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        MainViewModel = new();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow()
            {
                DataContext = MainViewModel
            };
            desktop.ShutdownRequested += DesktopOnShutdownRequested;
            TopLevel = TopLevel.GetTopLevel(desktop.MainWindow);
            Clipboard = TopLevel?.Clipboard;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = MainViewModel
            };
            TopLevel = TopLevel.GetTopLevel(singleViewPlatform.MainView);
            Clipboard = TopLevel?.Clipboard;
        }
        base.OnFrameworkInitializationCompleted();
    }

    private bool _canClose;
    private async void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        e.Cancel = !_canClose;

        if (!_canClose)
        {
            await DbService.CloseAsync();

            _canClose = true;
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
    }
}