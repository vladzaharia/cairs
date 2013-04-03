using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SasquatchCAIRS.Models;
using DataTable = System.Data.DataTable;
using Formula = DocumentFormat.OpenXml.Drawing.Charts.Formula;

namespace SasquatchCAIRS.Controllers {
    public class ExcelExportController : Controller {
        private int tableId = 0;
        #region Public Methods

        /// <summary>
        ///     Given a list of datatables, and the reportyType, it generates a excel file
        ///     using correct template and the data given
        /// </summary>
        /// <param name="reportType">Either Report or AuditLog from constants</param>
        /// <param name="tableDictionary">
        ///     dictionary of the tables to be exported. for either type,
        ///     the number of tables in the list must not exceed 15
        /// </param>
        /// <param name="templatePath">template file path</param>
        /// <param name="workingCopyPath">working copy path</param>
        public void exportDataTable(Constants.ReportType reportType,
                                    Dictionary<string, DataTable>
                                        tableDictionary,
                                    string templatePath, string workingCopyPath) {
            //Instead of creating a new excel file, let's use the template and make a copy to work with.
            System.IO.File.Copy(templatePath, workingCopyPath, true);

            if (tableDictionary.Count > 0) {
                //populate the data into the spreadsheet
                using (SpreadsheetDocument spreadsheet =
                    SpreadsheetDocument.Open(workingCopyPath, true)) {
                    WorkbookPart workbook = spreadsheet.WorkbookPart;
                    //Workbook workbook = spreadsheet.WorkbookPart.Workbook;
                    IEnumerable<WorksheetPart> sheets =
                        //workbook.Descendants<WorksheetPart>();
                        workbook.GetPartsOfType<WorksheetPart>();

                    DataTable table;
                    SheetData data;
                    string sheetName;
                    WorksheetPart worksheetPart;

                    for (int x = 16; x < tableDictionary.Count - 15; x++) {
                        CopySheet(spreadsheet.DocumentType, workbook, "Sheet1",
                                  "Sheet" + x.ToString());
                    }

                    int i = 1;
                    foreach (var keyValue in tableDictionary) {
                        if (i < 16) {
                            table = keyValue.Value;
                            sheetName = "Sheet" + (i).ToString();
                            worksheetPart = getWorksheetPart(workbook, sheetName);

                            //Merge Cells for title
                            string endColumnIndex =
                                getColumnName(table.Columns.Count);
                            MergeTwoCells(worksheetPart.Worksheet, "A1",
                                          endColumnIndex + "1");

                            data =
                                worksheetPart.Worksheet.GetFirstChild<SheetData>
                                    ();

                            //creates title
                            var titleRow = new Row();
                            titleRow.RowIndex = (UInt32) 1;

                            Cell titleCell = createTextCell(1, 1, keyValue.Key);
                            titleRow.AppendChild(titleCell);
                            data.AppendChild(titleRow);

                            //add column names to the first row
                            var header = new Row();
                            header.RowIndex = (UInt32) Constants.reportHeaderRow;

                            foreach (DataColumn column in table.Columns) {
                                Cell headerCell = createTextCell(
                                    table.Columns.IndexOf(column) + 1,
                                    Constants.reportHeaderRow,
                                    column.ColumnName);

                                header.AppendChild(headerCell);
                            }
                            data.AppendChild(header);

                            //loop through each data row
                            DataRow contentRow;
                            for (int j = 0; j < table.Rows.Count; j++) {
                                contentRow = table.Rows[j];
                                switch (reportType) {
                                    case Constants.ReportType.Report:
                                        data.AppendChild(
                                            createContentRow(
                                                Constants.CellDataType.Number,
                                                contentRow,
                                                j + Constants.dataStartRow));
                                        break;
                                    default:
                                        data.AppendChild(
                                            createContentRow(
                                                Constants.CellDataType.Text,
                                                contentRow,
                                                j + Constants.dataStartRow));
                                        break;
                                }
                            }

                            if (reportType == Constants.ReportType.Report) {
                                DrawingsPart drawingsPart =
                                    worksheetPart.GetPartsOfType<DrawingsPart>()
                                                 .FirstOrDefault();
                                if (drawingsPart != null) {
                                    ChartPart chartPart =
                                        drawingsPart.GetPartsOfType<ChartPart>()
                                                    .FirstOrDefault();
                                    fixChartData(chartPart, table.Rows.Count,
                                                 table.Columns.Count);
                                }
                            }

                            //incerement i to get the nextsheet
                            i++;
                        }
                    }

                    for (int j = i; j < 16; j++) {
                        sheetName = "Sheet" + (j).ToString();
                        deleteAWorkSheet(workbook, sheetName);
                    }
                }
            }

            string fileName = (reportType == Constants.ReportType.Report)
                                  ? "Reports.xlsx"
                                  : "AuditLog.xlsx";

            HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition",
                               "attachment; filename=" + fileName + ";");
            response.TransmitFile(workingCopyPath);
            response.Flush();
            response.End();

