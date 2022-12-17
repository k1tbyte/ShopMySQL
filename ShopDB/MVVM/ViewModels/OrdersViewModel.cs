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
    internal class OrdersViewModel : ObservableObject
    {
        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        private void getOrders(string request)
        {
            if (Utilities.SQL.ExecuteCommand(request, true) == Utilities.SQLResponse.Success)
            {
                while (Utilities.SQL.MySqlReader.Read())
                {
                    Orders.Add(new Order()
                    {
                        Id = Utilities.SQL.MySqlReader.GetFieldValue<int>(0),
                        OrderDate = Utilities.Main.UnixTimeToDateTime(Utilities.SQL.MySqlReader.GetFieldValue<ulong>(1)),
                        TotalPrice = Utilities.SQL.MySqlReader.GetFieldValue<float>(2),
                        OrderStatus = Utilities.SQL.MySqlReader.GetFieldValue<string>(3),
                        OrderDeliveryType = Utilities.SQL.MySqlReader.GetFieldValue<string>(4),
                        Items = new List<OrderItem>()
                    });
                }
                Utilities.SQL.MySqlReader.Close();
            }

            foreach (var order in Orders)
            {
                if (Utilities.SQL.ExecuteCommand("SELECT * FROM `order_item` WHERE order_id=" + order.Id, true) == Utilities.SQLResponse.Success)
                {
                    while (Utilities.SQL.MySqlReader.Read())
                    {
                        order.Items.Add(new OrderItem()
                        {
                            Id = Utilities.SQL.MySqlReader.GetFieldValue<int>(0),
                            Quantity = Utilities.SQL.MySqlReader.GetFieldValue<int>(1),
                            Price = Utilities.SQL.MySqlReader.GetFieldValue<float>(2),
                            ProductId = Utilities.SQL.MySqlReader.GetFieldValue<int>(4)
                        });
                    }
                    Utilities.SQL.MySqlReader.Close();

                    foreach (var item in order.Items)
                    {
                        if (Utilities.SQL.ExecuteCommand("SELECT * FROM `product` WHERE id=" + item.Id, true) != Utilities.SQLResponse.Success)
                            continue;

                        if (Utilities.SQL.MySqlReader.Read())
                        {
                            item.Name = Utilities.SQL.MySqlReader.GetFieldValue<string>(1);
                        }
                    }
                }
            }

        }
        public OrdersViewModel()
        {
            Orders = new ObservableCollection<Order>();
            if (MainWindowViewModel.AuthenticatedUser.AcessLevel == AcessLevel.User)
            {
                getOrders("SELECT * FROM `order_details` WHERE user_id="+ MainWindowViewModel.AuthenticatedUser.Id);
            }
            else if(MainWindowViewModel.AuthenticatedUser.AcessLevel == AcessLevel.Admin)
            {
                getOrders("SELECT * FROM `order_details`");

            }
            
        }

    }
}
