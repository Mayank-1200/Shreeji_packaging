using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using shreeji_packaging.Models;
using shreeji_packaging.Services;

namespace shreeji_packaging.Forms
{
    public class ImageManagementDialog : Form
    {
        private BoxRecord _record;
        private string _customerName;
        private FlowLayoutPanel imagePanel;
        private Panel previewPanel;
        private PictureBox previewPictureBox;
        private Panel scrollablePreviewPanel;
        private Button btnDownload;
        private List<string> _imagePaths;
        private string _currentPreviewImagePath;

        public ImageManagementDialog(string customerName, BoxRecord record)
        {
            _customerName = customerName;
            _record = record;
            _imagePaths = new List<string>(record.ImagePaths ?? new List<string>());

            this.Text = $"Image Management - {record.BoxName}";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.MinimumSize = new Size(800, 500);

            InitializeComponents();
            LoadImages();
        }

        private void InitializeComponents()
        {
            // Header
            Label lblHeader = new Label()
            {
                Text = $"Images for: {_record.BoxName}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            this.Controls.Add(lblHeader);

            // Add Image Button
            Button btnAddImage = new Button()
            {
                Text = "➕ Add Image",
                Size = new Size(150, 40),
                Location = new Point(20, 50),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddImage.FlatAppearance.BorderSize = 0;
            btnAddImage.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnAddImage.Click += BtnAddImage_Click;
            this.Controls.Add(btnAddImage);

            // Split container for images and preview
            SplitContainer splitContainer = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 100),
                SplitterDistance = 600,
                SplitterWidth = 5,
                Orientation = Orientation.Vertical
            };

            // Left panel - Image thumbnails
            Panel leftPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            Label lblImages = new Label()
            {
                Text = "Images:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            leftPanel.Controls.Add(lblImages);

            imagePanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Location = new Point(10, 40),
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(5)
            };
            leftPanel.Controls.Add(imagePanel);

            splitContainer.Panel1.Controls.Add(leftPanel);

            // Right panel - Preview
            previewPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(236, 240, 241),
                Padding = new Padding(10)
            };

