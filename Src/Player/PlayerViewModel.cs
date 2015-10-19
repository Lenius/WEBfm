using System;
using System.Collections;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace Player
{
    public class PlayerViewModel : DependencyObject, IDisposable
    {
        private IWavePlayer _wavePlayer;
        private static AudioFileReader _audioReader;
        private SoundPlayer _soundPlayer;
        private static Timer _autoTimer;
        private int _counter;
        public Hashtable Timers;
        private Stream _str;
        private static int _dingInterval;

        public PlayerViewModel()
        {
            try
            {
                _counter = 0;

                Status = "Waiting for WEBfm";

                Timers = new Hashtable();

                var oneSecTimer = new Timer { Interval = 1000 };
                oneSecTimer.Elapsed += OneSecTimer_Elapsed;
                oneSecTimer.Enabled = true;

                _autoTimer = new Timer { Interval = 700 };
                _autoTimer.Elapsed += AutoTimer_Elapsed;

                Init();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void AutoTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_wavePlayer == null)
                {
                    return;
                }

                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    var hour = DateTime.Now.Hour;
                    var t = (CheckBox)Timers[hour];
                    if (t.IsChecked != null && (bool)t.IsChecked)
                    {
                        _wavePlayer.Play();
                    }
                    else
                    {
                        _wavePlayer.Stop();
                    }
                }));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async void Init()
        {
            await Task.Run(() =>
            {
                _soundPlayer = new SoundPlayer();

                _audioReader = new AudioFileReader("http://netradio.webfm.dk/Mobil") { Volume = 0.2f };

                _wavePlayer = new WaveOutEvent();
                _wavePlayer.Init(_audioReader);

                _str = Properties.Resources.ding6;
                _soundPlayer.Stream = _str;
                _soundPlayer.LoadAsync();

                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    Volume = 0.5f;
                    Status = "WEBfm ready";
                    Ready = true;
                }));

            });
        }

        private void OneSecTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                NextDing = _dingInterval != 0 ? $"Næste ding om : {(_dingInterval - _counter)} sek" : "";
            }));

            DingFunction();
        }

        private async void DingFunction()
        {
            if (_dingInterval <= 0)
            {
                return;
            }

            if (_counter != _dingInterval)
            {
                _counter++;
                return;
            }
            _counter = 0;

            await Task.Run(() =>
            {
                _soundPlayer?.PlaySync();
            });
        }

        public async void Ding()
        {
            await Task.Run(() =>
            {
                _soundPlayer?.PlaySync();
            });
        }

        public void Play()
        {
            _wavePlayer?.Play();
        }

        public void Stop()
        {
            _wavePlayer?.Stop();
        }


        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(
            "Volume", typeof(float), typeof(PlayerViewModel),
            new PropertyMetadata(
                default(float),
                OnVolumePropertyChanged
                ));

        public static void OnVolumePropertyChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            _audioReader.Volume = (float)e.NewValue;
        }

        public float Volume
        {
            get { return (float)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        public static readonly DependencyProperty ReadyProperty = DependencyProperty.Register(
            "Ready", typeof(bool), typeof(PlayerViewModel), new PropertyMetadata(default(bool)));

        public bool Ready
        {
            get { return (bool)GetValue(ReadyProperty); }
            set { SetValue(ReadyProperty, value); }
        }


        public static readonly DependencyProperty AutoPlayProperty = DependencyProperty.Register(
            "AutoPlay", typeof(bool), typeof(PlayerViewModel), new PropertyMetadata(default(bool), OnAutoPropertyChanged));

        private static void OnAutoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _autoTimer.Enabled = (bool)e.NewValue;
        }

        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValue(AutoPlayProperty, value); }
        }

        public static readonly DependencyProperty NextDingProperty = DependencyProperty.Register(
            "NextDing", typeof(string), typeof(PlayerViewModel), new PropertyMetadata(default(string)));

        public string NextDing
        {
            get { return (string)GetValue(NextDingProperty); }
            protected set { SetValue(NextDingProperty, value); }
        }

        public static readonly DependencyProperty DingIntervalProperty = DependencyProperty.Register(
            "DingInterval", typeof(int), typeof(PlayerViewModel), new PropertyMetadata(default(int), OnDingIntervalPropertyChanged));

        private static void OnDingIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _dingInterval = (int)e.NewValue;
        }

        public int DingInterval
        {
            get { return (int)GetValue(DingIntervalProperty); }
            set
            {
                SetValue(DingIntervalProperty, value);
            }
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(string), typeof(PlayerViewModel), new PropertyMetadata(default(string)));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }


        public void Dispose()
        {
            try
            {
                _wavePlayer?.Dispose();
                _soundPlayer?.Dispose();
                _audioReader?.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
