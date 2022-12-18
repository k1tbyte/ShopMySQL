using ShopDB.Models;
using ShopDB.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ShopDB.MVVM.ViewModels
{
    
    internal class MakeOrderViewModel : ObservableObject
    {
        private object _currentOrderView;
        public object CurrentOrderView
        {
            get => _currentOrderView;
            set => SetProperty(ref _currentOrderView, value);
        }
        private string _adress;
        public string Adress
        {
            get => _adress;
            set => SetProperty(ref _adress, value);
        }
        private float _tprice;
        public float Tprice
        {
            get => _tprice;
            set => SetProperty(ref _tprice, value);
        }

        public RelayCommand ActionButtonCommand { get; private set; }

        public readonly MakeOrderItemsViewModel MakeOrderItemsVM;
        public MakeOrderViewModel()
        {
            MakeOrderItemsVM = new MakeOrderItemsViewModel();
            CurrentOrderView = MakeOrderItemsVM;
            Tprice=MakeOrderItemsVM.getTprice();

            ActionButtonCommand = new RelayCommand(o =>
            {
                var time = Utilities.Main.GetSystemUnixTime();
                var deliveryType = String.IsNullOrWhiteSpace(Adress) ? "none" : "mail";
                var command = $"INSERT INTO `order_details` (`id`, `order_date`, `address`, `status`,`delivery_type`,`user_id`)" +
                $" VALUES(NULL, '{time}', '{(String.IsNullOrWhiteSpace(Adress) ? "NULL" : Adress)}', 'paid','{deliveryType}',{MainWindowViewModel.AuthenticatedUser.Id})";
                if (Utilities.SQL.ExecuteCommand(command, false) != Utilities.SQLResponse.Success) 
                    throw new InvalidOperationException();

                if(Utilities.SQL.ExecuteCommand($"SELECT * FROM `order_details` WHERE order_date = {time};",true) != Utilities.SQLResponse.Success)
                    throw new InvalidOperationException();

                uint id = 0;
                if (Utilities.SQL.MySqlReader.Read())
                {
                   id = Utilities.SQL.MySqlReader.GetFieldValue<uint>(0);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error");
                    return;
                }
                 

                foreach (var item in (App.MainWindow.DataContext as MainWindowViewModel).CartVM.CartProducts)
                {
                    command = $"INSERT INTO `order_item` (`id`, `quantity`, `price`, `order_id`, `product_id`) VALUES(NULL, '{item.Amount}', '{item.Price}','{id}','{item.Product_Id}');";
                    if (Utilities.SQL.ExecuteCommand(command, false) != Utilities.SQLResponse.Success)
                        throw new InvalidOperationException();

                    command = $"DELETE FROM `cart_item` WHERE id = {item.Id}";

                    if (Utilities.SQL.ExecuteCommand(command, false) != Utilities.SQLResponse.Success)
                        throw new InvalidOperationException();
                }

                (App.MainWindow.DataContext as MainWindowViewModel).CartVM.UpdateCart();
                (App.MainWindow.DataContext as MainWindowViewModel).OrdersVM.UpdateOrders();

                /*             
                 *             
                 *             for (int i = 0; i < 1000; i++)
                                {
                                    System.Windows.Forms.MessageBox.Show("Are you sure you want to buy goods?",
                    "Confirmation", System.Windows.Forms.MessageBoxButtons.OKCancel);
                                }*/

            });

        }
    }
}
