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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UP01_Quiz_Luzin.Pages;

namespace UP01_Quiz_Luzin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            UpdateCurrentTimeAndDate();
            MainFrame.Navigate(new AuthPage());
            FrameManager.MainFrame = MainFrame;
        }

        private void UpdateCurrentTimeAndDate()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += UpdCurrentTimeAndDate;
            timer.Start();

        }

        private void UpdCurrentTimeAndDate(object sender, EventArgs e)
        {
            CurrentTimeAndDate.Text = DateTime.Now.ToString("G");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти из приложения?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void goBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (!(e.Content is Page page)) return;
            this.Title = $"AppByTeodor - {page.Title}";

            if (page is Pages.AuthPage)
            {
                goBackButton.Visibility = Visibility.Hidden;
            }
            else if (page is Pages.RegPage)
            {
                goBackButton.Visibility = Visibility.Hidden;
            }
            else
            {
                goBackButton.Visibility = Visibility.Visible;
            }
        }
    }
}
