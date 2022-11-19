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
        public static bool? OpenDialogWindow(object obj)
        {
            var window = obj as Window;
            window.Owner = App.Current?.MainWindow ?? null;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return window.ShowDialog();
        }

        public static void OpenWindow(object obj)
        {
            var window = obj as Window;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }
    }
}
