using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUIAutomation.Drivers;
using FlaUIAutomation.Utilities;
using System.Threading;

namespace FlaUIAutomation.Pages;

public class HomePage : ControlHelpers
{
    private const string PatientDatabaseButtonAutomationId = "fittingpath_button_patient-database";
    private const string NewpatientButtonname = "patientlist_link_import";
    private const string Firstnamefield = "patientprofile_input-text_first-name";
    private const string Lastnamefield = "patientprofile_input-text_last-name";
    private const string audiogramTab = "patientdetailslayout_link_audiogram";
    private const string copytoright = "patientaudiogram_button_copy-right";
    private const string newsessionbutton = "savepatientconfirmationdialog_button_fit";
    private const string savebutton = "patientdetailslayout_button_save";
    private const string simulatesession = "fittingpath_link_simulate";
    private const string simulatebutton = "instruments_button_simulate";
    private const string pysicalproperties_continue = "StateMachineAutomationIds.ContinueAction";
    private const string viewbutton_patientlist = "patientlistitem_button_view";

    public void ClickPatientDatabase()
    {
        var button = SessionManager.SwitchToWindowContainingAutomationId(
            PatientDatabaseButtonAutomationId, TimeSpan.FromSeconds(60));
        ClickElement(button);
    }

    public void AddNewPatient(string firstName, string lastName)
    {
        var newPatientButton = SessionManager.SwitchToWindowContainingAutomationId(NewpatientButtonname);
        ClickElement(newPatientButton);
        EnterTextByAutomationId(Firstnamefield, firstName);
        Thread.Sleep(1000);
        EnterTextByAutomationId(Lastnamefield, lastName);
        EnterAudiogramValues();
        Thread.Sleep(2000);
        ClickByAutomationId(copytoright);
        ClickByAutomationId(savebutton);
        Thread.Sleep(2000);
        ClickByAutomationId(newsessionbutton);
        Thread.Sleep(2000);
        var simulateLink = SessionManager.SwitchToWindowContainingAutomationId(simulatesession);
        Thread.Sleep(2000);
        ClickElement(simulateLink);
    }

    private void EnterAudiogramValues()
    {
        ClickByAutomationId(audiogramTab);
        ClickByAutomationId("audiogramgrid_input-text_ac-125");
        Thread.Sleep(1000);
        ClickByAutomationId("audiogramgrid_input-text_ac-125");

        string[] values = { "40", "40", "40", "40", "40", "40", "40", "40", "40", "40", "40" };
        foreach (var value in values)
        {
            Keyboard.Type(value);
            Keyboard.Press(VirtualKeyShort.TAB);
            Thread.Sleep(1000);
        }
    }

    private void SelectDevice(string deviceName, string side)
    {
        var normalizedDevice = deviceName.Trim().ToLowerInvariant();
        var normalizedSide = side.Trim().ToLowerInvariant();
        if (normalizedSide == "both")
        {
            ClickByAutomationId($"catalogproduct_radio-button_right-{normalizedDevice}");
            ClickByAutomationId($"catalogproduct_radio-button_left-{normalizedDevice}");
            ClickByAutomationId(simulatebutton);
            return;
        }

        if (normalizedSide is not ("right" or "left"))
            throw new ArgumentException("Side must be Right, Left, or Both.", nameof(side));

        ClickByAutomationId($"catalogproduct_radio-button_{normalizedSide}-{normalizedDevice}");
        ClickByAutomationId(simulatebutton);
    }

    public void SelectDeviceWithFullname(string deviceName, string side)
    {
        SessionManager.SwitchToWindowContainingAutomationId("filters_button_filter");
        var familyName = GetDeviceFamily(deviceName);
        FindDeviceWithScroll(deviceName, familyName, side);
        SelectDevice(deviceName, side);
    }

