using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PwdGen.Models;

namespace PwdGen.ViewModels;

public partial class AcctDataAddViewModel : ViewModelBase
{
    [ObservableProperty]
    private AcctData inputAcctData = new();

    [ObservableProperty]
    private string mainPassword = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [RelayCommand]
    private void Back()
    {
        App.Current.MainViewModel.Back();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        InputAcctData.DateModified = DateTime.UtcNow.ToBinary();
        var r = await App.Current.DbService.InsertAsync(InputAcctData);
        App.Current.MainViewModel.Back();
        if (r == 0) return;
    }

    [RelayCommand]
    private void GenPwd()
    {
        Password = InputAcctData.Generate(MainPassword);
    }

    [RelayCommand]
    private async Task CopyAsync()
    {
        await (App.Current.Clipboard?.SetTextAsync(Password) ?? Task.CompletedTask);
    }
}
