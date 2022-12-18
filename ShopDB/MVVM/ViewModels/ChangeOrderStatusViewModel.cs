using ShopDB.Models;
using ShopDB.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopDB.MVVM.ViewModels
{
    internal class ChangeOrderStatusViewModel
    {
        public int OrderStatus { get; set; }
        public RelayCommand ChangeButtonCommand { get; private set; }
        public ChangeOrderStatusViewModel(Order order)
        {
            OrderStatus = 0;
            if(order.OrderStatus=="calceled")
            OrderStatus = 1;
            else if (order.OrderStatus == "done")
            OrderStatus = 2;
            
            ChangeButtonCommand = new RelayCommand(o =>
            {
                string status = "";
                if (OrderStatus == 0)
                    status ="paid";
                else if (OrderStatus == 1)
                    status = "calceled";
                else if (OrderStatus == 2)
                    status = "done";
                if(order.OrderStatus!=status)
                {
                    string command = $"UPDATE `order_details` SET `status` = '{status}' WHERE `order_details`.`id` = '{order.Id}';";
                    Utilities.SQL.ExecuteCommand(command);
                    Utilities.SQL.MySqlReader.Close();
                    (App.MainWindow.DataContext as MainWindowViewModel).OrdersVM.UpdateOrders();
                }
            });
        }
    }
}
