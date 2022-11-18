using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShopDB.MVVM.ViewModels;

namespace ShopDB.MVVM.View.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            Utilities.WinAPI.SendMessage(new WindowInteropHelper(this).Handle, 161, 2, 0);
        }

        private void MainWindowStateChange(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
            e.Handled = true;
        }

        private void Border_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                e.Handled = true;
                return;
            }


            if (e.Source != OtherMenu && otherMenuButton.IsChecked == true)
            {
                otherMenuButton.IsChecked = false;
                e.Handled = true;
            }

        }

    }
}
