using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PwdGen.Models;
using System.Collections.ObjectModel;

namespace PwdGen.ViewModels;

public partial class AcctDataViewModel : ViewModelBase
{
    [ObservableProperty]
    private string searchString = string.Empty;

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int perPage = 10;

    [ObservableProperty]
    private int maxPage = 1;

    [ObservableProperty]
    private int totolCount = 0;

    public ObservableCollection<AcctData> AcctDataList { get; } = [];

    public AcctDataViewModel()
    {
        JumpCommand.Execute(null);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        CurrentPage = 1;
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        CurrentPage++;
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task PrePageAsync()
    {
        CurrentPage--;
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task JumpAsync()
    {
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        AcctDataList.Clear();
        var x = await App.Current.DbService.GetAllAcctDataAsync(
            SearchString, (CurrentPage - 1) * PerPage, PerPage);
        TotolCount = x.TotolCount;
        foreach (var item in x.Result)
        {
            AcctDataList.Add(item);
        }
    }

    [RelayCommand]
    private void ViewDetail(AcctData item)
    {
        App.Current.MainViewModel.Forward(new AcctDataDetailViewModel(item));
    }

    [RelayCommand]
    private void ViewSetting()
    {
        App.Current.MainViewModel.Forward(new SettingViewModel());
    }

    [RelayCommand]
    private void ViewAcctDataAdd()
    {
        App.Current.MainViewModel.Forward(new AcctDataAddViewModel());
    }

    partial void OnPerPageChanged(int value)
    {
        CalculateMaxPage();
    }

    partial void OnTotolCountChanged(int value)
    {
        CalculateMaxPage();
    }

    partial void OnCurrentPageChanged(int oldValue, int newValue)
    {
        if (newValue == 1) return;
        if (newValue < 1)
        {
            CurrentPage = 1;
            return;
        }
        if (newValue == MaxPage) return;
        if (newValue > MaxPage) CurrentPage = MaxPage;
    }

    private void CalculateMaxPage()
    {
        if (TotolCount == 0)
        {
            MaxPage = 1;
            return;
        }
        (var quotient, var remainder) = Math.DivRem(TotolCount, PerPage);
        if (remainder == 0) MaxPage = quotient;
        else MaxPage = quotient + 1;
    }
}
