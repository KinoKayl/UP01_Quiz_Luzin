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
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        private static readonly Regex FIOregex = new Regex(@"^[А-ЯЁ][а-яё]+$");
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*\d)[a-zA-Z0-9]{6,}$");
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        private readonly Entities db;


        private Students currentuser = new Students();
        public RegPage()
        {
            InitializeComponent();
            db = new Entities();

        }

        private void GoBackPage() 
        {
            NavigationService?.Navigate(new RegPage());

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            LoginBox.Text = string.Empty;
            PassswordBox.Password = string.Empty;
            PassswordBoxAccept.Password = string.Empty;
            FIOBox.Text = string.Empty;
            Roles.SelectedIndex = 1;
            GoBackPage();
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

        private void NumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NumberBox.Text) && NumberBox.Text.Length > 0)
            {
                NumberBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                NumberBoxText.Visibility = Visibility.Visible;
            }
        }

        private void NumberBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NumberBox.Focus();
        }

        private void GroupNumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(GroupNumberBox.Text) && GroupNumberBox.Text.Length > 0)
            {
                GroupNumberBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                GroupNumberBoxText.Visibility = Visibility.Visible;
            }
        }

        private void GroupNumberBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GroupNumberBox.Focus();
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

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = GetHash(PassswordBox.Password);
            string passwordAccept = GetHash(PassswordBoxAccept.Password);
            string FIO = FIOBox.Text;
            int Role = (Roles.SelectedItem as ComboBoxItem)?.Tag as string == "1" ? 1 : 2;
            int number = 0;
            int groupNumber = 0;
            string photo;

            try
            {
                // Проверки полей для регистрации
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

                if (db.Students.Any(u => u.Login == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже зарегистрирован в системе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoginBox.Text = "";
                    return;
                }

                if (!PasswordRegex.IsMatch(PassswordBox.Password))
                {
                    MessageBox.Show("Пароль не может быть меньше 6 символов! Также пароль включает только английские символы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                if (BtnFoto.Content == null)
                {
                    MessageBox.Show("Выберите фото", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else { photo = BtnFoto.Content.ToString(); }

                if (!NumberCheck())
                {
                    MessageBox.Show("Номер студенческого билета введен не верно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else { number = int.Parse(NumberBox.Text); }

                if (!GroupNumberCheck())
                {
                    MessageBox.Show("Номер группы введен не верно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else { groupNumber = int.Parse(GroupNumberBox.Text); }

                
                       
                // Создание нового пользователя
                var newUser = new Students
                {
                    Login = login,
                    Password = password,
                    FIO = FIO,
                    Role = Role,
                    Number = number,
                    GroupNumber = groupNumber,
                    Photo = photo,
                };

                db.Students.Add(newUser);
                db.SaveChanges();
                GoBackPage();
                MessageBox.Show("Регистрация завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityErrors in ex.EntityValidationErrors)
                {
                    foreach (var valError in entityErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Ошибка: {valError.ErrorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidLogin(string login)
        {
            return login.Length >= 5 && (EmailRegex.IsMatch(login) || !string.IsNullOrWhiteSpace(login));
        }
        private bool NumberCheck()
        {
            if(int.TryParse(NumberBox.Text, out _))
            {
                if (NumberBox.Text.Length == 6)
                {
                    return true;
                }
                else
                { return false; }
            
            }
            else
            {
                return false;
            }
            
             
        }

        private bool GroupNumberCheck()
        {
            if (int.TryParse(GroupNumberBox.Text, out _))
            {
                if (GroupNumberBox.Text.Length == 3)
                {
                    return true;
                }
                else
                { return false; }

            }
            else
            {
                return false;
            }


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

        //Одностророннее хэширование
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }

        }

        //Выбор изображения
        private void BtnFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Title = "Выберите изображение";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                BtnFoto.Content = filePath;
                //BitmapImage bitmapImage = new BitmapImage(new Uri(filePath));
                //Image.Source = bitmapImage; // Убедитесь, что у вас есть элемент Image с таким именем
            }

            PhotoBoxText.Visibility = Visibility.Collapsed;
        }


    }
}
