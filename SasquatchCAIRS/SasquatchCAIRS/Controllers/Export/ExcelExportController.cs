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
using SasquatchCAIRS.Models.Common;
using DataTable = System.Data.DataTable;
using Formula = DocumentFormat.OpenXml.Drawing.Charts.Formula;

namespace SasquatchCAIRS.Controllers.Export {
    /// <summary>
    ///     Handles exporting to an xlsx file.
    /// </summary>
    public class ExcelExportController : Controller {
        /// <summary>Default ID of the Table.</summary>
        private int _tableId;

        #region Public Methods

        /// <summary>
        ///     Given a list of datatables, and the reportyType, it generates a excel file
        ///     using correct template and the data given
        /// </summary>
        /// <param name="reportType">Either Report or AuditLog from constants</param>
        /// <param name="tableDictionary">
        ///     Dictionary of the tables to be exported. For either type,
        ///     the number of tables in the list must not exceed 15.
        /// </param>
        /// <param name="templatePath">Template file path</param>
        /// <param name="workingCopyPath">Working copy path</param>
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

                    //if there are more than 15 worksheets that needs to be created, 
                    //the following method copies the first template and adds extra worksheets
                    //to the given workbook.
                    for (int x = 16; x <= tableDictionary.Count; x++) {
                        copySheet(spreadsheet.DocumentType, workbook, "Sheet1",
                                  "Sheet" + x.ToString());
                    }

