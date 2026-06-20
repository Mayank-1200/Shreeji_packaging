using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using shreeji_packaging.Models;
using shreeji_packaging.Services;

namespace shreeji_packaging.Forms
{
    public class AddRecordForm : Form
    {
        private Customer _customer;
        private BoxRecord _editingRecord;
        private TextBox txtBoxName, txtLength, txtBreadth, txtHeight, txtGSM, txtGSM2, txtPly, txtLastPly, txtDetail;
        private TextBox txtSheetSize, txtSheetSizeFull, txtGramage, txtPaper, txtLiner;
        private TextBox txtSellRate, txtNumBoxes, txtPerBoxRate, txtGrandTotal;
        private TextBox txtLamination, txtPrinting, txtPunching, txtPasting, txtSidePasting;
        private TextBox txtLaminationCalculated, txtLaminationTotal;
        private TextBox txtPaperWeightPerBox, txtPaperWeightTotal, txtLinerWeightPerBox, txtLinerWeightTotal;
        private TextBox txtLiner1WeightPerBox, txtLiner1WeightTotal, txtLiner2WeightPerBox, txtLiner2WeightTotal;
        private CheckBox chkHalfSheet;
        private CheckBox chkUseSheetSize;
        private Button btnSave;

        public AddRecordForm(Customer customer) : this(customer, null)
        {
        }

        private double ParseOptional(string input)
        {
            if (double.TryParse(input, out double value))
                return Math.Max(0, value);
            return 0;
        }

        private string FormatWeight(double value)
        {
            if (value >= 1)
                return Math.Round(value, 0).ToString("0");
            if (value <= 0)
                return "0";
            return value.ToString("0.###");
        }

        private void ClearCalculatedFields()
        {
            txtSheetSize.Text = "";
            if (!chkUseSheetSize.Checked)
                txtSheetSizeFull.Text = "";
            txtGramage.Text = "";
            txtPaper.Text = "";
            txtLiner.Text = "";
            txtPerBoxRate.Text = "";
            txtGrandTotal.Text = "";
            txtLaminationCalculated.Text = "";
            txtLaminationTotal.Text = "";
            txtPaperWeightPerBox.Text = "";
            txtPaperWeightTotal.Text = "";
            txtLinerWeightPerBox.Text = "";
            txtLinerWeightTotal.Text = "";
            txtLiner1WeightPerBox.Text = "";
            txtLiner1WeightTotal.Text = "";
            txtLiner2WeightPerBox.Text = "";
            txtLiner2WeightTotal.Text = "";
        }

