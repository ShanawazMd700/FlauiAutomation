using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Linq;
using System.Linq;
using System.Threading;

namespace FlaUIAutomation.Drivers;
public static class SessionManager
{
    public static void StartSession()
    {
        if (AutomationManager.IsStarted)
        {
            return;
        }

        AutomationManager.Automation = new UIA3Automation();
    }

    public static void AttachApplication(Application application, TimeSpan? timeout = null)
    {
        StartSession();

        AutomationManager.Application = application;
        AutomationManager.MainWindow = WaitForMainWindow(application, timeout ?? TimeSpan.FromSeconds(30));
    }
    public static void ReattachMainWindow(TimeSpan? timeout = null)
    {
        if (AutomationManager.Application == null)
        {
            throw new InvalidOperationException(
                "Application is not attached.");
        }

        AutomationManager.MainWindow =
            WaitForMainWindow(
                AutomationManager.Application,
                timeout ?? TimeSpan.FromSeconds(30));

       // Console.WriteLine(
            //$"Re-attached Window: " +
            //$"Title='{AutomationManager.MainWindow.Title}' | " +
            //$"AutomationId='{AutomationManager.MainWindow.AutomationId}'");
    }

    public static void StopSession()
    {
        try
        {
            AutomationManager.MainWindow = null;
            AutomationManager.Application?.Close();
        }
        finally
        {
            AutomationManager.Application?.Dispose();
            AutomationManager.Automation.Dispose();
            AutomationManager.Clear();
        }
    }

    private static Window WaitForMainWindow(
    Application application,
    TimeSpan timeout)
    {
        var endTime = DateTime.UtcNow + timeout;

        while (DateTime.UtcNow < endTime)
        {
            var windows = application.GetAllTopLevelWindows(
                AutomationManager.Automation);

            //Console.WriteLine(
            //    $"Found {windows.Length} top-level windows");

            foreach (var w in windows)
            {
                //Console.WriteLine(
                //    $"Window Title='{w.Title}'");
            }

            var smartFitWindow = windows.FirstOrDefault(
                w => !string.IsNullOrWhiteSpace(w.Title));

            if (smartFitWindow != null)
            {
                //Console.WriteLine(
                //    $"Selected Window='{smartFitWindow.Title}'");

                return smartFitWindow;
            }

            Thread.Sleep(1000);
        }

        throw new TimeoutException(
            $"Main window was not available within {timeout.TotalSeconds:n0} seconds.");
    }
    public static Window AttachToWindow(
    Func<Window, bool> condition,
    TimeSpan? timeout = null)
    {
        if (AutomationManager.Application == null)
        {
            throw new InvalidOperationException(
                "Application is not attached.");
        }

        var waitTime = timeout ?? TimeSpan.FromSeconds(30);
        var endTime = DateTime.UtcNow + waitTime;

        while (DateTime.UtcNow < endTime)
        {
            var windows =
                AutomationManager.Application.GetAllTopLevelWindows(
                    AutomationManager.Automation);

            foreach (var window in windows)
            {
                try
                {
                    if (condition(window))
                    {
                        AutomationManager.MainWindow = window;

                        //Console.WriteLine(
                        //    $"Attached to window: " +
                        //    $"Title='{window.Title}', " +
                        //    $"AutomationId='{window.AutomationId}'");

                        return window;
                    }
                }
                catch
                {
                    // Window may disappear while enumerating
                }
            }

            Thread.Sleep(500);
        }

        throw new TimeoutException(
            $"Could not find matching window within " +
            $"{waitTime.TotalSeconds:n0} seconds.");
    }

    public static Window AttachToWindowByTitle(
    string title,
    TimeSpan? timeout = null)
    {
        return AttachToWindow(
            w => string.Equals(
                w.Title,
                title,
                StringComparison.OrdinalIgnoreCase),
            timeout);
    }

    public static void SwitchToWindow(
        string windowName,
        TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(windowName))
        {
            throw new ArgumentException(
                "Window name cannot be empty.",
                nameof(windowName));
        }

        if (AutomationManager.Automation == null)
        {
            throw new InvalidOperationException(
                "UI Automation session has not started.");
        }

        var waitTime = timeout ?? TimeSpan.FromSeconds(15);
        var endTime = DateTime.UtcNow + waitTime;

        Console.WriteLine(
            $"Searching desktop for window: '{windowName}'");