                    int i = 1;
                    foreach (var keyValue in tableDictionary) {
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
                        header.RowIndex = (UInt32) Constants.REPORT_HEADER_ROW;

                        foreach (DataColumn column in table.Columns) {
                            Cell headerCell = createTextCell(
                                table.Columns.IndexOf(column) + 1,
                                Constants.REPORT_HEADER_ROW,
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
                                            j + Constants.DATA_START_ROW));
                                    break;
                                default:
                                    data.AppendChild(
                                        createContentRow(
                                            Constants.CellDataType.Text,
                                            contentRow,
                                            j + Constants.DATA_START_ROW));
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

                    //if there were less than 15 worksheets to create, the rest of the templates are deleted.
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

        /// <summary>
        ///     Copies a sheet with a given sheet name and saves it with the clonedSheetName in the workbook
        /// </summary>
        /// <param name="docType">DocType to created for temp file in the method</param>
        /// <param name="workbookPart">Workbook which the worksheet to be copied and the copied worksheet to be saved</param>
        /// <param name="sheetName">Name of the worksheet to copy</param>
        /// <param name="clonedSheetName">New name of the copied sheet</param>
        private void copySheet(SpreadsheetDocumentType docType,
                               WorkbookPart workbookPart, string sheetName,
                               string clonedSheetName) {
            //Get the source sheet to be copied
            WorksheetPart sourceSheetPart = getWorksheetPart(workbookPart,
                                                             sheetName);
            //Take advantage of AddPart for deep cloning
            SpreadsheetDocument tempSheet =
                SpreadsheetDocument.Create(new MemoryStream(), docType);
            WorkbookPart tempWorkbookPart = tempSheet.AddWorkbookPart();
            WorksheetPart tempWorksheetPart =
                tempWorkbookPart.AddPart(sourceSheetPart);
            //Add cloned sheet and all associated parts to workbook
            WorksheetPart clonedSheet = workbookPart.AddPart(tempWorksheetPart);

            //Table definition parts are somewhat special and need unique ids...so let's make an id based on count
            int numTableDefParts =
                sourceSheetPart.GetPartsCountOfType<TableDefinitionPart>();
            _tableId = numTableDefParts;
            //Clean up table definition parts (tables need unique ids)
            if (numTableDefParts != 0) {
                fixupTableParts(clonedSheet, numTableDefParts);
            }
            //There should only be one sheet that has focus
            cleanView(clonedSheet);

            //Add new sheet to main workbook part
            var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            var copiedSheet = new Sheet();
            copiedSheet.Name = clonedSheetName;
            copiedSheet.Id = workbookPart.GetIdOfPart(clonedSheet);
            copiedSheet.SheetId = (uint) sheets.ChildElements.Count + 1;
            sheets.Append(copiedSheet);
            //Save Changes
            workbookPart.Workbook.Save();
        }

        /// <summary>
        ///     Remove any view reference in the cloned worksheet.
        /// </summary>
        /// <param name="worksheetPart">Worksheet to clean</param>
        private void cleanView(WorksheetPart worksheetPart) {
            //There can only be one sheet that has focus
            var views = worksheetPart.Worksheet.GetFirstChild<SheetViews>();
            if (views != null) {
                views.Remove();
                worksheetPart.Worksheet.Save();
            }
        }

        /// <summary>
        ///     Make sure each table has a unique id and name
        /// </summary>
        /// <param name="worksheetPart">Worksheet for table part fix</param>
        /// <param name="numTableDefParts">Number of the tableDefPart in the original worksheet</param>
        private void fixupTableParts(WorksheetPart worksheetPart,
                                     int numTableDefParts) {
            //Every table needs a unique id and name
            foreach (
                TableDefinitionPart tableDefPart in
                    worksheetPart.TableDefinitionParts) {
                _tableId++;
                tableDefPart.Table.Id = (uint) _tableId;
                tableDefPart.Table.DisplayName = "CopiedTable" + _tableId;
                tableDefPart.Table.Name = "CopiedTable" + _tableId;
                tableDefPart.Table.Save();
            }
        }

        /// <summary>
        ///     Finds the worksheetPart with given sheetName
        /// </summary>
        /// <param name="workbookPart">Workbook part which contains the worksheet we are looking for</param>
        /// <param name="sheetName">Name of the worksheet it's looking for</param>
        /// <returns>WorksheetPart with given name</returns>
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
        ///     Given a chartPart, which has fixed data range from the template
        ///     This method corrects the XML so the chart covers the correct range of data
        /// </summary>
        /// <param name="chartPart">Chartpart to be fixed</param>
        /// <param name="totalRowCount">Total number of dataRows of the data</param>
        /// <param name="totalColCount">Total nu</param>
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
                                                      Constants.DATA_START_ROW
                                                               .ToString())) {
                                formula.Text =
                                    formula.Text.Replace(
                                        "$" +
                                        Constants.DATA_START_ROW.ToString(),
                                        "$" + (numOfInitBarChartSeries +
                                               i + Constants.REPORT_HEADER_ROW)
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
        ///     Creates a Cell for the given column+row index and the value
        /// </summary>
        /// <param name="columnIndex">Column index of the cell</param>
        /// <param name="rowIndex">Row index of the cell</param>
        /// <param name="cellValue">Cell value</param>
        /// <returns>A cell with given value filled in as a string representation</returns>
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
        ///     Creates a Cell for the given column+row index and the value
        /// </summary>
        /// <param name="columnIndex">Column index of the cell</param>
        /// <param name="rowIndex">Row index of the cell</param>
        /// <param name="cellValue">Cell value</param>
        /// <returns>A cell with given value filled in as a number representation</returns>
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
        /// <param name="cellType">The type of the Cell</param>
        /// <param name="dataRow">DataRow that contains the data for the row it's creating</param>
        /// <param name="rowIndex">Index for the row in the table</param>
        /// <returns>Returns dataRow with proper data filled in</returns>
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
        ///     Merges the cell with the given range in the given worksheet
        /// </summary>
        /// <param name="worksheet">Worksheet the mergecell belongs to</param>
        /// <param name="cell1Name">Start range of the merge cell</param>
        /// <param name="cell2Name">End range of the merge cell</param>
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
        ///     Deletes a sheet given the workbook the sheet is in and the sheetname;
        /// </summary>
        /// <param name="wbPart">Workbook the sheet is part of</param>
        /// <param name="sheetToDelete">The name of the sheet to be deleted</param>
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
                      .FirstOrDefault(s => s.Name == sheetToDelete);
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

        //Basic Code from OpenXML tutorial blog: http://blogs.msdn.com/b/brian_jones/
        //Customized to create data in more than one excel sheet
    }
}