            if (System.IO.File.Exists(workingCopyPath)) {
                System.IO.File.Delete(workingCopyPath);
            }
        }

        private void CopySheet(SpreadsheetDocumentType docType, WorkbookPart workbookPart, string sheetName,
                               string clonedSheetName) {
            //Get the source sheet to be copied
            WorksheetPart sourceSheetPart = getWorksheetPart(workbookPart,
                                                             sheetName);
            //Take advantage of AddPart for deep cloning
            SpreadsheetDocument tempSheet = SpreadsheetDocument.Create(new MemoryStream(), docType);
            WorkbookPart tempWorkbookPart = tempSheet.AddWorkbookPart();
            WorksheetPart tempWorksheetPart = tempWorkbookPart.AddPart<WorksheetPart>(sourceSheetPart);
            //Add cloned sheet and all associated parts to workbook
            WorksheetPart clonedSheet = workbookPart.AddPart<WorksheetPart>(tempWorksheetPart);

            //Table definition parts are somewhat special and need unique ids...so let's make an id based on count
            int numTableDefParts = sourceSheetPart.GetPartsCountOfType<TableDefinitionPart>();
            tableId = numTableDefParts;
            //Clean up table definition parts (tables need unique ids)
            if (numTableDefParts != 0)
                FixupTableParts(clonedSheet, numTableDefParts);
            //There should only be one sheet that has focus
            CleanView(clonedSheet);

            //Add new sheet to main workbook part
            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            Sheet copiedSheet = new Sheet();
            copiedSheet.Name = clonedSheetName;
            copiedSheet.Id = workbookPart.GetIdOfPart(clonedSheet);
            copiedSheet.SheetId = (uint) sheets.ChildElements.Count + 1;
            sheets.Append(copiedSheet);
            //Save Changes
            workbookPart.Workbook.Save();
        }

        private void CleanView(WorksheetPart worksheetPart) {
            //There can only be one sheet that has focus
            SheetViews views = worksheetPart.Worksheet.GetFirstChild<SheetViews>();
            if (views != null) {
                views.Remove();
                worksheetPart.Worksheet.Save();
            }
        }


        private void FixupTableParts(WorksheetPart worksheetPart, int numTableDefParts) {
            //Every table needs a unique id and name
            foreach (TableDefinitionPart tableDefPart in worksheetPart.TableDefinitionParts) {
                tableId++;
                tableDefPart.Table.Id = (uint) tableId;
                tableDefPart.Table.DisplayName = "CopiedTable" + tableId;
                tableDefPart.Table.Name = "CopiedTable" + tableId;
                tableDefPart.Table.Save();
            }
        }

        /// <summary>
        ///     Finds the worksheetPart with given sheetName
        /// </summary>
        /// <param name="workbookPart">workbook part which contains the worksheet we are looking for</param>
        /// <param name="sheetName">name of the worksheet it's looking for</param>
        /// <returns>worksheetPart with given name</returns>
        private WorksheetPart getWorksheetPart(WorkbookPart workbookPart,
                                               string sheetName) {
            Sheet sheet =
                workbookPart.Workbook.Descendants<Sheet>()
                            .FirstOrDefault(s => sheetName.Equals(s.Name));
            if (sheet == null) {
                throw new Exception(
                    string.Format("Could not find a sheet with name {0}",
                                  sheetName));
            } else {
                return workbookPart.GetPartById(sheet.Id) as WorksheetPart;
            }
        }

