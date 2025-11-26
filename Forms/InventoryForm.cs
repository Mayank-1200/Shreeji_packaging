using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using shreeji_packaging.Models;
using shreeji_packaging.Services;

namespace shreeji_packaging.Forms
{
    public class InventoryForm : Form
    {
        private Inventory _inventory;
        private DataGridView dataGridView1;
        private TextBox txtGeneralPaper, txtGeneralLiner;
        private Button btnSave;

        public InventoryForm()
        {
            this.Text = "Inventory Management - Shreeji Packaging";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.MinimumSize = new Size(1000, 600);

            InitializeComponents();
            LoadInventoryData();
            
            // Reload inventory when form is activated (switched back to)
            this.Activated += (s, e) => LoadInventoryData();
        }

        private void InitializeComponents()
        {
            // Header Panel
            Panel headerPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(30, 15, 30, 15)
            };

            Label lblTitle = new Label()
            {
                Text = "📊 Inventory Management",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            Button btnGoToCustomers = new Button()
            {
                Text = "👥 Customers",
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGoToCustomers.FlatAppearance.BorderSize = 0;
            btnGoToCustomers.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnGoToCustomers.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            };

            headerPanel.Controls.AddRange(new Control[] { lblTitle, btnGoToCustomers });
            btnGoToCustomers.Location = new Point(headerPanel.Width - btnGoToCustomers.Width - 30, 22);
            this.Controls.Add(headerPanel);

            // Main Content Panel
            Panel contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 100, 30, 30),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Stock Table Section
            Label lblTableHeader = new Label()
            {
                Text = "Stock Table (Rows: 26-52, Columns: GSM Values)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(30, 560)
            };
            contentPanel.Controls.Add(lblTableHeader);

            dataGridView1 = new DataGridView()
            {
                Location = new Point(30, 85),
                Size = new Size(1200, 450),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(223, 230, 233),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 45
            };

            // Add Row Number column
            DataGridViewTextBoxColumn rowCol = new DataGridViewTextBoxColumn()
            {
                Name = "Row",
                HeaderText = "Row",
                Width = 80,
                ReadOnly = true
            };
            dataGridView1.Columns.Add(rowCol);

            // Add GSM columns
            int[] gsmValues = { 80, 90, 100, 120, 150, 180, 200, 230, 250, 300 };
            foreach (int gsm in gsmValues)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn()
                {
                    Name = $"GSM_{gsm}",
                    HeaderText = gsm.ToString(),
                    Width = 110,
                    ReadOnly = false
                };
                dataGridView1.Columns.Add(col);
            }

            // Populate rows
            for (int row = 26; row <= 52; row++)
            {
                int rowIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowIndex].Cells["Row"].Value = row;
                dataGridView1.Rows[rowIndex].Cells["Row"].ReadOnly = true;
                dataGridView1.Rows[rowIndex].Cells["Row"].Style.BackColor = Color.FromArgb(236, 240, 241);
            }

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            contentPanel.Controls.Add(dataGridView1);

            // General Stock Section
            Panel generalPanel = new Panel()
            {
                Location = new Point(1250, 105),
                Size = new Size(300, 150),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            Label lblGeneralHeader = new Label()
            {
                Text = "General Stock",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 15)
            };
            generalPanel.Controls.Add(lblGeneralHeader);

            Label lblGeneralPaper = new Label()
            {
                Text = "Paper (kg):",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(15, 50),
                Width = 100
            };
            txtGeneralPaper = new TextBox()
            {
                Location = new Point(120, 47),
                Width = 150,
                Height = 25,
                Font = new Font("Segoe UI", 10)
            };
            generalPanel.Controls.AddRange(new Control[] { lblGeneralPaper, txtGeneralPaper });

            Label lblGeneralLiner = new Label()
            {
                Text = "Liner (kg):",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(15, 85),
                Width = 100
            };
            txtGeneralLiner = new TextBox()
            {
                Location = new Point(120, 82),
                Width = 150,
                Height = 25,
                Font = new Font("Segoe UI", 10)
            };
            generalPanel.Controls.AddRange(new Control[] { lblGeneralLiner, txtGeneralLiner });

            contentPanel.Controls.Add(generalPanel);

            // Save Button
            btnSave = new Button()
            {
                Text = "💾 Save Inventory",
                Location = new Point(1250, 280),
                Size = new Size(300, 50),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnSave.Click += BtnSave_Click;
            contentPanel.Controls.Add(btnSave);

            this.Controls.Add(contentPanel);
        }

        private void LoadInventoryData()
        {
            // Reload inventory from storage to get latest values
            _inventory = StorageService.LoadInventory();
            
            // Load table data
            int[] gsmValues = { 80, 90, 100, 120, 150, 180, 200, 230, 250, 300 };
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                int row = 26 + i;
                foreach (int gsm in gsmValues)
                {
                    double value = _inventory.GetStock(row, gsm);
                    dataGridView1.Rows[i].Cells[$"GSM_{gsm}"].Value = value.ToString("0.##");
                }
            }

            // Load general stock
            txtGeneralPaper.Text = _inventory.GeneralPaperStock.ToString("0.##");
            txtGeneralLiner.Text = _inventory.GeneralLinerStock.ToString("0.##");
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Save table data
                int[] gsmValues = { 80, 90, 100, 120, 150, 180, 200, 230, 250, 300 };
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    int row = 26 + i;
                    foreach (int gsm in gsmValues)
                    {
                        string cellValue = dataGridView1.Rows[i].Cells[$"GSM_{gsm}"].Value?.ToString() ?? "0";
                        if (double.TryParse(cellValue, out double value))
                        {
                            _inventory.SetStock(row, gsm, value);
                        }
                    }
                }

                // Save general stock
                if (double.TryParse(txtGeneralPaper.Text, out double paperStock))
                    _inventory.GeneralPaperStock = paperStock;
                if (double.TryParse(txtGeneralLiner.Text, out double linerStock))
                    _inventory.GeneralLinerStock = linerStock;

                StorageService.SaveInventory(_inventory);
                MessageBox.Show("Inventory saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving inventory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

