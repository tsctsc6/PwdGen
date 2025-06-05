using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PwdGen.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Stack<ViewModelBase> viewModelStack = new(2);

    [ObservableProperty]
    private ViewModelBase currentViewModel = new AcctDataViewModel();
    
    [ObservableProperty]
    private Thickness keyboardArea;
    
    public void Back()
    {
        if (viewModelStack.Count == 0) return;
        CurrentViewModel = viewModelStack.Pop();
        if (CurrentViewModel is AcctDataViewModel vm)
        {
            vm.JumpCommand.Execute(null);
        }
    }

    public void Forward(ViewModelBase viewModel)
    {
        viewModelStack.Push(CurrentViewModel);
        CurrentViewModel = viewModel;
    }
}
