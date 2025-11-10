namespace Courier.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestCreateOrder_NoSpeedy()
    {

        // lazily set weight to 0 as we're uninterested in this atm
        var order = CourierService.CreateOrder([
            new CourierService.OrderInputPackage { Size = 5, Weight = 0 },
            new CourierService.OrderInputPackage { Size = 20, Weight = 0 },
            new CourierService.OrderInputPackage { Size = 80, Weight = 0 },
            new CourierService.OrderInputPackage { Size = 120, Weight = 0 }
        ], new CourierService.OrderInputOptions { SpeedyDelivery = false });

        Assert.AreEqual(51, order.PackageTotalBaseCost); // package total should be $51
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

        Assert.AreEqual(51, order1.PackageTotalBaseCost); // package total should be $51
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

    [TestMethod]
    public void TestCreateOrder_Overweight()
    {
        var order = CourierService.CreateOrder([
            new CourierService.OrderInputPackage { Size = 5, Weight = 1 }, //not overweight, actually on the exact weight limit
            new CourierService.OrderInputPackage { Size = 20, Weight = 6 }, //3kg overweight should incur an additional cost of $6
            new CourierService.OrderInputPackage { Size = 80, Weight = 4 }, //not overweight, under the limit
            new CourierService.OrderInputPackage { Size = 120, Weight = 11 } //1kg overweight, should incur $2
        ], new CourierService.OrderInputOptions { SpeedyDelivery = false });

        // check with dotnet test --logger "console;verbosity=detailed"
        // Console.WriteLine(order);

        int expectedPackageBase = 51;
        int expectedOverweightTotal = 8; // from the 2nd and 4th packages

        Assert.AreEqual(expectedPackageBase, order.PackageTotalBaseCost);
        Assert.AreEqual(false, order.SpeedyDelivery);
        Assert.AreEqual(0, order.SpeedyCost);
        Assert.AreEqual(expectedOverweightTotal, order.OverWeightTotalCost);
        Assert.AreEqual(expectedPackageBase + expectedOverweightTotal, order.TotalCost); //should be base packages + overweight (no speedy)

        // check the packages got size labelled correctly
        CollectionAssert.AreEqual(
            new[]
            {
                new CourierService.OrderSummaryPackage { Size = 5, Cost = 3, SizeLabel = CourierService.PackageSizeLabel.S, OverWeightAmount = 0, OverweightCharge = 0 },
                new CourierService.OrderSummaryPackage { Size = 20, Cost = 8, SizeLabel = CourierService.PackageSizeLabel.M, OverWeightAmount = 3, OverweightCharge = 6 },
                new CourierService.OrderSummaryPackage { Size = 80, Cost = 15, SizeLabel = CourierService.PackageSizeLabel.L, OverWeightAmount = 0, OverweightCharge = 0 },
                new CourierService.OrderSummaryPackage { Size = 120, Cost = 25, SizeLabel = CourierService.PackageSizeLabel.XL, OverWeightAmount = 1, OverweightCharge = 2 }
            },
            order.Packages.ToArray()
        );
    }
}