            Label lblPreview = new Label()
            {
                Text = "Preview:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            previewPanel.Controls.Add(lblPreview);

            // Scrollable panel for preview
            scrollablePreviewPanel = new Panel()
            {
                Location = new Point(10, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            previewPictureBox = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                BackColor = Color.White
            };
            scrollablePreviewPanel.Controls.Add(previewPictureBox);
            previewPanel.Controls.Add(scrollablePreviewPanel);

            // Download button
            btnDownload = new Button()
            {
                Text = "⬇️ Download Image",
                Size = new Size(180, 35),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Location = new Point(10, 0), // Will be adjusted in resize handler
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnDownload.Click += BtnDownload_Click;
            previewPanel.Controls.Add(btnDownload);

            splitContainer.Panel2.Controls.Add(previewPanel);

            this.Controls.Add(splitContainer);

            // Save and Close buttons
            Button btnSave = new Button()
            {
                Text = "💾 Save",
                Size = new Size(120, 40),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(this.Width - 260, this.Height - 60),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClose = new Button()
            {
                Text = "Close",
                Size = new Size(120, 40),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(this.Width - 130, this.Height - 60),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 140, 141);
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnClose);

            // Handle resize
            this.Resize += (s, e) =>
            {
                btnSave.Location = new Point(this.Width - 260, this.Height - 60);
                btnClose.Location = new Point(this.Width - 130, this.Height - 60);

                // Update preview panel layout
                if (previewPanel != null && scrollablePreviewPanel != null && btnDownload != null)
                {
                    int labelHeight = 30; // Label height
                    int buttonHeight = 35; // Download button height
                    int padding = 20; // Top and bottom padding

                    btnDownload.Location = new Point(10, previewPanel.Height - buttonHeight - 10);
                    scrollablePreviewPanel.Location = new Point(10, labelHeight + 10);
                    scrollablePreviewPanel.Height = previewPanel.Height - labelHeight - buttonHeight - padding - 10;
                    scrollablePreviewPanel.Width = previewPanel.Width - 20;
                }
            };

            // Initial layout update
            previewPanel.Resize += (s, e) =>
            {
                if (scrollablePreviewPanel != null && btnDownload != null)
                {
                    int labelHeight = 30;
                    int buttonHeight = 35;
                    int padding = 20;

                    btnDownload.Location = new Point(10, previewPanel.Height - buttonHeight - 10);
                    scrollablePreviewPanel.Location = new Point(10, labelHeight + 10);
                    scrollablePreviewPanel.Height = previewPanel.Height - labelHeight - buttonHeight - padding - 10;
                    scrollablePreviewPanel.Width = previewPanel.Width - 20;
                }
            };
        }

        private void LoadImages()
        {
            imagePanel.Controls.Clear();

            if (_imagePaths == null || _imagePaths.Count == 0)
            {
                Label lblNoImages = new Label()
                {
                    Text = "No images added yet. Click 'Add Image' to upload.",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(127, 140, 141),
                    AutoSize = true,
                    Location = new Point(10, 10)
                };
                imagePanel.Controls.Add(lblNoImages);
                return;
            }

            foreach (var imagePath in _imagePaths)
            {
                if (!File.Exists(imagePath)) continue;

                try
                {
                    Panel imageContainer = new Panel()
                    {
                        Size = new Size(180, 220),
                        Margin = new Padding(5),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    PictureBox thumbBox = new PictureBox()
                    {
                        Size = new Size(170, 150),
                        Location = new Point(5, 5),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BackColor = Color.White
                    };

                    // Load image
                    using (var img = Image.FromFile(imagePath))
                    {
                        thumbBox.Image = new Bitmap(img);
                    }

                    thumbBox.Click += (s, e) => ShowPreview(imagePath);
                    imageContainer.Controls.Add(thumbBox);

                    // Delete button
                    Button btnDelete = new Button()
                    {
                        Text = "🗑️ Delete",
                        Size = new Size(170, 30),
                        Location = new Point(5, 160),
                        BackColor = Color.FromArgb(231, 76, 60),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        Cursor = Cursors.Hand,
                        Tag = imagePath
                    };
                    btnDelete.FlatAppearance.BorderSize = 0;
                    btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
                    btnDelete.Click += BtnDelete_Click;
                    imageContainer.Controls.Add(btnDelete);

                    imagePanel.Controls.Add(imageContainer);
                }
                catch
                {
                    // Skip images that can't be loaded
                }
            }
        }

        private void ShowPreview(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    // Dispose previous image if exists
                    if (previewPictureBox.Image != null)
                    {
                        previewPictureBox.Image.Dispose();
                    }

                    previewPictureBox.Image = Image.FromFile(imagePath);
                    _currentPreviewImagePath = imagePath;

                    // Enable download button
                    if (btnDownload != null)
                    {
                        btnDownload.Enabled = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Unable to load image for preview.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.webp|All Files|*.*";
                dialog.Multiselect = true;
                dialog.Title = "Select Images to Upload";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string sourcePath in dialog.FileNames)
                    {
                        try
                        {
                            // Generate record ID from BoxName (or use a hash)
                            string recordId = StorageService.MakeSafeName(_record.BoxName);
                            string savedPath = StorageService.SaveImage(_customerName, sourcePath, recordId);
                            _imagePaths.Add(savedPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    LoadImages();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string imagePath = btn?.Tag as string;

            if (string.IsNullOrEmpty(imagePath))
                return;

            var result = MessageBox.Show("Are you sure you want to delete this image?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    StorageService.DeleteImage(imagePath);
                    _imagePaths.Remove(imagePath);

                    // Clear preview if deleted image was shown
                    if (previewPictureBox.Image != null)
                    {
                        previewPictureBox.Image.Dispose();
                        previewPictureBox.Image = null;
                    }

                    LoadImages();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentPreviewImagePath) || !File.Exists(_currentPreviewImagePath))
            {
                MessageBox.Show("No image selected for download.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    string fileName = Path.GetFileName(_currentPreviewImagePath);
                    string extension = Path.GetExtension(_currentPreviewImagePath);

                    saveDialog.FileName = fileName;
                    saveDialog.Filter = $"Image Files (*{extension})|*{extension}|All Files (*.*)|*.*";
                    saveDialog.Title = "Save Image As";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(_currentPreviewImagePath, saveDialog.FileName, true);
                        MessageBox.Show("Image downloaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Update record with current image paths
            _record.ImagePaths = new List<string>(_imagePaths);

            // Save the record to update the JSON file
            try
            {
                // Load all records to find the matching one
                var allRecords = StorageService.LoadRecords(_customerName);
                var recordsDir = Path.Combine(StorageService.StorageRoot,
                    StorageService.MakeSafeName(_customerName), "records");

                if (Directory.Exists(recordsDir))
                {
                    var files = Directory.GetFiles(recordsDir, "*.json").OrderBy(f => f).ToArray();

                    // Find the record index by matching key fields
                    int recordIndex = -1;
                    for (int i = 0; i < allRecords.Count && i < files.Length; i++)
                    {
                        var loadedRecord = allRecords[i];
                        // Match by BoxName and BoxSize (or other unique combination)
                        if (loadedRecord.BoxName == _record.BoxName &&
                            loadedRecord.BoxSize == _record.BoxSize &&
                            loadedRecord.GSM == _record.GSM &&
                            Math.Abs(loadedRecord.PerBoxRate - _record.PerBoxRate) < 0.01)
                        {
                            recordIndex = i;
                            break;
                        }
                    }

                    // If found, delete old file and save updated record
                    if (recordIndex >= 0 && recordIndex < files.Length)
                    {
                        try
                        {
                            File.Delete(files[recordIndex]);
                        }
                        catch
                        {
                            // Ignore deletion errors
                        }
                    }
                }

                // Save the updated record (creates new file)
                StorageService.SaveRecord(_customerName, _record);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving images: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

