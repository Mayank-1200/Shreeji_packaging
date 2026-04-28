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
        private Label lblGrandTotal;

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
                Location = new Point(30, 30)
            };
            contentPanel.Controls.Add(lblTableHeader);

            dataGridView1 = new DataGridView()
            {
                Location = new Point(30, 65),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(223, 230, 233),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 45,
                ScrollBars = ScrollBars.Both,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                RowHeadersVisible = false
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

            // Populate data rows (26-52)
            for (int row = 26; row <= 52; row++)
            {
                int rowIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowIndex].Cells["Row"].Value = row;
                dataGridView1.Rows[rowIndex].Cells["Row"].ReadOnly = true;
                dataGridView1.Rows[rowIndex].Cells["Row"].Style.BackColor = Color.FromArgb(236, 240, 241);
            }

            // Add Totals row (for column totals)
            int totalsRowIndex = dataGridView1.Rows.Add();
            dataGridView1.Rows[totalsRowIndex].Cells["Row"].Value = "Total";
            dataGridView1.Rows[totalsRowIndex].Cells["Row"].ReadOnly = true;
            dataGridView1.Rows[totalsRowIndex].Cells["Row"].Style.BackColor = Color.FromArgb(241, 196, 15);
            dataGridView1.Rows[totalsRowIndex].Cells["Row"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.Rows[totalsRowIndex].Cells["Row"].Style.ForeColor = Color.White;
            foreach (int gsm in gsmValues)
            {
                dataGridView1.Rows[totalsRowIndex].Cells[$"GSM_{gsm}"].ReadOnly = true;
                dataGridView1.Rows[totalsRowIndex].Cells[$"GSM_{gsm}"].Style.BackColor = Color.FromArgb(241, 196, 15);
                dataGridView1.Rows[totalsRowIndex].Cells[$"GSM_{gsm}"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dataGridView1.Rows[totalsRowIndex].Cells[$"GSM_{gsm}"].Style.ForeColor = Color.White;
            }

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Handle cell value changes to recalculate totals
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellValueChanged;

            contentPanel.Controls.Add(dataGridView1);

            // Grand Total Label - Positioned below the DataGridView
            lblGrandTotal = new Label()
            {
                Text = "Grand Total: 0",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(231, 76, 60),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(250, 45),
                Location = new Point(30, 0), // Will be adjusted in resize handler
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            contentPanel.Controls.Add(lblGrandTotal);

            // General Stock Section - Positioned at top right
            Panel generalPanel = new Panel()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(0, 30), // Will be adjusted after adding to panel
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

            // Adjust general panel position after adding to contentPanel
            generalPanel.Location = new Point(contentPanel.Width - generalPanel.Width - 30, 30);

            // Save Button - Positioned below General Stock panel
            btnSave = new Button()
            {
                Text = "💾 Save Inventory",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(0, 200), // Will be adjusted after adding to panel
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

            // Adjust save button position after adding to contentPanel
            btnSave.Location = new Point(contentPanel.Width - btnSave.Width - 30, 200);

            // Handle resize to keep panels positioned correctly and adjust DataGridView size
            contentPanel.Resize += (s, e) =>
            {
                generalPanel.Location = new Point(contentPanel.Width - generalPanel.Width - 30, 30);
                btnSave.Location = new Point(contentPanel.Width - btnSave.Width - 30, 200);

                // Adjust DataGridView to fill available space
                // Leave room for: right panel (300px) + margins (30 left + 30 right + 20 gap) = 380px reserved on right
                int tableWidth = contentPanel.Width - 30 - 350; // 30=left margin, 350=space for right panel
                int tableHeight = contentPanel.Height - 65 - 60 - 30; // 65=top offset, 60=space for label, 30=bottom margin
                dataGridView1.Size = new Size(tableWidth, tableHeight);

                // Position grand total label below the DataGridView
                lblGrandTotal.Location = new Point(30, dataGridView1.Bottom + 10);
            };

            this.Controls.Add(contentPanel);
        }

        private void LoadInventoryData()
        {
            // Reload inventory from storage to get latest values
            _inventory = StorageService.LoadInventory();

            // Load table data (only data rows, not totals rows)
            int[] gsmValues = { 80, 90, 100, 120, 150, 180, 200, 230, 250, 300 };
            int dataRowCount = 52 - 26 + 1; // Rows 26 to 52

            for (int i = 0; i < dataRowCount; i++)
            {
                int row = 26 + i;
                foreach (int gsm in gsmValues)
                {
                    double value = _inventory.GetStock(row, gsm);
                    dataGridView1.Rows[i].Cells[$"GSM_{gsm}"].Value = value.ToString("0.##");
                }
            }

            // Calculate and update totals
            UpdateTotals();

            // Load general stock
            txtGeneralPaper.Text = _inventory.GeneralPaperStock.ToString("0.##");
            txtGeneralLiner.Text = _inventory.GeneralLinerStock.ToString("0.##");
        }

        private void UpdateTotals()
        {
            int[] gsmValues = { 80, 90, 100, 120, 150, 180, 200, 230, 250, 300 };
            int dataRowCount = 52 - 26 + 1; // Rows 26 to 52
            int totalsRowIndex = dataRowCount; // Index of totals row

            double grandTotalSum = 0;

            // Calculate column totals
            foreach (int gsm in gsmValues)
            {
                double columnTotal = 0;
                for (int i = 0; i < dataRowCount; i++)
                {
                    string cellValue = dataGridView1.Rows[i].Cells[$"GSM_{gsm}"].Value?.ToString() ?? "0";
                    if (double.TryParse(cellValue, out double value))
                    {
                        columnTotal += value;
                    }
                }
                dataGridView1.Rows[totalsRowIndex].Cells[$"GSM_{gsm}"].Value = columnTotal.ToString("0.##");
                grandTotalSum += columnTotal;
            }

            // Update grand total label
            lblGrandTotal.Text = $"Grand Total: {grandTotalSum:0.##}";
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Only recalculate if a data cell was changed (not totals rows or Row column)
            if (e == null) return;

            int dataRowCount = 52 - 26 + 1;
            if (e.RowIndex >= 0 && e.RowIndex < dataRowCount && e.ColumnIndex > 0)
            {
                UpdateTotals();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Save table data (only data rows, not totals rows)
                int[] gsmValues = { 80, 90, 100, 120, 150, 180, 200, 230, 250, 300 };
                int dataRowCount = 52 - 26 + 1; // Rows 26 to 52

                for (int i = 0; i < dataRowCount; i++)
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

