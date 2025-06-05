using Avalonia.Controls;
using System.Diagnostics;
using Avalonia.Controls.Platform;
using Avalonia.Interactivity;
using PwdGen.ViewModels;

namespace PwdGen.Views;

public partial class MainView : UserControl
{
    private IInputPane? InputPane { get; set; }
    private IInsetsManager? InsetsManager { get; set; }
    
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;
        InputPane = topLevel.InputPane;
        InsetsManager = topLevel.InsetsManager;
        if (InputPane is null) return;
        InputPane.StateChanged += InputPane_StateChanged;
    }
    
    private void InputPane_StateChanged(object? sender, InputPaneStateEventArgs e)
    {
        if (DataContext is not MainViewModel model || InputPane is null || InsetsManager is null) return;
        var safeArea = InsetsManager.SafeAreaPadding;
        var occludedArea = InputPane.OccludedRect;
        model.KeyboardArea = new Avalonia.Thickness(safeArea.Left, safeArea.Top, safeArea.Right, occludedArea.Height);
    }
}