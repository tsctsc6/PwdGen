using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using PwdGen.Models;

namespace PwdGen.CustomControl;

public partial class AcctDataInputBox : UserControl
{
    public static readonly DirectProperty<AcctDataInputBox, AcctData> AcctDataProperty =
        AvaloniaProperty.RegisterDirect<AcctDataInputBox, AcctData>(
            nameof(AcctData),
            o => o.AcctData,
            (o, v) => o.AcctData = v,
            defaultBindingMode: BindingMode.TwoWay);

    private AcctData acctData;
    public AcctData AcctData
    {
        get => acctData;
        set => SetAndRaise(AcctDataProperty, ref acctData, value);
    }
    
    public static readonly DirectProperty<AcctDataInputBox, bool> IsDateModifiedVisibleProperty =
        AvaloniaProperty.RegisterDirect<AcctDataInputBox, bool>(
            nameof(IsDateModifiedVisible),
            o => o.IsDateModifiedVisible,
            (o, v) => o.IsDateModifiedVisible = v,
            defaultBindingMode: BindingMode.TwoWay,
            unsetValue: true);

    private bool isDateModifiedVisible = true;
    public bool IsDateModifiedVisible
    {
        get => isDateModifiedVisible;
        set => SetAndRaise(IsDateModifiedVisibleProperty, ref isDateModifiedVisible, value);
    }

    public static readonly DirectProperty<AcctDataInputBox, bool> IsReadOnlyProperty =
        AvaloniaProperty.RegisterDirect<AcctDataInputBox, bool>(
            nameof(IsReadOnly),
            o => o.IsReadOnly,
            (o, v) => o.IsReadOnly = v,
            defaultBindingMode: BindingMode.TwoWay);

    private bool isReadOnly;
    public bool IsReadOnly
    {
        get => isReadOnly;
        set => SetAndRaise(IsReadOnlyProperty, ref isReadOnly, value);
    }

    public AcctDataInputBox()
    {
        InitializeComponent();
        RegisterDataModel();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        switch (change.Property.Name)
        {
            case nameof(AcctData):
                LoadDataModel();
                break;
            case nameof(IsDateModifiedVisible):
                DateModified_TextBlock_Key.IsVisible = IsDateModifiedVisible;
                DateModified_TextBlock_Value.IsVisible = IsDateModifiedVisible;
                break;
            case nameof(IsReadOnly):
                SetChildrenIsReadOnlyProperty(IsReadOnly);
                break;
        }
    }

    private void SetChildrenIsReadOnlyProperty(bool value)
    {
        UserName_TextBox.IsReadOnly = value;
        Platform_TextBox.IsReadOnly = value;
        Remark_TextBox.IsReadOnly = value;
        SkipCount_NumericUpDown.IsReadOnly = value;
        UseUpLetter_CheckBox.IsEnabled = !value;
        UseLowLetter_CheckBox.IsEnabled = !value;
        UseNumber_CheckBox.IsEnabled = !value;
        UseSpecialCharacter_CheckBox.IsEnabled = !value;
        PasswordLength_NumericUpDown.IsReadOnly = value;
    }

    private void LoadDataModel()
    {
        UserName_TextBox.Text = AcctData.UserName;
        Platform_TextBox.Text = AcctData.Platform;
        Remark_TextBox.Text = AcctData.Remark;
        SkipCount_NumericUpDown.Value = AcctData.SkipCount;
        UseUpLetter_CheckBox.IsChecked = AcctData.UseUpLetter;
        UseLowLetter_CheckBox.IsChecked = AcctData.UseLowLetter;
        UseNumber_CheckBox.IsChecked = AcctData.UseNumber;
        UseSpecialCharacter_CheckBox.IsChecked = AcctData.UseSpChar;
        PasswordLength_NumericUpDown.Value = AcctData.PwdLen;
        DateModified_TextBlock_Value.Text = DateTime.FromBinary(AcctData.DateModified).ToLocalTime()
            .ToString(CultureInfo.CurrentCulture);
    }

    private void RegisterDataModel()
    {
        UserName_TextBox.TextChanged += (sender, _) =>
        {
            if (sender is TextBox textBox)
            {
                AcctData.UserName = textBox.Text ?? string.Empty;
            }
        };
        Platform_TextBox.TextChanged += (sender, _) =>
        {
            if (sender is TextBox textBox)
            {
                AcctData.Platform = textBox.Text ?? string.Empty;
            }
        };
        Remark_TextBox.TextChanged += (sender, _) =>
        {
            if (sender is TextBox textBox)
            {
                AcctData.Remark = textBox.Text ?? string.Empty;
            }
        };
        SkipCount_NumericUpDown.ValueChanged += (sender, _) =>
        {
            if (sender is NumericUpDown numericUpDown)
            {
                AcctData.SkipCount = (int)(numericUpDown.Value ?? 0);
            }
        };
        UseUpLetter_CheckBox.IsCheckedChanged += (sender, _) =>
        {
            if (sender is CheckBox checkBox)
            {
                AcctData.UseUpLetter = checkBox.IsChecked ?? false;
            }
        };
        UseLowLetter_CheckBox.IsCheckedChanged += (sender, _) =>
        {
            if (sender is CheckBox checkBox)
            {
                AcctData.UseLowLetter = checkBox.IsChecked ?? false;
            }
        };
        UseNumber_CheckBox.IsCheckedChanged += (sender, _) =>
        {
            if (sender is CheckBox checkBox)
            {
                AcctData.UseNumber = checkBox.IsChecked ?? false;
            }
        };
        UseSpecialCharacter_CheckBox.IsCheckedChanged += (sender, _) =>
        {
            if (sender is CheckBox checkBox)
            {
                AcctData.UseSpChar = checkBox.IsChecked ?? false;
            }
        };
        PasswordLength_NumericUpDown.ValueChanged += (sender, _) =>
        {
            if (sender is NumericUpDown numericUpDown)
            {
                AcctData.PwdLen = (int)(numericUpDown.Value ?? 0);
            }
        };
    }
}
