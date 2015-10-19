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

        private PlayerViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataContext = _viewModel = new PlayerViewModel();
                AddCheckboxes();
            }
            catch (Exception)
            {
                // ignored
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

                    c.Margin = i != 0 ? new Thickness(2, 2, 2, 2) : new Thickness(0, 2, 2, 2);

                    _viewModel.Timers.Add(i, c);
                    Hours.Children.Add(c);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }


        public void Dispose()
        {
            _viewModel?.Dispose();
        }

        private void BtnPlay(object sender, RoutedEventArgs e)
        {
            _viewModel?.Play();
        }

        private void BtnStop(object sender, RoutedEventArgs e)
        {
            _viewModel?.Stop();
        }

        private void BtnDing(object sender, RoutedEventArgs e)
        {
            _viewModel?.Ding();
        }

        private void BtnExit(object sender, RoutedEventArgs e)
        {
            _viewModel?.Dispose();
            Close();
        }
    }
}
