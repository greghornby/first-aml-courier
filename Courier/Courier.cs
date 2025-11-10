using System.Linq;

namespace Courier;

public class CourierService
{
    static private int OVERWEIGHT_PER_KG_CHARGE = 2;

    static public OrderSummary CreateOrder(IEnumerable<OrderInputPackage> packages, OrderInputOptions options)
    {
        int packageTotalCostInDollars = 0;
        int totalOverweightCost = 0;
        var summaryPackages = new List<OrderSummaryPackage>();

        foreach (OrderInputPackage package in packages)
        {
            int costInDollars = GetBaseCostForPackage(package);
            PackageSizeLabel sizeLabel = GetSizeLabelForPackage(package);
            int weightLimit = GetWeightLimitForPackage(package, sizeLabel);
            int overWeightAmount = 0;
            int overWeightCharge = 0;
            if (package.Weight > weightLimit)
            {
                // assuming weights are rounded to integers (up, down or closest, who knows!)
                int weightDiff = package.Weight - weightLimit;
                overWeightAmount = weightDiff;
                overWeightCharge = weightDiff * OVERWEIGHT_PER_KG_CHARGE;
                totalOverweightCost += overWeightCharge;
            }
            summaryPackages.Add(new OrderSummaryPackage { Size = package.Size, Cost = costInDollars, SizeLabel = sizeLabel, OverWeightAmount = overWeightAmount, OverweightCharge = overWeightCharge });
            packageTotalCostInDollars += costInDollars;
        }

        bool isSpeedy = options.SpeedyDelivery;
        int totalCostInDollars = packageTotalCostInDollars * (isSpeedy ? 2 : 1) + totalOverweightCost;
        int speedyCost = isSpeedy ? packageTotalCostInDollars : 0;

        return new OrderSummary { TotalCost = totalCostInDollars, Packages = summaryPackages, PackageTotalBaseCost = packageTotalCostInDollars, OverWeightTotalCost = totalOverweightCost, SpeedyDelivery = isSpeedy, SpeedyCost = speedyCost };
    }

    static private int GetBaseCostForPackage(OrderInputPackage package)
    {
        int costInDollars = package.Size switch
        {
            var s when s < 10 => 3,
            var s when s < 50 => 8,
            var s when s < 100 => 15,
            _ => 25
        };

        return costInDollars;
    }

    static private PackageSizeLabel GetSizeLabelForPackage(OrderInputPackage package)
    {
        PackageSizeLabel sizelabel = package.Size switch
        {
            var s when s < 10 => PackageSizeLabel.S,
            var s when s < 50 => PackageSizeLabel.M,
            var s when s < 100 => PackageSizeLabel.L,
            _ => PackageSizeLabel.XL
        };

        return sizelabel;
    }

    // Can provide the sizeLabel if it is already known, otherwise it will be calculated
    static private int GetWeightLimitForPackage(OrderInputPackage package, PackageSizeLabel? sizeLabel)
    {
        var calculatedSizeLabel = sizeLabel ?? GetSizeLabelForPackage(package);

        int weightLimit = calculatedSizeLabel switch
        {
            PackageSizeLabel.S => 1,
            PackageSizeLabel.M => 3,
            PackageSizeLabel.L => 6,
            PackageSizeLabel.XL => 10,
            _ => 10 //for some reason it said the switch cases were non-exhaustive (even though all the enum values are covered), so add the default case (might take too much time to investigate)
        };

        return weightLimit;
    }



    public enum PackageSizeLabel { S, M, L, XL };
    public readonly record struct OrderInputPackage(int Size, int Weight);
    public readonly record struct OrderInputOptions(bool SpeedyDelivery);
    public readonly record struct OrderSummaryPackage(int Size, int Cost, PackageSizeLabel SizeLabel, int OverWeightAmount, int OverweightCharge);
    public readonly record struct OrderSummary(IReadOnlyList<OrderSummaryPackage> Packages, int PackageTotalBaseCost, int OverWeightTotalCost, bool SpeedyDelivery, int? SpeedyCost, int TotalCost);
}
