using MySqlConnector;
using ShopDB.MVVM.ViewModels;
using ShopDB.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        bool IsUpperContain = false, IsLowerContain = false, IsDigitContain = false;

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
            if(Popup.IsOpen) Popup.IsOpen= false;
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

            if(Utilities.StrHelper.IsNullOrWhiteSpaceCollection(collection))
            {
                errorBlock.Text = "Not all fields are completed or fields contain spaces";
                return;
            }

            if (Utilities.SQL.ExecuteCommand($"SELECT * FROM user WHERE login = '{lloginField.Text}'", true) == Utilities.SQLResponse.Success)
            {
                Utilities.SQL.MySqlReader.Read();
                if (!Utilities.SQL.MySqlReader.HasRows)
                {
                    errorBlock.Text = "This user was not found";
                    return;
                }

                if (Utilities.SQL.MySqlReader.GetFieldValue<string>(4) != Utilities.Main.Sha256(lpassField.Password))
                {
                    errorBlock.Text = "Invalid password!";
                    return;
                }

                Utilities.UI.OpenWindow(new MainWindow());
                MainWindowViewModel.AuthenticatedUser = lloginField.Text;
                this.Close();

            }
                
        }

        private void SignUpAction()
        {
            errorBlock.Text = "";
            var collection = new List<string>()
            {
                loginField.Text,
                usernameField.Text,
                firstNameField.Text,
                lastNameField.Text,
            };

            if (Utilities.StrHelper.IsNullOrWhiteSpaceCollection(collection))
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
                Utilities.UI.OpenWindow(new MainWindow());
                MainWindowViewModel.AuthenticatedUser = loginField.Text;
                this.Close();
            }

        }


        private bool IsFieldsValid()
        {
            errorBlock.Text = String.Empty;
            if (loginField.Text.Length < 4)
                errorBlock.Text = "Login cannot be less than 4 characters";
            else if(!IsUpperContain || !IsLowerContain || !IsDigitContain)
                errorBlock.Text = "Password does not meet the requirements!";
            return String.IsNullOrEmpty(errorBlock.Text);
        }

        private void phoneField_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (((e.Key < Key.D0 || e.Key > Key.D9 ) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9))
                && e.Key != Key.Right && e.Key != Key.Left && e.Key != Key.Delete && e.Key != Key.Back && e.Key != Key.NumLock)
            {
                e.Handled = true;
                return;
            }
            
            
        }

        private void passField_GotFocus(object sender, RoutedEventArgs e)
        {
            Popup.PlacementTarget = passField;
            Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
            Popup.IsOpen = true;
        }

        private void loginField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void passField_LostFocus(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void passField_PasswordChanged(object sender, RoutedEventArgs e)
        {
            IsUpperContain = IsLowerContain = IsDigitContain = false;

            if (passField.Password.Length > 8)
            {
                CharsLenPass.Foreground = Brushes.Green;
            }
            else
            {
                CharsLenPass.Foreground = Brushes.PaleVioletRed;
            }
                

            foreach (var item in passField.Password)
            {
                if (char.IsDigit(item) && !IsDigitContain)
                {
                    IsDigitContain = true;
                    NumberContPass.Foreground = Brushes.Green;
                }
                else if (char.IsLower(item) && !IsLowerContain)
                {
                    IsLowerContain = true;
                    LowerContPass.Foreground = Brushes.Green;
                }
                else if (char.IsUpper(item) && !IsUpperContain)
                {
                    IsUpperContain = true;
                    UpperContPass.Foreground = Brushes.Green;
                }
                    
            }

            if (!IsLowerContain)
                LowerContPass.Foreground = Brushes.PaleVioletRed;
            if (!IsUpperContain)
                UpperContPass.Foreground = Brushes.PaleVioletRed;
            if (!IsDigitContain)
                NumberContPass.Foreground = Brushes.PaleVioletRed;

        }
    }
}
