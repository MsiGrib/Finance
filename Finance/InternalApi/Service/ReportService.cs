using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using OxyPlot.Axes;
using OxyPlot.SkiaSharp;
using OxyPlot;
using SkiaSharp;
using Svg.Skia;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DW1 = DocumentFormat.OpenXml.Drawing;
using A = DocumentFormat.OpenXml.Drawing;
using DSP = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using OxyPlot.Series;
using DocumentFormat.OpenXml.Spreadsheet;
using DataModel.DataBase;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace InternalApi.Service
{
    public class ReportService
    {
        private readonly IPlotService _plotService;
        private readonly ITableService _tableService;
        private readonly IWebHostEnvironment _env;

        public ReportService(IPlotService plotService, ITableService tableService, IWebHostEnvironment env)
        {
            _plotService = plotService;
            _tableService = tableService;
            _env = env;
        }

        public async Task<byte[]> GenerateReportWordAsync()
        {
            using var ms = new MemoryStream();
            using (var wordDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                var body = mainPart.Document.Body;

                var tables = await _tableService.GetTablesAllAsync();

                foreach (var table in tables)
                {
                    var plots = await _plotService.GetPlotsByTableIdAsync(table.Id);

                    if (!string.IsNullOrEmpty(table.ImagePath))
                    {
                        var svgFile = Path.Combine(_env.WebRootPath, table.ImagePath);
                        if (File.Exists(svgFile))
                        {
                            using var svg = new SKSvg();
                            svg.Load(svgFile);
                            var pic = svg.Picture;

                            float origW = pic.CullRect.Width;
                            float origH = pic.CullRect.Height;

                            const int targetSize = 128;
                            int targetW = targetSize;
                            int targetH = targetSize;

                            using var bmp = new SKBitmap(targetW, targetH);
                            using (var canvas = new SKCanvas(bmp))
                            {
                                canvas.Clear(SkiaSharp.SKColors.Transparent);

                                float scaleX = targetW / origW;
                                float scaleY = targetH / origH;

                                canvas.Scale(scaleX, scaleY);
                                canvas.DrawPicture(pic);
                            }

                            using var skData = bmp.Encode(SKEncodedImageFormat.Png, 100);
                            using var msImg = new MemoryStream();
                            skData.SaveTo(msImg);
                            msImg.Position = 0;

                            var imgPart = mainPart.AddImagePart(DocumentFormat.OpenXml.Packaging.ImagePartType.Png);
                            imgPart.FeedData(msImg);

                            var element = CreateImageElement(mainPart.GetIdOfPart(imgPart), targetW, targetH, "Logo");

                            body.Append(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(element)));
                        }
                    }

                    body.Append(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"The name of the company's stock: {table.Name}")))
                    {
                        ParagraphProperties = new ParagraphProperties(new DocumentFormat.OpenXml.Wordprocessing.RunProperties(new DocumentFormat.OpenXml.Wordprocessing.Bold(), new DocumentFormat.OpenXml.Wordprocessing.FontSize { Val = "32" }))
                    });

                    if (!string.IsNullOrEmpty(table.SubName))
                    {
                        body.Append(new Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"The sub name of the company's stock: {table.SubName}")))
                        {
                            ParagraphProperties = new ParagraphProperties(new DocumentFormat.OpenXml.Wordprocessing.RunProperties(new DocumentFormat.OpenXml.Wordprocessing.Italic(), new DocumentFormat.OpenXml.Wordprocessing.FontSize { Val = "24" }))
                        });
                    }

                    var lastPlot = plots.OrderBy(p => p.Date).LastOrDefault();
                    var priceText = lastPlot != null
                        ? $"{lastPlot.Price:N2} {table.Currency}"
                        : "(no data)";
                    body.Append(new Paragraph(
                        new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"Price (latest): {priceText}")))
                    {
                        ParagraphProperties = new ParagraphProperties(
                            new SpacingBetweenLines { After = "200" })
                    });

                    if (plots.Any())
                    {
                        var plotModel = new PlotModel { Title = "Price over time" };
                        var series = new LineSeries { MarkerType = OxyPlot.MarkerType.Circle, MarkerSize = 3 };
                        foreach (var p in plots.OrderBy(p => p.Date))
                            series.Points.Add(OxyPlot.Axes.DateTimeAxis.CreateDataPoint(p.Date, (double)p.Price));
                        plotModel.Series.Add(series);
                        plotModel.Axes.Add(new DateTimeAxis
                        {
                            Position = AxisPosition.Bottom,
                            StringFormat = "yyyy-MM-dd"
                        });

                        using var chartStream = new MemoryStream();
                        var pngExporter = new PngExporter { Width = 600, Height = 300 };
                        pngExporter.Export(plotModel, chartStream);
                        chartStream.Seek(0, SeekOrigin.Begin);

                        var chartPart = mainPart.AddImagePart(ImagePartType.Png);
                        chartPart.FeedData(chartStream);

                        var chartElem = CreateImageElement(mainPart.GetIdOfPart(chartPart), 600, 300, "Chart");
                        body.Append(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(chartElem)));
                    }

                    body.Append(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break { Type = BreakValues.Page })));
                }

                mainPart.Document.Save();
            }

            return ms.ToArray();
        }

        public async Task<byte[]> GenerateReportExcelAsync()
        {
            using var ms = new MemoryStream();
            using (var doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = doc.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                uint sheetId = 1;
                var tables = await _tableService.GetTablesAllAsync();

                foreach (var table in tables)
                {
                    var sheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    sheetPart.Worksheet = new Worksheet(new SheetData());
                    var sheetName = $"Sheet{sheetId}";
                    sheets.Append(new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(sheetPart),
                        SheetId = sheetId,
                        Name = sheetName
                    });

                    var sheetData = sheetPart.Worksheet.GetFirstChild<SheetData>();

                    if (!string.IsNullOrEmpty(table.ImagePath))
                    {
                        var pngBytes = RenderSvgToPng(Path.Combine(_env.WebRootPath, table.ImagePath), 128, 128);
                        InsertImageIntoSheet(sheetPart, pngBytes, "A1");
                    }

                    int rowIndex = 10;
                    sheetData.Append(CreateTextCell("A", rowIndex++, $"Name: {table.Name}"));
                    if (!string.IsNullOrEmpty(table.SubName))
                        sheetData.Append(CreateTextCell("A", rowIndex++, $"SubName: {table.SubName}"));

                    var plots = await _plotService.GetPlotsByTableIdAsync(table.Id);
                    var last = plots.OrderBy(p => p.Date).LastOrDefault();
                    string priceText = last != null
                        ? $"{last.Price:N2} {table.Currency}"
                        : "(no data)";
                    sheetData.Append(CreateTextCell("A", rowIndex++, $"Price (latest): {priceText}"));

                    if (plots.Any())
                    {
                        var chartPng = RenderPlotsToPng(plots);
                        InsertImageIntoSheet(sheetPart, chartPng, "F10");
                    }

                    sheetId++;
                }

                workbookPart.Workbook.Save();
            }

            return ms.ToArray();
        }

        public async Task<byte[]> GenerateReportJsonAsync()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var tables = await _tableService.GetTablesAllAsync();

            foreach (var item in tables)
            {
                var plots = await _plotService.GetPlotsByTableIdAsync(item.Id);
                item.Plots = plots;
            }

            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true });
            JsonSerializer.Serialize(writer, tables, options);
            writer.Flush();

            return ms.ToArray();
        }

        private byte[] RenderSvgToPng(string svgPath, int width, int height)
        {
            using var svg = new SKSvg();
            svg.Load(svgPath);
            using var bmp = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bmp);
            canvas.Clear(SKColors.Transparent);

            var pic = svg.Picture;
            float scaleX = width / pic.CullRect.Width;
            float scaleY = height / pic.CullRect.Height;
            canvas.Scale(scaleX, scaleY);
            canvas.DrawPicture(pic);

            using var img = bmp.Encode(SKEncodedImageFormat.Png, 100);
            return img.ToArray();
        }

        private byte[] RenderPlotsToPng(IEnumerable<PlotDTO> points)
        {
            var model = new PlotModel { Title = "Price over time" };
            var series = new LineSeries { MarkerType = OxyPlot.MarkerType.Circle, MarkerSize = 3 };
            foreach (var p in points.OrderBy(p => p.Date))
                series.Points.Add(DateTimeAxis.CreateDataPoint(p.Date, (double)p.Price));
            model.Series.Add(series);
            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "yyyy-MM-dd" });

            using var stream = new MemoryStream();
            var exporter = new PngExporter { Width = 600, Height = 300 };
            exporter.Export(model, stream);
            return stream.ToArray();
        }

        private Row CreateTextCell(string col, int rowIndex, string text)
        {
            var row = new Row { RowIndex = (uint)rowIndex };
            var cell = new Cell
            {
                CellReference = $"{col}{rowIndex}",
                DataType = CellValues.String,
                CellValue = new CellValue(text)
            };
            row.Append(cell);
            return row;
        }

        private void InsertImageIntoSheet(WorksheetPart sheetPart, byte[] imageBytes, string topLeftCell)
        {
            (int colIndex1, int rowIndex1) = ParseCellReference(topLeftCell);
            string colId = (colIndex1 - 1).ToString();
            string rowId = (rowIndex1 - 1).ToString();

            using var bmp = SKBitmap.Decode(imageBytes);
            int pxW = bmp.Width;
            int pxH = bmp.Height;
            long emuW = pxW * 9525L;
            long emuH = pxH * 9525L;

            var drawingsPart = sheetPart.DrawingsPart
                               ?? sheetPart.AddNewPart<DrawingsPart>();

            var imgPart = drawingsPart.AddImagePart(ImagePartType.Png);
            using (var imgStream = new MemoryStream(imageBytes))
                imgPart.FeedData(imgStream);

            var wsDr = drawingsPart.WorksheetDrawing;
            if (wsDr == null)
            {
                wsDr = new DSP.WorksheetDrawing();
                drawingsPart.WorksheetDrawing = wsDr;
            }

            var anchor = new DSP.OneCellAnchor(
                new DSP.FromMarker(
                    new DSP.ColumnId(colId),
                    new DSP.ColumnOffset("0"),
                    new DSP.RowId(rowId),
                    new DSP.RowOffset("0")
                ),
                new DSP.Extent { Cx = emuW, Cy = emuH },
                new DSP.Picture(
                    new DSP.NonVisualPictureProperties(
                        new DSP.NonVisualDrawingProperties { Id = 1U, Name = "Picture" },
                        new DSP.NonVisualPictureDrawingProperties()
                    ),
                    new DSP.BlipFill(
                        new A.Blip { Embed = drawingsPart.GetIdOfPart(imgPart) },
                        new A.Stretch(new A.FillRectangle())
                    ),
                    new DSP.ShapeProperties(
                        new A.Transform2D(
                            new A.Offset(),
                            new A.Extents { Cx = emuW, Cy = emuH }
                        ),
                        new A.PresetGeometry(new A.AdjustValueList())
                        { Preset = A.ShapeTypeValues.Rectangle }
                    )
                ),
                new DSP.ClientData()
            );

            wsDr.Append(anchor);
            drawingsPart.WorksheetDrawing.Save();

            if (!sheetPart.Worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.Drawing>().Any())
            {
                sheetPart.Worksheet.Append(
                    new DocumentFormat.OpenXml.Spreadsheet.Drawing { Id = sheetPart.GetIdOfPart(drawingsPart) }
                );
                sheetPart.Worksheet.Save();
            }
        }

        private (int col, int row) ParseCellReference(string cell)
        {
            int i = 0;
            int col = 0;
            while (i < cell.Length && Char.IsLetter(cell[i]))
            {
                col = col * 26 + (Char.ToUpper(cell[i]) - 'A' + 1);
                i++;
            }
            if (i >= cell.Length || !int.TryParse(cell.Substring(i), out int row))
                throw new ArgumentException($"Invalid cell reference '{cell}'", nameof(cell));
            return (col, row);
        }

        private DocumentFormat.OpenXml.Wordprocessing.Drawing CreateImageElement(string relId, int w, int h, string name)
        {
            long cx = w * 9525L, cy = h * 9525L;
            return new DocumentFormat.OpenXml.Wordprocessing.Drawing(
                new DW.Inline(
                    new DW.Extent { Cx = cx, Cy = cy },
                    new DW.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                    new DW.DocProperties { Id = 1U, Name = name },
                    new DW1.Graphic(
                        new A.GraphicData(
                            new A.Pictures.Picture(
                            new A.Pictures.NonVisualPictureProperties(
                                new A.Pictures.NonVisualDrawingProperties { Id = 0U, Name = name },
                                new A.Pictures.NonVisualPictureDrawingProperties()),
                            new A.Pictures.BlipFill(
                                new A.Blip { Embed = relId },
                                new A.Stretch(new A.FillRectangle())
                            ),
                            new A.Pictures.ShapeProperties(
                                new A.Transform2D(
                                new A.Offset { X = 0, Y = 0 },
                                new A.Extents { Cx = cx, Cy = cy }
                            ),
                                new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
                            )
                            )
                        )
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                )
              )
            );
        }
    }
}