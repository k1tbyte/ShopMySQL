using ShopDB.Models;
using ShopDB.MVVM.ViewModels;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShopDB.MVVM.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangeOrderStatusWindow.xaml
    /// </summary>
    public partial class ChangeOrderStatusWindow : Window
    {
        public ChangeOrderStatusWindow(Order order)
        {
            InitializeComponent();
            this.DataContext = new ChangeOrderStatusViewModel(order);
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
