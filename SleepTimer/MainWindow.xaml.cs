using System;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using PowerManagerLibrary;

namespace SleepTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer = new DispatcherTimer();
        private DateTime expectedTime;
        private TimerMode selectedMode;
        IPowerManager powerManager = new PowerManager();

        enum TimerMode
        {
            Nope,
            ShutDown,
            Sleep
        }

        public MainWindow()
        {
            InitializeComponent();
            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);

            btnStart.Background = Brushes.Green;
            expectedTime = DateTime.MaxValue;
            label.Content = "Please choose time.";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (btnStart.Content.ToString().StartsWith("Start"))
            {
                var strHours = Hours.SelectionBoxItem.ToString();
                var strMinutes = Minutes.SelectionBoxItem.ToString();
                var timerMode = Type.SelectionBoxItem.ToString();

                double hours;
                double minutes;

                double.TryParse(strHours, out hours);
                double.TryParse(strMinutes, out minutes);

                expectedTime = DateTime.Now.AddHours(hours).AddMinutes(minutes);
                selectedMode = timerMode.StartsWith("Sleep") ? TimerMode.Sleep : TimerMode.ShutDown;

                btnStart.Content = "Stop";
                btnStart.Background = Brushes.Red;

                Timer.Start();
            }
            else
            {
                selectedMode = TimerMode.Nope;
                expectedTime = DateTime.MaxValue;
                label.Content = "Please choose time.";

                btnStart.Content = "Start";
                btnStart.Background = Brushes.Green;

                label.Content = "Please choose time.";
                Timer.Stop();
            }
        }

        private void Timer_Click(object sender, EventArgs e)
        {
            if (expectedTime > DateTime.Now)
            {
                int totalMinutes = (int)expectedTime.Subtract(DateTime.Now).TotalMinutes;
                int hours = totalMinutes / 60;
                int mins = totalMinutes % 60;
                string hoursValue = (hours < 10) ? $"0{hours}" : hours.ToString();
                string minsValue = (mins < 10) ? $"0{mins}" : mins.ToString();
                label.Content = $"{selectedMode} in {hoursValue}:{minsValue}";
            }
            else
            {
                switch (selectedMode)
                {
                    case TimerMode.ShutDown:
                        powerManager.ShutDown();
                        break;

                    case TimerMode.Sleep:
                        powerManager.Sleep();
                        break;
                }
                Timer.Stop();
            }
        }

        
    }
}
