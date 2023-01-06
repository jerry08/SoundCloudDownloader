using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SoundCloudDownloader.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    private object GetVisibility(object value)
    {
        if (value is not bool)
        {
            return Visibility.Collapsed;
        }

        bool objValue = (bool)value;
        return objValue ? Visibility.Visible : (object)Visibility.Collapsed;
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        return GetVisibility(value);
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        throw new NotImplementedException();
    }
}