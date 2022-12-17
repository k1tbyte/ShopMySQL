using ShopDB.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopDB.MVVM.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для CartView.xaml
    /// </summary>
    public partial class CartView : UserControl
    {
        Regex regex = new Regex("^[0-9]+$");
        public CartView()
        {
            InitializeComponent();
        }


        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regex.IsMatch(e.Text);

            if(e.Text == "0" && (sender as TextBox).Text.Length == 0)
            {
                e.Handled = true;
                return;
            }

            var innerText = (sender as TextBox).Text + e.Text;
            var id = (int)(sender as TextBox).Tag;
            int.TryParse(innerText, out int query);

            if(query > (App.MainWindow.DataContext as MainWindowViewModel).CatalogVM.Products.Where(o => o.Id == id).First().Amount)
            {
                e.Handled = true;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var obj = sender as TextBox;
            if(obj.Text.Length == 0 || obj.Text[0] == '0')
            {
                obj.Text = "1";
                e.Handled = true;
                return;
            }

            var command = $"UPDATE cart_item SET quantity = '{obj.Text}' WHERE cart_item.id = {obj.Uid}";
            Utilities.SQL.ExecuteCommand(command);
        }
    }
}
