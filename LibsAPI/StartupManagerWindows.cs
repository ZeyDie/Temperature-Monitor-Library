// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) LibreHardwareMonitor and Contributors.
// Partial Copyright (C) Michael Möller <mmoeller@openhardwaremonitor.org> and Contributors.
// All Rights Reserved.

using System.Security;
using System.Security.Principal;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;

using Action = Microsoft.Win32.TaskScheduler.Action;
using Task = Microsoft.Win32.TaskScheduler.Task;

namespace TemperatureLibrary.LibsAPI;

public class StartupManagerWindows
{
    private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private bool _startup;

    private static string executablePath = Path.GetFullPath(Programm.location).Replace(".dll", ".exe");

    public StartupManagerWindows()
    {
        if (Environment.OSVersion.Platform >= PlatformID.Unix)
        {
            IsAvailable = false;
            return;
        }

        if (IsAdministrator() && TaskService.Instance.Connected)
        {
            IsAvailable = true;

            Task task = GetTask();
            if (task != null)
            {
                foreach (Action action in task.Definition.Actions)
                {
                    if (action.ActionType == TaskActionType.Execute && action is ExecAction execAction)
                    {
                        if (execAction.Path.Equals(executablePath, StringComparison.OrdinalIgnoreCase))
                            _startup = true;
                    }
                }
            }
        }
        else
        {
            try
            {
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    string value = (string)registryKey?.GetValue(Programm.name);

                    if (value != null)
                        _startup = value == executablePath;
                }

                IsAvailable = true;
            }
            catch (SecurityException)
            {
                IsAvailable = false;
            }
        }
    }

    public bool IsAvailable { get; }

    public bool Startup
    {
        get { return _startup; }
        set
        {
            if (_startup != value)
            {
                if (IsAvailable)
                {
                    if (TaskService.Instance.Connected)
                    {
                        if (value)
                            CreateTask();
                        else
                            DeleteTask();

                        _startup = value;
                    }
                    else
                    {
                        try
                        {
                            if (value)
                                CreateRegistryKey();
                            else
                                DeleteRegistryKey();

                            _startup = value;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            throw new InvalidOperationException();
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

    private static bool IsAdministrator()
    {
        try
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    private static Task GetTask()
    {
        try
        {
            return TaskService.Instance.AllTasks.FirstOrDefault(x => x.Name.Equals(Programm.name, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return null;
        }
    }

    private void CreateTask()
    {
        TaskDefinition taskDefinition = TaskService.Instance.NewTask();
        taskDefinition.RegistrationInfo.Description = "Starts TemperatureLibrary on Windows startup.";

        BootTrigger bootTrigger = new BootTrigger();
        bootTrigger.Delay = TimeSpan.FromMinutes(1);

        taskDefinition.Triggers.Add(bootTrigger);


        taskDefinition.Settings.StartWhenAvailable = true;
        taskDefinition.Settings.DisallowStartIfOnBatteries = false;
        taskDefinition.Settings.StopIfGoingOnBatteries = false;
        taskDefinition.Settings.ExecutionTimeLimit = TimeSpan.Zero;
        taskDefinition.Settings.AllowHardTerminate = false;

        taskDefinition.Principal.UserId = "SYSTEM";
        taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
        taskDefinition.Principal.LogonType = TaskLogonType.ServiceAccount;

        taskDefinition.Actions.Add(new ExecAction(executablePath, "", Path.GetDirectoryName(executablePath)));

        TaskService.Instance.RootFolder.RegisterTaskDefinition(Programm.name, taskDefinition);
    }

    private static void DeleteTask()
    {
        Task task = GetTask();
        task?.Folder.DeleteTask(task.Name, false);
    }

    private static void CreateRegistryKey()
    {
        RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(RegistryPath);
        registryKey?.SetValue(Programm.name, executablePath);
    }

    private static void DeleteRegistryKey()
    {
        RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(RegistryPath);
        registryKey?.DeleteValue(Programm.name);
    }
}