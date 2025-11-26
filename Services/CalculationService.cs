using System; 
using shreeji_packaging.Models;

namespace shreeji_packaging.Services
{
    public static class CalculationService
    {
        public static string CalculateSheetSize(string boxSize)
        {
            // boxSize format: "16 x 12.25 x 7.75"
            var parts = boxSize.Split('x');
            if (parts.Length != 3) return "";

            double l = double.Parse(parts[0].Trim());
            double b = double.Parse(parts[1].Trim());
            double h = double.Parse(parts[2].Trim());

            double sheetLength = b + h + 1;      // b + h + 1
            double sheetBreadth = l + b + 1.5;   // l + b + 1.5

            return $"{Math.Round(sheetLength,2)} x {Math.Round(sheetBreadth,2)}";
        }

        public static string CalculateFullSheetSize(string boxSize)
        {
            // boxSize format: "L x B x H"
            var parts = boxSize.Split('x');
            if (parts.Length != 3) return "";

            double l = double.Parse(parts[0].Trim());
            double b = double.Parse(parts[1].Trim());
            double h = double.Parse(parts[2].Trim());

            // Full sheet as per requirement:
            // Length = (b + h) + 1
            // Breadth = (l + b) * 2 + 1.5
            double fullLength = b + h + 1;
            double fullBreadth = (l + b) * 2 + 1.5;

            return $"{Math.Round(fullLength, 2)} x {Math.Round(fullBreadth, 2)}";
        }

        public static double CalculateGramage(double gsm, int ply, double lastPly)
        {
            if (ply % 2 == 0)
                throw new ArgumentException("Ply must be an odd number.");

            // Step 1: base calculation
            double baseVal = gsm + gsm * 0.4; // 120 + 40% = 168

            // Step 2: Multiply by 2 for additional plies (if ply > 3)
            int extraMultiplication = (ply - 3) / 2;
            double result = baseVal;
            for (int i = 0; i < extraMultiplication; i++)
            {
                result = result * 2 + gsm; // double and add gsm again
            }

            // Step 3: Add initial gsm once more
            result += gsm; // final addition of initial gsm

            // Step 4: Add user-provided last ply value
            result += lastPly;

            return result;
        }

        public static (double Paper, double Liner) CalculatePaperAndLiner(int ply, bool useHalfSheet)
        {
            // New rule:
            // - Exactly one paper per box (full sheet) or two papers per box (half sheet)
            // - Number of liners = (ply - 1) / 2 for odd ply counts
            double multiplier = useHalfSheet ? 2.0 : 1.0;
            double paper = 1.0 * multiplier;
            double liner = ((ply - 1) / 2.0) * multiplier;
            return (paper, liner);
        }

        public static double CalculatePerBoxRate(double l, double b, double h, double gramage, double sellRate)
        {
            // Full sheet dimensions per requirement
            double fullLength = b + h + 1;            // (b + h) + 1
            double fullBreadth = (l + b) * 2 + 1.5;   // (l + b) * 2 + 1.5

            // Base as per requirement: (fullLength * fullBreadth * gramage) / 1550
            double baseValue = (fullLength * fullBreadth * gramage) / 1550.0;

            // Per-box rate: (baseValue * sellRate) / 1000
            return (baseValue * sellRate) / 1000.0;
        }

        public static double CalculateGrandTotal(double perBoxRate, int numberOfBoxes)
        {
            return perBoxRate * Math.Max(0, numberOfBoxes);
        }

        public static double CalculateLinerBaseGsm(double gsm, int ply)
        {
            if (ply % 2 == 0)
                throw new ArgumentException("Ply must be an odd number.");

            double baseVal = gsm + gsm * 0.4;
            int extraMultiplication = (ply - 3) / 2;
            double result = baseVal;
            for (int i = 0; i < extraMultiplication; i++)
            {
                result = result * 2 + gsm;
            }
            result += gsm;
            return result;
        }
    }
}
