using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace FlaUIAutomation.Drivers
{
    public static class WindowManager
    {
        private static readonly UIA3Automation _automation =
        new UIA3Automation();
    public static Window GetWindowOrThrow(
        string processName,
        int maxRetries = 20,
        int retryDelayMs = 500)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var processes =
                        Process.GetProcessesByName(processName);

                    foreach (var process in processes)
                    {
                        var app = Application.Attach(process);
                        var window = app.GetMainWindow(_automation);

                        if (window != null && IsReady(window))
                        {
                            window.Focus();

                            AutomationManager.Application = app;
                            AutomationManager.MainWindow = window;

                            Console.WriteLine(
                                $"Attached to main window: " +
                                $"'{window.Title}'");

                            return window;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Window attachment attempt {i + 1}: " +
                        ex.Message);
                }

                Thread.Sleep(retryDelayMs);
            }

            throw new TimeoutException(
                $"Could not attach to the main window of " +
                $"process '{processName}'.");
        }

        public static Window WaitForWindowByTitle(
            string title,
            TimeSpan? timeout = null)
        {
            var app = AutomationManager.Application
                ?? throw new InvalidOperationException(
                    "Application is not attached.");

            var waitTime = timeout ?? TimeSpan.FromSeconds(30);
            var endTime = DateTime.UtcNow + waitTime;

            while (DateTime.UtcNow < endTime)
            {
                var windows = app.GetAllTopLevelWindows(_automation);

                foreach (var window in windows)
                {
                    try
                    {
                        if (string.Equals(
                                window.Title,
                                title,
                                StringComparison.OrdinalIgnoreCase)
                            && IsReady(window))
                        {
                            window.Focus();
                            AutomationManager.MainWindow = window;

                            Console.WriteLine(
                                $"Attached to window: '{window.Title}'");

                            return window;
                        }
                    }
                    catch
                    {
                        // The window may be changing or closing.
                    }
                }

                Thread.Sleep(500);
            }

            throw new TimeoutException(
                $"Could not find window '{title}' " +
                $"within {waitTime.TotalSeconds:n0} seconds.");
        }

        public static Window WaitForNewWindow(
            Window[] previousWindows,
            TimeSpan? timeout = null)
        {
            var app = AutomationManager.Application
                ?? throw new InvalidOperationException(
                    "Application is not attached.");

            var waitTime = timeout ?? TimeSpan.FromSeconds(30);
            var endTime = DateTime.UtcNow + waitTime;

            while (DateTime.UtcNow < endTime)
            {
                var currentWindows =
                    app.GetAllTopLevelWindows(_automation);

                foreach (var window in currentWindows)
                {
                    try
                    {
                        bool existedBefore = previousWindows.Any(
                            previous =>
                                previous.FrameworkAutomationElement.Equals(
                                    window.FrameworkAutomationElement));

                        if (!existedBefore && IsReady(window))
                        {
                            window.Focus();
                            AutomationManager.MainWindow = window;

                            Console.WriteLine(
                                $"Attached to new window: " +
                                $"'{window.Title}'");

                            return window;
                        }
                    }
                    catch
                    {
                        // A window may disappear during enumeration.
                    }
                }

                Thread.Sleep(500);
            }

            throw new TimeoutException(
                $"No new ready window appeared within " +
                $"{waitTime.TotalSeconds:n0} seconds.");
        }

        public static Window[] GetTopLevelWindows()
        {
            var app = AutomationManager.Application
                ?? throw new InvalidOperationException(
                    "Application is not attached.");

            return app.GetAllTopLevelWindows(_automation);
        }

        private static bool IsReady(Window window)
        {
            try
            {
                if (!window.Properties.IsOffscreen.TryGetValue(
                        out var isOffscreen) || isOffscreen)
                {
                    return false;
                }

                if (!window.Properties.IsEnabled.TryGetValue(
                        out var isEnabled) || !isEnabled)
                {
                    return false;
                }

                if (!window.Properties.BoundingRectangle.TryGetValue(
                        out var bounds)
                    || bounds.Width <= 0
                    || bounds.Height <= 0)
                {
                    return false;
                }

                if (!window.Patterns.Window.IsSupported)
                {
                    return true;
                }

                return window.Patterns.Window.Pattern
                           .WindowInteractionState.TryGetValue(
                               out var state)
                       && state ==
                           WindowInteractionState.ReadyForUserInteraction;
            }
            catch
            {
                return false;
            }
        }
    }

}