        /// <summary>
        ///     given a chartPart, which has fixed data range from the template
        ///     this method corrects the XML so the chart covers the correct range of data
        /// </summary>
        /// <param name="chartPart">Chartpart to be fixed</param>
        /// <param name="totalRowCount">total number of dataRows of the data</param>
        /// <param name="totalColCount">total nu</param>
        private void fixChartData(ChartPart chartPart, int totalRowCount,
                                  int totalColCount) {
            //Get the appropriate chart part from template file.
            //ChartPart chartPart = workbookPart.ChartsheetParts.First().DrawingsPart.ChartParts.First();
            //Change the ranges to accomodate the newly inserted data.
            if (chartPart != null) {
                //the following code changes the range of columns in the chart
                foreach (
                    Formula formula in
                        chartPart.ChartSpace.Descendants<Formula>()) {
                    if (formula.Text.Contains("$D")) {
                        formula.Text = formula.Text.Replace("$D",
                                                            "$" + getColumnName(
                                                                totalColCount));
                    }
                }

                //the following code changes the range of rows in the chart 
                //(by adding BarChartSeries element for additional rows filled with correct data)
                BarChart barChart =
                    chartPart.ChartSpace.Descendants<BarChart>()
                             .FirstOrDefault();

                List<BarChartSeries> descendants =
                    chartPart.ChartSpace.Descendants<BarChartSeries>().ToList();
                int numOfInitBarChartSeries = descendants.Count;

                BarChartSeries refChild = descendants.Last();
                for (int i = 1;
                     i <= totalRowCount - numOfInitBarChartSeries;
                     i++) {
                    BarChartSeries barChartSeriesTemplate =
                        descendants.FirstOrDefault();
                    if (barChartSeriesTemplate != null) {
                        var barChartSeriesToBeAdded =
                            barChartSeriesTemplate.CloneNode(true) as
                            BarChartSeries;

                        foreach (
                            Formula formula in
                                barChartSeriesToBeAdded
                                    .Descendants
                                    <Formula>()
                            ) {
                            if (
                                formula.Text.Contains("$" +
                                                      Constants.dataStartRow
                                                               .ToString())) {
                                formula.Text =
                                    formula.Text.Replace(
                                        "$" + Constants.dataStartRow.ToString(),
                                        "$" + (numOfInitBarChartSeries +
                                               i + Constants.reportHeaderRow)
                                                  .ToString
                                                  ());
                            }
                        }

                        //saves the correct order/index value for the current BarChartSeries
                        barChartSeriesToBeAdded.Index.Val =
                            UInt32Value.FromUInt32((uint) i + 1);
                        barChartSeriesToBeAdded.Order.Val =
                            UInt32Value.FromUInt32((uint) i + 1);

                        barChart.InsertAfter(barChartSeriesToBeAdded, refChild);
                        refChild = barChartSeriesToBeAdded;
                    }
                }

                if (totalRowCount == 1) {
                    barChart.RemoveChild(descendants[1]);
                }

                chartPart.ChartSpace.Save();
            }
        }

        #endregion

        #region WorkBook Methods

        /// <summary>
        ///     Gets the Excel column name based on a supplied index number.
        /// </summary>
        /// <returns>1 = A, 2 = B... 27 = AA, etc.</returns>
        private string getColumnName(int columnIndex) {
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0) {
                modifier = (dividend - 1)%26;
                columnName =
                    Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = ((dividend - modifier)/26);
            }

