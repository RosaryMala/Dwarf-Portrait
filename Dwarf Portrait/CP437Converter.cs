using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Dwarf_Portrait
{
    class CP437Converter : IValueConverter
    {
        static Encoding encoding = Encoding.GetEncoding(437);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte)
            {
                return Convert(new byte[] { (byte)value });
            }
            else if(value is int)
            {
                return Convert(new byte[] { (byte)(int)value });
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return encoding.GetBytes((string)value);
        }

        static string Convert(byte[] bytes)
        {
            return encoding.GetString(bytes);
        }
    }
}
