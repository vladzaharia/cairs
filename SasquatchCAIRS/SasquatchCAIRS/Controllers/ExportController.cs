using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SasquatchCAIRS.Models;
//Everyone will need to download and install the OpenXML SDK, then add reference to the project.
// website:http://www.microsoft.com/en-us/download/details.aspx?id=5124
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace SasquatchCAIRS.Controllers
{
    public class ExportController : Controller
    {
      public void createXslFile(List<XSLContent> xslContents)
      {
          //SAMPLE CODE TO CREATE AN EXCEL FILE
          using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Create(
    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports.xlsx"),
    SpreadsheetDocumentType.Workbook)) {
              // create the workbook
              spreadSheet.AddWorkbookPart();
              spreadSheet.WorkbookPart.Workbook = new Workbook();     // create the worksheet
              spreadSheet.WorkbookPart.AddNewPart<WorksheetPart>();
              spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet = new Worksheet();

              // create sheet data
              spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.AppendChild(new SheetData());

              // create row
              spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());

              // create cell with data
              spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().First().AppendChild(
                    new Cell() {
                        CellValue = new CellValue("101")
                    });

              // save worksheet
              spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.Save();

              // create the worksheet to workbook relation
              spreadSheet.WorkbookPart.Workbook.AppendChild(new Sheets());
              spreadSheet.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet() {
                  Id = spreadSheet.WorkbookPart.GetIdOfPart(spreadSheet.WorkbookPart.WorksheetParts.First()),
                  SheetId = 1,
                  Name = "test"
              });

              spreadSheet.WorkbookPart.Workbook.Save();
          }
      }
    }
}
