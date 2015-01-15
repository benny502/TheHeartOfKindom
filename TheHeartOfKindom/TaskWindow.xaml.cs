using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TheHeartOfKindom
{
    /// <summary>
    /// TaskWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TaskWindow : Window
    {
        private List<TextModel> Jap;
        private List<TextModel> Chi;
        ObservableCollection<RepeatModel> repeater = new ObservableCollection<RepeatModel>();
        int globle_ID = 0;

        public TaskWindow()
        {
            InitializeComponent();
            listView.ItemsSource = repeater;
        }

        public void DoWork(string textPath, string translationTextPath)
        {
            GetText(textPath,translationTextPath);
        }

        private void GetText(string path1, string path2)
        {
            BackgroundWorker backgroundWorder = new BackgroundWorker();
            backgroundWorder.DoWork += (sender, e) => 
            { 
                GetTextAsync(path1,path2);
            };
            backgroundWorder.RunWorkerCompleted += backgroundWorder_RunWorkerCompleted;
            backgroundWorder.RunWorkerAsync();
        }

        private void GetTextAsync(string path1, string path2)
        {
            Jap = Read(path1);
            Chi = Read(path2);
        }

        private List<TextModel> Read(string path)
        {
            List<TextModel> list = new List<TextModel>();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            var files = directoryInfo.GetFiles("*.txt",SearchOption.AllDirectories);
            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    FileEncoder fileEncoder = new FileEncoder();
                    string encoding = fileEncoder.GetEncodingName(file);
                    list.AddRange(ReadEachFile(file.FullName, encoding));
                }
            }
            return list;
        }

        private List<TextModel> ReadEachFile(string path , string encoding)
        {
            List<TextModel> list = new List<TextModel>();
            string fileName = Path.GetFileName(path);
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open);
                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                {
                    while (!reader.EndOfStream)
                    {
                        TextModel textModel = new TextModel();
                        string str = reader.ReadLine();
                        if (!string.IsNullOrEmpty(str))
                        {
                            string[] val = str.Split(new char[] { ',' });
                            textModel.Path = fileName;
                            textModel.Address = val[0];
                            textModel.Length = val[1];
                            textModel.Text = val[2];
                            list.Add(textModel);
                        }
                    }
                }
                stream.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return list;
        }

        private void backgroundWorder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        { 
            MessageBox.Show("查找完毕");
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (Jap == null || Jap.Count == 0)
                {
                    throw new Exception("读取文本出错");
                }
                for (int i = 0; i < Jap.Count; ++i)
                {
                    List<TextModel> matches = Jap.FindAll(it => it.Text.Equals(Jap[i].Text));
                    if (matches.Count > 1)
                    {
                        //RepeatModel repeat = new RepeatModel();
                        //repeat.ID = globle_ID.ToString();
                        //repeat.Path = Jap[i].Path;
                        //repeat.Address = Jap[i].Address;
                        //repeat.Text = Jap[i].Text;
                        //repeat.TraslationText = Chi.Find(it => (it.Address == Jap[i].Address && it.Path.Equals(Jap[i].Path))).Text;
                        //this.Dispatcher.Invoke(new Action(() =>
                        //{
                        //    ++globle_ID;
                        //    repeater.Add(repeat);
                        //}));
                        SaveMatches(matches);
                        RemoveMatches(matches);
                    }
                    int percent = Convert.ToInt32((Convert.ToDouble(i) / Convert.ToDouble(Jap.Count)) * 100) ;
                    (sender as BackgroundWorker).ReportProgress(percent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("worker_DoWork：" + ex.Message);
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressbar.Value = e.ProgressPercentage;
        }

        private void RemoveMatches(List<TextModel> matches)
        {
            try
            {
                if (matches.Count > 1)
                {
                    for (int i = matches.Count - 1; i > 0; i--)
                    {
                        Jap.Remove(matches[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("RemoveMatches:" + ex.Message);
            }
        }

        private void SaveMatches(List<TextModel> matches)
        {
            RepeatModel repeat = null;
            if (matches.Count > 1)
            {
                foreach(var match in matches)
                {
                    repeat = new RepeatModel();
                    repeat.ID = globle_ID.ToString();
                    repeat.Path = match.Path;
                    repeat.Text = match.Text;
                    repeat.Address = match.Address;
                    repeat.TraslationText = Chi.Find(it => (it.Address == match.Address && it.Path.Equals(match.Path))).Text;
                    this.Dispatcher.Invoke(new Action(()=>
                    {
                        ++globle_ID;
                        repeater.Add(repeat);
                    }));
                }
            }
        }

    }
}
