using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using Reservations.Classes.Application.Home;
using Reservations.Models;
using Reservations.Models.Queries;
using Reservations.ViewModels.Admin;

namespace Reservations.Classes.Utils
{
    public class ExcelManager
    {

        public MemoryStream ExportQuery1ToExcel()
        {
            var stream = new MemoryStream();
            DbManager dbHome = new DbManager();
            List<Querie1> results = dbHome.GetQueriesQuery1();

            using (ExcelPackage package = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Report");
                ApplicationHome app = new ApplicationHome();
                var listCollection = app.GetQuery1Data();
                var dataRange = ws.Cells["B2"].LoadFromCollection<QueryOneVM>(listCollection, true);
                dataRange.AutoFitColumns();

                package.SaveAs(stream);

                stream.Position = 0;
            }

            return stream;
        }

        public MemoryStream ExportQuery2ToExcel()
        {
            var stream = new MemoryStream();
            DbManager dbHome = new DbManager();

            List<Querie2> results = dbHome.GetQueriesQuery2();

            using (ExcelPackage package = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Report");
                ApplicationHome app = new ApplicationHome();
                var listCollection = app.GetQuery2Data();

                var dataRange = ws.Cells["B2"].LoadFromCollection<QueryTwoVM>(listCollection, true);
                dataRange.AutoFitColumns();

                package.SaveAs(stream);

                stream.Position = 0;
            }

            return stream;
        }

        public MemoryStream ExportQuery3ToExcel()
        {
            var stream = new MemoryStream();
            DbManager dbHome = new DbManager();

            List<Querie3> results = dbHome.GetQueriesQuery3();

            using (ExcelPackage package = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Report");
                ApplicationHome app = new ApplicationHome();
                var listCollection = app.GetQuery3Data();

                var dataRange = ws.Cells["B2"].LoadFromCollection<QueryThreeVM>(listCollection, true);
                dataRange.AutoFitColumns();

                package.SaveAs(stream);

                stream.Position = 0;
            }

            return stream;
        }

    }
}