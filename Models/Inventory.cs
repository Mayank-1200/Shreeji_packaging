using System.Collections.Generic;

namespace shreeji_packaging.Models
{
    public class Inventory
    {
        public Dictionary<int, Dictionary<int, double>> StockTable { get; set; } = new Dictionary<int, Dictionary<int, double>>();
        public double GeneralPaperStock { get; set; } = 0;
        public double GeneralLinerStock { get; set; } = 0;

        public Inventory()
        {
            // Initialize table with rows 26-52 and columns 80, 90, 100, 120, 150, 180, 200, 230, 250, 300
            int[] gsmValues = { 80, 90, 100, 120, 150, 180, 200, 230, 250, 300 };
            for (int row = 26; row <= 52; row++)
            {
                StockTable[row] = new Dictionary<int, double>();
                foreach (int gsm in gsmValues)
                {
                    StockTable[row][gsm] = 0;
                }
            }
        }

        public double GetStock(int row, int gsm)
        {
            if (StockTable.ContainsKey(row) && StockTable[row].ContainsKey(gsm))
                return StockTable[row][gsm];
            return 0;
        }

        public void SetStock(int row, int gsm, double value)
        {
            if (!StockTable.ContainsKey(row))
                StockTable[row] = new Dictionary<int, double>();
            
            StockTable[row][gsm] = value;
        }

        public bool DeductStock(int row, int gsm, double amount)
        {
            if (StockTable.ContainsKey(row) && StockTable[row].ContainsKey(gsm))
            {
                if (StockTable[row][gsm] >= amount)
                {
                    StockTable[row][gsm] -= amount;
                    return true;
                }
            }
            return false;
        }

        public bool DeductGeneralPaper(double amount)
        {
            if (GeneralPaperStock >= amount)
            {
                GeneralPaperStock -= amount;
                return true;
            }
            return false;
        }

        public bool DeductGeneralLiner(double amount)
        {
            if (GeneralLinerStock >= amount)
            {
                GeneralLinerStock -= amount;
                return true;
            }
            return false;
        }
    }
}

