using System;
using System.Collections.Generic;
using System.Linq;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using shreeji_packaging.Models;

namespace shreeji_packaging.Services
{
    public static class PdfExportService
    {
        public static void ExportToPdf(List<(string CustomerName, BoxRecord Record)> records, string filePath)
        {
            if (records == null || records.Count == 0)
                throw new ArgumentException("No records to export.");

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Box Records Export";
            document.Info.Author = "Shreeji Packaging";
            document.Info.Subject = "Box Records Export";
            document.Info.CreationDate = DateTime.Now;

            // Create landscape page - A4 landscape: 842 x 595 points (width x height)
            // A4 portrait is 595 x 842, so landscape swaps them
            PdfPage page = document.AddPage();
            page.Width = 842;  // A4 landscape width (A4 height in portrait)
            page.Height = 595; // A4 landscape height (A4 width in portrait)
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Define fonts
            XFont titleFont = new XFont("Arial", 16, XFontStyle.Bold);
            XFont headerFont = new XFont("Arial", 9, XFontStyle.Bold);
            XFont cellFont = new XFont("Arial", 8, XFontStyle.Regular);

            // Define colors
            XColor headerBackColor = XColor.FromArgb(52, 152, 219);
            XColor headerTextColor = XColors.White;
            XColor rowBackColor = XColors.White;
            XColor altRowBackColor = XColor.FromArgb(250, 251, 252);
            XColor borderColor = XColor.FromArgb(223, 230, 233);

            // Page margins
            double margin = 30;
            double pageWidth = page.Width - (margin * 2);
            double pageHeight = page.Height - (margin * 2);
            double yPosition = margin;
            double rowHeight = 28; // Increased for better text visibility
            double headerHeight = 32; // Increased for better header text visibility

            // Title
            gfx.DrawString("Box Records Export", titleFont, XBrushes.Black, 
                new XRect(margin, yPosition, pageWidth, 25), XStringFormats.TopCenter);
            yPosition += 35;

            // Export date
            XFont dateFont = new XFont("Arial", 9, XFontStyle.Regular);
            gfx.DrawString($"Exported on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", dateFont, XBrushes.Gray, 
                new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 25;

            // Record count
            gfx.DrawString($"Total Records: {records.Count}", dateFont, XBrushes.Gray, 
                new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 30;

            // Define columns with proper widths for landscape A4 (width ~ 794 points)
            // We have 11 columns, so we need to distribute the width carefully
            // Using proportional widths that will be scaled to fit page width
            List<ColumnDefinition> columns = new List<ColumnDefinition>
            {
                new ColumnDefinition { Header = "Customer", Width = 75 },
                new ColumnDefinition { Header = "Box Size", Width = 85 },
                new ColumnDefinition { Header = "Sheet Size", Width = 75 },
                new ColumnDefinition { Header = "GSM", Width = 50 },
                new ColumnDefinition { Header = "Liner Usage", Width = 75 },
                new ColumnDefinition { Header = "Ply", Width = 40 },
                new ColumnDefinition { Header = "Top Paper", Width = 65 },
                new ColumnDefinition { Header = "Paper Usage", Width = 75 },
                new ColumnDefinition { Header = "Box Qty", Width = 55 },
                new ColumnDefinition { Header = "Paper Weight (kg)", Width = 95 },
                new ColumnDefinition { Header = "Liner Weight (kg)", Width = 95 }
            };

            double totalWidth = columns.Sum(c => c.Width);
            double availableWidth = pageWidth - 20; // Small padding for borders

            // Scale columns proportionally to fit exactly in available width
            // This ensures all columns are visible and nothing is cut off
            double scale = availableWidth / totalWidth;
            foreach (var col in columns)
            {
                col.Width *= scale;
            }

            // Draw table header
            double xPosition = margin;
            gfx.DrawRectangle(new XSolidBrush(headerBackColor), 
                new XRect(xPosition, yPosition, availableWidth, headerHeight));

            foreach (var column in columns)
            {
                XRect headerRect = new XRect(xPosition, yPosition, column.Width, headerHeight);
                gfx.DrawRectangle(new XPen(borderColor, 0.5), headerRect);
                
                // Draw header text with word wrapping
                XRect textRect = new XRect(xPosition + 3, yPosition + 3, column.Width - 6, headerHeight - 6);
                XStringFormat headerFormat = XStringFormats.TopLeft;
                headerFormat.Alignment = XStringAlignment.Near;
                headerFormat.LineAlignment = XLineAlignment.Near;
                gfx.DrawString(column.Header, headerFont, new XSolidBrush(headerTextColor), 
                    textRect, headerFormat);
                
                xPosition += column.Width;
            }

            yPosition += headerHeight;

            // Draw table rows
            int rowIndex = 0;
            foreach (var (customerName, record) in records)
            {
                // Check if we need a new page
                if (yPosition + rowHeight > page.Height - margin)
                {
                    page = document.AddPage();
                    page.Width = 842;  // A4 landscape width
                    page.Height = 595; // A4 landscape height
                    gfx = XGraphics.FromPdfPage(page);
                    yPosition = margin;

                    // Redraw header on new page
                    xPosition = margin;
                    gfx.DrawRectangle(new XSolidBrush(headerBackColor), 
                        new XRect(xPosition, yPosition, availableWidth, headerHeight));

                    foreach (var column in columns)
                    {
                        XRect headerRect = new XRect(xPosition, yPosition, column.Width, headerHeight);
                        gfx.DrawRectangle(new XPen(borderColor, 0.5), headerRect);
                        
                        XRect textRect = new XRect(xPosition + 3, yPosition + 3, column.Width - 6, headerHeight - 6);
                        XStringFormat headerFormat2 = XStringFormats.TopLeft;
                        headerFormat2.Alignment = XStringAlignment.Near;
                        headerFormat2.LineAlignment = XLineAlignment.Near;
                        gfx.DrawString(column.Header, headerFont, new XSolidBrush(headerTextColor), 
                            textRect, headerFormat2);
                        
                        xPosition += column.Width;
                    }

                    yPosition += headerHeight;
                }

                // Alternate row colors
                XColor currentRowColor = (rowIndex % 2 == 0) ? rowBackColor : altRowBackColor;
                xPosition = margin;

                // Get sheet size based on UseHalfSheetForUsage
                string sheetSize = record.UseHalfSheetForUsage ? record.SheetSize : record.SheetSizeFull;
                if (string.IsNullOrEmpty(sheetSize))
                    sheetSize = "-";

                // Prepare cell values
                string[] cellValues = new string[]
                {
                    customerName,
                    record.BoxSize ?? "-",
                    sheetSize,
                    record.GSM.ToString("0"),
                    record.LinerUsage.ToString("0.##"),
                    record.Ply.ToString(),
                    record.LastPlyValue.ToString("0"),
                    record.PaperUsage.ToString("0.##"),
                    record.NumberOfBoxes.ToString(),
                    record.PaperWeightTotal.ToString("0.###"),
                    record.LinerWeightTotal.ToString("0.###")
                };

                // Draw row background
                gfx.DrawRectangle(new XSolidBrush(currentRowColor), 
                    new XRect(xPosition, yPosition, availableWidth, rowHeight));

                // Draw cells
                for (int i = 0; i < columns.Count && i < cellValues.Length; i++)
                {
                    XRect cellRect = new XRect(xPosition, yPosition, columns[i].Width, rowHeight);
                    
                    // Draw border
                    gfx.DrawRectangle(new XPen(borderColor, 0.5), cellRect);
                    
                    // Draw cell text - ensure it fits within cell bounds
                    XRect textRect = new XRect(xPosition + 3, yPosition + 3, columns[i].Width - 6, rowHeight - 6);
                    XStringFormat cellFormat = XStringFormats.TopLeft;
                    cellFormat.Alignment = XStringAlignment.Near;
                    cellFormat.LineAlignment = XLineAlignment.Near;
                    
                    // Use MeasureString to check if text fits, if not, truncate or wrap
                    XSize textSize = gfx.MeasureString(cellValues[i], cellFont);
                    if (textSize.Width > textRect.Width)
                    {
                        // Text is too wide - try to fit it or show truncated version
                        // For now, we'll just draw it and let it clip (better than losing data)
                        // In a production system, you might want to implement smart truncation
                    }
                    
                    gfx.DrawString(cellValues[i], cellFont, XBrushes.Black, textRect, cellFormat);
                    
                    xPosition += columns[i].Width;
                }

                yPosition += rowHeight;
                rowIndex++;
            }

            // Save document
            document.Save(filePath);
            document.Dispose();
            gfx.Dispose();
        }

        private class ColumnDefinition
        {
            public string Header { get; set; }
            public double Width { get; set; }
        }
    }
}

