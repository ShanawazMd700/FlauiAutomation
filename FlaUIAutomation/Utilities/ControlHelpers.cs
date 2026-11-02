using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Conditions;
using FlaUIAutomation.Drivers;
using System.Threading;

namespace FlaUIAutomation.Utilities;

public class ControlHelpers
{
    protected AutomationElement FindByAutomationId(string automationId, int timeout = 90) =>
        WaitHelper.WaitForElementByAutomationId(automationId, timeout);

    protected AutomationElement? FindByAutomationIdInCurrentWindow(string automationId)
    {
        var window = AutomationManager.MainWindow
            ?? throw new InvalidOperationException("Main window is not attached.");
        return window.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
    }

    protected AutomationElement FindByName(string name, int timeout = 90) =>
        WaitHelper.WaitForElementByName(name, timeout);

    protected void ClickByAutomationId(string automationId) =>
        FindByAutomationId(automationId).Click();

    protected void EnterTextByAutomationId(string automationId, string text)
    {
        var element = FindByAutomationId(automationId);
        element.Focus();
        element.AsTextBox().Enter(text);
    }

    protected void EnterTextByName(string name, string text)
    {
        var element = FindByName(name);
        element.Focus();
        element.AsTextBox().Enter(text);
    }

    protected string GetTextByAutomationId(string automationId)
    {
        var element = FindByAutomationId(automationId);
        return element.AsLabel()?.Text ?? element.Name;
    }

    protected bool IsDisplayedByAutomationId(string automationId) =>
        !FindByAutomationId(automationId).Properties.IsOffscreen.ValueOrDefault;

    public static void ClickElement(
        AutomationElement element,
        bool useCoordinateFallback = true)
    {
        ArgumentNullException.ThrowIfNull(element);

        try
        {
            element.Click();
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Direct click failed: {ex.Message}");
        }

        try
        {
            var parent = element.Parent;
            if (parent != null &&
                (parent.ControlType == FlaUI.Core.Definitions.ControlType.Button ||
                 parent.ControlType == FlaUI.Core.Definitions.ControlType.Custom))
            {
                parent.Click();
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Parent click failed: {ex.Message}");
        }

        if (!useCoordinateFallback)
            throw new InvalidOperationException($"Could not click element '{element.Name}'.");

        var bounds = element.BoundingRectangle;
        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
            throw new InvalidOperationException("Cannot click element: it has no usable screen bounds.");

        var x = (int)(bounds.Left + bounds.Width / 2);
        var y = (int)(bounds.Top + bounds.Height / 2);
        Mouse.Click(new System.Drawing.Point(x, y));
        Thread.Sleep(200);
    }

    public static void ClickByName(AutomationElement root, string name, bool useCoordinateFallback = true)
    {
        var element = root.FindFirstDescendant(cf => cf.ByName(name))
            ?? throw new InvalidOperationException($"Element with name '{name}' was not found.");
        ClickElement(element, useCoordinateFallback);
    }

    public AutomationElement FindByNameContains(string name, int timeout = 10)
    {
        var window = AutomationManager.MainWindow
            ?? throw new InvalidOperationException("Main window is not attached.");

        var endTime = DateTime.UtcNow.AddSeconds(timeout);

        while (DateTime.UtcNow < endTime)
        {
            var element = window.FindAllDescendants()
                .FirstOrDefault(e =>
                    !string.IsNullOrEmpty(e.Name) &&
                    e.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (element != null)
                return element;

            Thread.Sleep(250);
        }

        throw new TimeoutException(
            $"Element containing name '{name}' was not found within {timeout} seconds.");
    }

    public static void MoveMouseToElement(AutomationElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        var bounds = element.BoundingRectangle;
        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
            throw new InvalidOperationException($"Cannot move mouse: element '{element.Name}' has no usable screen bounds.");
        var x = (int)(bounds.Left + bounds.Width / 2);
        var y = (int)(bounds.Top + bounds.Height / 2);
        Mouse.MoveTo(new System.Drawing.Point(x, y));
    }
}
