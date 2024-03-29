﻿using ShopDB.Models;
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

        public void UpdateOrders()
        {
            Orders.Clear();
            string request = "";
            if (MainWindowViewModel.AuthenticatedUser.AcessLevel == AcessLevel.User)
            {
                request="SELECT * FROM `order_details` WHERE user_id=" + MainWindowViewModel.AuthenticatedUser.Id;
            }
            else if (MainWindowViewModel.AuthenticatedUser.AcessLevel == AcessLevel.Admin)
            {
                request = "SELECT * FROM `order_details`";
            }
            if (Utilities.SQL.ExecuteCommand(request, true) == Utilities.SQLResponse.Success)
            {
                while (Utilities.SQL.MySqlReader.Read())
                {
                    Orders.Add(new Order()
                    {
                        Id = Utilities.SQL.MySqlReader.GetFieldValue<int>(0),
                        OrderDate = Utilities.Main.UnixTimeToDateTime(Utilities.SQL.MySqlReader.GetFieldValue<ulong>(1)),
                        DeliveryAdress = Utilities.SQL.MySqlReader.GetFieldValue<string>(2),
                        OrderStatus = Utilities.SQL.MySqlReader.GetFieldValue<string>(3),
                        OrderDeliveryType = Utilities.SQL.MySqlReader.GetFieldValue<string>(4),
                        Items = new List<OrderItem>()
                    });
                }
                Utilities.SQL.MySqlReader.Close();
            }

            foreach (var order in Orders)
            {
                float total_price=0;
                if (Utilities.SQL.ExecuteCommand("SELECT * FROM `order_item` WHERE order_id=" + order.Id, true) == Utilities.SQLResponse.Success)
                {
                    while (Utilities.SQL.MySqlReader.Read())
                    {
                        var q = Utilities.SQL.MySqlReader.GetFieldValue<int>(1);
                        order.Items.Add(new OrderItem()
                        {
                            Id = Utilities.SQL.MySqlReader.GetFieldValue<int>(0),
                            Quantity = q,
                            Price = Utilities.SQL.MySqlReader.GetFieldValue<float>(2),
                            ProductId = Utilities.SQL.MySqlReader.GetFieldValue<int>(4)
                        });
                        total_price += Utilities.SQL.MySqlReader.GetFieldValue<float>(2) * q;
                    }
                    Utilities.SQL.MySqlReader.Close();
                    order.TotalPrice=total_price;
                    foreach (var item in order.Items)
                    {
                        if (Utilities.SQL.ExecuteCommand("SELECT * FROM `product` WHERE id=" + item.ProductId, true) != Utilities.SQLResponse.Success)
                            continue;

                        if (Utilities.SQL.MySqlReader.Read())
                        {
                            item.Name = Utilities.SQL.MySqlReader.GetFieldValue<string>(1);
                        }
                    }
                }
            }

        }
        public RelayCommand ChangeStatusCommand { get; private set; }
        public RelayCommand RemoveOrderCommand { get; private set; }
        public OrdersViewModel()
        {
            Orders = new ObservableCollection<Order>();
            UpdateOrders();
            ChangeStatusCommand = new RelayCommand(o =>
            {
                if (MainWindowViewModel.AuthenticatedUser.AcessLevel != AcessLevel.Admin)
                {
                    System.Windows.Forms.MessageBox.Show("You dont have such permissions",
                    "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
                var order = o as Order;
                Utilities.UI.OpenDialogWindow(new ChangeOrderStatusWindow(order));
            });
            RemoveOrderCommand = new RelayCommand(o =>
            {
                if (MainWindowViewModel.AuthenticatedUser.AcessLevel != AcessLevel.Admin)
                {
                    System.Windows.Forms.MessageBox.Show("You dont have such permissions",
                    "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
                var command = "";
                command = $"DELETE FROM `order_item` WHERE order_id = {(o as Order).Id}";
                Utilities.SQL.ExecuteCommand(command);
                command =$"DELETE FROM `order_details` WHERE id = {(o as Order).Id}";
                Utilities.SQL.ExecuteCommand(command);
                Utilities.SQL.MySqlReader.Close();
                UpdateOrders();
            });
        }

    }
}
