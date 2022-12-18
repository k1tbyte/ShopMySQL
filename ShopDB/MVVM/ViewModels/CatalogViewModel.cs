using ShopDB.Models;
using ShopDB.MVVM.Core;
using ShopDB.MVVM.View.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopDB.MVVM.ViewModels
{
    internal class CatalogViewModel : ObservableObject
    {
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public Visibility Vis
        {
            get => Visibility.Collapsed;
        }

        public RelayCommand EditProductComamnd { get; private set; }
        public RelayCommand RemoveProductCommand { get; private set; }
        public RelayCommand AddToCartCommand { get; private set; }
        public RelayCommand AddToWishlistCommand { get; private set; }
        public void UpdateCatalog()
        {
            Products.Clear();
            if (Utilities.SQL.ExecuteCommand("SELECT * FROM `product`", true) == Utilities.SQLResponse.Success)
            {
                while (Utilities.SQL.MySqlReader.Read())
                {
                    Products.Add(new Product()
                    {
                        Id = Utilities.SQL.MySqlReader.GetFieldValue<int>(0),
                        Name = Utilities.SQL.MySqlReader.GetFieldValue<string>(1),
                        Description = Utilities.SQL.MySqlReader.GetFieldValue<string>(2),
                        Price = Utilities.SQL.MySqlReader.GetFieldValue<float>(3),
                        Amount = Utilities.SQL.MySqlReader.GetFieldValue<int>(4),
                        Image = $@"\Resources\Products\{Utilities.SQL.MySqlReader.GetFieldValue<string>(5)}.jpg",
                    });
                }
                Utilities.SQL.MySqlReader.Close();
            }
        }

        public CatalogViewModel()
        {
            Products = new ObservableCollection<Product>();
            UpdateCatalog();

            EditProductComamnd = new RelayCommand(o =>
            {
                if (MainWindowViewModel.AuthenticatedUser.AcessLevel != AcessLevel.Admin)
                {
                    System.Windows.Forms.MessageBox.Show("You dont have such permissions",
                    "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
                var prod = o as Product;
                if (Utilities.UI.OpenDialogWindow(new AddEditProductWindow(prod)) == true)
                {
                    /*Products[Products.IndexOf(prod)] = new Product()
                    {
                        Amount = prod.Amount,
                        Description = prod.Description,
                        Id = prod.Id,
                        Image = prod.Image,
                        Name = prod.Name,
                        Price = prod.Price
                    };*/
                    UpdateCatalog();
                }   
            });

            RemoveProductCommand = new RelayCommand(o =>
            {
                if (MainWindowViewModel.AuthenticatedUser.AcessLevel != AcessLevel.Admin)
                {
                    System.Windows.Forms.MessageBox.Show("You dont have such permissions",
                    "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
                var command = $"DELETE FROM `product` WHERE id = {(o as Product).Id}";
                Utilities.SQL.ExecuteCommand(command);
                Utilities.SQL.MySqlReader.Close();
                UpdateCatalog();
            });

            AddToCartCommand = new RelayCommand(o =>
            {
                var command = $"INSERT INTO `cart_item` (`id`, `user_id`, `quantity`, `product_id`) VALUES(NULL, '{MainWindowViewModel.AuthenticatedUser.Id}', '1', '{(o as Product).Id}')";
                Utilities.SQL.ExecuteCommand(command);
                Utilities.SQL.MySqlReader.Close();
            });

            AddToWishlistCommand = new RelayCommand(o =>
            {
                System.Windows.Forms.MessageBox.Show("Adding to wishlist");
            });
        }
    }
}