            return columnName;
        }

        /// <summary>
        ///     creates a Cell for the given column+row index and the value
        /// </summary>
        /// <param name="columnIndex">column index of the cell</param>
        /// <param name="rowIndex">row index of the cell</param>
        /// <param name="cellValue">cell value</param>
        /// <returns>a cell with given value filled in as a string representation</returns>
        private Cell createTextCell(
            int columnIndex,
            int rowIndex,
            object cellValue) {
            var cell = new Cell();

            cell.DataType = CellValues.InlineString;
            cell.CellReference = getColumnName(columnIndex) + rowIndex;

            var inlineString = new InlineString();
            var t = new Text();

            t.Text = cellValue.ToString();
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);

            return cell;
        }

        /// <summary>
        ///     creates a Cell for the given column+row index and the value
        /// </summary>
        /// <param name="columnIndex">column index of the cell</param>
        /// <param name="rowIndex">row index of the cell</param>
        /// <param name="cellValue">cell value</param>
        /// <returns>a cell with given value filled in as a number representation</returns>
        private Cell createNumCell(
            int columnIndex,
            int rowIndex,
            object cellValue) {
            var cell = new Cell();

            cell.DataType = CellValues.Number;
            cell.CellReference = getColumnName(columnIndex) + rowIndex;

            cell.CellValue = new CellValue(cellValue.ToString());

            return cell;
        }

        /// <summary>
        ///     Creates a content row for the given dataRow
        /// </summary>
        /// <param name="cellType"></param>
        /// <param name="dataRow">dataRow that contains the data for the row it's creating</param>
        /// <param name="rowIndex">index for the row in the table</param>
        /// <returns>returns dataRow with proper data filled in</returns>
        private Row createContentRow(Constants.CellDataType cellType,
                                     DataRow dataRow,
                                     int rowIndex) {
            var row = new Row {
                RowIndex = (UInt32) rowIndex
            };

            for (int i = 0; i < dataRow.Table.Columns.Count; i++) {
                Cell dataCell;
                if (i > 0) {
                    switch (cellType) {
                        case Constants.CellDataType.Number:
                            dataCell = createNumCell(i + 1, rowIndex, dataRow[i]);
                            break;
                        default:
                            dataCell = createTextCell(i + 1, rowIndex,
                                                      dataRow[i]);
                            break;
                    }
                } else {
                    dataCell = createTextCell(i + 1, rowIndex, dataRow[i]);
                }
                row.AppendChild(dataCell);
            }
            return row;
        }

        /// <summary>
        ///     merges the cell with the given range in the given worksheet
        /// </summary>
        /// <param name="worksheet">worksheet the mergecell belongs to</param>
        /// <param name="cell1Name">start range of the merge cell</param>
        /// <param name="cell2Name">end range of the merge cell</param>
        private static void MergeTwoCells(Worksheet worksheet, string cell1Name,
                                          string cell2Name) {
            if (worksheet == null || string.IsNullOrEmpty(cell1Name) ||
                string.IsNullOrEmpty(cell2Name)) {
                return;
            }

            MergeCells mergeCells;
            if (worksheet.Elements<MergeCells>().Any()) {
                mergeCells = worksheet.Elements<MergeCells>().First();
            } else {
                mergeCells = new MergeCells();

                // Insert a MergeCells object into the specified position.
                if (worksheet.Elements<CustomSheetView>().Any()) {
                    worksheet.InsertAfter(mergeCells,
                                          worksheet.Elements<CustomSheetView>()
                                                   .First());
                } else if (worksheet.Elements<DataConsolidate>().Any()) {
                    worksheet.InsertAfter(mergeCells,
                                          worksheet.Elements<DataConsolidate>()
                                                   .First());
                } else if (worksheet.Elements<SortState>().Any()) {
                    worksheet.InsertAfter(mergeCells,
                                          worksheet.Elements<SortState>()
                                                   .First());
                } else if (worksheet.Elements<AutoFilter>().Any()) {
                    worksheet.InsertAfter(mergeCells,
                                          worksheet.Elements<AutoFilter>()
                                                   .First());
                } else if (worksheet.Elements<Scenarios>().Any()) {
                    worksheet.InsertAfter(mergeCells,
                                          worksheet.Elements<Scenarios>()
                                                   .First());
                } else if (worksheet.Elements<ProtectedRanges>().Any()) {
                    worksheet.InsertAfter(mergeCells,
                                          worksheet.Elements<ProtectedRanges>()
                                                   .First());
                } else if (worksheet.Elements<SheetProtection>().Any()) {
                    worksheet.InsertAfter(mergeCells,
                                          worksheet.Elements<SheetProtection>()
                                                   .First());
                } else if (worksheet.Elements<SheetCalculationProperties>().Any()) {
                    worksheet.InsertAfter(mergeCells,
                                          worksheet
                                              .Elements
                                              <SheetCalculationProperties>()
                                              .First());
                } else {
                    worksheet.InsertAfter(mergeCells,
                                          worksheet.Elements<SheetData>()
                                                   .First());
                }
            }

            // Create the merged cell and append it to the MergeCells collection.
            var mergeCell = new MergeCell {
                Reference = new StringValue(cell1Name + ":" + cell2Name)
            };
            mergeCells.Append(mergeCell);

            worksheet.Save();
        }

        /// <summary>
        ///     deletes a sheet given the workbook the sheet is in and the sheetname;
        /// </summary>
        /// <param name="wbPart">workbook the sheet is part of</param>
        /// <param name="sheetToDelete">the name of the sheet to be deleted</param>
        private void deleteAWorkSheet(WorkbookPart wbPart, string sheetToDelete) {
            string Sheetid = "";

            // Get the pivot Table Parts
            IEnumerable<PivotTableCacheDefinitionPart> pvtTableCacheParts =
                wbPart.PivotTableCacheDefinitionParts;
            var pvtTableCacheDefinationPart =
                new Dictionary<PivotTableCacheDefinitionPart, string>();
            foreach (PivotTableCacheDefinitionPart Item in pvtTableCacheParts) {
                PivotCacheDefinition pvtCacheDef = Item.PivotCacheDefinition;
                //Check if this CacheSource is linked to SheetToDelete
                IEnumerable<CacheSource> pvtCahce =
                    pvtCacheDef.Descendants<CacheSource>()
                               .Where(
                                   s => s.WorksheetSource.Sheet == sheetToDelete);
                if (pvtCahce.Count() > 0) {
                    pvtTableCacheDefinationPart.Add(Item, Item.ToString());
                }
            }
            foreach (var Item in pvtTableCacheDefinationPart) {
                wbPart.DeletePart(Item.Key);
            }
            //Get the SheetToDelete from workbook.xml
            Sheet theSheet =
                wbPart.Workbook.Descendants<Sheet>()
                      .Where(s => s.Name == sheetToDelete)
                      .FirstOrDefault();
            if (theSheet == null) {
                // The specified sheet doesn't exist.
            }
            //Store the SheetID for the reference
            Sheetid = theSheet.SheetId;

            // Remove the sheet reference from the workbook.
            var worksheetPart =
                (WorksheetPart) (wbPart.GetPartById(theSheet.Id));
            theSheet.Remove();

            // Delete the worksheet part.
            wbPart.DeletePart(worksheetPart);

            //Get the DefinedNames
            DefinedNames definedNames =
                wbPart.Workbook.Descendants<DefinedNames>().FirstOrDefault();
            if (definedNames != null) {
                var defNamesToDelete = new List<DefinedName>();

                foreach (DefinedName Item in definedNames) {
                    // This condition checks to delete only those names which are part of Sheet in question
                    if (Item.Text.Contains(sheetToDelete + "!")) {
                        defNamesToDelete.Add(Item);
                    }
                }

                foreach (DefinedName Item in defNamesToDelete) {
                    Item.Remove();
                }
            }

            // Save the workbook.
            wbPart.Workbook.Save();
        }

        #endregion

        //Basic Code from OpenXML tutorial: http://lateral8.com/articles/2010/3/5/openxml-sdk-20-export-a-datatable-to-excel.aspx
        //Customized to create data in more than one excel sheet
    }
}