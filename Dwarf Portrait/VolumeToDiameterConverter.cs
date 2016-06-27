using System;
using System.Globalization;
using System.Windows.Data;

namespace Dwarf_Portrait
{
    class VolumeToDiameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
                return Convert((int)value);
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
                return (int)ConvertBack((double)value);
            else
                return value;
        }

        public static double Convert(double value)
        {
            return Math.Pow(6.0 / Math.PI, 1.0 / 3.0) * Math.Pow(value, 1.0 / 3.0);
        }

        public static double ConvertBack(double value)
        {
            return Math.Pow(value, 3) * Math.PI / 6.0;
        }
    }
}
