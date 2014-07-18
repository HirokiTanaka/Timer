using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Timer
{
    /// <summary>
    /// TimerPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TimerPage : Page
    {
        private BackgroundWorker _bw;

        private UserData.UserData userData
        {
            get { return UserData.UserData.GetInstance(); }
        }

        public TimerPage()
        {
            InitializeComponent();

            var window = Application.Current.MainWindow;
            window.Height = 160;

            if (userData.HasWorkSet)
            {
                // プログレスバーの設定
                TimerBar.Maximum = userData.TotalWorkSeconds;
                SetTimerBarProp();
                _bw = new BackgroundWorker();
                _bw.WorkerReportsProgress = true;
                _bw.WorkerSupportsCancellation = true;
                _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                _bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                _bw.RunWorkerAsync();

                // ボタン設定
                var hasUndone = !userData.IsAllFinished;
                BtnFinish.IsEnabled = hasUndone;
                BtnStart.IsEnabled = hasUndone;
                BtnStop.IsEnabled = false;
            }
            else
            {
                BtnStart.IsEnabled = false;
                BtnStop.IsEnabled = false;
            }

            // 現在の作業情報の設定
            SetCurrentWorkInfo();
        }
        
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            // 最小化ボタン押下時
            SystemCommands.MinimizeWindow(Application.Current.MainWindow);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // ユーザデータ保存処理
            userData.SaveData();

            // アプリ終了
            Application.Current.Shutdown();
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            if (userData.IsAllFinished)
                return;

            userData.FinishWorkItem();
            if (userData.IsAllFinished)
            {
                userData.CountStop();
                BtnStart.IsEnabled = false;
                BtnStop.IsEnabled = false;
                _bw.CancelAsync();
            }

            SetTimerBarProp();
            SetCurrentWorkInfo();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            userData.CountStart();
            BtnStart.IsEnabled = false;
            BtnStop.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            userData.CountStop();
            BtnStart.IsEnabled = true;
            BtnStop.IsEnabled = false;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            userData.CountStop();
            BtnStart.IsEnabled = true;
            BtnStop.IsEnabled = false;

            var window = Application.Current.MainWindow;
            ((MainWindow)window).PageFrame.Source = new Uri("/SettingsPage.xaml", UriKind.Relative);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                _bw.ReportProgress(0, string.Empty);
                System.Threading.Thread.Sleep(1);
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetTimerBarProp();
            SetCurrentWorkTimeInfo();
        }

        private void SetTimerBarProp()
        {
            var current = userData.GetSecond();
            TimerBar.Value = Math.Min(current, TimerBar.Maximum);

            var nextLimit = userData.GetNextLimitSecond();
            var styleName = nextLimit < current ? "WarnProgressBarStyle" : "NormalProgressBarStyle";
            TimerBar.Style = (Style)(Application.Current.Resources[styleName]);
        }

        private void SetCurrentWorkInfo()
        {
            if (!userData.HasWorkSet)
            {
                LblWorkName.Content = "未設定";
                LblTimeLimit.Content = string.Empty;
                BtnFinish.IsEnabled = false;
            }
            else if (userData.IsAllFinished)
            {
                LblWorkName.Content = "完了!!";
                LblTimeLimit.Content = string.Empty;
                BtnFinish.IsEnabled = false;
            }
            else
            {
                LblWorkName.Content = userData.CurrentWorkName;
                SetCurrentWorkTimeInfo();
                BtnFinish.IsEnabled = true;
            }
        }

        private void SetCurrentWorkTimeInfo()
        {
            var current = userData.GetSecond();
            var nextLimit = userData.GetNextLimitSecond();
            var strLimit = TimeSpan.FromSeconds(Math.Abs(nextLimit - current)).ToTimeSpanString();

            if (current < nextLimit || strLimit == "0s")
            {
                LblTimeLimit.Content = string.Format("{0}", strLimit);
                LblTimeLimit.Foreground = Brushes.White;
            }
            else
            {
                LblTimeLimit.Content = string.Format("-{0}", strLimit);
                LblTimeLimit.Foreground = Brushes.OrangeRed;
            }
        }
    }
}
