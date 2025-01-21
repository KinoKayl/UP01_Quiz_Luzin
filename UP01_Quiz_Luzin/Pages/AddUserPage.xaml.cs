using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
    /// Логика взаимодействия для AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Page
    {
        private static readonly Regex FIOregex = new Regex(@"^[А-ЯЁ][а-яё]+$");
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*\d)[a-zA-Z0-9]{6,}$");
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        private readonly Entities db;

        public bool isRedact = false;

        private Students currentuser = new Students();

        public string PhotoPath = null;

        public AddUserPage(Students selecteduser)
        {
            InitializeComponent();
            db = new Entities();

            if (selecteduser != null)
            {
                currentuser = selecteduser;
                isRedact = true;
                AddButton.Content = "Изменить";
                AddLabel.Content = "Редактирование пользователя";
            }
            else
            {
                isRedact = false;
                AddButton.Content = "Добавить";
                AddLabel.Content = "Добавление пользователя";
            }

            DataContext = currentuser;
            PhotoPath = null;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            LoginBox.Text = string.Empty;
            PassswordBox.Password = string.Empty;
            PassswordBoxAccept.Password = string.Empty;
            FIOBox.Text = string.Empty;
            Roles.SelectedIndex = 1;
            NavigationService.GoBack();
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

        private void LoginBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginBox.Focus();
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

        private void PasswordBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PassswordBox.Focus();
        }

        private void PassswordBoxAccept_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PassswordBoxAccept.Password) && PassswordBoxAccept.Password.Length > 0)
            {
                PasswordBoxAcceptText.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordBoxAcceptText.Visibility = Visibility.Visible;
            }
        }

        private void PasswordBoxAcceptText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PassswordBoxAccept.Focus();
        }

        private void FIOBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FIOBox.Text) && FIOBox.Text.Length > 0)
            {
                FIOBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                FIOBoxText.Visibility = Visibility.Visible;
            }
        }

        private void FIOBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FIOBox.Focus();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = GetHash(PassswordBox.Password);
            string passwordAccept = GetHash(PassswordBoxAccept.Password);
            string FIO = FIOBox.Text;
            int Role = (Roles.SelectedItem as ComboBoxItem)?.Tag as string == "1" ? 1 : 2;
            string foto = PhotoPath;


            try
            {
                if (login.Length < 5)
                {
                    MessageBox.Show("Логин не может быть меньше 5 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!IsValidLogin(login))
                {
                    MessageBox.Show("Логин должен быть либо email, либо логином, состоящим из 5 и более символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (isRedact != true)
                {
                    if (db.Students.Any(u => u.Login == login))
                    {
                        MessageBox.Show("Пользователь с таким логином уже зарегистрирован в системе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        LoginBox.Text = "";
                        return;
                    }

                }


                if (!PasswordRegex.IsMatch(password))
                {
                    MessageBox.Show("Пароль не может быть меньше 6 символов! Также, пароль включает только английские символы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (passwordAccept != password)
                {
                    MessageBox.Show("Пароль не совпадает с введённым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Roles.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите роль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!FIOcheck(FIO))
                {
                    MessageBox.Show("ФИО введено некорректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existingUser = db.Students.FirstOrDefault(u => u.Login == login);

                if (existingUser != null)
                {
                    existingUser.Password = password;
                    existingUser.FIO = FIO;
                    existingUser.Role = Role;
                    existingUser.Photo = foto;
                    db.SaveChanges();
                }
                else
                {
                    var newUser = new Students
                    {
                        Login = login,
                        Password = password,
                        FIO = FIO,
                        Role = Role,
                        Photo = foto
                    };
                    db.Students.Add(newUser);
                    db.SaveChanges();
                }

                switch (isRedact)
                {
                    case true:
                        MessageBox.Show("Успешное изменение данных", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case false:
                        MessageBox.Show("Регистрация завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }

                NavigationService.GoBack();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityErrors in ex.EntityValidationErrors)
                {
                    foreach (var valError in entityErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Ошибка: {valError.ErrorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
        private void BtnFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Title = "Выберите изображение";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                BtnFoto.Content = filePath;
                PhotoPath = filePath;
                //BitmapImage bitmapImage = new BitmapImage(new Uri(filePath));
                //Image.Source = bitmapImage; // Убедитесь, что у вас есть элемент Image с таким именем
            }

            PhotoBoxText.Visibility = Visibility.Collapsed;
        }

        private bool IsValidLogin(string login)
        {
            return login.Length >= 5 && (EmailRegex.IsMatch(login) || !string.IsNullOrWhiteSpace(login));
        }

        private bool FIOcheck(string FIO)
        {
            var splitFIO = FIO.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Подразумевается, что у пользователя нет отчества. В качестве данного условия, пришлось разделить ФИО на три части.
            if (splitFIO.Length < 2 || splitFIO.Length > 3)
            {
                return false;
            }

            foreach (var part in splitFIO)
            {
                if (string.IsNullOrWhiteSpace(part) || !FIOregex.IsMatch(part) || part.Length > 50)
                {
                    return false;
                }
            }
            return true;
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
