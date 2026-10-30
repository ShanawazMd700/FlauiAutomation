using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUIAutomation.Drivers;
using FlaUIAutomation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaUIAutomation.Pages
{
    public class NOAHactions : ControlHelpers
    {
        private const string OKButton = "cmdOK";
        private const string passwordfield = "Password";

        private const string fileoptionmenu = "_File";
        private const string lastnamefield = "LastName";
        private const string firstnamefield = "FirstName";


        public void EnterPassword(string password)
        {
            EnterTextByAutomationId(passwordfield, password);
        }

        public void ClickOKButton()
        {
            ClickByAutomationId(OKButton);
            Thread.Sleep(2000);
        }



        public void AddPatient(string fname, string lname)
        {
            SessionManager.ReattachMainWindow();
            Console.WriteLine("========== AFTER REATTACH ==========");
            ClickByAutomationId("cmdAddPatient");
            SessionManager.ReattachMainWindow();
            EnterTextByAutomationId(lastnamefield, lname);
            EnterTextByAutomationId(firstnamefield, fname);
            FindByName("Male").Click();
            FindByName("OK").Click();
            SessionManager.ReattachMainWindow();
            Thread.Sleep(4000);
            // Open the module
            Console.WriteLine("===== STEP 1: BEFORE FILE MENU =====");
            FindByName("_File").Click();
            Thread.Sleep(1000);
            Console.WriteLine("===== STEP 2: AFTER FILE MENU =====");
            Console.WriteLine("===== STEP 3: CLICK OPEN =====");
            FindByName("Open").Click();
            Thread.Sleep(1000);
            FindByName("Noah 4 Audiogram Module").Click();
            Thread.Sleep(2000);
            SessionManager.SwitchToWindow("Audiogram Module");
            Console.WriteLine("===== STEP 4: AFTER OPEN =====");
            Console.WriteLine("===============Inspect Audiogram Grid==================");
            Thread.Sleep(1500);
            //SessionManager.InspectAudiogramGrid();

            enterAudiogramValues();
            SessionManager.CloseWindow("Audiogram Module");
            Thread.Sleep(3000);
            SessionManager.ClickSaveMeasurementOK();

            Thread.Sleep(6000);
            //SessionManager.CloseWindow("Audiogram Module");
        }
        private const string audiogramcell = "Item: Himsa.Noah.CommonControls.TestValuesViewModel, Column Display Index: 0"; // Name	Item: Himsa.Noah.CommonControls.TestValuesViewModel, Column Display Index: 0
        private void enterAudiogramValues()
        {
            var window = AutomationManager.MainWindow
                ?? throw new InvalidOperationException(
                    "Audiogram Module is not attached.");
            AutomationElement? cell = null;
            // Retry because the module controls may take time to load.
            for (int attempt = 1; attempt <= 10; attempt++)
            {
                cell = window.FindFirstDescendant(
                    cf => cf.ByName(audiogramcell));
                if (cell != null)
                    break;
                Thread.Sleep(500);
            }
            if (cell == null)
            {
                Console.WriteLine(
                    "Audiogram cell not found using exact name: " +
                    audiogramcell);

                var candidates = window.FindAllDescendants()
                    .Where(e =>
                    {
                        try
                        {
                            return e.Name?.Contains(
                                "TestValuesViewModel",
                                StringComparison.OrdinalIgnoreCase) == true;
                        }
                        catch
                        {
                            return false;
                        }
                    });

                foreach (var candidate in candidates)
                {
                    Console.WriteLine(
                        $"Candidate: Name='{candidate.Name}', " +
                        $"Type='{candidate.ControlType}'");
                }

                throw new InvalidOperationException(
                    "Audiogram cell was not found. Check the candidate output.");
            }
            cell.Click();
            Thread.Sleep(200);
            Keyboard.Type("50");
            Thread.Sleep(200);
            for (int i = 0; i < 13; i++)
            {
                Keyboard.Press(VirtualKeyShort.TAB);
                Thread.Sleep(100);
                Keyboard.Press(VirtualKeyShort.TAB);
                Thread.Sleep(100);
                Keyboard.Type("40");
                Thread.Sleep(200);
            }
        }

        private static void PrintElementTree(
    AutomationElement element,
    int level = 0)
        {
            string indent = new string(' ', level * 2);

            string name = "";
            string automationId = "";
            string controlType = "";

            try
            {
                name = element.Name;
            }
            catch
            {
            }

            try
            {
                automationId =
                    element.Properties.AutomationId.ValueOrDefault ?? "";
            }
            catch
            {
            }

            try
            {
                controlType =
                    element.ControlType.ToString();
            }
            catch
            {
            }

            Console.WriteLine(
                $"{indent}" +
                $"Name='{name}' | " +
                $"AutomationId='{automationId}' | " +
                $"ControlType='{controlType}'");

            foreach (var child in element.FindAllChildren())
            {
                PrintElementTree(child, level + 1);
            }
        }
        public void launchfsw(string fsw)
        {
            SessionManager.SwitchToWindow("Noah 4");
            FindByName("_File").Click();
            Thread.Sleep(1000);
            FindByName("Open").Click();
            Thread.Sleep(1000);
            FindByNameContains(fsw).Click();
        }

    }
}