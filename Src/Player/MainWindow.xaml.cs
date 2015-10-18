using System;
using System.Windows;
using System.Windows.Controls;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IDisposable
    {

        private PlayerViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataContext = ViewModel = new PlayerViewModel();
                AddCheckboxes();
            }
            catch (Exception)
            {

            }
        }

        private void AddCheckboxes()
        {
            try
            {
                var hour = DateTime.Now.Hour;
                for (int i = 0; i < 24; i++)
                {
                    var c = new CheckBox { ToolTip = i + " hour" };

                    if (hour == i)
                    {
                        c.IsChecked = true;
                    }

                    if (i != 0)
                    {
                        c.Margin = new Thickness(2, 2, 2, 2);
                    }
                    else
                    {
                        c.Margin = new Thickness(0, 2, 2, 2);
                    }

                    ViewModel.timers.Add(i, c);
                    hours.Children.Add(c);
                }
            }
            catch (Exception eb) { }

        }


        public void Dispose()
        {
            ViewModel?.Dispose();
        }

        private void BtnPlay(object sender, RoutedEventArgs e)
        {
            ViewModel?.Play();
        }

        private void BtnStop(object sender, RoutedEventArgs e)
        {
            ViewModel?.Stop();
        }

        private void BtnExit(object sender, RoutedEventArgs e)
        {
            ViewModel.Dispose();
            Close();
        }
    }
}
