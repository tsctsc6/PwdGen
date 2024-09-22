using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PwdGen.Models;

namespace PwdGen.ViewModels;

public partial class AcctDataDetailViewModel : ViewModelBase
{
    [ObservableProperty]
    private AcctData acctData;

    [ObservableProperty]
    private string mainPassword = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public AcctDataDetailViewModel(AcctData item)
    {
        AcctData = item;
    }

    [RelayCommand]
    private void Back()
    {
        App.Current.MainViewModel.Back();
    }

    [RelayCommand]
    private void Edit()
    {
        App.Current.MainViewModel.Forward(new AcctDataEditViewModel(AcctData));
    }

    [RelayCommand]
    private void GenPwd()
    {
        Password = AcctData.Generate(MainPassword);
    }

    [RelayCommand]
    private async Task CopyAsync()
    {
        await (App.Current.Clipboard?.SetTextAsync(Password) ?? Task.CompletedTask);
    }
}
