using ShopDB.MVVM.ViewModels;
using ShopDB.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ShopDB.MVVM.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        bool IsUpperContain = false, IsLowerContain = false, IsDigitContain = false;
        Regex regex = new Regex("^[a-zA-Z0-9]+$");

        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

            if(StrHelper.IsNullOrWhiteSpaceCollection(collection))
            {
                errorBlock.Text = "Not all fields are completed or fields contain spaces";
                return;
            }

            if (SQL.ExecuteCommand($"SELECT * FROM user WHERE login = '{lloginField.Text}'", true) == SQLResponse.Success)
            {
                SQL.MySqlReader.Read();
                if (!SQL.MySqlReader.HasRows)
                {
                    errorBlock.Text = "This user was not found";
                    return;
                }

                if (SQL.MySqlReader.GetFieldValue<string>(4) != Main.Sha256(lpassField.Password))
                {
                    errorBlock.Text = "Invalid password!";
                    return;
                }

                MainWindowViewModel.AuthenticatedUser = new Models.LoggedUser()
                {
                    AcessLevel = SQL.MySqlReader.GetFieldValue<string>(1) == "user" ? Models.AcessLevel.User : Models.AcessLevel.Admin,
                    Id = SQL.MySqlReader.GetFieldValue<uint>(0),
                    Login = lloginField.Text,
                    Username = SQL.MySqlReader.GetFieldValue<string>(2),
                };

                if (App.MainWindow == null)
                    UI.OpenWindow(App.MainWindow = new MainWindow());
                else
                    App.MainWindow.Show();
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

            if (StrHelper.IsNullOrWhiteSpaceCollection(collection))
            {
                errorBlock.Text = "Not all fields are completed or fields contain spaces";
                return;
            }
            else if (!IsFieldsValid())
                return;

            var checkCommand = $"SELECT * FROM user WHERE login = '{loginField.Text}'";

            if (SQL.ExecuteCommand(checkCommand, true) != SQLResponse.Success)
                return;

            SQL.MySqlReader.Read();
            if (SQL.MySqlReader.HasRows)
            {
                errorBlock.Text = "User with this login already exists";
                return;
            }

            var passHash = Main.Sha256(passField.Password);
            var email = String.IsNullOrEmpty(emailField.Text) ? "NULL" : $"'{emailField.Text}'" ;
            var address = String.IsNullOrEmpty(addressField.Text) ? "NULL" : $"'{addressField.Text}'";
            var phone = String.IsNullOrEmpty(phoneField.Text) ? "NULL" : $"'{phoneField.Text}'";

            var insertCommand = $"INSERT INTO `user` (`id`, `acess_level`, `username`, `login`, `password`, `email`, `address`, `first_name`, `last_name`, `phone_number`) VALUES" +
                $"(NULL,'user','{usernameField.Text}','{loginField.Text}','{passHash}',{email},{address},'{firstNameField.Text}','{lastNameField.Text}',{phone})";

            if(SQL.ExecuteCommand(insertCommand) == SQLResponse.Success && SQL.ExecuteCommand($"SELECT * FROM user WHERE login = '{loginField.Text}'", true) == SQLResponse.Success)
            {
                MainWindowViewModel.AuthenticatedUser = new Models.LoggedUser()
                {
                    AcessLevel =  Models.AcessLevel.User,
                    Id = SQL.MySqlReader.GetFieldValue<uint>(0),
                    Login = loginField.Text,
                    Username = SQL.MySqlReader.GetFieldValue<string>(2),
                };
                if (App.MainWindow == null)
                    UI.OpenWindow(App.MainWindow = new MainWindow());
                else
                    App.MainWindow.Show();
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
