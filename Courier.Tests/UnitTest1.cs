namespace Courier.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestCreateOrder()
    {
        var order1 = CourierService.CreateOrder([
            new CourierService.OrderInputPackage { Size = 5 },
            new CourierService.OrderInputPackage { Size = 20 },
            new CourierService.OrderInputPackage { Size = 80 },
            new CourierService.OrderInputPackage { Size = 120 }
        ]);

        Assert.AreEqual(51, order1.TotalCost);
        CollectionAssert.AreEqual(
            new[]
            {
                new CourierService.OrderSummaryPackage { Size = 5, Cost = 3, SizeLabel = CourierService.PackageSizeLabel.S },
                new CourierService.OrderSummaryPackage { Size = 20, Cost = 8, SizeLabel = CourierService.PackageSizeLabel.M },
                new CourierService.OrderSummaryPackage { Size = 80, Cost = 15, SizeLabel = CourierService.PackageSizeLabel.L },
                new CourierService.OrderSummaryPackage { Size = 120, Cost = 25, SizeLabel = CourierService.PackageSizeLabel.XL }
            },
            order1.Packages.ToArray()
        );
    }
}