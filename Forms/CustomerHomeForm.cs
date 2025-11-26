using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using shreeji_packaging.Services;

namespace shreeji_packaging.Forms
{
    public class CustomerHomeForm : Form
    {
        private ListBox lstCustomers;
        private Button btnAdd, btnDelete, btnOpen;
        private TextBox txtNewCustomer;

        public string SelectedCustomer { get; private set; }

        public CustomerHomeForm()
        {
            this.Text = "Select Customer";
            this.Size = new Size(420, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.MaximizeBox = false;

            lstCustomers = new ListBox()
            {
                Left = 20,
                Top = 20,
                Width = 370,
                Height = 260,
                Font = new Font("Segoe UI", 10)
            };

            txtNewCustomer = new TextBox()
            {
                Left = 20,
                Top = 290,
                Width = 220,
                Height = 28,
                Font = new Font("Segoe UI", 10)
            };

            btnAdd = new Button()
            {
                Text = "Add",
                Left = 250,
                Top = 288,
                Width = 60,
                Height = 32,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                var name = (txtNewCustomer.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name)) return;
                StorageService.CreateCustomer(name);
                txtNewCustomer.Text = string.Empty;
                LoadCustomers();
            };

            btnDelete = new Button()
            {
                Text = "Delete",
                Left = 315,
                Top = 288,
                Width = 75,
                Height = 32,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, e) =>
            {
                if (lstCustomers.SelectedItem == null) return;
                var name = lstCustomers.SelectedItem.ToString();
                var confirm = MessageBox.Show($"Delete customer '{name}' and all records?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    StorageService.DeleteCustomer(name);
                    LoadCustomers();
                }
            };

            btnOpen = new Button()
            {
                Text = "Open",
                Left = 20,
                Top = 330,
                Width = 370,
                Height = 40,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnOpen.FlatAppearance.BorderSize = 0;
            btnOpen.Click += (s, e) =>
            {
                if (lstCustomers.SelectedItem == null) return;
                SelectedCustomer = lstCustomers.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(lstCustomers);
            this.Controls.Add(txtNewCustomer);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnOpen);

            LoadCustomers();
        }

        private void LoadCustomers()
        {
            var customers = StorageService.ListCustomers();
            lstCustomers.Items.Clear();
            if (customers != null && customers.Length > 0)
            {
                lstCustomers.Items.AddRange(customers.Cast<object>().ToArray());
            }
        }
    }
}


