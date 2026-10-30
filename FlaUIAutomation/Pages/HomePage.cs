using FlaUI.Core.WindowsAPI;
using FlaUIAutomation.Drivers;
using FlaUIAutomation.Utilities;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using FlaUI.Core.Input;

namespace FlaUIAutomation.Pages;

public class HomePage : ControlHelpers
{
    private const string PatientDatabaseButtonAutomationId = "fittingpath_button_patient-database"; // AutomationId    fittingpath_button_patient-database
    private const string NewpatientButtonname = "patientlist_link_import"; // AutomationId    patientlist_link_import
    private const string Firstnamefield = "patientprofile_input-text_first-name"; // AutomationId    patientprofile_input-text_first-name
    private const string Lastnamefield = "patientprofile_input-text_last-name"; //AutomationId	patientprofile_input-text_last-name

    private const string audiogramTab = "patientdetailslayout_link_audiogram"; // AutomationId    patientdetailslayout_link_audiogram
    private const string audiogramFieldValue ="audiogramgrid_input-text_ac-125"; //AutomationId audiogramgrid_input-text_ac - 125
    private const string copytoright = "patientaudiogram_button_copy-right"; // AutomationId patientaudiogram_button_copy-right
    private const string newsessionbutton = "savepatientconfirmationdialog_button_fit"; // AutomationId    savepatientconfirmationdialog_button_fit
    private const string savebutton = "patientdetailslayout_button_save"; // AutomationId    patientdetailslayout_button_save
    private const string simulatesession = "fittingpath_link_simulate"; // AutomationId	fittingpath_link_simulate
    private const string simulatebutton = "instruments_button_simulate"; //AutomationId	instruments_button_simulate
    private const string pysicalproperties_continue = "StateMachineAutomationIds.ContinueAction"; //AutomationId	StateMachineAutomationIds.ContinueAction
    private const string viewbutton_patientlist = "patientlistitem_button_view"; // AutomationId	patientlistitem_button_view









    public void ClickPatientDatabase()
    {
        var allElements = AutomationManager.MainWindow!
            .FindAllDescendants();

        foreach (var e in allElements)
        {
            try
            {
                var automationId =
                    e.Properties.AutomationId.ValueOrDefault;

                if (!string.IsNullOrEmpty(automationId))
                {
                    Console.WriteLine(
                        $"Name='{e.Name}' | AutomationId='{automationId}'");
                }
            }
            catch
            {
                // Ignore Chrome elements that don't support AutomationId
            }
        }

        ClickByAutomationId(PatientDatabaseButtonAutomationId);
    }


    public void AddNewpatient(string firstName, string lastName)
    {
        SessionManager.SwitchToWindowContainingAutomationId(
            NewpatientButtonname);

        ClickByAutomationId(NewpatientButtonname);

        EnterTextByAutomationId(Firstnamefield, firstName);
        Thread.Sleep(1000);
        EnterTextByAutomationId(Lastnamefield, lastName);

        enterAudiogramValues();
        Thread.Sleep(2000);
        ClickByAutomationId(copytoright);
        ClickByAutomationId(savebutton);
        Thread.Sleep(2000);
        ClickByAutomationId(newsessionbutton);
        Thread.Sleep(2000);
        SessionManager.SwitchToWindowContainingAutomationId(simulatesession);
        Thread.Sleep(2000);
        ClickByAutomationId(simulatesession);
        //selectDevice();
    }

    private void enterAudiogramValues()
    {
        ClickByAutomationId(audiogramTab);

        // Focus the first AC cell (125 Hz).
        ClickByAutomationId("audiogramgrid_input-text_ac-125");
        Thread.Sleep(1000);
        ClickByAutomationId("audiogramgrid_input-text_ac-125");

        string[] values =
        {
        "40", "40", "40", "40", "40", "40",
        "40", "40", "40", "40", "40"
    };

        foreach (string value in values)
        {
            Keyboard.Type(value);
            Keyboard.Press(VirtualKeyShort.TAB);
            Thread.Sleep(1000);
        }
    }
    private void printTree() //string deviceName
    {
        var allElements = AutomationManager.MainWindow!
            .FindAllDescendants();
        foreach (var e in allElements)
        {
            try
            {
                var automationId =
                    e.Properties.AutomationId.ValueOrDefault;
                if (!string.IsNullOrEmpty(automationId))
                {
                    Console.WriteLine(
                        $"Name='{e.Name}' | AutomationId='{automationId}'");
                }
            }
            catch
            {
                // Ignore Chrome elements that don't support AutomationId
            }
        }
        //ClickByAutomationId(deviceName);
    }
    private void SelectDevice(string deviceName, string side)
    {
        string normalizedDevice = deviceName
       .Trim()
       .ToLowerInvariant();

        string normalizedSide = side
            .Trim()
            .ToLowerInvariant();

        if (normalizedSide == "both")
        {
            FindByName("Both").Click();
            return;
        }

        if (normalizedSide != "right" && normalizedSide != "left")
        {
            throw new ArgumentException(
                "Side must be Right, Left, or Both.",
                nameof(side));
        }

        string automationId =
            $"catalogproduct_radio-button_{normalizedSide}-{normalizedDevice}";

        Console.WriteLine(
            $"Selecting device: {deviceName}, Side: {side}");

        ClickByAutomationId(automationId);
        ClickByAutomationId(simulatebutton);
    }
    
    public void SelectDeviceWithFullname(string deviceName, string side)
    {
        if (deviceName.Contains("sn") || deviceName.Contains("SN"))
        {
            FindByName("ReSound Sensia").Click();
            SelectDevice(deviceName, side);
        }
    }
    public void NavigateToFitScreeen()
    {
        Thread.Sleep(3000);
        SessionManager.SwitchToWindowContainingAutomationId(pysicalproperties_continue);
        ClickByAutomationId(pysicalproperties_continue);
        Thread.Sleep(3000);
        ClickByAutomationId(pysicalproperties_continue);
    }
    public void SelectExixtingPatient(string firstName)
    {
        SessionManager.SwitchToWindowContainingAutomationId(NewpatientButtonname);
        var element = FindByName(firstName);
        MoveMouseToElement(element);
        ClickByAutomationId(viewbutton_patientlist);
    }
}