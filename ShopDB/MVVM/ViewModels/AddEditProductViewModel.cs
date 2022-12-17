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
    internal class AddEditProductViewModel : ObservableObject
    {
        private int _id, _amount;
        private float _price;
        private string _desc, _name, _image;

        #region Props
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public int Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public float Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string Description
        {
            get => _desc;
            set => SetProperty(ref _desc, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        } 
        #endregion

        public RelayCommand ActionButtonCommand { get; private set; }

        public AddEditProductViewModel(Product product = null)
        {
            if(product != null)
            {
                Id = product.Id;
                Amount = product.Amount;
                Price = product.Price;
                Description = product.Description;
                Name = product.Name;
                Image = product.Image;
            }
            else
            {
                Image = "\\Resources\\Products\\DefaultProductImg.jpg";
            }

                ActionButtonCommand = new RelayCommand(o =>
                {
                    string command = "";
                    if(product != null)
                    {
                        /*product.Amount = Amount;
                        product.Price = Price;
                        product.Description = Description;
                        product.Name = Name;
                        product.Image = Image;*/
                        command = $"UPDATE `product` SET `name` = '{Name.Replace("\'", "\\\'")}', `description` = '{Description.Replace("\'", "\\\'")}', `price` = '{Price}', `amount` = '{Amount}' WHERE `product`.`id` = {Id}";
                    }
                    else
                    {
                       var prods = (App.Current.MainWindow.DataContext as MainWindowViewModel).CatalogVM.Products;
                        /*(App.Current.MainWindow.DataContext as MainWindowViewModel).CatalogVM.Products.Add(new Product
                        {
                            Amount = Amount,
                            Price = Price,
                            Description = Description,
                            Name = Name,
                            Image = Image,
                            Id = prods[prods.Count-1].Id + 1
                        });
                        */
                        command = $"INSERT INTO `product` (`id`, `name`, `description`, `price`, `amount`, `img_path`) VALUES (NULL,'{Name.Replace("\'", "\\\'")}','{Description.Replace("\'", "\\\'")}','{Price}','{Amount}','DefaultProductImg');";
                    }

                    Utilities.SQL.ExecuteCommand(command);
                    Utilities.SQL.MySqlReader.Close();
                    (o as Window).DialogResult = true;
                });

            
        }
    }
}
