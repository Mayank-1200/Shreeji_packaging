using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using shreeji_packaging.Models;
using shreeji_packaging.Services;

namespace shreeji_packaging.Forms
{
    public class PartyDetailsForm : Form
    {
        private Customer _customer;
        private DataGridView dataGridView1;
        private TextBox txtAddress;
        private Button btnSaveAddress;

        public PartyDetailsForm(Customer customer)
        {
            _customer = customer;
            this.Text = $"Party Details - {customer.Name}";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.MinimumSize = new Size(1200, 700);

            InitializeComponents();
            LoadData();
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
                Text = "📋 Party Details",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            Button btnClose = new Button()
            {
                Text = "✕ Close",
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            btnClose.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, btnClose });
            btnClose.Location = new Point(headerPanel.Width - btnClose.Width - 30, 22);
            this.Controls.Add(headerPanel);

            // Main Content Panel
            Panel contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 30),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Customer Info Panel (Top Left)
            Panel customerInfoPanel = new Panel()
            {
                Location = new Point(30, 20),
                Size = new Size(400, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            Label lblCustomerName = new Label()
            {
                Text = "Customer Name:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            Label lblCustomerNameValue = new Label()
            {
                Text = _customer.Name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                AutoSize = true,
                Location = new Point(15, 40)
            };

            Label lblAddress = new Label()
            {
                Text = "Address:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 75)
            };

            txtAddress = new TextBox()
            {
                Location = new Point(15, 100),
                Size = new Size(370, 60),
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                ScrollBars = ScrollBars.Vertical
            };

            btnSaveAddress = new Button()
            {
                Text = "💾 Save Address",
                Size = new Size(150, 35),
                Location = new Point(15, 165),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveAddress.FlatAppearance.BorderSize = 0;
            btnSaveAddress.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnSaveAddress.Click += BtnSaveAddress_Click;

            customerInfoPanel.Controls.AddRange(new Control[] {
                lblCustomerName, lblCustomerNameValue, lblAddress, txtAddress, btnSaveAddress
            });

            contentPanel.Controls.Add(customerInfoPanel);

            // Table Section
            Label lblTableHeader = new Label()
            {
                Text = "Box Records",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(30, 240)
            };
            contentPanel.Controls.Add(lblTableHeader);

            // DataGridView
            dataGridView1 = new DataGridView()
            {
                Location = new Point(30, 275),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(223, 230, 233),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 45,
                ScrollBars = ScrollBars.Both,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // Header Style
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.EnableHeadersVisualStyles = false;

            // Row Styles - Increased spacing
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            dataGridView1.DefaultCellStyle.Padding = new Padding(12, 10, 12, 10); // Increased padding
            dataGridView1.RowTemplate.Height = 45; // Increased row height
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 252);
            dataGridView1.AlternatingRowsDefaultCellStyle.Padding = new Padding(12, 10, 12, 10);

            // Add Images column
            DataGridViewButtonColumn imagesColumn = new DataGridViewButtonColumn()
            {
                Name = "Images",
                HeaderText = "Images",
                Text = "📷 Manage Images",
                UseColumnTextForButtonValue = true,
                Width = 180,
                MinimumWidth = 180
            };
            dataGridView1.Columns.Add(imagesColumn);

            dataGridView1.CellClick += DataGridView1_CellClick;

            contentPanel.Controls.Add(dataGridView1);

            // Handle resize
            contentPanel.Resize += (s, e) =>
            {
                int tableHeight = contentPanel.Height - 275 - 30; // 275 = top position, 30 = bottom margin
                dataGridView1.Size = new Size(contentPanel.Width - 60, Math.Max(200, tableHeight));
            };

            this.Controls.Add(contentPanel);
        }

        private void LoadData()
        {
            // Load customer address
            _customer.Address = StorageService.LoadCustomerAddress(_customer.Name);
            txtAddress.Text = _customer.Address ?? "";

            // Load records
            var records = StorageService.LoadRecords(_customer.Name) ?? _customer.Records;
            _customer.Records = records;

            // Bind to DataGridView with only specified columns
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Add Images column first
            DataGridViewButtonColumn imagesColumn = new DataGridViewButtonColumn()
            {
                Name = "Images",
                HeaderText = "Images",
                Text = "📷 Manage Images",
                UseColumnTextForButtonValue = true,
                Width = 180,
                MinimumWidth = 180
            };
            dataGridView1.Columns.Add(imagesColumn);

            // Add only the specified columns - Increased widths for better spacing
            AddColumn("BoxName", "Box Name", 180);
            AddColumn("BoxSize", "Box Size", 180);
            AddColumn("GSM", "GSM 1", 100);
            AddColumn("GSM2", "GSM 2", 100);
            AddColumn("Ply", "Ply", 80);
            AddColumn("LastPlyValue", "Top Paper", 120);
            AddColumn("PerBoxRate", "Rate/Box", 130);
            AddColumn("LaminationCalculatedPerBox", "Lamination/box", 150);
            AddColumn("PrintingPerBox", "Printing", 120);
            AddColumn("PunchingPerBox", "Punching", 120);
            AddColumn("PastingPerBox", "Pasting", 120);
            AddColumn("SidePastingPerBox", "Side Pasting", 140);

            // Populate rows
            foreach (var record in records)
            {
                int rowIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowIndex].Tag = record; // Store record reference
                dataGridView1.Rows[rowIndex].Cells["BoxName"].Value = record.BoxName;
                dataGridView1.Rows[rowIndex].Cells["BoxSize"].Value = record.BoxSize;
                dataGridView1.Rows[rowIndex].Cells["GSM"].Value = record.GSM;
                dataGridView1.Rows[rowIndex].Cells["GSM2"].Value = record.GSM2 > 0 ? record.GSM2.ToString() : "";
                dataGridView1.Rows[rowIndex].Cells["Ply"].Value = record.Ply;
                dataGridView1.Rows[rowIndex].Cells["LastPlyValue"].Value = record.LastPlyValue;
                dataGridView1.Rows[rowIndex].Cells["PerBoxRate"].Value = record.PerBoxRate.ToString("0.####");
                dataGridView1.Rows[rowIndex].Cells["LaminationCalculatedPerBox"].Value = record.LaminationCalculatedPerBox.ToString("0.##");
                dataGridView1.Rows[rowIndex].Cells["PrintingPerBox"].Value = record.PrintingPerBox.ToString("0.##");
                dataGridView1.Rows[rowIndex].Cells["PunchingPerBox"].Value = record.PunchingPerBox.ToString("0.##");
                dataGridView1.Rows[rowIndex].Cells["PastingPerBox"].Value = record.PastingPerBox.ToString("0.##");
                dataGridView1.Rows[rowIndex].Cells["SidePastingPerBox"].Value = record.SidePastingPerBox.ToString("0.##");
            }

            // Set column alignment
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Name != "BoxName" && col.Name != "BoxSize" && col.Name != "Images")
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        private void AddColumn(string name, string headerText, int width)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn()
            {
                Name = name,
                HeaderText = headerText,
                Width = width,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Padding = new Padding(12, 10, 12, 10),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
            dataGridView1.Columns.Add(col);
        }

        private void BtnSaveAddress_Click(object sender, EventArgs e)
        {
            try
            {
                _customer.Address = txtAddress.Text;
                StorageService.SaveCustomerAddress(_customer.Name, _customer.Address);
                MessageBox.Show("Address saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving address: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _customer.Records.Count) return;
            if (dataGridView1.Columns[e.ColumnIndex].Name != "Images") return;

            var record = dataGridView1.Rows[e.RowIndex].Tag as BoxRecord;
            if (record == null) return;

            // Open image management dialog
            using (var imageDialog = new ImageManagementDialog(_customer.Name, record))
            {
                if (imageDialog.ShowDialog() == DialogResult.OK)
                {
                    // Reload data to refresh image counts if needed
                    LoadData();
                }
            }
        }
    }
}

