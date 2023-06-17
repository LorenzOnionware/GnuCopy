using System;
using System.Management;

namespace Project1;

public class USBMonitor
{
 private ManagementEventWatcher watcher;

    public event EventHandler<USBEventArgs> USBInserted;
    public event EventHandler<USBEventArgs> USBRemoved;

    public void StartMonitoring()
    {
        WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
        WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");

        watcher = new ManagementEventWatcher();
        watcher.EventArrived += USBEventArrived;

        watcher.Query = insertQuery;
        watcher.Start();

        watcher.Query = removeQuery;
        watcher.Start();
    }

    public void StopMonitoring()
    {
        if (watcher != null)
        {
            watcher.Stop();
            watcher.Dispose();
        }
    }

    private void USBEventArrived(object sender, EventArrivedEventArgs e)
    {
        ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
        string deviceId = targetInstance["DeviceID"].ToString();
        string driveLetter = GetDriveLetter(deviceId);

        if (e.NewEvent.ClassPath.ClassName.Equals("__InstanceCreationEvent"))
        {
            USBInserted?.Invoke(this, new USBEventArgs(driveLetter));
        }
        else if (e.NewEvent.ClassPath.ClassName.Equals("__InstanceDeletionEvent"))
        {
            USBRemoved?.Invoke(this, new USBEventArgs(driveLetter));
        }
    }

    private string GetDriveLetter(string deviceId)
    {
        string driveLetter = "";

        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'"))
        {
            foreach (ManagementObject drive in searcher.Get())
            {
                string driveId = drive.GetPropertyValue("PNPDeviceID").ToString();
                if (driveId.Equals(deviceId))
                {
                    using (ManagementObject partition = new ManagementObject("ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + driveId + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition"))
                    {
                        using (ManagementObject logical = new ManagementObject("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass = Win32_LogicalDiskToPartition"))
                        {
                            driveLetter = logical["Name"].ToString();
                        }
                    }
                }
            }
        }

        return driveLetter;
    }
}

public class USBEventArgs : EventArgs
{
    public string DriveLetter { get; private set; }

    public USBEventArgs(string driveLetter)
    {
        DriveLetter = driveLetter;
    }
}