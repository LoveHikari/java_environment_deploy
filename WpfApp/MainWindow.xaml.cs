using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fs.common;
using Masuit.Tools.Net;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var str = CmdHelper.RunCmd("javac --version");
            Regex reg = new Regex("javac (.+)$");
            var match = reg.Match(str);
            var ver = match.Groups[1].Value; // 版本号
            this.tbVer.Text = ver;

            str = CmdHelper.RunCmd("java -verbose");
            reg = new Regex("opened: (.+)\\\\lib\\\\modules");
            match = reg.Match(str);
            var path = match.Groups[1].Value; // 路径
            this.tbPath.Text = path;
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbPath.Text))
            {
                System.Windows.MessageBox.Show(this, "目录不能为空", "提示");
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", this.tbPath.Text);
        }

        private void btnSelFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择JDK所在文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    System.Windows.MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }

                this.tbPath.Text = dialog.SelectedPath;
            }
        }

        private void btnSetEnv_Click(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable("JAVA_HOME", this.tbPath.Text, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable("CLASSPATH", @".;%JAVA_HOME%\lib;", EnvironmentVariableTarget.Machine);
            string sPath = Environment.GetEnvironmentVariable("Path"); //获取环境变量
            Environment.SetEnvironmentVariable("JAVA_HOME", sPath + @"%JAVA_HOME%\bin;", EnvironmentVariableTarget.Machine);
            System.Windows.MessageBox.Show(this, "环境变量添加成功", "提示");
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            var mtd = new MultiThreadDownloader("https://download.oracle.com/otn-pub/java/jdk/15+36/779bf45e88a44cbd9ea6621d33e33db1/jdk-15_windows-x64_bin.zip", Environment.GetEnvironmentVariable("temp"), "jdk-15_windows-x64_bin.zip", 8);
            mtd.TotalProgressChanged += (sender, e) =>
            {
                var downloader = sender as MultiThreadDownloader;
                Console.WriteLine("下载进度：" + downloader.TotalProgress + "%");
                Console.WriteLine("下载速度：" + downloader.TotalSpeedInBytes / 1024 / 1024 + "MBps");
            };
            mtd.FileMergeProgressChanged += (sender, e) =>
            {
                Console.WriteLine("下载完成");
            };
            mtd.Start();//开始下载
            //mtd.Pause(); // 暂停下载
            //mtd.Resume(); // 继续下载
        }
    }
}
