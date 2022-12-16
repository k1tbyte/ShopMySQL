using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ShopDB.MVVM.View.Windows;

namespace ShopDB
{
    public partial class App : Application
    {
        private static readonly Mutex Mutex = new Mutex(true, "ShopDB");
        public static string WorkingDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static new MainWindow MainWindow { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Config.Load();
                if(Utilities.SQL.Connect("datasource=127.0.0.1;port=3306;username=root;password=;database=electronics_store") != Utilities.SQLResponse.Success)
                {
                    System.Windows.Forms.MessageBox.Show("Database connection failed!");
                    App.Current.Shutdown();
                }
                    
                new AuthWindow()
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                }.Show();

               // new MainWindow().Show();
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                App.Current.Shutdown();
            }

        }
    }
}
