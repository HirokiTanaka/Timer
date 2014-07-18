using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Timer.UserData;

namespace Timer
{
    /// <summary>
    /// SettingsPage.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsPage : Page
    {
        private const int DEFAULT_MAX_SHOW_NUM = 10;


        private UserData.UserData userData
        {
            get { return UserData.UserData.GetInstance(); }
        }

        public SettingsPage()
        {
            InitializeComponent();

            var window = Application.Current.MainWindow;
            window.Height = 300;

            // 画面の初期設定
            var workItems = new ObservableCollection<WorkItemViewListInfo>();
            if (userData.HasWorkSet)
            {
                foreach (var wi in userData.GetAllWorkItems())
                {
                    workItems.Add(new WorkItemViewListInfo() { Name = wi.Name, Minutes = wi.Minutes.ToString() });
                }
            }
            
            while (workItems.Count() < DEFAULT_MAX_SHOW_NUM)
            {
                workItems.Add(new WorkItemViewListInfo());
            }

            LvWorkItems.ItemsSource = workItems;
        }

        private void BackToTimerPage()
        {
            var window = Application.Current.MainWindow;
            ((MainWindow)window).PageFrame.Source = new Uri("/TimerPage.xaml", UriKind.Relative);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackToTimerPage();
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 画面からユーザデータに設定
            var workItems
                = LvWorkItems.Items.OfType<WorkItemViewListInfo>().ToList()
                    .Select(x => x.ToWorkItem()).Where(x => !WorkItem.IsNullOrEmpty(x));
            var workSet = new WorkSet(workItems);
            userData.Edit(workSet);

            // ユーザデータ保存処理
            userData.SaveData();

            BackToTimerPage();
        }

        public class WorkItemViewListInfo
        {
            public string Name { get; set; }
            public string Minutes { get; set; }

            public WorkItemViewListInfo()
            {
                Name = string.Empty;
                Minutes = string.Empty;
            }

            public WorkItem ToWorkItem()
            {
                var minutes = 0;
                int.TryParse(Minutes, out minutes);
                return new WorkItem(Name, minutes);
            }
        }
    }
}
