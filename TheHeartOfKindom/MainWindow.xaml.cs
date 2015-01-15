using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TheHeartOfKindom
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private TaskWindow taskWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.DesktopDirectory;
            dlg.Description = "选择日文文本文件夹路径";
            dlg.ShowNewFolderButton = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Text.Text = dlg.SelectedPath;
            }
            dlg.Dispose();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.DesktopDirectory;
            dlg.Description = "选择译文文本文件夹路径";
            dlg.ShowNewFolderButton = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TranslationText.Text = dlg.SelectedPath;
            }
            dlg.Dispose();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(Text.Text) || string.IsNullOrEmpty(TranslationText.Text))
            {
                System.Windows.MessageBox.Show("请先选择原文和译文的路径");
                return;
            }
            Text.IsEnabled = false;
            TranslationText.IsEnabled = false;
            taskWindow = new TaskWindow();
            taskWindow.Owner = this;
            taskWindow.Loaded += taskWindow_Loaded;
            taskWindow.Unloaded += taskWindow_Unloaded;
            taskWindow.ShowDialog();
        }

        private void taskWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Text.IsEnabled = true;
            TranslationText.IsEnabled = true;
        }

        private void taskWindow_Loaded(object sender, RoutedEventArgs e)
        {
            taskWindow.DoWork(Text.Text, TranslationText.Text);
        }
    }
}
