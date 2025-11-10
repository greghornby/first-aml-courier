namespace Courier.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestCreateOrder_NoSpeedy()
    {
        var order = CourierService.CreateOrder([
            new CourierService.OrderInputPackage { Size = 5 },
            new CourierService.OrderInputPackage { Size = 20 },
            new CourierService.OrderInputPackage { Size = 80 },
            new CourierService.OrderInputPackage { Size = 120 }
        ], new CourierService.OrderInputOptions { SpeedyDelivery = false });

        Assert.AreEqual(51, order.PackageTotalCost); // package total should be $51
        Assert.AreEqual(false, order.SpeedyDelivery); // no speedy
        Assert.AreEqual(0, order.SpeedyCost); // no speedy, no extra cost!
        Assert.AreEqual(51, order.TotalCost); // therefore total order should be $51

        // check the packages got size labelled correctly
        CollectionAssert.AreEqual(
            new[]
            {
                new CourierService.OrderSummaryPackage { Size = 5, Cost = 3, SizeLabel = CourierService.PackageSizeLabel.S },
                new CourierService.OrderSummaryPackage { Size = 20, Cost = 8, SizeLabel = CourierService.PackageSizeLabel.M },
                new CourierService.OrderSummaryPackage { Size = 80, Cost = 15, SizeLabel = CourierService.PackageSizeLabel.L },
                new CourierService.OrderSummaryPackage { Size = 120, Cost = 25, SizeLabel = CourierService.PackageSizeLabel.XL }
            },
            order.Packages.ToArray()
        );
    }

    [TestMethod]
    public void TestCreateOrder_WithSpeedy()
    {
        var order1 = CourierService.CreateOrder([
            new CourierService.OrderInputPackage { Size = 5 },
            new CourierService.OrderInputPackage { Size = 20 },
            new CourierService.OrderInputPackage { Size = 80 },
            new CourierService.OrderInputPackage { Size = 120 }
        ], new CourierService.OrderInputOptions { SpeedyDelivery = true });

        Assert.AreEqual(51, order1.PackageTotalCost); // package total should be $51
        Assert.AreEqual(true, order1.SpeedyDelivery); // speedy should be anbled
        Assert.AreEqual(51, order1.SpeedyCost); // speedy costs should match package cost
        Assert.AreEqual(102, order1.TotalCost); // therefore total order should be $102 ($51 base cost + $51 speedy cost)

        // check the packages got size labelled correctly
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