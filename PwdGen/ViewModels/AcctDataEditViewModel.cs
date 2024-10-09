using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PwdGen.Models;

namespace PwdGen.ViewModels;

public partial class AcctDataEditViewModel : ViewModelBase
{
    [ObservableProperty]
    private AcctData acctData;

    public AcctDataEditViewModel(AcctData item)
    {
        AcctData = item.Clone();
    }

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
        AcctData.DateModified = DateTime.UtcNow.ToBinary();
        var r = await App.Current.DbService.UpdateAsync(AcctData);
        App.Current.MainViewModel.Back();
        if (r == 0) return;
        App.Current.MainViewModel.Back();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        var r = await App.Current.DbService.DeleteAsync(AcctData);
        App.Current.MainViewModel.Back();
        if (r == 0) return;
        App.Current.MainViewModel.Back();
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
