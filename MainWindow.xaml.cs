using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Timer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // どこでもドラッグして移動できるようにする
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
        }
    }
}
