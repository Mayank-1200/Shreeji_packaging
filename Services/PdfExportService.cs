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
            // We have 12 columns now (added separate liner weight columns for GSM1 and GSM2)
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
                new ColumnDefinition { Header = "Liner Weight 1 (kg)", Width = 95 },
                new ColumnDefinition { Header = "Liner Weight 2 (kg)", Width = 95 }
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
            double lineHeight = cellFont.GetHeight();
            int rowIndex = 0;
            foreach (var (customerName, record) in records)
            {
                // Get sheet size based on UseHalfSheetForUsage
                string sheetSize = record.UseHalfSheetForUsage ? record.SheetSize : record.SheetSizeFull;
                if (string.IsNullOrEmpty(sheetSize))
                    sheetSize = "-";

                // Calculate individual liner weights for each GSM
                double fullLength = 0, fullBreadth = 0;
                if (!string.IsNullOrEmpty(record.SheetSizeFull))
                {
                    var parts = record.SheetSizeFull.Split('x');
                    if (parts.Length == 2)
                    {
                        double.TryParse(parts[0].Trim(), out fullLength);
                        double.TryParse(parts[1].Trim(), out fullBreadth);
                    }
                }

                double baseLinerCountPerBox = (record.Ply - 1) / 2.0;

                double linerWeightPerBox1 = 0;
                double linerWeightPerBox2 = 0;
                if (fullLength > 0 && fullBreadth > 0)
                {
                    if (record.GSM2 > 0)
                    {
                        linerWeightPerBox1 = (fullLength * fullBreadth * (record.GSM + 40)) / 1550.0 / 1000.0;
                        linerWeightPerBox2 = (fullLength * fullBreadth * record.GSM2) / 1550.0 / 1000.0;
                    }
                    else
                    {
                        double linerGsmValue = record.GSM + record.GSM * 0.4 + record.GSM;
                        linerWeightPerBox1 = (fullLength * fullBreadth * linerGsmValue) / 1550.0 / 1000.0;
                    }
                }

                double linerWeightTotal1 = linerWeightPerBox1 * baseLinerCountPerBox * record.NumberOfBoxes;
                double linerWeightTotal2 = linerWeightPerBox2 * baseLinerCountPerBox * record.NumberOfBoxes;

                string[] cellValues = new string[]
                {
                    customerName,
                    !string.IsNullOrEmpty(record.BoxDimensions) ? record.BoxDimensions : (record.BoxSize ?? "-"),
                    sheetSize,
                    record.GSM.ToString("0"),
                    record.LinerUsage.ToString("0.##"),
                    record.Ply.ToString(),
                    record.LastPlyValue.ToString("0"),
                    record.PaperUsage.ToString("0.##"),
                    record.NumberOfBoxes.ToString(),
                    record.PaperWeightTotal.ToString("0.###"),
                    linerWeightTotal1 > 0 ? linerWeightTotal1.ToString("0.###") : "-",
                    linerWeightTotal2 > 0 ? linerWeightTotal2.ToString("0.###") : "-"
                };

                // Wrap each cell to its column width and compute actual row height
                var wrappedCells = new List<List<string>>();
                int maxLines = 1;
                for (int i = 0; i < cellValues.Length; i++)
                {
                    var wrappedLines = WrapText(cellValues[i], columns[i].Width - 6, cellFont, gfx);
                    wrappedCells.Add(wrappedLines);
                    if (wrappedLines.Count > maxLines) maxLines = wrappedLines.Count;
                }
                double actualRowHeight = Math.Max(rowHeight, maxLines * lineHeight + 6);

                // Page break check using the actual (possibly taller) row height
                if (yPosition + actualRowHeight > page.Height - margin)
                {
                    page = document.AddPage();
                    page.Width = 842;
                    page.Height = 595;
                    gfx = XGraphics.FromPdfPage(page);
                    yPosition = margin;

                    // Redraw header on new page
                    double xpHeader = margin;
                    gfx.DrawRectangle(new XSolidBrush(headerBackColor),
                        new XRect(xpHeader, yPosition, availableWidth, headerHeight));
                    foreach (var column in columns)
                    {
                        XRect headerRect = new XRect(xpHeader, yPosition, column.Width, headerHeight);
                        gfx.DrawRectangle(new XPen(borderColor, 0.5), headerRect);

                        XRect textRect = new XRect(xpHeader + 3, yPosition + 3, column.Width - 6, headerHeight - 6);
                        XStringFormat headerFormat2 = XStringFormats.TopLeft;
                        headerFormat2.Alignment = XStringAlignment.Near;
                        headerFormat2.LineAlignment = XLineAlignment.Near;
                        gfx.DrawString(column.Header, headerFont, new XSolidBrush(headerTextColor),
                            textRect, headerFormat2);

                        xpHeader += column.Width;
                    }
                    yPosition += headerHeight;
                }

                // Draw row
                XColor currentRowColor = (rowIndex % 2 == 0) ? rowBackColor : altRowBackColor;
                xPosition = margin;
                gfx.DrawRectangle(new XSolidBrush(currentRowColor),
                    new XRect(xPosition, yPosition, availableWidth, actualRowHeight));

                for (int i = 0; i < columns.Count && i < cellValues.Length; i++)
                {
                    XRect cellRect = new XRect(xPosition, yPosition, columns[i].Width, actualRowHeight);
                    gfx.DrawRectangle(new XPen(borderColor, 0.5), cellRect);

                    double textY = yPosition + 3;
                    foreach (var line in wrappedCells[i])
                    {
                        XRect textRect = new XRect(xPosition + 3, textY, columns[i].Width - 6, lineHeight);
                        XStringFormat cellFormat = XStringFormats.TopLeft;
                        cellFormat.Alignment = XStringAlignment.Near;
                        cellFormat.LineAlignment = XLineAlignment.Near;
                        gfx.DrawString(line, cellFont, XBrushes.Black, textRect, cellFormat);
                        textY += lineHeight;
                    }

                    xPosition += columns[i].Width;
                }

                yPosition += actualRowHeight;
                rowIndex++;
            }

            // Save document
            document.Save(filePath);
            document.Dispose();
            gfx.Dispose();
        }

        private static List<string> WrapText(string text, double maxWidth, XFont font, XGraphics gfx)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text))
            {
                lines.Add("");
                return lines;
            }

            var words = text.Split(' ');
            string currentLine = "";

            foreach (var word in words)
            {
                string candidate = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                if (gfx.MeasureString(candidate, font).Width <= maxWidth)
                {
                    currentLine = candidate;
                    continue;
                }

                if (!string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = "";
                }

                // The single word is wider than the cell — break it by characters
                if (gfx.MeasureString(word, font).Width > maxWidth)
                {
                    string chunk = "";
                    foreach (var c in word)
                    {
                        string testChunk = chunk + c;
                        if (gfx.MeasureString(testChunk, font).Width <= maxWidth)
                        {
                            chunk = testChunk;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(chunk)) lines.Add(chunk);
                            chunk = c.ToString();
                        }
                    }
                    currentLine = chunk;
                }
                else
                {
                    currentLine = word;
                }
            }

            if (!string.IsNullOrEmpty(currentLine)) lines.Add(currentLine);
            if (lines.Count == 0) lines.Add("");
            return lines;
        }

        private class ColumnDefinition
        {
            public string Header { get; set; }
            public double Width { get; set; }
        }
    }
}

