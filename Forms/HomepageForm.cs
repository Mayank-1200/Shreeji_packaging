using System;
using System.Drawing;
using System.Windows.Forms;

namespace shreeji_packaging.Forms
{
    public class HomepageForm : Form
    {
        public string SelectedOption { get; private set; }

        public HomepageForm()
        {
            this.Text = "Shreeji Packaging - Home";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.MaximizeBox = false;

            // Title
            Label titleLabel = new Label()
            {
                Text = "📦 Shreeji Packaging",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(150, 50)
            };
            this.Controls.Add(titleLabel);

            // Subtitle
            Label subtitleLabel = new Label()
            {
                Text = "Select an option to continue",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(180, 100)
            };
            this.Controls.Add(subtitleLabel);

            // Inventory Button
            Button btnInventory = new Button()
            {
                Text = "📊 Inventory",
                Size = new Size(200, 80),
                Location = new Point(50, 180),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnInventory.FlatAppearance.BorderSize = 0;
            btnInventory.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnInventory.Click += (s, e) =>
            {
                SelectedOption = "Inventory";
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnInventory);

            // Customers Button
            Button btnCustomers = new Button()
            {
                Text = "👥 Customers",
                Size = new Size(200, 80),
                Location = new Point(350, 180),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCustomers.FlatAppearance.BorderSize = 0;
            btnCustomers.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnCustomers.Click += (s, e) =>
            {
                SelectedOption = "Customers";
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnCustomers);
        }
    }
}

