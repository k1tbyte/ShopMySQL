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

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {

                Config.Load();

                new MainWindow().Show();
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                App.Current.Shutdown();
            }

        }
    }
}
