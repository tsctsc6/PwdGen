using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PwdGen.Models;
using RustSharp;

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
        var result = await App.Current.DbService.InsertAcctDataAsync(InputAcctData);
        if (result is ErrResult<int, string> errResult)
            await Console.Error.WriteLineAsync(errResult.Value);
        App.Current.MainViewModel.Back();
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
