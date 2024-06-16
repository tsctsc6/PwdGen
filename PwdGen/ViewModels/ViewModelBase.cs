using CommunityToolkit.Mvvm.ComponentModel;

namespace PwdGen.ViewModels;

public class ViewModelBase : ObservableObject
{
    public ViewModelBase? ParentViewModel { get; private set; } = null;

    public ViewModelBase()
    {
    }

    public ViewModelBase(ViewModelBase? parentViewModel)
    {
        ParentViewModel = parentViewModel;
    }
}
