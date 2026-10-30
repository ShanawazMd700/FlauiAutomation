using FlaUIAutomation.Drivers;

namespace FlaUIAutomation.Hooks;

[Binding]
public sealed class TestHooks
{
    [BeforeScenario]
    public void BeforeScenario()
    {
        SessionManager.StartSession();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        Thread.Sleep(9000);
        SessionManager.StopSession();
    }
}