    private void FindDeviceWithScroll(string deviceName, string familyName, string side, int maxScrollAttempts = 20)
    {
        var normalizedDevice = deviceName.Trim().ToLowerInvariant();
        var normalizedSide = side.Trim().ToLowerInvariant();
        if (normalizedSide is not ("right" or "left" or "both"))
            throw new ArgumentException("Side must be Right, Left, or Both.", nameof(side));

        var familyElement = FindNameContainsAcrossWindows(familyName);
        ClickElement(familyElement);
        Thread.Sleep(1500);

        var sides = normalizedSide == "both" ? new[] { "right", "left" } : new[] { normalizedSide };
        var automationIds = sides.Select(s => $"catalogproduct_radio-button_{s}-{normalizedDevice}").ToArray();
        if (automationIds.All(id => FindByAutomationIdInCurrentWindow(id) != null)) return;

        MoveMouseToElement(familyElement);
        for (var attempt = 1; attempt <= maxScrollAttempts; attempt++)
        {
            Mouse.Scroll(-3);
            Thread.Sleep(700);
            if (automationIds.All(id => FindByAutomationIdInCurrentWindow(id) != null)) return;
        }

        throw new TimeoutException($"Device '{deviceName}' was not found in '{familyName}' after {maxScrollAttempts} scroll attempts.");
    }

    private AutomationElement FindNameContainsAcrossWindows(string name, int timeoutSeconds = 10)
    {
        var automation = AutomationManager.Automation;
        var endTime = DateTime.UtcNow.AddSeconds(timeoutSeconds);

        while (DateTime.UtcNow < endTime)
        {
            foreach (var window in automation.GetDesktop().FindAllChildren(
                         cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window)))
            {
                try
                {
                    var element = window.FindAllDescendants()
                        .FirstOrDefault(e =>
                            !string.IsNullOrEmpty(e.Name) &&
                            e.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

                    if (element != null)
                        return element;
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    // A window can close while inspecting the desktop tree. Add exception handling to ignore such cases and continue searching other windows.
                }
            }

            Thread.Sleep(250);
        }

        throw new TimeoutException(
            $"Element containing name '{name}' was not found within {timeoutSeconds} seconds.");
    }

    private static string GetDeviceFamily(string deviceName)
    {
        if (deviceName.StartsWith("SN", StringComparison.OrdinalIgnoreCase)) return "ReSound Sensia";
        if (deviceName.StartsWith("VI", StringComparison.OrdinalIgnoreCase)) return "ReSound Vivia";
        if (deviceName.StartsWith("NX", StringComparison.OrdinalIgnoreCase)) return "ReSound Nexia";
        if (deviceName.StartsWith("EI", StringComparison.OrdinalIgnoreCase)) return "ReSound Enzo IA";
        if (deviceName.StartsWith("SA", StringComparison.OrdinalIgnoreCase)) return "ReSound Savi";
        if (deviceName.StartsWith("RT", StringComparison.OrdinalIgnoreCase)) return "ReSound One";
        if (deviceName.StartsWith("RU", StringComparison.OrdinalIgnoreCase)) return "ReSound Omnia";
        if (deviceName.StartsWith("RE", StringComparison.OrdinalIgnoreCase)) return "ReSound Linx Quattro";
        throw new ArgumentException($"Unknown device model: {deviceName}", nameof(deviceName));
    }

    public void NavigateToFitScreen()
    {
        Thread.Sleep(3000);
        SessionManager.SwitchToWindowContainingAutomationId(pysicalproperties_continue);
        ClickByAutomationId(pysicalproperties_continue);
        Thread.Sleep(3000);
        ClickByAutomationId(pysicalproperties_continue);
    }

    public void SelectExistingPatient(string firstName)
    {
        SessionManager.SwitchToWindowContainingAutomationId(NewpatientButtonname);
        var element = FindByName(firstName);
        MoveMouseToElement(element);
        ClickByAutomationId(viewbutton_patientlist);
    }
}
