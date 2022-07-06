
using JetBrains.Annotations;

public interface ITestController
{
    int GetNumber();
    int IncreaseNumber(int orig);
}
[UsedImplicitly]
public class TestController : ITestController
{
    public int GetNumber()
    {
        return 43;
    }

    public int IncreaseNumber(int orig)
    {
        var returnValue = orig + 1;
        return returnValue;
    }
}
