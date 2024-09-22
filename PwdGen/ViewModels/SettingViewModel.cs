using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PwdGen.Services;

namespace PwdGen.ViewModels;

public partial class SettingViewModel : ViewModelBase
{
    [ObservableProperty]
    private string message = PwdGenDbService.DatabasePath;

    [RelayCommand]
    private void Back()
    {
        App.Current.MainViewModel.Back();
    }

    [RelayCommand]
    private async Task BackupAsync()
    {
        using var file = await App.Current.TopLevel!.StorageProvider
            .SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Backup",
            ShowOverwritePrompt = true,
            DefaultExtension = "db",
            SuggestedFileName = "PwdGen",
            FileTypeChoices = [new FilePickerFileType("SQLite File")
            {
                Patterns = ["*.db"],
            }]
        });
        if (file is null) return;
        Message = $"{file.Path}\n{file.Path.AbsolutePath}\nfile://{PwdGenDbService.DatabasePath}";
        await App.Current.DbService.CloseAsync();
        await using var localStream = new FileStream(PwdGenDbService.DatabasePath, FileMode.Open);
        await using var backupSteam = await file.OpenWriteAsync();
        await localStream.CopyToAsync(backupSteam);
        await backupSteam.FlushAsync();
        App.Current.MainViewModel.Back();
    }

    [RelayCommand]
    private async Task RestoreAsync()
    {
        var files = await App.Current.TopLevel!.StorageProvider
            .OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Restore",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("SQLite File")
            {
                Patterns = ["*.db"],
            }]
        });
        if (files is null) return;
        if (!files.Any()) return;
        using var file = files[0];
        await App.Current.DbService.CloseAsync();
        await using var restoreSteam = await file.OpenReadAsync();
        await using var localStream = new FileStream(PwdGenDbService.DatabasePath, FileMode.Create);
        await restoreSteam.CopyToAsync(localStream);
        await localStream.FlushAsync();
        App.Current.MainViewModel.Back();
    }
}
