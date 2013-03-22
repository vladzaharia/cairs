using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SasquatchCAIRS.Models;
using DataTable = System.Data.DataTable;
using Text = DocumentFormat.OpenXml.Spreadsheet.Text;

namespace SasquatchCAIRS.Controllers {
    public class ExcelExportController : Controller {

        //Basic Code from OpenXML tutorial: http://lateral8.com/articles/2010/3/5/openxml-sdk-20-export-a-datatable-to-excel.aspx
        //Customized to create data in more than one excel sheet
        #region Public Methods

        /// <summary>
        /// Given a list of datatables, and the reportyType, it generates a excel file
        /// using correct template and the data given
        /// </summary>
        /// <param name="reportType">Either Report or AuditLog from constants</param>
        /// <param name="tableList">list of the tables to be exported. for either type,
        ///     the number of tables in the list must not exceed 15</param>
        /// <param name="fromPath">template file path</param>
        /// <param name="toPath">working copy path</param>
        public void exportDataTable(Constants.ReportType reportType,
            List<DataTable> tableList, string fromPath, string toPath) {
            //Instead of creating a new excel file, let's use the template and make a copy to work with.
            System.IO.File.Copy(fromPath, toPath, true);

            if (tableList.Count > 0) {
                //populate the data into the spreadsheet
                using (SpreadsheetDocument spreadsheet =
                    SpreadsheetDocument.Open(toPath, true)) {
                    WorkbookPart workbook = spreadsheet.WorkbookPart;
                    //Workbook workbook = spreadsheet.WorkbookPart.Workbook;
                    IEnumerable<WorksheetPart> sheets =
                        //workbook.Descendants<WorksheetPart>();
                        workbook.GetPartsOfType<WorksheetPart>();
                    IEnumerator<WorksheetPart> enumerator = sheets.GetEnumerator();
                    bool movedNext = enumerator.MoveNext();

                    DataTable table;
                    SheetData data;
                    string sheetName;
                    WorksheetPart worksheetPart;

                    int i = 0;
                    while (i < tableList.Count() && movedNext && (i+1) < 15) {
                        table = tableList[i];
                        sheetName = "Sheet" + (i + 1).ToString();
                        worksheetPart = getWorksheetPart(workbook, sheetName);
                        
                        data = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                        // = enumerator.Current.Worksheet.GetFirstChild<SheetData>();

                        //add column names to the first row
                        var header = new Row();
                        header.RowIndex = (UInt32) 1;

                        foreach (DataColumn column in table.Columns) {
                            Cell headerCell = createTextCell(
                                table.Columns.IndexOf(column) + 1,
                                1,
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
                                    data.AppendChild(createContentRow(Constants.CellDataType.Number, contentRow, j + 2));
                                    break;
                                default:
                                    data.AppendChild(createContentRow(Constants.CellDataType.Text, contentRow, j + 2));
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
                                fixChartData(chartPart, table.Rows.Count + 1, table.Columns.Count);
                            }
                        }
                        
                        //Move to the next worksheetPart.
                        movedNext = enumerator.MoveNext();
                        i++;
                    }

                    //TODO(optional) delete unnecessary pages 
                }
            }

            string fileName = (reportType == Constants.ReportType.Report) ? "Reports.xlsx" : "AuditLog.xlsx";

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ";");
            response.TransmitFile(toPath);
            response.Flush();
            response.End();

            if (System.IO.File.Exists(toPath)) {
                System.IO.File.Delete(toPath);
            }
        }

        /// <summary>
        /// Finds the worksheetPart with given sheetName
        /// </summary>
        /// <param name="workbookPart">workbook part which contains the worksheet we are looking for</param>
        /// <param name="sheetName">name of the worksheet it's looking for</param>
        /// <returns>worksheetPart with given name</returns>
        private WorksheetPart getWorksheetPart(WorkbookPart workbookPart, string sheetName) {
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => sheetName.Equals(s.Name));
            if (sheet == null) {
                throw new Exception(
                    string.Format("Could not find a sheet with name {0}",
                                  sheetName));
            } else {
                return workbookPart.GetPartById(sheet.Id) as WorksheetPart;
            }
        }

        /// <summary>
        /// given a chartPart, which has fixed data range from the template
        /// this method corrects the XML so the chart covers the correct range of data
        /// </summary>
        /// <param name="chartPart">Chartpart to be fixed</param>
        /// <param name="totalRowCount">total number of rows of the data</param>
        /// <param name="totalColCount">total nu</param>
        private void fixChartData(ChartPart chartPart, int totalRowCount, int totalColCount) {
            //Get the appropriate chart part from template file.
            //ChartPart chartPart = workbookPart.ChartsheetParts.First().DrawingsPart.ChartParts.First();
            //Change the ranges to accomodate the newly inserted data.
            if (chartPart != null) {
                foreach (DocumentFormat.OpenXml.Drawing.Charts.Formula formula in chartPart.ChartSpace.Descendants<DocumentFormat.OpenXml.Drawing.Charts.Formula>()) {
                    //if (formula.Text.Contains("$3")) {
                    //    TODO replacing does not work -> need to create new formulas for rows...
                    //    formula.Text = formula.Text.Replace("3", totalRowCount.ToString());
                    //}

                    if (formula.Text.Contains("$D")) {
                        //string s = formula.Text.Split('$')[1];
                        //formula.Text += ":$" + s + "$" + (totalRowCount+1);
                        formula.Text = formula.Text.Replace("D", getColumnName(totalColCount));
                    }
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
        /// creates a Cell for the given column+row index and the value 
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
        /// creates a Cell for the given column+row index and the value 
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
        /// Creates a content row for the given dataRow
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
                if (i>0) {
                    switch (cellType) {
                        case Constants.CellDataType.Number:
                            dataCell = createNumCell(i + 1, rowIndex, dataRow[i]);
                            break;
                        default:
                            dataCell = createTextCell(i + 1, rowIndex, dataRow[i]);
                            break;
                    }
                } else {
                    dataCell = createTextCell(i + 1, rowIndex, dataRow[i]);
                }
                row.AppendChild(dataCell);
            }
            return row;
        }

        #endregion
    }
}