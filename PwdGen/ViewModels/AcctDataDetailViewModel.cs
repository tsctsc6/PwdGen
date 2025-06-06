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
        var r = await App.Current.DbService.UpdateAcctDataAsync(AcctData);
        IsSaving = false;
    }
    
    [RelayCommand]
    private async Task DeleteAsync()
    {
        IsSaving = true;
        var r = await App.Current.DbService.DeleteAcctDataAsync(AcctData);
        if (r == 0) return;
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
