﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ShopDB.MVVM.Core;
using MySqlConnector.Authentication;
using MySqlConnector;
using System.Data;
using ShopDB.Models;
using ShopDB.MVVM.View.Windows;

namespace ShopDB.MVVM.ViewModels
{
    internal sealed class MainWindowViewModel : ObservableObject
    {

        #region Properties

        private WindowState _windowState;
        public WindowState WindowState
        {
            get => _windowState;
            set => SetProperty(ref _windowState, value);
        }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public bool IsSidebarMinimize
        {
            get => Config.UserSettings.IsSidebarMinimize;
            set
            {
                if(value != Config.UserSettings.IsSidebarMinimize)
                {
                    Config.UserSettings.IsSidebarMinimize = value;
                    Config.Save();
                }
                OnPropertyChanged(nameof(IsSidebarMinimize));
            }
        }

        #endregion


        #region Commands

        public RelayCommand CloseAppCommand    { get; }
        public RelayCommand MinimizeAppCommand { get; } 
        public RelayCommand MaximizeAppCommand { get; }
        public RelayCommand CatalogViewCommand { get; }
        public RelayCommand AddProductCommand  { get; }
        public RelayCommand LogoutCommand      { get; }
        public RelayCommand OrdersViewCommand      { get; }
        public RelayCommand CartViewCommand { get; }

        #endregion

        public static event EventHandler AuthenticatedUserChanged;
        private static LoggedUser _authenticatedUser;
        public static LoggedUser AuthenticatedUser
        {
            get { return _authenticatedUser; }
            set
            {
                _authenticatedUser = value;
                AuthenticatedUserChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public readonly CatalogViewModel CatalogVM;
        public readonly OrdersViewModel OrdersVM;
        public readonly CartViewModel CartVM;
        internal MainWindowViewModel()
        {
            CatalogVM = new CatalogViewModel();
            OrdersVM = new OrdersViewModel();
            CartVM = new CartViewModel();
            CurrentView = CatalogVM;

            CloseAppCommand = new RelayCommand(o =>
            {
                App.Current.Shutdown();
            });

            MinimizeAppCommand = new RelayCommand(o =>
            {
                WindowState = WindowState.Minimized;
            });

            MaximizeAppCommand = new RelayCommand(o =>
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            });

            AddProductCommand = new RelayCommand(o =>
            {
                try
                {
                    if (Utilities.UI.OpenDialogWindow(new AddEditProductWindow()) == true)
                    {
                        CatalogVM.UpdateCatalog();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }

            });

            #region Переключение вкладок
            CatalogViewCommand = new RelayCommand(o =>
            {
                if (CurrentView != CatalogVM)
                {
                    CurrentView = CatalogVM;
                    CatalogVM.UpdateCatalog(); 
                }
            });

            OrdersViewCommand = new RelayCommand(o =>
            {
                if (CurrentView != OrdersVM)
                {
                    CurrentView = OrdersVM;
                    OrdersVM.UpdateOrders();
                }
                    
            });
            CartViewCommand = new RelayCommand(o =>
            {
                if (CurrentView != CartVM)
                {
                    CurrentView = CartVM;
                    CartVM.UpdateCart();
                }   
            });
            #endregion

            LogoutCommand = new RelayCommand(o =>
            {
                App.MainWindow.Hide();
                Utilities.UI.OpenWindow(new AuthWindow());
                MainWindowViewModel.AuthenticatedUser = null;

            });
            
        }
    }
}
