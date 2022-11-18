using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopDB.Utilities
{
    internal class UI
    {
        public static bool? OpenWindow(object obj)
        {
            var window = obj as Window;
            window.Owner = App.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return window.ShowDialog();
        }
    }
}
