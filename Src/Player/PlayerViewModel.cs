using System;
using System.Collections;
using System.Diagnostics;
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
        private IWavePlayer wavePlayer;
        private static AudioFileReader audioReader;
        private SoundPlayer soundPlayer;
        private Timer oneSecTimer;
        private static Timer autoTimer;
        private int _counter;
        public Hashtable timers;
        private Stream str;
        private static int _dingInterval = 0;

        public PlayerViewModel()
        {
            _counter = 0;

            Status = "Waiting for WEBfm";

            timers = new Hashtable();

            oneSecTimer = new Timer { Interval = 1000 };
            oneSecTimer.Elapsed += OneSecTimer_Elapsed;
            oneSecTimer.Enabled = true;

            autoTimer = new Timer { Interval = 700 };
            autoTimer.Elapsed += AutoTimer_Elapsed;

            Init();
        }

        private void AutoTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (wavePlayer == null)
                {
                    return;
                }

                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    var hour = DateTime.Now.Hour;
                    var t = (CheckBox)timers[hour];
                    if ((bool)t.IsChecked)
                    {
                        wavePlayer.Play();
                    }
                    else
                    {
                        wavePlayer.Stop();
                    }
                }));
            }
            catch (Exception te) { }
            Debug.WriteLine("Autotimer" + DateTime.Now);
        }

        private async void Init()
        {
            await Task.Run((Action)(() =>
            {
                soundPlayer = new SoundPlayer();

                audioReader = new AudioFileReader("http://netradio.webfm.dk/Mobil");
                audioReader.Volume = 0.2f;

                wavePlayer = new WaveOutEvent();
                wavePlayer.Init(audioReader);

                str = Properties.Resources.ding6;
                soundPlayer.Stream = str;
                soundPlayer.LoadAsync();

                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    Volume = 0.5f;
                    Status = "WEBfm ready";
                    Ready = true;
                }));

            }));
        }

        private void OneSecTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                NextDing = _dingInterval != 0 ? string.Format("Næste ding om : {0} sek", (_dingInterval - _counter)) : "";
            }));

            Ding();
        }

        private async void Ding()
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

            await Task.Run((Action)(() =>
            {
                soundPlayer.PlaySync();
            }));
        }

        public void Play()
        {
            wavePlayer?.Play();
        }

        public void Stop()
        {
            wavePlayer?.Stop();
        }


        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(
            "Volume", typeof(float), typeof(PlayerViewModel),
            new PropertyMetadata(
                default(float),
                OnVolumePropertyChanged
                ));

        public static void OnVolumePropertyChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            audioReader.Volume = (float)e.NewValue;
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
            autoTimer.Enabled = (bool)e.NewValue;
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
                wavePlayer?.Dispose();
                soundPlayer?.Dispose();
                audioReader?.Dispose();
            }
            catch (Exception ed) { }
        }
    }
}
