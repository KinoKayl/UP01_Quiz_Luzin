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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UP01_Quiz_Luzin.Pages
{
    /// <summary>
    /// Логика взаимодействия для QuizPage.xaml
    /// </summary>
    public partial class QuizPage : Page
    {
        public QuizPage()
        {
            InitializeComponent();

            Canvas.SetLeft(FirstPanel, 300);
            Canvas.SetTop(FirstPanel, 50);

            Canvas.SetLeft(SecondPanel, this.Width);
            Canvas.SetTop(SecondPanel, 50);
            AnimatePanels();

        }

        private void AnimatePanels()
        {
            // Получение ширины окна
            double windowWidth = Application.Current.MainWindow.ActualWidth;

            if (double.IsNaN(windowWidth) || windowWidth <= 0)
            {
                MessageBox.Show("Ошибка: ширина окна не определена.");
                return;
            }

            // Анимация для первой панели (уходит влево)
            DoubleAnimation firstPanelAnimation = new DoubleAnimation
            {
                From = 300,
                To = -windowWidth,
                Duration = TimeSpan.FromSeconds(1)
            };

            // Анимация для второй панели (приходит справа)
            DoubleAnimation secondPanelAnimation = new DoubleAnimation
            {
                From = windowWidth,
                To = 300,
                Duration = TimeSpan.FromSeconds(1)
            };

            // Применяем анимации
            FirstPanel.BeginAnimation(Canvas.LeftProperty, firstPanelAnimation);
            SecondPanel.Visibility = Visibility.Visible;
            SecondPanel.BeginAnimation(Canvas.LeftProperty, secondPanelAnimation);

            // Скрываем первую панель после завершения анимации
            firstPanelAnimation.Completed += (s, e) =>
            {
                FirstPanel.Visibility = Visibility.Collapsed;
            };
        }

        private void AniButton_Click(object sender, RoutedEventArgs e)
        {
            AnimatePanels();
        }
    }
}
