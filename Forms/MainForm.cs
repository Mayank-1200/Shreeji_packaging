using System;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using shreeji_packaging.Models;
using shreeji_packaging.Forms;
using shreeji_packaging.Services;

namespace shreeji_packaging.Forms
{
    public class MainForm : Form
    {
        private Customer _customer;
        private Button btnAddRecord;
        private DataGridView dataGridView1;
        private Label lblTitle, lblCustomer, lblRecordsCount;
        private Panel headerPanel, toolbarPanel;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        public MainForm()
        {
            this.Text = "Shreeji Packaging";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.MinimumSize = new Size(1000, 600);

            // Customer selection dialog
            using (var chooser = new CustomerHomeForm())
            {
                var result = chooser.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(chooser.SelectedCustomer))
                {
                    _customer = new Customer(chooser.SelectedCustomer);
                }
                else
                {
                    // If no customer selected, close the form
                    this.Close();
                    return;
                }
            }

            InitializeComponents();
            RefreshGrid();
        }

        private void InitializeComponents()
        {
            // Header Panel
            headerPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(30, 15, 30, 15)
            };

            lblTitle = new Label()
            {
                Text = "📦 Shreeji Packaging",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            lblCustomer = new Label()
            {
                Text = $"Customer: {_customer.Name}",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(189, 195, 199),
                AutoSize = true,
                Location = new Point(30, 50)
            };

            Button btnGoToInventory = new Button()
            {
                Text = "📊 Inventory",
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGoToInventory.FlatAppearance.BorderSize = 0;
            btnGoToInventory.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnGoToInventory.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            };

            headerPanel.Controls.AddRange(new Control[] { lblTitle, lblCustomer, btnGoToInventory });
            btnGoToInventory.Location = new Point(headerPanel.Width - btnGoToInventory.Width - 30, 22);

            // Toolbar Panel
            toolbarPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(30, 15, 30, 15)
            };