        while (DateTime.UtcNow < endTime)
        {
            var desktop = AutomationManager.Automation.GetDesktop();

            // Enumerate immediate desktop children, including top-level windows.
            var windows = desktop.FindAllChildren(
                cf => cf.ByControlType(
                    FlaUI.Core.Definitions.ControlType.Window));

            foreach (var element in windows)
            {
                try
                {
                    Console.WriteLine(
                        $"Desktop window: Name='{element.Name}', " +
                        $"AutomationId='{element.AutomationId}'");

                    if (string.Equals(
                        element.Name?.Trim(),
                        windowName.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        var window = element.AsWindow();

                        AutomationManager.MainWindow = window;
                        window.Focus();

                        Console.WriteLine(
                            $"Switched to window: '{window.Title}'");

                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Could not inspect a desktop element: {ex.Message}");
                }
            }

            Thread.Sleep(500);
        }

        throw new TimeoutException(
            $"Desktop window '{windowName}' was not found " +
            $"within {waitTime.TotalSeconds} seconds.");
    }

    public static void InspectAudiogramGrid()
    {
        var window = AutomationManager.MainWindow
            ?? throw new InvalidOperationException(
                "Audiogram Module is not attached.");

        Console.WriteLine(
            $"Inspecting window: '{window.Title}'");

        var elements = window.FindAllDescendants();

        Console.WriteLine(
            $"Total descendant elements: {elements.Length}");

        foreach (var element in elements)
        {
            string name = "";
            string automationId = "";
            string controlType = "";

            try { name = element.Name ?? ""; } catch { }
            try { automationId = element.AutomationId ?? ""; } catch { }
            try { controlType = element.ControlType.ToString(); } catch { }

            if (name.Contains(
                    "TestValuesViewModel",
                    StringComparison.OrdinalIgnoreCase)
                || name.Contains("theGrid",
                    StringComparison.OrdinalIgnoreCase)
                || controlType.Contains("Grid",
                    StringComparison.OrdinalIgnoreCase)
                || controlType.Contains("DataItem",
                    StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    $"Name='{name}' | " +
                    $"AutomationId='{automationId}' | " +
                    $"ControlType='{controlType}'");
            }
        }
    }
    public static void SwitchToWindowContainingAutomationId(
    string automationId,
    TimeSpan? timeout = null)
    {
        var waitTime = timeout ?? TimeSpan.FromSeconds(15);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var desktop = AutomationManager.Automation.GetDesktop();

        while (stopwatch.Elapsed < waitTime)
        {
            var windows = desktop.FindAllChildren(
                cf => cf.ByControlType(
                    FlaUI.Core.Definitions.ControlType.Window));

            foreach (var element in windows)
            {
                try
                {
                    var match = element.FindFirstDescendant(
                        cf => cf.ByAutomationId(automationId));

                    if (match != null)
                    {
                        var window = element.AsWindow();

                        AutomationManager.MainWindow = window;
                        window.Focus();

                        Console.WriteLine(
                            $"Switched to window '{window.Title}' " +
                            $"containing AutomationId '{automationId}'.");

                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Could not inspect window: {ex.Message}");
                }
            }

            Thread.Sleep(500);
        }

        throw new TimeoutException(
            $"No desktop window containing AutomationId '{automationId}' " +
            $"was found within {waitTime.TotalSeconds} seconds.");
    }

    public static void CloseWindow(
        string windowName,
        TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(windowName))
        {
            throw new ArgumentException(
                "Window name cannot be empty.",
                nameof(windowName));
        }

        if (AutomationManager.Automation == null)
        {
            throw new InvalidOperationException(
                "UI Automation session has not started.");
        }

        var waitTime = timeout ?? TimeSpan.FromSeconds(15);
        var endTime = DateTime.UtcNow + waitTime;

        Console.WriteLine(
            $"Searching desktop to close window: '{windowName}'");

        while (DateTime.UtcNow < endTime)
        {
            var desktop = AutomationManager.Automation.GetDesktop();

            var windows = desktop.FindAllChildren(
                cf => cf.ByControlType(
                    FlaUI.Core.Definitions.ControlType.Window));

            foreach (var element in windows)
            {
                try
                {
                    if (!string.Equals(
                        element.Name?.Trim(),
                        windowName.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var window = element.AsWindow();

                    Console.WriteLine(
                        $"Closing window: '{window.Title}'");

                    window.Close();

                    Console.WriteLine(
                        $"Close command sent to: '{windowName}'");

                    // Do not change MainWindow unless it referred
                    // to the window that was just closed.
                    if (AutomationManager.MainWindow != null &&
                        string.Equals(
                            AutomationManager.MainWindow.Title?.Trim(),
                            windowName.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        AutomationManager.MainWindow = null;
                    }

                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Could not close window '{windowName}': " +
                        ex.Message);
                }
            }

            Thread.Sleep(500);
        }

        throw new TimeoutException(
            $"Window '{windowName}' was not found on the desktop " +
            $"within {waitTime.TotalSeconds} seconds.");
    }

    public static void ClickSaveMeasurementOK()
    {
        var automation = AutomationManager.Automation
            ?? throw new InvalidOperationException(
                "UI Automation session has not started.");

        var desktop = automation.GetDesktop();

        var windows = desktop.FindAllChildren(
            cf => cf.ByControlType(
                FlaUI.Core.Definitions.ControlType.Window));

        foreach (var element in windows)
        {
            var window = element.AsWindow();

            var saveLabel = window.FindFirstDescendant(
                cf => cf.ByName("Save measurement"));

            if (saveLabel == null)
                continue;

            Console.WriteLine(
                $"Found Save measurement dialog: '{window.Title}'");

            var ok = window.FindFirstDescendant(
                cf => cf.ByName("OK"));

            if (ok == null)
            {
                throw new InvalidOperationException(
                    "OK control was not found in Save measurement dialog.");
            }

            Console.WriteLine(
                $"OK control: Type='{ok.ControlType}', " +
                $"Bounds='{ok.BoundingRectangle}'");

            // Click the visible OK label within the button.
            ok.Click();

            Console.WriteLine("Clicked OK on Save measurement dialog.");
            return;
        }

        throw new InvalidOperationException(
            "Save measurement dialog was not found.");
    }

}