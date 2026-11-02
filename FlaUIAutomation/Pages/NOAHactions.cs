using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUIAutomation.Drivers;
using FlaUIAutomation.Utilities;
using System.Threading;

namespace FlaUIAutomation.Pages;

public class NOAHactions : ControlHelpers
{
    private const string OKButton = "cmdOK";
    private const string passwordfield = "Password";
    private const string lastnamefield = "LastName";
    private const string firstnamefield = "FirstName";
    private const string audiogramcell = "Item: Himsa.Noah.CommonControls.TestValuesViewModel, Column Display Index: 0";

    public void EnterPassword(string password) => EnterTextByAutomationId(passwordfield, password);

    public void ClickOKButton()
    {
        ClickByAutomationId(OKButton);
        Thread.Sleep(2000);
    }

    public void AddPatient(string fname, string lname)
    {
        SessionManager.ReattachMainWindow();
        ClickByAutomationId("cmdAddPatient");
        SessionManager.ReattachMainWindow();
        EnterTextByAutomationId(lastnamefield, lname);
        EnterTextByAutomationId(firstnamefield, fname);
        FindByName("Male").Click();
        FindByName("OK").Click();
        SessionManager.ReattachMainWindow();
        Thread.Sleep(4000);
        FindByName("_File").Click();
        Thread.Sleep(1000);
        FindByName("Open").Click();
        Thread.Sleep(1000);
        FindByName("Noah 4 Audiogram Module").Click();
        Thread.Sleep(2000);
        SessionManager.SwitchToWindow("Audiogram Module");
        Thread.Sleep(1500);

        EnterAudiogramValues();
        SessionManager.CloseWindow("Audiogram Module");
        Thread.Sleep(3000);
        SessionManager.ClickSaveMeasurementOK();
        Thread.Sleep(6000);
    }

    private void EnterAudiogramValues()
    {
        var window = AutomationManager.MainWindow
            ?? throw new InvalidOperationException("Audiogram Module is not attached.");
        AutomationElement? cell = null;
        for (var attempt = 1; attempt <= 10; attempt++)
        {
            cell = window.FindFirstDescendant(cf => cf.ByName(audiogramcell));
            if (cell != null) break;
            Thread.Sleep(500);
        }
        if (cell == null)
        {
            Console.WriteLine("Audiogram cell not found using exact name: " + audiogramcell);
            foreach (var candidate in window.FindAllDescendants().Where(e =>
                         e.Name?.Contains("TestValuesViewModel", StringComparison.OrdinalIgnoreCase) == true))
                Console.WriteLine($"Candidate: Name='{candidate.Name}', Type='{candidate.ControlType}'");
            throw new InvalidOperationException("Audiogram cell was not found. Check the candidate output.");
        }

        cell.Click();
        Thread.Sleep(200);
        Keyboard.Type("50");
        Thread.Sleep(200);
        for (var i = 0; i < 13; i++)
        {
            Keyboard.Press(VirtualKeyShort.TAB);
            Thread.Sleep(100);
            Keyboard.Press(VirtualKeyShort.TAB);
            Thread.Sleep(100);
            Keyboard.Type("40");
            Thread.Sleep(200);
        }
    }

    public void LaunchFsw(string fsw)
    {
        SessionManager.SwitchToWindow("Noah 4");
        FindByName("_File").Click();
        Thread.Sleep(1000);
        FindByName("Open").Click();
        Thread.Sleep(1000);
        FindByNameContains(fsw).Click();
    }
}
