using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;

public partial class DeleteScansService : ServiceBase
{
    private EventLog eventLog;

    public DeleteScansService()
    {
        // References manually defined method
        InitializeComponent(); 
        
        eventLog = new EventLog();

        if (!EventLog.SourceExists("DeleteScansService"))
        {
            EventLog.CreateEventSource("DeleteScansService", "Application");
        }

        eventLog.Source = "DeleteScansService";
        eventLog.Log = "Application";
    }

    private void InitializeComponent()
    {
        // Initialize service's components
        this.ServiceName = "DeleteScansService";
        this.CanStop = true;
        this.CanPauseAndContinue = false;
        this.AutoLog = true;
    }

    protected override void OnStart(string[] args)
    {
        // Start the service
        try
        {
            string scansPath = @"C:\ProgramData\Microsoft\Windows Defender\Scans";
            DeleteFiles(scansPath);
        }
        catch (Exception ex)
        {
            eventLog.WriteEntry("Error in DeleteScansService: " + ex.Message, EventLogEntryType.Error);
        }
    }

    private void DeleteFiles(string path)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (var file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    eventLog.WriteEntry("Deleted file: " + file.FullName, EventLogEntryType.Information);
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry("Failed to delete file: " + file.FullName + " - " + ex.Message, EventLogEntryType.Error);
                }
            }
            foreach (var dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    eventLog.WriteEntry("Deleted directory: " + dir.FullName, EventLogEntryType.Information);
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry("Failed to delete directory: " + dir.FullName + " - " + ex.Message, EventLogEntryType.Error);
                }
            }
        }
    }

    protected override void OnStop()
    {
        // Clean up resources if needed
    }
}
