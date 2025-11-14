using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Buyalot;

namespace Buyalot
{
    public class NullOrNegativeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return true;
            if (value is int index && index < 0) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
