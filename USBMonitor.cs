using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.Versioning;

namespace Project1;

public class USBMonitor : IDisposable
{
    private readonly List<ManagementEventWatcher> _watchers = new();

    public event EventHandler<USBEventArgs>? USBInserted;
    public event EventHandler<USBEventArgs>? USBRemoved;

    public void StartMonitoring()
    {
        // System.Management/WMI is only available in Windows desktop applications.
        if (!OperatingSystem.IsWindows())
            return;

        StopMonitoring();
        try
        {
            StartWatcher(
                "SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'",
                USBInserted);
            StartWatcher(
                "SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'",
                USBRemoved);
        }
        catch (Exception ex) when (ex is ManagementException or PlatformNotSupportedException or UnauthorizedAccessException or InvalidOperationException)
        {
            Debug.WriteLine($"USB monitoring could not be started: {ex}");
            StopMonitoring();
        }
    }

    private void StartWatcher(string query, EventHandler<USBEventArgs>? handler)
    {
        var watcher = new ManagementEventWatcher(new WqlEventQuery(query));
        watcher.EventArrived += (_, args) =>
        {
            try
            {
                var target = args.NewEvent["TargetInstance"] as ManagementBaseObject;
                var deviceId = target?["DeviceID"]?.ToString();
                if (string.IsNullOrEmpty(deviceId))
                    return;

                handler?.Invoke(this, new USBEventArgs(GetDriveLetter(deviceId)));
            }
            catch (Exception ex) when (ex is ManagementException or InvalidOperationException or UnauthorizedAccessException)
            {
                Debug.WriteLine($"USB event could not be processed: {ex}");
            }
        };

        watcher.Start();
        _watchers.Add(watcher);
    }

    public void StopMonitoring()
    {
        foreach (var watcher in _watchers)
        {
            try
            {
                watcher.Stop();
            }
            catch (ManagementException ex)
            {
                Debug.WriteLine($"USB watcher could not be stopped cleanly: {ex}");
            }
            finally
            {
                watcher.Dispose();
            }
        }

        _watchers.Clear();
    }

    public void Dispose() => StopMonitoring();

    [SupportedOSPlatform("windows")]
    private static string GetDriveLetter(string deviceId)
    {
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
        foreach (ManagementObject drive in searcher.Get())
        {
            using (drive)
            {
                var driveId = drive["PNPDeviceID"]?.ToString();
                if (!string.Equals(driveId, deviceId, StringComparison.OrdinalIgnoreCase))
                    continue;

                var escapedId = deviceId.Replace("\\", "\\\\").Replace("'", "\\'");
                using var partition = new ManagementObject($"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{escapedId}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
                partition.Get();
                var partitionId = partition["DeviceID"]?.ToString();
                if (string.IsNullOrEmpty(partitionId))
                    continue;

                using var logical = new ManagementObject($"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partitionId}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");
                logical.Get();
                return logical["Name"]?.ToString() ?? string.Empty;
            }
        }

        return string.Empty;
    }
}

public class USBEventArgs : EventArgs
{
    public string DriveLetter { get; }

    public USBEventArgs(string driveLetter) => DriveLetter = driveLetter;
}
