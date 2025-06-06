﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PwdGen.Models;
using System.Collections.ObjectModel;
using RustSharp;

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
    private int totalCount = 0;

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
        var result = await App.Current.DbService.GetAllAcctDataAsync(
            SearchString, (CurrentPage - 1) * PerPage, PerPage);
        switch (result)
        {
            case OkResult<(AcctData[] Result, int TotolCount), string> okResult:
                TotalCount = okResult.Value.TotolCount;
                foreach (var item in okResult.Value.Result) AcctDataList.Add(item);
                break;
            case ErrResult<(AcctData[] Result, int TotolCount), string> errResult:
                await Console.Error.WriteLineAsync(errResult.Value);
                break;
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

    partial void OnTotalCountChanged(int value)
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
        if (TotalCount == 0)
        {
            MaxPage = 1;
            return;
        }
        var (quotient, remainder) = Math.DivRem(TotalCount, PerPage);
        if (remainder == 0) MaxPage = quotient;
        else MaxPage = quotient + 1;
    }
}
