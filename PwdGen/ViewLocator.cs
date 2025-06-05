using Avalonia.Controls;
using Avalonia.Controls.Templates;
using PwdGen.ViewModels;
using PwdGen.Views;

namespace PwdGen;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        return data switch
        {
            AcctDataAddViewModel => new AcctDataAddView(),
            AcctDataDetailViewModel => new AcctDataDetailView(),
            AcctDataEditViewModel => new AcctDataEditView(),
            AcctDataViewModel => new AcctDataView(),
            MainViewModel => new MainView(),
            SettingViewModel => new SettingView(),
            _ => new TextBlock { Text = "Not Found: ViewModel" }
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}