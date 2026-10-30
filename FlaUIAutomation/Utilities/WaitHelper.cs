using FlaUI.Core.AutomationElements;
using FlaUIAutomation.Drivers;
using System.Runtime.InteropServices;

namespace FlaUIAutomation.Utilities;

public static class WaitHelper
{
    public static AutomationElement WaitForElementByAutomationId(
    string automationId,
    int timeout = 60)
    {
        var endTime = DateTime.UtcNow.AddSeconds(timeout);

        while (DateTime.UtcNow < endTime)
        {
            try
            {
                var element = AutomationManager.MainWindow!
                    .FindFirstDescendant(
                        cf => cf.ByAutomationId(automationId));

                if (element != null &&
                    !element.Properties.IsOffscreen.ValueOrDefault)
                {
                    return element;
                }
            }
            catch (COMException)
            {
                // Chrome UI tree busy
            }

            Thread.Sleep(500);
        }

        throw new TimeoutException(
            $"Element '{automationId}' not found.");
    }

    public static AutomationElement WaitForElementByName( string name, int timeout = 60)
    {
        var endTime = DateTime.UtcNow.AddSeconds(timeout);
        while (DateTime.UtcNow < endTime)
        {
            try
            {
                var element = AutomationManager.MainWindow!
                    .FindFirstDescendant(
                        cf => cf.ByName(name));
                if (element != null &&
                    !element.Properties.IsOffscreen.ValueOrDefault)
                {
                    return element;
                }
            }
            catch (COMException)
            {
                // Chrome UI tree busy
            }
            Thread.Sleep(500);
        }
        throw new TimeoutException(
            $"Element '{name}' not found.");
    }
}
