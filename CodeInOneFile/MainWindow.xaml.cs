using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace CodeInOneFile
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();
            this.TBPath.Text = m_Dir;

        }

        bool isTaskRunning = false;

        void Director(string dir)
        {
            var p = dir;
            Task.Factory.StartNew(() =>
            {
                isTaskRunning = true;
                DirectoryInfo d = new DirectoryInfo(dir);
                FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
                foreach (FileSystemInfo fsinfo in fsinfos)
                {
                    if (fsinfo is DirectoryInfo)     //判断是否为文件夹  
                    {
                        Director(fsinfo.FullName);//递归调用  
                    }
                    else
                    {
                        if(fsinfo.Extension == ".cs")
                        { 
                            _ShowText(fsinfo.FullName);//输出文件的全部路径  
                            CopyFileToDst(fsinfo.FullName, p + "CodeIn.txt");
                        }
                    }
                }
                isTaskRunning = false;

            });
        }

        void CopyFileToDst(string srcPath, string dstPath)
        { 
            if(File.Exists(dstPath) == false)
            {
                FileStream fs1 = new FileStream(dstPath, FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine("软件著作权代码!-hu.");//开始写入值
                sw.Close();
                fs1.Close();
            }
            try
            { 
                StreamReader sr = new StreamReader(srcPath, Encoding.Default);
                var text = sr.ReadToEnd();  
                StreamWriter sw = new StreamWriter(dstPath,true);
                sw.Write(text);
                sw.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        void _ShowText(string s)
        {
            Dispatcher.Invoke((Action)delegate() 
            {

                TBShow.Text += s;
                TBShow.Text += "\n";
                TBShow.ScrollToEnd();
            });
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if(isTaskRunning == false)
            {

                Director(TBPath.Text);
            }
            else
            {
                System.Windows.MessageBox.Show("等待上一次操作完成");
            }
        }
    }
}
