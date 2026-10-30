using FlaUIAutomation.FSWLaunch;
using FlaUIAutomation.Pages;

namespace FlaUIAutomation.StepDefinitions;

[Binding]
public class DemoSteps
{
    private readonly HomePage _homePage; 
    private readonly AppLaunch _appLaunch;
    private readonly NOAHactions _noahActions;
    private readonly FittingScreen _fittingScreen;

    public DemoSteps()
    {
        _homePage = new HomePage();
        _appLaunch = new AppLaunch();
        _noahActions = new NOAHactions();
        _fittingScreen = new FittingScreen();
    }

    [When("Add or Select Patients is clicked")]
    public void WhenAddOrSelectPatientsIsClicked()
    {
        _homePage.ClickPatientDatabase();
    }

    [Given("Launch the application {string} in system")]
    public void GivenLaunchTheApplicationInSystem(string app)
    {
        _appLaunch.launchapps(app);
    }

    [When("The login password to NOAH {string} is entered")]
    public void WhenTheLoginPasswordToNOAHIsEntered(string p0)
    {
        _noahActions.EnterPassword(p0);
    }


    [When("I click OK button in NOAH")]
    public void WhenIClickOKButtonInNOAH()
    {
        _noahActions.ClickOKButton();
    }
    [When("I add First name {string} and last Name {string}")]
    public void WhenIAddFirstNameAndLastName(string fname, string lname)
    {
        _noahActions.AddPatient(fname, lname);
    }
    [When("I Launch FSW {string}")]
    public void WhenILaunchFSW(string fsw)
    {
        _noahActions.launchfsw(fsw);
    }
    [When("I add patient in SA mode")]
    public void WhenIAddPatientInSAMode()
    {
        _homePage.AddNewpatient("Demoooo", "JSDAWRG4229");
    }

    [When("I select device {string} with the side {string}")]
    public void WhenISelectDeviceWithTheSide(string device, string direction)
    {
        _homePage.SelectDeviceWithFullname(device, direction);
    }
    [When("We navigate to the Fitting screen")]
    public void WhenWeNavigateToTheFittingScreen()
    {
        _homePage.NavigateToFitScreeen();
    }
    [When("We Save session")]
    public void WhenWeSaveSession()
    {
        _fittingScreen.savesession();
    }
    [When("We Select the exixting patient {string}")]
    public void WhenWeSelectTheExixtingPatient(string firstName)
    {
        _homePage.SelectExixtingPatient(firstName);
    }


}
