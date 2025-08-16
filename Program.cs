using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Parcel
{
    private const decimal DimensionalWeightDivisor = 139m; 
    private const decimal MaxLength = 60m; 
    private const decimal MaxWeight = 150m; 

    public decimal Length { get; }
    public decimal Width { get; }
    public decimal Height { get; }
    public decimal ActualWeight { get; } 
    public decimal DimensionalWeight { get; }
    public decimal BillableWeight { get; }

    public Parcel(decimal length, decimal width, decimal height, decimal weight)
    {
 
        if (length <= 0 || width <= 0 || height <= 0 || weight <= 0)
            throw new ArgumentException("All dimensions and weight must be positive numbers");

        if (length > MaxLength || width > MaxLength || height > MaxLength)
            throw new ArgumentException($"No dimension may exceed {MaxLength} inches");

        if (weight > MaxWeight)
            throw new ArgumentException($"Weight may not exceed {MaxWeight} lbs");

        Length = length;
        Width = width;
        Height = height;
        ActualWeight = weight;

        DimensionalWeight = Math.Ceiling((Length * Width * Height) / DimensionalWeightDivisor);

        BillableWeight = Math.Max(ActualWeight, DimensionalWeight);
    }
}
public enum ShippingSpeed
{
    Standard,
    Express
}

public static class ShippingCalculator
{
    public static decimal CalculateShippingCost(Parcel parcel, string zoneCode, ShippingSpeed speed)
    {

        decimal baseRate = GetBaseRate(parcel.BillableWeight);

        decimal zoneMultiplier = GetZoneMultiplier(zoneCode);

        decimal speedMultiplier = GetSpeedMultiplier(speed);

        decimal totalCost = baseRate * zoneMultiplier * speedMultiplier;

        return Math.Round(totalCost, 2); 
    }

    private static decimal GetBaseRate(decimal billableWeight)
    {
        if (billableWeight <= 1m) return 3.00m;
        if (billableWeight <= 5m) return 8.00m;
        if (billableWeight <= 20m) return 15.00m;
        if (billableWeight <= 50m) return 35.00m;
        return 70.00m; 
    }

    private static decimal GetZoneMultiplier(string zoneCode)
    {
        switch (zoneCode.ToUpper())
        {
            case "1": return 1.00m; 
            case "2": return 1.30m; 
            case "3": return 1.70m; 
            case "4": return 2.20m; 
            case "5": return 3.00m; 
            default:
                throw new ArgumentException("Invalid zone code. Must be 1-5");
        }
    }

    private static decimal GetSpeedMultiplier(ShippingSpeed speed)
    {
        switch (speed)
        {
            case ShippingSpeed.Standard:
                return 1.00m;
            case ShippingSpeed.Express:
                return 1.75m;
            default:
                throw new ArgumentException("Invalid shipping speed");
        }
    }
}

namespace Parcel_Shipping_Estimator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Shipping Cost Calculator");

                decimal length = GetDecimalInput("Enter package length (inches): ");
                decimal width = GetDecimalInput("Enter package width (inches): ");
                decimal height = GetDecimalInput("Enter package height (inches): ");
                decimal weight = GetDecimalInput("Enter package weight (lbs): ");

                var parcel = new Parcel(length, width, height, weight);

                Console.WriteLine("\nParcel details:");
                Console.WriteLine($"Dimensions: {parcel.Length} × {parcel.Width} × {parcel.Height} in");
                Console.WriteLine($"Actual weight: {parcel.ActualWeight} lbs");
                Console.WriteLine($"Dimensional weight: {parcel.DimensionalWeight} lbs");
                Console.WriteLine($"Billable weight: {parcel.BillableWeight} lbs");

                Console.Write("\nEnter destination zone (1-5): ");
                string zone = Console.ReadLine();

                Console.Write("Enter shipping speed (Standard/Express): ");
                ShippingSpeed speed;
                while (!Enum.TryParse<ShippingSpeed>(Console.ReadLine(), true, out speed))
                {
                    Console.WriteLine("Invalid shipping speed. Please enter 'Standard' or 'Express'.");
                }

                decimal cost = ShippingCalculator.CalculateShippingCost(parcel, zone, speed);

                Console.WriteLine($"\nTotal Shipping Cost: ${cost:F2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine("Please check your inputs and try again.");
            }
        }

        private static decimal GetDecimalInput(string prompt)
        {
            decimal result;
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out result) && result > 0)
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a positive decimal number.");
            }
        }
    }
}
