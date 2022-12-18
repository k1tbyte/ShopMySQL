using ShopDB.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;


namespace ShopDB.Converters
{
    internal class IntToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AcessLevel)
                return (AcessLevel)value == AcessLevel.Admin ? Visibility.Visible : Visibility.Collapsed; 

            if (value is int vis)
            {
                return vis >= 1;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AcessLevel)
                return (AcessLevel)value == AcessLevel.Admin ? Visibility.Visible : Visibility.Collapsed;

            if (value is int vis)
            {
                return vis >= 1;
            }
            return false;
        }
    }
}