        public AddRecordForm(Customer customer, BoxRecord existingRecord)
        {
            _customer = customer;
            _editingRecord = existingRecord;
            this.Text = existingRecord == null ? "Add Box Record - Shreeji Packaging" : "Edit Box Record - Shreeji Packaging";
            this.Size = new Size(780, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.MaximizeBox = false;
            this.AutoScroll = true;

            Font labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Font inputFont = new Font("Segoe UI", 10, FontStyle.Regular);
            Font sectionFont = new Font("Segoe UI", 12, FontStyle.Bold);

            // Title Label
            Label titleLabel = new Label()
            {
                Text = "Box Record Information",
                Left = 30,
                Top = 15,
                Width = 300,
                Height = 30,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            this.Controls.Add(titleLabel);

            // Input Section Header
            Label inputHeader = new Label()
            {
                Text = "Box Details",
                Left = 30,
                Top = 55,
                Width = 150,
                Height = 25,
                Font = sectionFont,
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            this.Controls.Add(inputHeader);

            // Input Panel
            Panel inputPanel = new Panel()
            {
                Left = 30,
                Top = 85,
                Width = 700,
                Height = 320,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Row 1 - Box Name
            Label lblBoxName = new Label() { Text = "Box Name:", Left = 20, Top = 25, Width = 100, Height = 20, Font = labelFont };
            txtBoxName = new TextBox() { Left = 130, Top = 22, Width = 470, Height = 25, Font = inputFont };

            // Row 2 - Dimensions
            Label lblDimensions = new Label() { Text = "Dimensions (L × B × H):", Left = 20, Top = 65, Width = 180, Height = 20, Font = labelFont };

            Label lblLength = new Label() { Text = "Length:", Left = 20, Top = 95, Width = 60, Height = 20, Font = labelFont };
            txtLength = new TextBox() { Left = 85, Top = 92, Width = 80, Height = 25, Font = inputFont };

            Label lblBreadth = new Label() { Text = "Breadth:", Left = 190, Top = 95, Width = 70, Height = 20, Font = labelFont };
            txtBreadth = new TextBox() { Left = 265, Top = 92, Width = 80, Height = 25, Font = inputFont };

            Label lblHeight = new Label() { Text = "Height:", Left = 370, Top = 95, Width = 60, Height = 20, Font = labelFont };
            txtHeight = new TextBox() { Left = 435, Top = 92, Width = 80, Height = 25, Font = inputFont };

            // Row 3 - Material Properties
            Label lblMaterial = new Label() { Text = "Material Properties:", Left = 20, Top = 135, Width = 150, Height = 20, Font = labelFont };

            Label lblGSM = new Label() { Text = "GSM 1:", Left = 20, Top = 165, Width = 60, Height = 20, Font = labelFont };
            txtGSM = new TextBox() { Left = 85, Top = 162, Width = 80, Height = 25, Font = inputFont };

            Label lblGSM2 = new Label() { Text = "GSM 2 (opt.):", Left = 180, Top = 165, Width = 100, Height = 20, Font = labelFont };
            txtGSM2 = new TextBox() { Left = 285, Top = 162, Width = 80, Height = 25, Font = inputFont };

            Label lblPly = new Label() { Text = "Ply (Odd):", Left = 380, Top = 165, Width = 80, Height = 20, Font = labelFont };
            txtPly = new TextBox() { Left = 465, Top = 162, Width = 60, Height = 25, Font = inputFont };

            Label lblLastPly = new Label() { Text = "Top Paper:", Left = 540, Top = 165, Width = 100, Height = 20, Font = labelFont };
            txtLastPly = new TextBox() { Left = 645, Top = 162, Width = 80, Height = 25, Font = inputFont };

            // Row 4 - Detail
            Label lblDetail = new Label() { Text = "Detail:", Left = 20, Top = 205, Width = 60, Height = 20, Font = labelFont };
            txtDetail = new TextBox() { Left = 85, Top = 202, Width = 510, Height = 25, Font = inputFont };

            inputPanel.Controls.AddRange(new Control[] {
                lblBoxName, txtBoxName,
                lblDimensions, lblLength, txtLength, lblBreadth, txtBreadth, lblHeight, txtHeight,
                lblMaterial, lblGSM, txtGSM, lblGSM2, txtGSM2, lblPly, txtPly, lblLastPly, txtLastPly,
                lblDetail, txtDetail
            });
            this.Controls.Add(inputPanel);

            // Results Section Header
            Label resultsHeader = new Label()
            {
                Text = "Calculated Results",
                Left = 30,
                Top = 420,
                Width = 200,
                Height = 25,
                Font = sectionFont,
                ForeColor = Color.FromArgb(52, 152, 219)
            };
            this.Controls.Add(resultsHeader);

            // Results Panel
            Panel resultPanel = new Panel()
            {
                Left = 30,
                Top = 460,
                Width = 720,
                Height = 400,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            // Row 5 - Sheet Size Half (in Box Details)
            Label lblSheetSize = new Label() { Text = "Sheet Size (Half):", Left = 20, Top = 240, Width = 150, Height = 20, Font = labelFont };
            txtSheetSize = new TextBox() { Left = 180, Top = 237, Width = 290, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            // Row 6 - Sheet Size Full + Use Sheet Size Directly checkbox (in Box Details)
            Label lblSheetSizeFull = new Label() { Text = "Sheet Size (Full):", Left = 20, Top = 275, Width = 150, Height = 20, Font = labelFont };
            txtSheetSizeFull = new TextBox() { Left = 180, Top = 272, Width = 290, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            chkUseSheetSize = new CheckBox()
            {
                Text = "Use Sheet Size Directly",
                Left = 480,
                Top = 275,
                Width = 200,
                Height = 20,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            chkUseSheetSize.CheckedChanged += ChkUseSheetSize_CheckedChanged;

            chkHalfSheet = new CheckBox()
            {
                Text = "Calculate usage on half sheet",
                Left = 480,
                Top = 240,
                Width = 240,
                Height = 20,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            // Row 1 - Calculations
            Label lblGramage = new Label() { Text = "Gramage:", Left = 20, Top = 25, Width = 80, Height = 20, Font = labelFont };
            txtGramage = new TextBox() { Left = 110, Top = 22, Width = 100, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            Label lblPaper = new Label() { Text = "Paper (total):", Left = 230, Top = 25, Width = 110, Height = 20, Font = labelFont };
            txtPaper = new TextBox() { Left = 350, Top = 22, Width = 110, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            Label lblLiner = new Label() { Text = "Liner (total):", Left = 480, Top = 25, Width = 110, Height = 20, Font = labelFont };
            txtLiner = new TextBox() { Left = 600, Top = 22, Width = 90, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            // Row - Pricing Inputs
            Label lblSellRate = new Label() { Text = "Sell Rate:", Left = 20, Top = 60, Width = 90, Height = 20, Font = labelFont };
            txtSellRate = new TextBox() { Left = 110, Top = 57, Width = 110, Height = 25, Font = inputFont };

            Label lblNumBoxes = new Label() { Text = "Box Quantity:", Left = 230, Top = 60, Width = 120, Height = 20, Font = labelFont };
            txtNumBoxes = new TextBox() { Left = 360, Top = 57, Width = 90, Height = 25, Font = inputFont };

            // Lamination row
            Label lblLamination = new Label() { Text = "Lamination rate:", Left = 20, Top = 95, Width = 130, Height = 20, Font = labelFont };
            txtLamination = new TextBox() { Left = 160, Top = 92, Width = 90, Height = 25, Font = inputFont };

            Label lblLaminationCalculated = new Label() { Text = "Lamination/box:", Left = 270, Top = 95, Width = 130, Height = 20, Font = labelFont };
            txtLaminationCalculated = new TextBox() { Left = 410, Top = 92, Width = 100, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            // Lamination Total on separate row
            Label lblLaminationTotal = new Label() { Text = "Lamination Total:", Left = 20, Top = 130, Width = 130, Height = 20, Font = labelFont };
            txtLaminationTotal = new TextBox() { Left = 160, Top = 127, Width = 140, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            // Extras row
            Label lblPrinting = new Label() { Text = "Printing/box:", Left = 20, Top = 170, Width = 110, Height = 20, Font = labelFont };
            txtPrinting = new TextBox() { Left = 130, Top = 167, Width = 90, Height = 25, Font = inputFont };

            Label lblPunching = new Label() { Text = "Punching/box:", Left = 230, Top = 170, Width = 120, Height = 20, Font = labelFont };
            txtPunching = new TextBox() { Left = 350, Top = 167, Width = 90, Height = 25, Font = inputFont };

            Label lblPasting = new Label() { Text = "Pasting/box:", Left = 450, Top = 170, Width = 110, Height = 20, Font = labelFont };
            txtPasting = new TextBox() { Left = 560, Top = 167, Width = 90, Height = 25, Font = inputFont };

            Label lblSidePasting = new Label() { Text = "Side-pasting/box:", Left = 20, Top = 210, Width = 140, Height = 20, Font = labelFont };
            txtSidePasting = new TextBox() { Left = 170, Top = 207, Width = 90, Height = 25, Font = inputFont };

            // Pricing outputs
            Label lblPerBoxRate = new Label() { Text = "Rate / Box:", Left = 280, Top = 210, Width = 100, Height = 20, Font = labelFont };
            txtPerBoxRate = new TextBox() { Left = 380, Top = 207, Width = 110, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            Label lblGrandTotal = new Label() { Text = "Grand Total:", Left = 500, Top = 210, Width = 110, Height = 20, Font = labelFont };
            txtGrandTotal = new TextBox() { Left = 610, Top = 207, Width = 90, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            // Weight outputs
            Label lblPaperWeightPerBox = new Label() { Text = "Paper weight/box (kg):", Left = 20, Top = 250, Width = 180, Height = 20, Font = labelFont };
            txtPaperWeightPerBox = new TextBox() { Left = 210, Top = 247, Width = 100, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            Label lblPaperWeightTotal = new Label() { Text = "Paper weight total (kg):", Left = 330, Top = 250, Width = 190, Height = 20, Font = labelFont };
            txtPaperWeightTotal = new TextBox() { Left = 530, Top = 247, Width = 110, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            Label lblLinerWeightPerBox = new Label() { Text = "Liner weight/box (kg):", Left = 20, Top = 290, Width = 180, Height = 20, Font = labelFont };
            txtLinerWeightPerBox = new TextBox() { Left = 210, Top = 287, Width = 100, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            Label lblLinerWeightTotal = new Label() { Text = "Liner weight total (kg):", Left = 330, Top = 290, Width = 190, Height = 20, Font = labelFont };
            txtLinerWeightTotal = new TextBox() { Left = 530, Top = 287, Width = 110, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            // Detailed liner weights per GSM
            Label lblLiner1WeightPerBox = new Label() { Text = "Liner 1 wt/box (kg):", Left = 20, Top = 325, Width = 180, Height = 20, Font = labelFont };
            txtLiner1WeightPerBox = new TextBox() { Left = 210, Top = 322, Width = 100, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            Label lblLiner1WeightTotal = new Label() { Text = "Liner 1 total (kg):", Left = 330, Top = 325, Width = 190, Height = 20, Font = labelFont };
            txtLiner1WeightTotal = new TextBox() { Left = 530, Top = 322, Width = 110, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            Label lblLiner2WeightPerBox = new Label() { Text = "Liner 2 wt/box (kg):", Left = 20, Top = 360, Width = 180, Height = 20, Font = labelFont };
            txtLiner2WeightPerBox = new TextBox() { Left = 210, Top = 357, Width = 100, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };

            Label lblLiner2WeightTotal = new Label() { Text = "Liner 2 total (kg):", Left = 330, Top = 360, Width = 190, Height = 20, Font = labelFont };
            txtLiner2WeightTotal = new TextBox() { Left = 530, Top = 357, Width = 110, Height = 25, Font = inputFont, ReadOnly = true, BackColor = Color.FromArgb(236, 240, 241) };


            inputPanel.Controls.AddRange(new Control[] {
                lblSheetSize, txtSheetSize,
                lblSheetSizeFull, txtSheetSizeFull,
                chkUseSheetSize,
                chkHalfSheet
            });

            resultPanel.Controls.AddRange(new Control[] {
                lblGramage, txtGramage,
                lblPaper, txtPaper,
                lblLiner, txtLiner,
                lblSellRate, txtSellRate,
                lblNumBoxes, txtNumBoxes,
                lblLamination, txtLamination,
                lblLaminationCalculated, txtLaminationCalculated,
                lblLaminationTotal, txtLaminationTotal,
                lblPrinting, txtPrinting,
                lblPunching, txtPunching,
                lblPasting, txtPasting,
                lblSidePasting, txtSidePasting,
                lblPerBoxRate, txtPerBoxRate,
                lblGrandTotal, txtGrandTotal,
                lblPaperWeightPerBox, txtPaperWeightPerBox,
                lblPaperWeightTotal, txtPaperWeightTotal,
                lblLinerWeightPerBox, txtLinerWeightPerBox,
                lblLinerWeightTotal, txtLinerWeightTotal,
                lblLiner1WeightPerBox, txtLiner1WeightPerBox,
                lblLiner1WeightTotal, txtLiner1WeightTotal,
                lblLiner2WeightPerBox, txtLiner2WeightPerBox,
                lblLiner2WeightTotal, txtLiner2WeightTotal
            });
            this.Controls.Add(resultPanel);

            // Save Button
            btnSave = new Button()
            {
                Text = "Save Record",
                Left = 330,
                Top = 880,
                Width = 120,
                Height = 40,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Event handlers for real-time calculation
            txtLength.TextChanged += InputChanged;
            txtBreadth.TextChanged += InputChanged;
            txtHeight.TextChanged += InputChanged;
            txtGSM.TextChanged += InputChanged;
            txtGSM2.TextChanged += InputChanged;
            txtPly.TextChanged += InputChanged;
            txtLastPly.TextChanged += InputChanged;
            txtSellRate.TextChanged += InputChanged;
            txtNumBoxes.TextChanged += InputChanged;
            txtLamination.TextChanged += InputChanged;
            txtPrinting.TextChanged += InputChanged;
            txtPunching.TextChanged += InputChanged;
            txtPasting.TextChanged += InputChanged;
            txtSidePasting.TextChanged += InputChanged;
            chkHalfSheet.CheckedChanged += InputChanged;
            txtSheetSizeFull.TextChanged += InputChanged;

            // Pre-fill form if editing existing record
            if (existingRecord != null)
            {
                PrefillForm(existingRecord);
            }
        }

        private void ChkUseSheetSize_CheckedChanged(object sender, EventArgs e)
        {
            bool useSheetSize = chkUseSheetSize.Checked;

            // Enable/disable dimension inputs
            txtLength.Enabled = !useSheetSize;
            txtBreadth.Enabled = !useSheetSize;
            txtHeight.Enabled = !useSheetSize;

            // Set visual appearance for disabled fields
            if (useSheetSize)
            {
                txtLength.BackColor = Color.FromArgb(236, 240, 241);
                txtBreadth.BackColor = Color.FromArgb(236, 240, 241);
                txtHeight.BackColor = Color.FromArgb(236, 240, 241);
            }
            else
            {
                txtLength.BackColor = Color.White;
                txtBreadth.BackColor = Color.White;
                txtHeight.BackColor = Color.White;
            }

            // Enable/disable sheet size full input
            txtSheetSizeFull.ReadOnly = !useSheetSize;
            if (useSheetSize)
            {
                txtSheetSizeFull.BackColor = Color.White;
                if (string.IsNullOrEmpty(txtSheetSizeFull.Text))
                    txtSheetSizeFull.Text = "";
            }
            else
            {
                txtSheetSizeFull.BackColor = Color.FromArgb(236, 240, 241);
            }

            // Trigger recalculation
            InputChanged(null, null);
        }

        private void PrefillForm(BoxRecord record)
        {
            txtBoxName.Text = record.BoxName ?? "";
            txtDetail.Text = record.Detail ?? "";

            // Check if this record was saved using direct sheet size mode
            bool wasUsingSheetSize = !string.IsNullOrEmpty(record.BoxSize) && record.BoxSize.StartsWith("Sheet Size:");

            if (wasUsingSheetSize)
            {
                // This record used direct sheet size mode
                chkUseSheetSize.Checked = true;

                // Extract sheet size from BoxSize or use SheetSizeFull
                if (!string.IsNullOrEmpty(record.SheetSizeFull))
                {
                    txtSheetSizeFull.Text = record.SheetSizeFull;
                }
                else if (!string.IsNullOrEmpty(record.BoxSize))
                {
                    // Extract from "Sheet Size: 35 x 40.5"
                    var prefix = "Sheet Size: ";
                    if (record.BoxSize.StartsWith(prefix))
                    {
                        txtSheetSizeFull.Text = record.BoxSize.Substring(prefix.Length).Trim();
                    }
                }

                // Restore L/B/H values stored alongside the sheet size, if present
                if (!string.IsNullOrEmpty(record.BoxDimensions))
                {
                    var dimParts = record.BoxDimensions.Split('x');
                    if (dimParts.Length == 3)
                    {
                        txtLength.Text = dimParts[0].Trim();
                        txtBreadth.Text = dimParts[1].Trim();
                        txtHeight.Text = dimParts[2].Trim();
                    }
                }
            }
            else
            {
                // Normal mode: parse box dimensions
                chkUseSheetSize.Checked = false;

                if (!string.IsNullOrEmpty(record.BoxSize))
                {
                    var parts = record.BoxSize.Split('x');
                    if (parts.Length == 3)
                    {
                        txtLength.Text = parts[0].Trim();
                        txtBreadth.Text = parts[1].Trim();
                        txtHeight.Text = parts[2].Trim();
                    }
                }
            }

            txtGSM.Text = record.GSM.ToString();
            txtGSM2.Text = record.GSM2 > 0 ? record.GSM2.ToString() : "";
            txtPly.Text = record.Ply.ToString();
            txtLastPly.Text = record.LastPlyValue.ToString();
            txtSellRate.Text = record.SellRate.ToString();
            txtNumBoxes.Text = record.NumberOfBoxes.ToString();
            txtLamination.Text = record.LaminationPerBox.ToString();
            txtPrinting.Text = record.PrintingPerBox.ToString();
            txtPunching.Text = record.PunchingPerBox.ToString();
            txtPasting.Text = record.PastingPerBox.ToString();
            txtSidePasting.Text = record.SidePastingPerBox.ToString();
            chkHalfSheet.Checked = record.UseHalfSheetForUsage;

            // Trigger calculation to update all calculated fields
            InputChanged(null, null);
        }

        private void InputChanged(object sender, EventArgs e)
        {
            try
            {
                if (!double.TryParse(txtGSM.Text, out double gsm)) return;
                double gsm2 = ParseOptional(txtGSM2.Text);
                if (!int.TryParse(txtPly.Text, out int ply)) return;
                if (!double.TryParse(txtLastPly.Text, out double topPaper)) return;

                if (ply % 2 == 0)
                {
                    ClearCalculatedFields();
                    return;
                }

                bool useHalfSheet = chkHalfSheet.Checked;
                bool useSheetSize = chkUseSheetSize.Checked;
                double fullLength, fullBreadth;
                double l = 0, b = 0, h = 0;

                if (useSheetSize)
                {
                    // Parse sheet size directly from txtSheetSizeFull
                    if (string.IsNullOrEmpty(txtSheetSizeFull.Text))
                    {
                        ClearCalculatedFields();
                        return;
                    }

                    var parts = txtSheetSizeFull.Text.Split(new[] { 'x', 'X' });
                    if (parts.Length != 2)
                    {
                        ClearCalculatedFields();
                        return;
                    }

                    if (!double.TryParse(parts[0].Trim(), out fullLength) ||
                        !double.TryParse(parts[1].Trim(), out fullBreadth))
                    {
                        ClearCalculatedFields();
                        return;
                    }

                    txtSheetSize.Text = ""; // Leave empty when using direct sheet size
                }
                else
                {
                    // Original calculation mode using L, B, H
                    if (!double.TryParse(txtLength.Text, out l)) return;
                    if (!double.TryParse(txtBreadth.Text, out b)) return;
                    if (!double.TryParse(txtHeight.Text, out h)) return;

                    string halfSheet = CalculationService.CalculateSheetSize($"{l} x {b} x {h}");
                    string fullSheet = CalculationService.CalculateFullSheetSize($"{l} x {b} x {h}");
                    txtSheetSize.Text = halfSheet;
                    txtSheetSizeFull.Text = fullSheet;

                    fullLength = b + h + 1;
                    fullBreadth = (l + b) * 2 + 1.5;
                }

                // Gramage calculation: different logic for single GSM vs 2 GSMs
                double gramage;
                if (gsm2 > 0)
                {
                    // When 2 GSMs are entered:
                    // Step 1: Calculate baseVal = GSM1 + GSM1*40/100 (this gives 140 for GSM1=100)
                    double baseVal = gsm + gsm * 0.4;
                    // Step 2: Add GSM2
                    double result = baseVal + gsm2;
                    // Step 3: Multiply by 2 based on ply (ply=3: no multiply, ply=5: multiply once, ply=7: multiply twice)
                    int multiplyCount = (ply - 3) / 2;
                    for (int i = 0; i < multiplyCount; i++)
                    {
                        result = result * 2;
                    }
                    // Step 4: Add topPaper
                    gramage = result + topPaper;
                }
                else
                {
                    // Single GSM: use existing logic (unchanged)
                    gramage = CalculationService.CalculateGramage(gsm, ply, topPaper);
                }
                txtGramage.Text = gramage.ToString("0.##");

                int numBoxes = 0;
                int.TryParse(txtNumBoxes.Text, out numBoxes);
                numBoxes = Math.Max(0, numBoxes);

                var (paperPerBox, linerPerBox) = CalculationService.CalculatePaperAndLiner(ply, useHalfSheet);
                double paperTotal = paperPerBox * numBoxes;
                double linerTotal = linerPerBox * numBoxes;
                txtPaper.Text = paperTotal.ToString("0.##");
                txtLiner.Text = linerTotal.ToString("0.##");

                double sellRate = 0;
                double.TryParse(txtSellRate.Text, out sellRate);

                // For per box rate calculation, we need to use the fullLength and fullBreadth
                // Since CalculatePerBoxRate uses l, b, h to calculate these again, we can either:
                // 1. Modify CalculatePerBoxRate to accept fullLength and fullBreadth directly, OR
                // 2. Calculate the rate manually here when using sheet size
                double basePerBox;
                if (useSheetSize)
                {
                    // Calculate directly using fullLength and fullBreadth
                    double baseValue = (fullLength * fullBreadth * gramage) / 1550.0;
                    basePerBox = (baseValue * sellRate) / 1000.0;
                }
                else
                {
                    basePerBox = CalculationService.CalculatePerBoxRate(l, b, h, gramage, sellRate);
                }

                double printing = ParseOptional(txtPrinting.Text);
                double punching = ParseOptional(txtPunching.Text);
                double pasting = ParseOptional(txtPasting.Text);
                double sidePasting = ParseOptional(txtSidePasting.Text);
                double laminationRate = ParseOptional(txtLamination.Text);

                double extrasPerBox = printing + punching + pasting + sidePasting;
                double perBoxWithExtras = basePerBox + extrasPerBox;
                txtPerBoxRate.Text = perBoxWithExtras.ToString("0.####");

                double laminationFactor = (fullLength * fullBreadth) / 100.0;
                double laminationPerBox = laminationFactor * laminationRate;
                double laminationTotal = laminationPerBox * numBoxes;
                txtLaminationCalculated.Text = laminationPerBox.ToString("0.##");
                txtLaminationTotal.Text = laminationTotal.ToString("0.##");

                // Paper weight: calculate per unit (1 paper per box), not per usage
                // Weight should remain the same regardless of half sheet usage
                double paperWeightPerBox = (fullLength * fullBreadth * topPaper * 1.0) / 1550.0 / 1000.0;
                double paperWeightTotal = paperWeightPerBox * numBoxes;

                // Liner weights: calculate per unit liner (base count without multiplier), not per usage
                // Weight should remain the same regardless of half sheet usage
                // Base liner count per box = (ply - 1) / 2.0 (without the half sheet multiplier)
                double baseLinerCountPerBox = (ply - 1) / 2.0;

                // Liner weight calculation: different logic for single GSM vs 2 GSMs
                double linerWeightPerBox1;
                double linerWeightPerBox2 = 0;

                if (gsm2 > 0)
                {
                    // When 2 GSMs are entered:
                    // Liner 1: Sheet size × (GSM1 + 40) / 1550 / 1000 (40 is fixed value)
                    linerWeightPerBox1 = (fullLength * fullBreadth * (gsm + 40)) / 1550.0 / 1000.0;
                    // Liner 2: Sheet size × GSM2 / 1550 / 1000
                    linerWeightPerBox2 = (fullLength * fullBreadth * gsm2) / 1550.0 / 1000.0;
                }
                else
                {
                    // Single GSM: calculate GSM + GSM*40/100, then add GSM again
                    // Example: 120 + 120*40/100 = 168, then 168 + 120 = 288
                    double linerGsmValue = gsm + gsm * 0.4 + gsm; // GSM*2.4
                    linerWeightPerBox1 = (fullLength * fullBreadth * linerGsmValue) / 1550.0 / 1000.0;
                }
                // Combined weight per box (using base liner count, not multiplied usage)
                double linerWeightPerBoxCombined = (linerWeightPerBox1 + linerWeightPerBox2) * baseLinerCountPerBox;
                double linerWeightTotalCombined = linerWeightPerBoxCombined * numBoxes;

                txtPaperWeightPerBox.Text = FormatWeight(paperWeightPerBox);
                txtPaperWeightTotal.Text = FormatWeight(paperWeightTotal);
                // Combined liner values (using base liner count, not multiplied usage)
                txtLinerWeightPerBox.Text = FormatWeight(linerWeightPerBoxCombined);
                txtLinerWeightTotal.Text = FormatWeight(linerWeightTotalCombined);
                // Per-GSM liner values (using base liner count, not multiplied usage)
                txtLiner1WeightPerBox.Text = FormatWeight(linerWeightPerBox1 * baseLinerCountPerBox);
                txtLiner1WeightTotal.Text = FormatWeight(linerWeightPerBox1 * baseLinerCountPerBox * numBoxes);
                txtLiner2WeightPerBox.Text = FormatWeight(linerWeightPerBox2 * baseLinerCountPerBox);
                txtLiner2WeightTotal.Text = FormatWeight(linerWeightPerBox2 * baseLinerCountPerBox * numBoxes);

                double grandTotal = perBoxWithExtras * numBoxes + laminationTotal;
                txtGrandTotal.Text = grandTotal.ToString("0.##");
            }
            catch
            {
                // handle format errors for realtime updates
            }
        }

        private bool DeductStockFromInventory(int sheetLength, int gsm1, int gsm2, int topPaper, double paperWeight, double linerWeight1, double linerWeight2)
        {
            var inventory = StorageService.LoadInventory();
            int row = sheetLength;

            // Calculate row number based on range
            if (row < 13)
            {
                // Subtract from general stock
                if (!inventory.DeductGeneralPaper(paperWeight))
                {
                    MessageBox.Show("Insufficient paper stock in general inventory!", "Low Stock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!inventory.DeductGeneralLiner(linerWeight1 + linerWeight2))
                {
                    // Rollback paper deduction
                    inventory.GeneralPaperStock += paperWeight;
                    MessageBox.Show("Insufficient liner stock in general inventory!", "Low Stock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else if (row < 26)
            {
                // Multiply by 2
                row = row * 2;
                if (row >= 26 && row <= 52)
                {
                    // Deduct from table
                    if (!inventory.DeductStock(row, gsm1, paperWeight))
                    {
                        MessageBox.Show($"Insufficient paper stock for row {row}, GSM {gsm1}!", "Low Stock",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    // Deduct liner for GSM1
                    if (linerWeight1 > 0 && !inventory.DeductStock(row, gsm1, linerWeight1))
                    {
                        // Rollback paper deduction
                        inventory.SetStock(row, gsm1, inventory.GetStock(row, gsm1) + paperWeight);
                        MessageBox.Show($"Insufficient liner stock for row {row}, GSM {gsm1}!", "Low Stock",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    // Deduct liner for GSM2 (optional)
                    if (gsm2 > 0 && linerWeight2 > 0)
                    {
                        if (!inventory.DeductStock(row, gsm2, linerWeight2))
                        {
                            // Rollback previous deductions
                            inventory.SetStock(row, gsm1, inventory.GetStock(row, gsm1) + paperWeight + linerWeight1);
                            MessageBox.Show($"Insufficient liner stock for row {row}, GSM {gsm2}!", "Low Stock",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
                else
                {
                    // Fallback to general stock
                    if (!inventory.DeductGeneralPaper(paperWeight))
                    {
                        MessageBox.Show("Insufficient paper stock in general inventory!", "Low Stock",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (!inventory.DeductGeneralLiner(linerWeight1 + linerWeight2))
                    {
                        inventory.GeneralPaperStock += paperWeight;
                        MessageBox.Show("Insufficient liner stock in general inventory!", "Low Stock",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            else if (row >= 26 && row <= 52)
            {
                // Use directly
                if (!inventory.DeductStock(row, gsm1, paperWeight))
                {
                    MessageBox.Show($"Insufficient paper stock for row {row}, GSM {gsm1}!", "Low Stock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (linerWeight1 > 0 && !inventory.DeductStock(row, gsm1, linerWeight1))
                {
                    // Rollback paper deduction
                    inventory.SetStock(row, gsm1, inventory.GetStock(row, gsm1) + paperWeight);
                    MessageBox.Show($"Insufficient liner stock for row {row}, GSM {gsm1}!", "Low Stock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (gsm2 > 0 && linerWeight2 > 0)
                {
                    if (!inventory.DeductStock(row, gsm2, linerWeight2))
                    {
                        // Rollback previous deductions
                        inventory.SetStock(row, gsm1, inventory.GetStock(row, gsm1) + paperWeight + linerWeight1);
                        MessageBox.Show($"Insufficient liner stock for row {row}, GSM {gsm2}!", "Low Stock",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            else
            {
                // row > 52, subtract from general stock
                if (!inventory.DeductGeneralPaper(paperWeight))
                {
                    MessageBox.Show("Insufficient paper stock in general inventory!", "Low Stock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!inventory.DeductGeneralLiner(linerWeight1 + linerWeight2))
                {
                    inventory.GeneralPaperStock += paperWeight;
                    MessageBox.Show("Insufficient liner stock in general inventory!", "Low Stock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // Save updated inventory
            StorageService.SaveInventory(inventory);
            return true;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validate GSM values and Top Paper
            int[] validValues = { 80, 90, 100, 120, 150, 180, 200, 230, 250, 300 };
            double.TryParse(txtGSM.Text, out double gsmValue);
            double.TryParse(txtGSM2.Text, out double gsm2Value);
            double.TryParse(txtLastPly.Text, out double topPaperValue);

            int gsmInt = (int)gsmValue;
            int gsm2Int = (int)gsm2Value;
            int topPaperInt = (int)topPaperValue;

            if (!validValues.Contains(gsmInt) || !validValues.Contains(topPaperInt) ||
                (gsm2Int != 0 && !validValues.Contains(gsm2Int)))
            {
                MessageBox.Show("GSM, optional GSM 2 and Top Paper should be one of: 80, 90, 100, 120, 150, 180, 200, 230, 250, 300", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get sheet size for stock deduction
            double fullLength, fullBreadth;
            double l = 0, b = 0, h = 0;
            string boxSizeStr;

            if (chkUseSheetSize.Checked)
            {
                // Parse sheet size directly
                if (string.IsNullOrEmpty(txtSheetSizeFull.Text))
                {
                    MessageBox.Show("Please enter the Sheet Size (Full) value.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var parts = txtSheetSizeFull.Text.Split('x');
                if (parts.Length != 2 ||
                    !double.TryParse(parts[0].Trim(), out fullLength) ||
                    !double.TryParse(parts[1].Trim(), out fullBreadth))
                {
                    MessageBox.Show("Please enter Sheet Size (Full) in the format: 'length x breadth'", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                boxSizeStr = $"Sheet Size: {fullLength} x {fullBreadth}";

                // Capture L/B/H if user filled them — they may exist even in direct-sheet mode
                double.TryParse(txtLength.Text, out l);
                double.TryParse(txtBreadth.Text, out b);
                double.TryParse(txtHeight.Text, out h);
            }
            else
            {
                // Original mode: calculate from L, B, H
                if (!double.TryParse(txtLength.Text, out l) ||
                    !double.TryParse(txtBreadth.Text, out b) ||
                    !double.TryParse(txtHeight.Text, out h))
                {
                    MessageBox.Show("Please enter valid Length, Breadth, and Height values.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                fullLength = b + h + 1;
                fullBreadth = (l + b) * 2 + 1.5;
                boxSizeStr = $"{l} x {b} x {h}";
            }

            string boxDimensionsStr = (l > 0 || b > 0 || h > 0) ? $"{l} x {b} x {h}" : "";

            double.TryParse(txtPaperWeightTotal.Text, out double paperWeightTotal);

            // Compute liner weights per GSM for stock deduction
            int.TryParse(txtNumBoxes.Text, out int numBoxesLocal);
            numBoxesLocal = Math.Max(0, numBoxesLocal);
            int.TryParse(txtPly.Text, out int plyValue);

            // Calculate base liner count per box (without half sheet multiplier)
            // Weight should remain the same regardless of half sheet usage
            double baseLinerCountPerBox = (plyValue - 1) / 2.0;

            // Liner weight calculation: different logic for single GSM vs 2 GSMs
            double linerWeightPerBox1;
            double linerWeightPerBox2 = 0;

            if (gsm2Int > 0)
            {
                // When 2 GSMs are entered:
                // Liner 1: Sheet size × (GSM1 + 40) / 1550 / 1000 (40 is fixed value)
                linerWeightPerBox1 = (fullLength * fullBreadth * (gsmInt + 40)) / 1550.0 / 1000.0;
                // Liner 2: Sheet size × GSM2 / 1550 / 1000
                linerWeightPerBox2 = (fullLength * fullBreadth * gsm2Int) / 1550.0 / 1000.0;
            }
            else
            {
                // Single GSM: calculate GSM + GSM*40/100, then add GSM again
                // Example: 120 + 120*40/100 = 168, then 168 + 120 = 288
                double linerGsmValue = gsmInt + gsmInt * 0.4 + gsmInt; // GSM*2.4
                linerWeightPerBox1 = (fullLength * fullBreadth * linerGsmValue) / 1550.0 / 1000.0;
            }

            // Total weight = weight per liner * base liner count per box * number of boxes
            double linerWeightTotal1 = linerWeightPerBox1 * baseLinerCountPerBox * numBoxesLocal;
            double linerWeightTotal2 = linerWeightPerBox2 * baseLinerCountPerBox * numBoxesLocal;

            // Deduct stock (paper based on primary GSM/top paper; liner based on GSM1 and GSM2)
            if (!DeductStockFromInventory((int)fullLength, gsmInt, gsm2Int, topPaperInt, paperWeightTotal, linerWeightTotal1, linerWeightTotal2))
            {
                return; // Error message already shown
            }

            // plyValue is already parsed above, reuse it
            double.TryParse(txtGramage.Text, out double gramageValue);
            double.TryParse(txtPaper.Text, out double paperUsage);
            double.TryParse(txtLiner.Text, out double linerUsage);
            double.TryParse(txtSellRate.Text, out double sellRateValue);
            int.TryParse(txtNumBoxes.Text, out int numBoxesValue);
            double.TryParse(txtPerBoxRate.Text, out double perBoxRateValue);
            double.TryParse(txtGrandTotal.Text, out double grandTotalValue);
            double.TryParse(txtLamination.Text, out double laminationInputValue);
            double.TryParse(txtPrinting.Text, out double printingValue);
            double.TryParse(txtPunching.Text, out double punchingValue);
            double.TryParse(txtPasting.Text, out double pastingValue);
            double.TryParse(txtSidePasting.Text, out double sidePastingValue);
            double.TryParse(txtLaminationCalculated.Text, out double laminationCalculatedValue);
            double.TryParse(txtLaminationTotal.Text, out double laminationTotalValue);
            double.TryParse(txtPaperWeightPerBox.Text, out double paperWeightPerBoxValue);
            double.TryParse(txtPaperWeightTotal.Text, out double paperWeightTotalValue);
            double.TryParse(txtLinerWeightPerBox.Text, out double linerWeightPerBoxValue);
            double.TryParse(txtLinerWeightTotal.Text, out double linerWeightTotalValue);

            if (_editingRecord != null)
            {
                // Update existing record instead of creating a new one
                _editingRecord.BoxName = txtBoxName.Text;
                _editingRecord.BoxSize = boxSizeStr;
                _editingRecord.BoxDimensions = boxDimensionsStr;
                _editingRecord.SheetSize = txtSheetSize.Text;
                _editingRecord.SheetSizeFull = txtSheetSizeFull.Text;
                _editingRecord.GSM = gsmValue;
                _editingRecord.GSM2 = gsm2Value;
                _editingRecord.Ply = plyValue;
                _editingRecord.LastPlyValue = topPaperValue;
                _editingRecord.Gramage = gramageValue;
                _editingRecord.PaperUsage = paperUsage;
                _editingRecord.LinerUsage = linerUsage;
                _editingRecord.SellRate = sellRateValue;
                _editingRecord.NumberOfBoxes = numBoxesValue;
                _editingRecord.PerBoxRate = perBoxRateValue;
                _editingRecord.GrandTotal = grandTotalValue;
                _editingRecord.LaminationPerBox = laminationInputValue;
                _editingRecord.PrintingPerBox = printingValue;
                _editingRecord.PunchingPerBox = punchingValue;
                _editingRecord.PastingPerBox = pastingValue;
                _editingRecord.SidePastingPerBox = sidePastingValue;
                _editingRecord.LaminationCalculatedPerBox = laminationCalculatedValue;
                _editingRecord.LaminationTotal = laminationTotalValue;
                _editingRecord.UseHalfSheetForUsage = chkHalfSheet.Checked;
                _editingRecord.PaperWeightPerBox = paperWeightPerBoxValue;
                _editingRecord.PaperWeightTotal = paperWeightTotalValue;
                _editingRecord.LinerWeightPerBox = linerWeightPerBoxValue;
                _editingRecord.LinerWeightTotal = linerWeightTotalValue;
                _editingRecord.FinalTotal = grandTotalValue;
                _editingRecord.Detail = txtDetail.Text;

                // Delete old file and save updated record
                try
                {
                    var recordsDir = Path.Combine(StorageService.StorageRoot,
                        StorageService.MakeSafeName(_customer.Name), "records");
                    if (Directory.Exists(recordsDir))
                    {
                        var files = Directory.GetFiles(recordsDir, "*.json").OrderBy(f => f).ToArray();
                        int recordIndex = _customer.Records.IndexOf(_editingRecord);
                        if (recordIndex >= 0 && recordIndex < files.Length)
                        {
                            File.Delete(files[recordIndex]);
                        }
                    }
                    StorageService.SaveRecord(_customer.Name, _editingRecord);
                }
                catch
                {
                    // ignore persistence errors
                }
            }
            else
            {
                var record = new BoxRecord
                {
                    BoxName = txtBoxName.Text,
                    BoxSize = boxSizeStr,
                    BoxDimensions = boxDimensionsStr,
                    SheetSize = txtSheetSize.Text,
                    SheetSizeFull = txtSheetSizeFull.Text,
                    GSM = gsmValue,
                    GSM2 = gsm2Value,
                    Ply = plyValue,
                    LastPlyValue = topPaperValue,
                    Gramage = gramageValue,
                    PaperUsage = paperUsage,
                    LinerUsage = linerUsage,
                    SellRate = sellRateValue,
                    NumberOfBoxes = numBoxesValue,
                    PerBoxRate = perBoxRateValue,
                    GrandTotal = grandTotalValue,
                    LaminationPerBox = laminationInputValue,
                    PrintingPerBox = printingValue,
                    PunchingPerBox = punchingValue,
                    PastingPerBox = pastingValue,
                    SidePastingPerBox = sidePastingValue,
                    LaminationCalculatedPerBox = laminationCalculatedValue,
                    LaminationTotal = laminationTotalValue,
                    UseHalfSheetForUsage = chkHalfSheet.Checked,
                    PaperWeightPerBox = paperWeightPerBoxValue,
                    PaperWeightTotal = paperWeightTotalValue,
                    LinerWeightPerBox = linerWeightPerBoxValue,
                    LinerWeightTotal = linerWeightTotalValue,
                    FinalTotal = grandTotalValue,
                    Detail = txtDetail.Text
                };

                // Add to in-memory list
                _customer.Records.Add(record);

                // Persist to JSON file per record
                try
                {
                    StorageService.SaveRecord(_customer.Name, record);
                }
                catch
                {
                    // ignore persistence errors to not block UI; could show a message in future
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