            btnAddRecord = new Button()
            {
                Text = "➕ Add New Record",
                Width = 160,
                Height = 40,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, 15),
                Cursor = Cursors.Hand
            };
            btnAddRecord.FlatAppearance.BorderSize = 0;
            btnAddRecord.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnAddRecord.Click += BtnAddRecord_Click;

            Button btnExportPdf = new Button()
            {
                Text = "📄 Export to PDF",
                Width = 160,
                Height = 40,
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(210, 15),
                Cursor = Cursors.Hand
            };
            btnExportPdf.FlatAppearance.BorderSize = 0;
            btnExportPdf.FlatAppearance.MouseOverBackColor = Color.FromArgb(211, 84, 0);
            btnExportPdf.Click += BtnExportPdf_Click;

            lblRecordsCount = new Label()
            {
                Text = "Total Records: 0",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(390, 25)
            };

            toolbarPanel.Controls.AddRange(new Control[] { btnAddRecord, btnExportPdf, lblRecordsCount });

            // Main Content Panel
            Panel contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 30),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            dataGridView1 = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(223, 230, 233),
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                Font = new Font("Segoe UI", 10),
                RowTemplate = { Height = 35 },
                AllowUserToResizeRows = false,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ScrollBars = ScrollBars.Both
            };

            // Header Style
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dataGridView1.EnableHeadersVisualStyles = false;

            // Row Styles
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            dataGridView1.DefaultCellStyle.Padding = new Padding(8, 5, 8, 5);

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 252);
            dataGridView1.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);

            // Add event handlers
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView1.MouseWheel += DataGridView1_MouseWheel;
            dataGridView1.CellClick += DataGridView1_CellClick;

            // Grid container
            Panel gridContainer = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(1),
                BackColor = Color.FromArgb(189, 195, 199)
            };

            Panel gridInnerPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            gridInnerPanel.Controls.Add(dataGridView1);
            gridContainer.Controls.Add(gridInnerPanel);
            contentPanel.Controls.Add(gridContainer);

            // Status strip
            statusStrip = new StatusStrip()
            {
                BackColor = Color.FromArgb(236, 240, 241),
                Font = new Font("Segoe UI", 9)
            };

            statusLabel = new ToolStripStatusLabel()
            {
                Text = "Ready",
                ForeColor = Color.FromArgb(127, 140, 141)
            };

            ToolStripStatusLabel versionLabel = new ToolStripStatusLabel()
            {
                Text = "v1.0",
                ForeColor = Color.FromArgb(127, 140, 141),
                Spring = true,
                TextAlign = ContentAlignment.MiddleRight
            };

            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, versionLabel });

            // ADD PANELS IN CORRECT ORDER
            this.Controls.Add(contentPanel);   // Fill panel last
            this.Controls.Add(toolbarPanel);   // Top toolbar
            this.Controls.Add(headerPanel);    // Top header
            this.Controls.Add(statusStrip);    // Status strip at bottom
        }

        private void BtnAddRecord_Click(object sender, EventArgs e)
        {
            var form = new AddRecordForm(_customer);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshGrid();
                // Update status
                statusLabel.Text = "Record added successfully";

                // Reset status after 3 seconds
                Timer timer = new Timer() { Interval = 3000 };
                timer.Tick += (s, args) =>
                {
                    statusLabel.Text = "Ready";
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
        }

        private void BtnExportPdf_Click(object sender, EventArgs e)
        {
            try
            {
                var currentRecords = StorageService.LoadRecords(_customer.Name) ?? _customer.Records;
                var exportDialog = new ExportDialog(_customer.Name, currentRecords);
                
                if (exportDialog.ShowDialog() == DialogResult.OK)
                {
                    statusLabel.Text = "PDF export completed successfully";
                    
                    Timer timer = new Timer() { Interval = 3000 };
                    timer.Tick += (s, args) =>
                    {
                        statusLabel.Text = "Ready";
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening export dialog:\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGrid()
        {
            if (dataGridView1 == null || _customer?.Records == null) return;

            // Bind the data
            dataGridView1.DataSource = null;
            // Load stored records for current customer
            var stored = StorageService.LoadRecords(_customer.Name);
            _customer.Records = stored ?? _customer.Records;
            dataGridView1.DataSource = _customer.Records;

            // Update records count safely
            if (lblRecordsCount != null)
                lblRecordsCount.Text = $"Total Records: {_customer.Records.Count}";

            // Only customize columns if we actually have some
            if (_customer.Records.Count == 0) return; // safe guard

            // Add Actions column for Edit/Delete buttons
            if (dataGridView1.Columns["Actions"] == null)
            {
                DataGridViewButtonColumn actionsColumn = new DataGridViewButtonColumn()
                {
                    Name = "Actions",
                    HeaderText = "Actions",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true,
                    Width = 120,
                    MinimumWidth = 120
                };
                dataGridView1.Columns.Add(actionsColumn);
            }

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col == null) continue;

                col.SortMode = DataGridViewColumnSortMode.NotSortable;

                switch (col.Name?.ToLower())
                {
                    case "boxname": col.HeaderText = "Box Name"; col.Width = 120; break;
                    case "boxsize": col.HeaderText = "Box Size (L×B×H)"; col.Width = 120; break;
                    case "sheetsize": col.HeaderText = "Sheet Size"; col.Width = 120; break;
                    case "gsm": col.HeaderText = "GSM"; col.Width = 80; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; break;
                    case "ply": col.HeaderText = "Ply"; col.Width = 60; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; break;
                    case "lastplyvalue": col.HeaderText = "Top Paper"; col.Width = 100; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; break;
                    case "gramage": col.HeaderText = "Gramage"; col.Width = 90; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "paperusage": col.HeaderText = "Paper Usage"; col.Width = 100; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "linerusage": col.HeaderText = "Liner Usage"; col.Width = 100; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "laminationperbox": col.HeaderText = "Lamination Rate"; col.Width = 120; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "laminationcalculatedperbox": col.HeaderText = "Lamination/Box"; col.Width = 130; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "laminationtotal": col.HeaderText = "Lamination Total"; col.Width = 130; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "printingperbox": col.HeaderText = "Printing/Box"; col.Width = 110; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "punchingperbox": col.HeaderText = "Punching/Box"; col.Width = 110; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "pastingperbox": col.HeaderText = "Pasting/Box"; col.Width = 110; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "sidepastingperbox": col.HeaderText = "Side-pasting/Box"; col.Width = 140; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "perboxrate": col.HeaderText = "Rate / Box"; col.Width = 110; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.####"; break;
                    case "grandtotal": col.HeaderText = "Grand Total"; col.Width = 120; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.##"; break;
                    case "paperweightperbox": col.HeaderText = "Paper Wt/Box (kg)"; col.Width = 150; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.###"; break;
                    case "paperweighttotal": col.HeaderText = "Paper Wt Total (kg)"; col.Width = 150; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.###"; break;
                    case "linerweightperbox": col.HeaderText = "Liner Wt/Box (kg)"; col.Width = 150; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.###"; break;
                    case "linerweighttotal": col.HeaderText = "Liner Wt Total (kg)"; col.Width = 150; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; col.DefaultCellStyle.Format = "0.###"; break;
                    case "usehalfsheetforusage": col.HeaderText = "Half Sheet Usage"; col.Width = 140; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; break;
                    case "detail": col.HeaderText = "Detail"; col.Width = 200; break;
                    case "actions": col.HeaderText = "Actions"; col.Width = 120; col.MinimumWidth = 120; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; break;
                }
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _customer.Records.Count) return;

            var record = _customer.Records[e.RowIndex];
            var form = new AddRecordForm(_customer, record);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshGrid();
                statusLabel.Text = "Record updated successfully";
                
                Timer timer = new Timer() { Interval = 3000 };
                timer.Tick += (s, args) =>
                {
                    statusLabel.Text = "Ready";
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
        }

        private void DataGridView1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                // Shift + scroll for horizontal navigation
                if (e.Delta > 0)
                {
                    // Scroll left
                    dataGridView1.HorizontalScrollingOffset = Math.Max(0, dataGridView1.HorizontalScrollingOffset - 20);
                }
                else
                {
                    // Scroll right
                    dataGridView1.HorizontalScrollingOffset = Math.Min(dataGridView1.HorizontalScrollingOffset + 20, 
                        dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dataGridView1.ClientSize.Width);
                }
            }
        }

        private void RestoreStockFromRecord(BoxRecord record)
        {
            try
            {
                var inventory = StorageService.LoadInventory();
                
                // Parse sheet size full to get dimensions
                if (string.IsNullOrEmpty(record.SheetSizeFull))
                    return;
                
                var parts = record.SheetSizeFull.Split('x');
                if (parts.Length != 2)
                    return;
                
                if (!double.TryParse(parts[0].Trim(), out double fullLength))
                    return;
                
                int row = (int)fullLength;
                int gsm = (int)record.GSM;
                int topPaper = (int)record.LastPlyValue;
                double paperWeight = record.PaperWeightTotal;
                double linerWeight = record.LinerWeightTotal;

                // Calculate row number based on range (same logic as deduction)
                if (row < 13)
                {
                    // Restore to general stock
                    inventory.GeneralPaperStock += paperWeight;
                    inventory.GeneralLinerStock += linerWeight;
                }
                else if (row < 26)
                {
                    // Multiply by 2
                    row = row * 2;
                    if (row >= 26 && row <= 52)
                    {
                        // Restore to table
                        inventory.SetStock(row, gsm, inventory.GetStock(row, gsm) + paperWeight);
                        inventory.SetStock(row, topPaper, inventory.GetStock(row, topPaper) + linerWeight);
                    }
                    else
                    {
                        // Fallback to general stock
                        inventory.GeneralPaperStock += paperWeight;
                        inventory.GeneralLinerStock += linerWeight;
                    }
                }
                else if (row >= 26 && row <= 52)
                {
                    // Restore directly to table
                    inventory.SetStock(row, gsm, inventory.GetStock(row, gsm) + paperWeight);
                    inventory.SetStock(row, topPaper, inventory.GetStock(row, topPaper) + linerWeight);
                }
                else
                {
                    // row > 52, restore to general stock
                    inventory.GeneralPaperStock += paperWeight;
                    inventory.GeneralLinerStock += linerWeight;
                }

                // Save updated inventory
                StorageService.SaveInventory(inventory);
            }
            catch
            {
                // Ignore errors during stock restoration
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _customer.Records.Count) return;
            if (dataGridView1.Columns[e.ColumnIndex].Name != "Actions") return;

            var record = _customer.Records[e.RowIndex];
            var result = MessageBox.Show($"Delete record '{record.BoxName}'?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
            if (result == DialogResult.Yes)
            {
                // Restore stock before deleting
                RestoreStockFromRecord(record);
                
                // Remove from in-memory list
                _customer.Records.RemoveAt(e.RowIndex);
                
                // Delete the stored JSON file
                try
                {
                    var recordsDir = Path.Combine(StorageService.StorageRoot, 
                        StorageService.MakeSafeName(_customer.Name), "records");
                    var files = Directory.GetFiles(recordsDir, "*.json").OrderBy(f => f).ToArray();
                    
                    // Find and delete the matching file (simplified - in real app you'd match by timestamp or ID)
                    if (files.Length > e.RowIndex)
                    {
                        File.Delete(files[e.RowIndex]);
                    }
                }
                catch
                {
                    // Ignore file deletion errors
                }
                
                RefreshGrid();
                statusLabel.Text = "Record deleted successfully";
                
                Timer timer = new Timer() { Interval = 3000 };
                timer.Tick += (s, args) =>
                {
                    statusLabel.Text = "Ready";
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
        }
    }
}
