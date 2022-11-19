using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShopDB.MVVM.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SignUpClick(object sender, RoutedEventArgs e)
        {
            if(SignUpPanel.Visibility == Visibility.Visible)
            {
                SignUpAction();
                return;
            }
            LoginPanel.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Visible;
            Height = 520;
            Title.Text = "Sign up";
            signUpBttn.Content = "Sign up";
            signInBttn.Content = "Already sign up";
        }

        private void SignInClick(object sender, RoutedEventArgs e)
        {
            if (LoginPanel.Visibility == Visibility.Visible)
            {
                SignInAction();
                return;
            }
            LoginPanel.Visibility = Visibility.Visible;
            SignUpPanel.Visibility = Visibility.Collapsed;
            Height = 216;
            Title.Text = "Sign in";
            signUpBttn.Content = "Create account";
            signInBttn.Content = "Sign in";
        }

        private void SignInAction()
        {
            errorBlock.Text = "";
            var collection = new List<string>()
            {
                lloginField.Text,
                lpassField.Password
            };

            if(IsNullOrWhiteSpaceCollection(collection))
            {
                System.Windows.Forms.MessageBox.Show("Not all fields are completed or fields contain spaces");
                return;
            }    
                
        }

        private void SignUpAction()
        {
            errorBlock.Text = "";
            var collection = new List<string>()
            {
                loginField.Text,
                passField.Password,
                usernameField.Text,
                firstNameField.Text,
                lastNameField.Text,
            };

            if (IsNullOrWhiteSpaceCollection(collection))
            {
                errorBlock.Text = "Not all fields are completed or fields contain spaces";
                return;
            }
            else if (!IsFieldsValid())
                return;

            var checkCommand = $"SELECT * FROM user WHERE login = '{loginField.Text}'";

            if (Utilities.SQL.ExecuteCommand(checkCommand, true) != Utilities.SQLResponse.Success)
                return;

            Utilities.SQL.MySqlReader.Read();
            if (Utilities.SQL.MySqlReader.HasRows)
            {
                errorBlock.Text = "User with this login already exists";
                return;
            }

            var passHash = Utilities.Main.Sha256(passField.Password);
            var email = String.IsNullOrEmpty(emailField.Text) ? "NULL" : $"'{emailField.Text}'" ;
            var address = String.IsNullOrEmpty(addressField.Text) ? "NULL" : $"'{addressField.Text}'";
            var phone = String.IsNullOrEmpty(phoneField.Text) ? "NULL" : $"'{phoneField.Text}'";

            var insertCommand = $"INSERT INTO `user` (`id`, `acess_level`, `username`, `login`, `password`, `email`, `address`, `first_name`, `last_name`, `phone_number`) VALUES" +
                $"(NULL,'user','{usernameField.Text}','{loginField.Text}','{passHash}',{email},{address},'{firstNameField.Text}','{lastNameField.Text}',{phone})";



            if(Utilities.SQL.ExecuteCommand(insertCommand) == Utilities.SQLResponse.Success)
            {
                new MainWindow().Show();
                this.Close();
            }

        }

        private bool IsNullOrWhiteSpaceCollection(List<string> collection)
        {
            foreach (var item in collection)
            {
                if (String.IsNullOrEmpty(item) || item.Contains(' ')) return true;
            }
            return false;
        }

        private bool IsFieldsValid()
        {
            errorBlock.Text = String.Empty;
            if (loginField.Text.Length < 4)
                errorBlock.Text = "Login cannot be less than 4 characters";
            else if(passField.Password.Length < 8)
                errorBlock.Text = "Password cannot be less than 8 characters";
            return String.IsNullOrEmpty(errorBlock.Text);
        }

        private void phoneField_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key < Key.D0 || e.Key > Key.D9) && e.Key != Key.Right && e.Key != Key.Left && e.Key != Key.Delete && e.Key != Key.Back)
                e.Handled = true;
        }
    }
}
