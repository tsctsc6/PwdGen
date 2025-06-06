using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PwdGen.Models;
using RustSharp;

namespace PwdGen.ViewModels;

public partial class AcctDataDetailViewModel : ViewModelBase
{
    [ObservableProperty]
    private AcctData acctData;

    [ObservableProperty]
    private string mainPassword = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;
    
    [ObservableProperty]
    private bool isEditing = false;
    
    [ObservableProperty]
    private bool isSaving = false;

    public AcctDataDetailViewModel(AcctData item)
    {
        AcctData = item;
    }

    [RelayCommand]
    private void Back()
    {
        App.Current.MainViewModel.Back();
    }

    async partial void OnIsEditingChanged(bool value)
    {
        if (value) return;
        IsSaving = true;
        AcctData.DateModified = DateTime.UtcNow.ToBinary();
        var result = await App.Current.DbService.UpdateAcctDataAsync(AcctData);
        if (result is ErrResult<int, string> errResult)
            await Console.Error.WriteLineAsync(errResult.Value);
        IsSaving = false;
    }
    
    [RelayCommand]
    private async Task DeleteAsync()
    {
        IsSaving = true;
        var result = await App.Current.DbService.DeleteAcctDataAsync(AcctData);
        if (result is ErrResult<int, string> errResult)
            await Console.Error.WriteLineAsync(errResult.Value);
        IsSaving = false;
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
