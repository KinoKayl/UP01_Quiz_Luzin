using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace UP01_Quiz_Luzin.Pages
{
    /// <summary>
    /// Логика взаимодействия для QuizPage.xaml
    /// </summary>
    public partial class QuizPage : Page
    {
        public int CorrectCount = 0;
        public int count = 1;
        private readonly Entities db;

        private DispatcherTimer timer;
        private Stopwatch stopwatch;


        public QuizPage()
        {
            InitializeComponent();
            db = new Entities();


            Canvas.SetLeft(FirstPanel, 500);
            Canvas.SetTop(FirstPanel, 50);

            Canvas.SetLeft(SecondPanel, this.Width);
            Canvas.SetTop(SecondPanel, 50);
            Canvas.SetTop(ThirdPanel, 50);
            Canvas.SetTop(FourthPanel, 50);
            Canvas.SetTop(ResultPanel, 50);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            stopwatch = new Stopwatch();



        }

        public int CorrectCheck(int choise)
        { 
                        
            return CorrectCount; 
        }

        private void AnimatePanels(StackPanel first, StackPanel second)
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
                To = 500,
                Duration = TimeSpan.FromSeconds(1)
            };

            // Применяем анимации
            first.BeginAnimation(Canvas.LeftProperty, firstPanelAnimation);
            second.Visibility = Visibility.Visible;
            second.BeginAnimation(Canvas.LeftProperty, secondPanelAnimation);

            // Скрываем первую панель после завершения анимации
            firstPanelAnimation.Completed += (s, e) =>
            {
                first.Visibility = Visibility.Collapsed;
            };

            /*
            App.Current.Resources["Count"] = 1;

            int count = int.Parse((string)App.Current.Resources["Count"]) ;
            switch (count)
            { 
                case 1: 
                    {
                        // Применяем анимации
                        FirstPanel.BeginAnimation(Canvas.LeftProperty, firstPanelAnimation);
                        SecondPanel.Visibility = Visibility.Visible;
                        SecondPanel.BeginAnimation(Canvas.LeftProperty, secondPanelAnimation);

                        // Скрываем первую панель после завершения анимации
                        firstPanelAnimation.Completed += (s, e) =>
                        {
                            FirstPanel.Visibility = Visibility.Collapsed;
                        };
                        count++;    
                        break;
                    }
                case 2:
                    {
                        SecondPanel.BeginAnimation(Canvas.LeftProperty, firstPanelAnimation);
                        ThirdPanel.Visibility = Visibility.Visible;
                        ThirdPanel.BeginAnimation(Canvas.LeftProperty, secondPanelAnimation);

                        firstPanelAnimation.Completed += (s, e) =>
                        {
                            SecondPanel.Visibility = Visibility.Collapsed;
                        };
                        count++;
                        break;
                    }
                case 3:
                    {
                        ThirdPanel.BeginAnimation(Canvas.LeftProperty, firstPanelAnimation);
                        FourthPanel.Visibility = Visibility.Visible;
                        FourthPanel.BeginAnimation(Canvas.LeftProperty, secondPanelAnimation);

                        firstPanelAnimation.Completed += (s, e) =>
                        {
                            ThirdPanel.Visibility = Visibility.Collapsed;
                        };
                        count++;
                        break;
                    }
                case 4:
                    {
                        FirstPanel.BeginAnimation(Canvas.LeftProperty, firstPanelAnimation);
                        SecondPanel.Visibility = Visibility.Visible;
                        SecondPanel.BeginAnimation(Canvas.LeftProperty, secondPanelAnimation);

                        firstPanelAnimation.Completed += (s, e) =>
                        {
                            FirstPanel.Visibility = Visibility.Collapsed;
                        };
                        break;
                    }
                
            }
            App.Current.Resources["Count"] = count;
            */


        }

        private void AniButton_Click(object sender, RoutedEventArgs e)
        {
            

            switch (count)
            {
                case 1:
                    {
                        AnimatePanels(FirstPanel, SecondPanel);
                        if ((FourthRadioButton1.IsChecked == true)) 
                        {
                            CorrectCount++;
                        }
                        count++;
                        break;
                    }
                case 2:
                    {
                        if ((FirstRadioButton2.IsChecked == true))
                        {
                            CorrectCount++;
                        }
                        AnimatePanels(SecondPanel, ThirdPanel);
                        count++;
                        break;
                    }
                case 3:
                    {
                        if ((SecondRadioButton3.IsChecked == true))
                        {
                            CorrectCount++;
                        }
                        AnimatePanels(ThirdPanel, FourthPanel);
                        count++;
                        AniButton.Content = "Получить результат";
                        AniButton.Width = 180;
                        break;
                    }
                case 4:
                    {

                        if ((ThirdRadioButton4.IsChecked == true))
                        {
                            CorrectCount++;
                        }
                        ResultTextBox.Text = CorrectCount.ToString();
                        AnimatePanels(FourthPanel, ResultPanel);
                        count++;
                        AniButton.Content = "Начать с начала";

                        stopwatch.Stop();
                        timer.Stop();

                        string login = (string)App.Current.Resources["Login"];
                        var user = db.Students.AsNoTracking()
                            .FirstOrDefault(u => u.Login == login);

                        int number = user != null ? user.Number : -1;
                        int question = 4;

                        var result = new Results
                        {
                            Result_StudentNumber = number,
                            Result_Question = question,
                            Result_QuestionCount = count,
                            Result_QuestionCorrectCount = CorrectCount,
                            Result_QuestionTime = TimerTextBlock.Text,
                            Result_Result = count,
                        };

                        db.Results.Add(result);
                        db.SaveChanges();
                        break;
                    }
                case 5:
                    {
                        AnimatePanels(ResultPanel, FirstPanel);
                        AniButton.Width = 180;
                        CorrectCount = 0;
                        AniButton.Width = 82;
                        count = 1;
                        
                        StartButton.Visibility = Visibility.Visible;

                        break;
                    }


            }

            
        }

        private void FourthRadioButton2_Checked(object sender, RoutedEventArgs e)
        {

        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
                timer.Start();
            }
            else
            {
                stopwatch.Stop();
                timer.Stop();
            }

            StartButton.Visibility = Visibility.Collapsed;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimerTextBlock.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
        }
    }
}
