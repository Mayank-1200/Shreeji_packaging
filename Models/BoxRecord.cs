namespace shreeji_packaging.Models
{
    public class BoxRecord
    {
        public string BoxName { get; set; }
        public string BoxSize { get; set; } // Format: L x B x H
        public string SheetSize { get; set; } // Calculated (Half)
        public string SheetSizeFull { get; set; } // Calculated (Full)
        public double GSM { get; set; } // User input
        public int Ply { get; set; } // Odd number
        public double LastPlyValue { get; set; } // User input
        public double Gramage { get; set; } // Calculated
        public double PaperUsage { get; set; } // Calculated
        public double LinerUsage { get; set; } // Calculated
        public double SellRate { get; set; } // User input
        public int NumberOfBoxes { get; set; } // User input
        public double PerBoxRate { get; set; } // Calculated
        public double GrandTotal { get; set; } // Calculated
        public double LaminationPerBox { get; set; } // Optional extra per box
        public double PrintingPerBox { get; set; } // Optional extra per box
        public double PunchingPerBox { get; set; }
        public double PastingPerBox { get; set; }
        public double SidePastingPerBox { get; set; }
        public double LaminationCalculatedPerBox { get; set; }
        public double LaminationTotal { get; set; }
        public bool UseHalfSheetForUsage { get; set; }
        public double PaperWeightPerBox { get; set; }
        public double PaperWeightTotal { get; set; }
        public double LinerWeightPerBox { get; set; }
        public double LinerWeightTotal { get; set; }
        public double FinalTotal { get; set; } // Final payable total
        public string Detail { get; set; }
    }
}
