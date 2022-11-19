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

        public RelayCommand EditProductComamnd { get; private set; }
        public RelayCommand RemoveProductCommand { get; private set; }
        public RelayCommand AddToCartCommand { get; private set; }
        public RelayCommand AddToWishlistCommand { get; private set; }


        public CatalogViewModel()
        {
            Products = new ObservableCollection<Product>();

            if(Utilities.SQL.ExecuteCommand("SELECT * FROM `product`",true) == Utilities.SQLResponse.Success)
            {
                while (Utilities.SQL.MySqlReader.Read())
                {
                    Products.Add(new Product()
                    {
                        Id = Utilities.SQL.MySqlReader.GetFieldValue<int>(0),
                        Name = Utilities.SQL.MySqlReader.GetFieldValue<string>(1),
                        Description = Utilities.SQL.MySqlReader.GetFieldValue<string>(2),
                        Price =  Utilities.SQL.MySqlReader.GetFieldValue<float>(3),
                        Amount = Utilities.SQL.MySqlReader.GetFieldValue<int>(4),
                        Image = $@"\Resources\Products\{Utilities.SQL.MySqlReader.GetFieldValue<string>(5)}.jpg",
                    });
                }
                Utilities.SQL.MySqlReader.Close();
            }

            EditProductComamnd = new RelayCommand(o =>
            {
                var prod = o as Product;
                if (Utilities.UI.OpenWindow(new AddEditProductWindow(prod)) == true)
                {
                    Products[Products.IndexOf(prod)] = new Product()
                    {
                        Amount = prod.Amount,
                        Description = prod.Description,
                        Id = prod.Id,
                        Image = prod.Image,
                        Name = prod.Name,
                        Price = prod.Price
                    };
                }
                    
            });

            RemoveProductCommand = new RelayCommand(o =>
            {
                var command = $"DELETE FROM `product` WHERE id = {(o as Product).Id}";
                Utilities.SQL.ExecuteCommand(command);
                Utilities.SQL.MySqlReader.Close();
                Products.Remove(o as Product);
            });

            AddToCartCommand = new RelayCommand(o =>
            {
                System.Windows.Forms.MessageBox.Show("Adding to cart");
            });

            AddToWishlistCommand = new RelayCommand(o =>
            {
                System.Windows.Forms.MessageBox.Show("Adding to wishlist");
            });
        }
    }
}
