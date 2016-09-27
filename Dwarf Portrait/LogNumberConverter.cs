using System;
using System.Globalization;
using System.Windows.Data;

namespace Dwarf_Portrait
{
    class LogNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Pow(2, (double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Log((double)value) / Math.Log(2);
        }
    }
}
