using System.Collections.Generic;
using CodeReviews.Console.CodingTracker;

[TestFixture]
public sealed class AppControllerTests
{
    private FakeAppView _appView = null!;
    private FakeCodingSessionController _codingController = null!;
    private AppController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _appView = new FakeAppView();
        _codingController = new FakeCodingSessionController();
        _controller = new AppController(_appView, _codingController);
    }

    [Test]
    public void Run_ViewSelected_CallsViewSessionsAndDisplaysGoodbye()
    {
        _appView.OptionsToReturn.Enqueue(MenuOption.View);
        _appView.OptionsToReturn.Enqueue(MenuOption.Close);

        _controller.Run();

        Assert.Multiple(() =>
        {
            Assert.That(_codingController.ViewSessionsCalls, Is.EqualTo(1));
            Assert.That(_appView.DisplayGoodbyeCalls, Is.EqualTo(1));
        });
    }

    [Test]
    public void Run_AddSelected_CallsAddSession()
    {
        _appView.OptionsToReturn.Enqueue(MenuOption.Add);
        _appView.OptionsToReturn.Enqueue(MenuOption.Close);

        _controller.Run();

        Assert.That(_codingController.AddSessionCalls, Is.EqualTo(1));
    }

    [Test]
    public void Run_EditSelected_CallsUpdateSession()
    {
        _appView.OptionsToReturn.Enqueue(MenuOption.Edit);
        _appView.OptionsToReturn.Enqueue(MenuOption.Close);

        _controller.Run();

        Assert.That(_codingController.UpdateSessionCalls, Is.EqualTo(1));
    }

    [Test]
    public void Run_DeleteSelected_CallsDeleteSession()
    {
        _appView.OptionsToReturn.Enqueue(MenuOption.Delete);
        _appView.OptionsToReturn.Enqueue(MenuOption.Close);

        _controller.Run();

        Assert.That(_codingController.DeleteSessionCalls, Is.EqualTo(1));
    }

    [Test]
    public void Run_CloseSelected_DisplaysGoodbyeOnly()
    {
        _appView.OptionsToReturn.Enqueue(MenuOption.Close);

        _controller.Run();

        Assert.Multiple(() =>
        {
            Assert.That(_codingController.ViewSessionsCalls, Is.Zero);
            Assert.That(_codingController.AddSessionCalls, Is.Zero);
            Assert.That(_codingController.UpdateSessionCalls, Is.Zero);
            Assert.That(_codingController.DeleteSessionCalls, Is.Zero);
            Assert.That(_appView.DisplayGoodbyeCalls, Is.EqualTo(1));
        });
    }
}
