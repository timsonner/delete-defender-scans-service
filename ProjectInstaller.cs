using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

[RunInstaller(true)]
public class DeleteScansServiceInstaller : Installer
{
    public DeleteScansServiceInstaller()
    {
        ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
        ServiceInstaller serviceInstaller = new ServiceInstaller();

        // Set the service account type
        processInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;

        // Set the service properties
        serviceInstaller.StartType = ServiceStartMode.Automatic;
        serviceInstaller.ServiceName = "DeleteScansService";

        // Add the installers to the collection
        Installers.Add(processInstaller);
        Installers.Add(serviceInstaller);
    }
}
