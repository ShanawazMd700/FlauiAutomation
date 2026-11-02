using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUIAutomation.Drivers;
using System.Runtime.InteropServices;
using System.Threading;

namespace FlaUIAutomation.Utilities;

public static class WaitHelper
{
    public static AutomationElement WaitForElementByAutomationId(string automationId, int timeout = 60)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(automationId);
        var automation = AutomationManager.Automation;
        var endTime = DateTime.UtcNow.AddSeconds(timeout);

        while (DateTime.UtcNow < endTime)
        {
            foreach (var window in automation.GetDesktop().FindAllChildren(
                         cf => cf.ByControlType(ControlType.Window)))
            {
                try
                {
                    var element = window.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
                    if (element != null && !element.Properties.IsOffscreen.ValueOrDefault)
                        return element;
                }
                catch (COMException)
                {
                    // A window can close while the desktop tree is being inspected.
                }
            }
            Thread.Sleep(500);
        }

        throw new TimeoutException($"Element '{automationId}' was not found in any top-level window within {timeout} seconds.");
    }

    public static AutomationElement WaitForElementByName(string name, int timeout = 60)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var automation = AutomationManager.Automation;
        var endTime = DateTime.UtcNow.AddSeconds(timeout);

        while (DateTime.UtcNow < endTime)
        {
            foreach (var window in automation.GetDesktop().FindAllChildren(
                         cf => cf.ByControlType(ControlType.Window)))
            {
                try
                {
                    var element = window.FindFirstDescendant(cf => cf.ByName(name));
                    if (element != null && !element.Properties.IsOffscreen.ValueOrDefault)
                        return element;
                }
                catch (COMException)
                {
                    // A window can close while the desktop tree is being inspected.
                }
            }
            Thread.Sleep(500);
        }

        throw new TimeoutException($"Element with name '{name}' was not found in any top-level window within {timeout} seconds.");
    }
}
