using System;
using System.Windows.Forms;
using shreeji_packaging.Forms;

namespace shreeji_packaging
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            string currentPage = null;
            
            while (true)
            {
                if (currentPage == null)
                {
                    // Show homepage
                    using (var homepage = new HomepageForm())
                    {
                        if (homepage.ShowDialog() != DialogResult.OK)
                            break;
                        currentPage = homepage.SelectedOption;
                    }
                }

                if (currentPage == "Inventory")
                {
                    using (var inventoryForm = new InventoryForm())
                    {
                        var result = inventoryForm.ShowDialog();
                        if (result == DialogResult.Yes)
                        {
                            currentPage = "Customers";
                        }
                        else
                        {
                            currentPage = null; // Return to homepage
                        }
                    }
                }
                else if (currentPage == "Customers")
                {
                    using (var mainForm = new MainForm())
                    {
                        var result = mainForm.ShowDialog();
                        if (result == DialogResult.Yes)
                        {
                            currentPage = "Inventory";
                        }
                        else
                        {
                            currentPage = null; // Return to homepage
                        }
                    }
                }
            }
        }
    }
}
