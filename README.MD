# Creating Service to Delete Defender Scans folder using Visual Studio  

C# implementation of a Windows service that deletes the Windows Defender scans folder at C:\ProgramData\Microsoft\Windows Defender\Scans. This service will log its actions to the Windows Event Log.

The reason for this is the Scans folder is locked down and I wanted to try deleting it with NT Authority System privileges. 

The service installs and starts correctly, the event log errors indicate some files in the directory are in use and can't be removed.

Scheduling the service as a sheduled task at reboot clears the logs.

## Project Creation  

### Step 1:  

Create a new C# Windows Service  

Open Visual Studio and create a new project.  

Choose Windows Service (.NET Framework) from the project templates.  

Name project (e.g., DeleteScansService).  

### Step 2:  

Implement the Service Code  

Replace the auto-generated Service1.cs code with the DeleteScanService.cs code  

### Step 3:  

Configure the Service Installer  

Right-click on your service project in Solution Explorer and select Add > New Item.  

Choose Installer Class and name it ProjectInstaller.cs.  

Open the ProjectInstaller.cs and add the following code  

Replace the auto-generated ProjectInstaller.cs code with the ProjectInstaller.cs code.  

Remove the Designer files  

### Step 4:  

Build project  

Project structure  
```  
YourProject
│
├── DeleteScansService.cs          // Main service implementation
├── ProjectInstaller.cs  // Installer implementation
└── (Other files, e.g., Program.cs) // If present, for service startup
```  

Install the service (admin command prompt cmd.exe)  
```  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe "C:\Users\<username>\source\repos\DeleteScans\bin\Debug\DeleteScans.exe"
```  

If errors installing, perhaps delete previous service  
```  
sc stop DeleteScansService
sc start DeleteScansService
```  

Start the service  
```  
sc start DeleteScansService
```  

Attempt to make Defender depend on our service. (This fails due to permissions)    
```  
sc config WinDefend depend= "DeleteScansService"  
```  
Check if service is running as NT Authority/System (cmd.exe)  
```  
sc qc DeleteScansService
```  

Check if service is running as NT Authority/System (PowerShell)  
```powershell    
Get-WmiObject Win32_Service | Where-Object { $_.Name -eq 'DeleteScansService' } | Select-Object Name, StartName
```  

Run as scheduled task...  
```powershell    
$action = New-ScheduledTaskAction -Execute "C:\Users\<username>\source\repos\DeleteScans\bin\Debug\DeleteScans.exe"
$trigger = New-ScheduledTaskTrigger -AtStartup
$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -StartWhenAvailable

Register-ScheduledTask -Action $action -Trigger $trigger -Principal $principal -Settings $settings -TaskName "DeleteDefenderScansStartup" -Description "Removes Defender Scans at Startup"
```  

Delete scheduled task  
```powershell    
schtasks /delete /tn "DeleteDefenderScansStartup" /f
```  



