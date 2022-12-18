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
using ShopDB.MVVM.View.Windows;

namespace ShopDB.MVVM.ViewModels
{
    internal class CartViewModel : ObservableObject
    {
        private ObservableCollection<Product> _cartproducts;
        public ObservableCollection<Product> CartProducts
        {
            get => _cartproducts;
            set => SetProperty(ref _cartproducts, value);
        }
        public RelayCommand FormOrderCommand { get; private set; }
        public RelayCommand RemoveFromCartCommand { get; private set; }
        /*public RelayCommand AddQuantity { get; private set; }
        public RelayCommand LowerQuantity { get; private set; }*/
        public RelayCommand ChangeQuantity { get; private set; }

        public void UpdateCart() {
            CartProducts.Clear();
            if (Utilities.SQL.ExecuteCommand("SELECT cart_item.id,product.name,cart_item.quantity,product.description,product.price,product.img_path,cart_item.product_id" +
                " FROM product" +
                " INNER JOIN cart_item" +
                " ON product.id = cart_item.product_id" +
                " AND cart_item.user_id = " 
                + MainWindowViewModel.AuthenticatedUser.Id, true) == Utilities.SQLResponse.Success)
            {
                while (Utilities.SQL.MySqlReader.Read())
                {
                    CartProducts.Add(new Product()
                    {
                        Id = Utilities.SQL.MySqlReader.GetFieldValue<int>(0),
                        Name = Utilities.SQL.MySqlReader.GetFieldValue<string>(1),
                        Description = Utilities.SQL.MySqlReader.GetFieldValue<string>(3),
                        Price = Utilities.SQL.MySqlReader.GetFieldValue<float>(4),
                        Amount = Utilities.SQL.MySqlReader.GetFieldValue<int>(2),
                        Image = $@"\Resources\Products\{Utilities.SQL.MySqlReader.GetFieldValue<string>(5)}.jpg",
                        Product_Id = Utilities.SQL.MySqlReader.GetFieldValue<int>(6)
                    });;
                }
                Utilities.SQL.MySqlReader.Close();
            }
        }
        public CartViewModel()
        {
            CartProducts = new ObservableCollection<Product>();
            UpdateCart();
            RemoveFromCartCommand = new RelayCommand(o =>
            {
                var command = $"DELETE FROM `cart_item` WHERE id = {(o as Product).Id}";
                Utilities.SQL.ExecuteCommand(command);
                Utilities.SQL.MySqlReader.Close();
                UpdateCart();
            });
            FormOrderCommand = new RelayCommand(o =>
            {
                if(Utilities.UI.OpenDialogWindow(new MakeOrder()) == true)
                {

                }    
                var command = "";
                /*foreach (var cart_item in CartProducts)
                {
                    command = $"INSERT INTO `order_item` (`id`, `quantity`, `price`, `order_id`, `product_id`) VALUES(NULL, '{cart_item.Amount}', '{cart_item.Price}', '3', '{cart_item.Product_Id}');";
                    Utilities.SQL.ExecuteCommand(command);
                }
                
                command = $"DELETE FROM `cart_item`";
                Utilities.SQL.ExecuteCommand(command);
                Utilities.SQL.MySqlReader.Close();*/
                UpdateCart();
            });
            /*ChangeQuantity = new RelayCommand(o =>
            {
                var command = $"UPDATE `cart_item` SET `quantity`={(o as Cart).Quantity} WHERE id = {(o as Cart).Id}";
                Utilities.SQL.ExecuteCommand(command);
                Utilities.SQL.MySqlReader.Close();
                UpdateCart();
            });
            AddQuantity = new RelayCommand(o =>
            {
                (o as Cart).Quantity++;
            });
            LowerQuantity = new RelayCommand(o =>
            {
                if((o as Cart).Quantity!=1)
                    (o as Cart).Quantity--;
            });*/

        }


    }
}
