using System.Linq;

namespace Courier;

public class CourierService
{
    static public OrderSummary CreateOrder(IEnumerable<OrderInputPackage> packages)
    {
        int totalCostInDollars = 0;
        var summaryPackages = new List<OrderSummaryPackage>();

        foreach (OrderInputPackage package in packages)
        {
            int costInDollars = package.Size switch
            {
                var s when s < 10 => 3,
                var s when s < 50 => 8,
                var s when s < 100 => 15,
                _ => 25
            };
            PackageSizeLabel sizelabel = package.Size switch
            {
                var s when s < 10 => PackageSizeLabel.S,
                var s when s < 50 => PackageSizeLabel.M,
                var s when s < 100 => PackageSizeLabel.L,
                _ => PackageSizeLabel.XL
            };
            summaryPackages.Add(new OrderSummaryPackage { Size = package.Size, Cost = costInDollars, SizeLabel = sizelabel });
            totalCostInDollars += costInDollars;
        }

        return new OrderSummary { TotalCost = totalCostInDollars, Packages = summaryPackages };
    }



    public enum PackageSizeLabel { S, M, L, XL};
    public readonly record struct OrderInputPackage(int Size);
    public readonly record struct OrderSummaryPackage(int Size, int Cost, PackageSizeLabel SizeLabel);
    public readonly record struct OrderSummary(IReadOnlyList<OrderSummaryPackage> Packages, int TotalCost);
}
