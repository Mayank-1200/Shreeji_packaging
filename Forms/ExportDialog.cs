using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using shreeji_packaging.Models;
using shreeji_packaging.Services;

namespace shreeji_packaging.Forms
{
    public class ExportDialog : Form
    {
        private TreeView treeViewRecords;
        private Label lblSelectedCount;
        private Button btnExport, btnCancel, btnSelectAll, btnDeselectAll;
        private Dictionary<string, List<BoxRecord>> customerRecords;
        private string currentCustomerName;

        public List<(string CustomerName, BoxRecord Record)> SelectedRecords { get; private set; }

        public ExportDialog(string currentCustomer, List<BoxRecord> currentCustomerRecords)
        {
            this.Text = "Export Records to PDF";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.MaximizeBox = false;

            currentCustomerName = currentCustomer;
            SelectedRecords = new List<(string, BoxRecord)>();

            InitializeComponents();
            LoadAllRecords();
            PopulateTreeView();
        }

        private void InitializeComponents()
        {
            // Header Label
            Label lblHeader = new Label()
            {
                Text = "Select Records to Export",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            this.Controls.Add(lblHeader);

            // Instructions Label
            Label lblInstructions = new Label()
            {
                Text = "Select records from one or more customers. All selected records will be exported to a single PDF.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(20, 45)
            };
            this.Controls.Add(lblInstructions);

            // TreeView for records
            treeViewRecords = new TreeView()
            {
                Location = new Point(20, 75),
                Size = new Size(950, 450),
                CheckBoxes = true,
                Font = new Font("Segoe UI", 10),
                FullRowSelect = true,
                HideSelection = false
            };
            treeViewRecords.AfterCheck += TreeViewRecords_AfterCheck;
            this.Controls.Add(treeViewRecords);

            // Selected Count Label
            lblSelectedCount = new Label()
            {
                Text = "Selected: 0 records",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                AutoSize = true,
                Location = new Point(20, 535)
            };
            this.Controls.Add(lblSelectedCount);

            // Select All Button
            btnSelectAll = new Button()
            {
                Text = "Select All",
                Size = new Size(100, 35),
                Location = new Point(200, 530),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSelectAll.FlatAppearance.BorderSize = 0;
            btnSelectAll.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnSelectAll.Click += (s, e) => SelectAllNodes(true);
            this.Controls.Add(btnSelectAll);

            // Deselect All Button
            btnDeselectAll = new Button()
            {
                Text = "Deselect All",
                Size = new Size(100, 35),
                Location = new Point(310, 530),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDeselectAll.FlatAppearance.BorderSize = 0;
            btnDeselectAll.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 140, 141);
            btnDeselectAll.Click += (s, e) => SelectAllNodes(false);
            this.Controls.Add(btnDeselectAll);

            // Export Button
            btnExport = new Button()
            {
                Text = "📄 Export to PDF",
                Size = new Size(150, 40),
                Location = new Point(700, 530),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnExport.Click += BtnExport_Click;
            this.Controls.Add(btnExport);

            // Cancel Button
            btnCancel = new Button()
            {
                Text = "Cancel",
                Size = new Size(120, 40),
                Location = new Point(870, 530),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 140, 141);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);
        }

        private void LoadAllRecords()
        {
            customerRecords = new Dictionary<string, List<BoxRecord>>();
            string[] customers = StorageService.ListCustomers();

            foreach (string customer in customers)
            {
                var records = StorageService.LoadRecords(customer);
                if (records != null && records.Count > 0)
                {
                    customerRecords[customer] = records;
                }
            }
        }

        private void PopulateTreeView()
        {
            treeViewRecords.Nodes.Clear();

            // Add current customer first (expanded)
            if (customerRecords.ContainsKey(currentCustomerName))
            {
                TreeNode currentCustomerNode = new TreeNode(currentCustomerName)
                {
                    Tag = currentCustomerName,
                    Checked = false
                };

                foreach (var record in customerRecords[currentCustomerName])
                {
                    TreeNode recordNode = new TreeNode($"{record.BoxName} ({record.BoxSize})")
                    {
                        Tag = (currentCustomerName, record),
                        Checked = false
                    };
                    currentCustomerNode.Nodes.Add(recordNode);
                }

                currentCustomerNode.Expand();
                treeViewRecords.Nodes.Add(currentCustomerNode);
            }

            // Add other customers
            foreach (var kvp in customerRecords)
            {
                if (kvp.Key == currentCustomerName) continue;

                TreeNode customerNode = new TreeNode(kvp.Key)
                {
                    Tag = kvp.Key,
                    Checked = false
                };

                foreach (var record in kvp.Value)
                {
                    TreeNode recordNode = new TreeNode($"{record.BoxName} ({record.BoxSize})")
                    {
                        Tag = (kvp.Key, record),
                        Checked = false
                    };
                    customerNode.Nodes.Add(recordNode);
                }

                treeViewRecords.Nodes.Add(customerNode);
            }

            UpdateSelectedCount();
        }

        private void TreeViewRecords_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Prevent recursive calls
            treeViewRecords.AfterCheck -= TreeViewRecords_AfterCheck;

            // If a customer node is checked/unchecked, check/uncheck all its children
            if (e.Node.Tag is string customerName)
            {
                foreach (TreeNode child in e.Node.Nodes)
                {
                    child.Checked = e.Node.Checked;
                }
            }
            // If a record node is checked/unchecked, update parent
            else if (e.Node.Tag is (string, BoxRecord))
            {
                TreeNode parent = e.Node.Parent;
                if (parent != null)
                {
                    bool allChecked = true;
                    foreach (TreeNode sibling in parent.Nodes)
                    {
                        if (!sibling.Checked)
                        {
                            allChecked = false;
                            break;
                        }
                    }
                    parent.Checked = allChecked;
                }
            }

            treeViewRecords.AfterCheck += TreeViewRecords_AfterCheck;
            UpdateSelectedCount();
        }

        private void SelectAllNodes(bool check)
        {
            treeViewRecords.AfterCheck -= TreeViewRecords_AfterCheck;

            foreach (TreeNode customerNode in treeViewRecords.Nodes)
            {
                customerNode.Checked = check;
                foreach (TreeNode recordNode in customerNode.Nodes)
                {
                    recordNode.Checked = check;
                }
            }

            treeViewRecords.AfterCheck += TreeViewRecords_AfterCheck;
            UpdateSelectedCount();
        }

        private void UpdateSelectedCount()
        {
            SelectedRecords.Clear();

            foreach (TreeNode customerNode in treeViewRecords.Nodes)
            {
                foreach (TreeNode recordNode in customerNode.Nodes)
                {
                    if (recordNode.Checked && recordNode.Tag is (string, BoxRecord))
                    {
                        var (customerName, record) = ((string, BoxRecord))recordNode.Tag;
                        SelectedRecords.Add((customerName, record));
                    }
                }
            }

            lblSelectedCount.Text = $"Selected: {SelectedRecords.Count} record(s)";
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (SelectedRecords.Count == 0)
            {
                MessageBox.Show("Please select at least one record to export.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveDialog.FileName = $"Export_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf";
                saveDialog.Title = "Save PDF As";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        PdfExportService.ExportToPdf(SelectedRecords, saveDialog.FileName);
                        MessageBox.Show($"PDF exported successfully!\n\nFile: {saveDialog.FileName}", 
                            "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting PDF:\n{ex.Message}", "Export Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

