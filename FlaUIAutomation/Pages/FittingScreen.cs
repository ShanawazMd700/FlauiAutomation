using System;
using System.Collections.Generic;
using System.Text;
using FlaUI.Core.WindowsAPI;
using FlaUIAutomation.Drivers;
using FlaUIAutomation.Utilities;
using FlaUI.Core.Input;
using System.Threading;

namespace FlaUIAutomation.Pages
{
    public class FittingScreen : ControlHelpers
    {
        private const string savebutton = "PatientAutomationIds.ProfileAutomationIds.SaveAction"; // AutomationId	PatientAutomationIds.ProfileAutomationIds.SaveAction
        private const string continuebutton = "SaveAutomationIds.PerformSaveAutomationIds.ContinueAction"; //AutomationId	SaveAutomationIds.PerformSaveAutomationIds.ContinueAction
        private const string exitbutton = "SaveAutomationIds.PerformSaveAutomationIds.ExitAction"; //AutomationId	SaveAutomationIds.PerformSaveAutomationIds.ExitAction



        public void SaveSession()
        {
            ClickByAutomationId(savebutton);
            //SessionManager.SwitchToWindowContainingAutomationId(exitbutton);
            Thread.Sleep(5000);
            ClickByAutomationId(exitbutton);
            Thread.Sleep(2000);
        }

    }
}
