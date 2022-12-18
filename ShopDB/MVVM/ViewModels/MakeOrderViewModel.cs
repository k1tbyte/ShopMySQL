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
/*                for (int i = 0; i < 1000; i++)
                {
                    System.Windows.Forms.MessageBox.Show("Are you sure you want to buy goods?",
    "Confirmation", System.Windows.Forms.MessageBoxButtons.OKCancel);
                }*/

            });

        }
    }
}
