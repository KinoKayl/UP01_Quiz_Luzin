using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UP01_Quiz_Luzin.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private readonly Entities db;
        public AuthPage()
        {
            InitializeComponent();
            db = new Entities();
        }

        private void LoginBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginBox.Focus();
        }
        private void PasswordBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PassswordBox.Focus();
        }

        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LoginBox.Text) && LoginBox.Text.Length > 0)
            {
                LoginBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginBoxText.Visibility = Visibility.Visible;
            }
        }

        private void PassswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PassswordBox.Password) && PassswordBox.Password.Length > 0)
            {
                PasswordBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordBoxText.Visibility = Visibility.Visible;
            }
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;

            App.Current.Resources["Login"] = login;

            string hashedPassword = GetHash(PassswordBox.Password);

            if (LoginBox.Text.Length < 5)
            {
                MessageBox.Show("Логин должен иметь не меньше 5 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PassswordBox.Password.Length < 5)
            {
                MessageBox.Show("Пароль должен иметь не меньше 5 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = db.Students.AsNoTracking().FirstOrDefault(u => u.Login == login && u.Password == hashedPassword);

            if (user == null)
            {
                MessageBox.Show("Пользователь с такими данными не найден в системе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Вы авторизовались!");

            ((App)Application.Current).CurrentUserID = user.ID;

            if (user.Role == 1)
            {
                NavigationService?.Navigate(new AdminPage());
            }
            else
            {
                NavigationService?.Navigate(new QuizPage());
            }
        }
        private bool IsValidInput(string input)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9]*$");
            return regex.IsMatch(input);
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            LoginBox.Text = string.Empty;
            PassswordBox.Password = string.Empty;
            NavigationService?.Navigate(new RegPage());
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
    }
}
