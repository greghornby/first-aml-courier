namespace Courier.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod()]
    public void TestMethod1()
    {
        Assert.AreEqual(5, CourierService.Test());
    }
}