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
    internal class MakeOrderItemsViewModel : CartViewModel
    {
        public float getTprice()
        {
            float price=0;
            foreach (var item in CartProducts)
            {
                price += item.Price;
            }
            return price;
        }
        public MakeOrderItemsViewModel()
        {
            CartProducts = new ObservableCollection<Product>();
            UpdateCart();
        }
    }
}
