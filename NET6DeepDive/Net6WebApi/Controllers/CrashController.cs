using Microsoft.AspNetCore.Mvc;

namespace Net6WebApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class CrashController : Controller
{
    [HttpGet]
    [Route("")]
    [STAThread()]
    public string Crash()
    {
        var foo = new StackOverflowSample();
        return foo.MyText;
    }

    [HttpGet]
    [Route("exception")]
    public string ThrowException()
    {
        Thread.Sleep(10000);
        throw new Exception("Unhandles...");
    }
}
public class StackOverflowSample
{
    private string m_MyText;

    public string MyText
    {
        get => MyText;
        set => m_MyText = value;
    }
}