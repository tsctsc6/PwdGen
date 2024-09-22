using Avalonia.Data.Converters;
using System.Globalization;

namespace PwdGen.Converters;

public class BinaryDateTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is long binaryDateTime)
        {
            return DateTime.FromBinary(binaryDateTime).ToLocalTime().ToString();
        }
        throw new ArgumentException("parameter should be a long");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
