using TugasAsinkron2.Interfaces;
using System.Data;
using ExcelDataReader;
using ClosedXML.Excel;

namespace TugasAsinkron2.Data
{
    public class ExcelHelper : IExcelHelper
    {
        private readonly IDataFunctions _dataFunctions;

        public ExcelHelper(IDataFunctions dataFunctions)
        {
            _dataFunctions = dataFunctions;
        }

        public DataTable ReadExcel(IFormFile excel)
        {
            Stream excelFileStream = excel.OpenReadStream();
            DataSet excelDataSet = null;
            using (var reader = ExcelReaderFactory.CreateReader(excelFileStream))
            {
                excelDataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                return excelDataSet.Tables[0];
            }
        }

        public async Task<string> ExportToExcel()
        {
            var data = await _dataFunctions.ReadProducts();
            string filename = Guid.NewGuid().ToString() + ".xlsx";
            using (var workbook = new XLWorkbook("Templates/Templates.xlsx"))
            {
                var ws = workbook.Worksheet(1);
                for (int i = 0; i < data.Count(); i++)
                {
                    //menuliskan setiap baris pada tabel produk ke file excel
                    string j = (i + 2).ToString();
                    ws.Cell("A" + j).SetValue(data[i].ProductName);
                    ws.Cell("B" + j).SetValue(data[i].SupplierId);
                    ws.Cell("C" + j).SetValue(data[i].UnitPrice);
                    ws.Cell("D" + j).SetValue(data[i].Package);
                    ws.Cell("E" + j).SetValue(data[i].IsDiscontinued);
                }
                workbook.SaveAs("Templates/" + filename);
            }
            return filename;
        }

        public async Task<string> ExportToExcelSupplier()
        {
            var data = await _dataFunctions.ReadSuppliers();
            string filename = Guid.NewGuid().ToString() + ".xlsx";
            using (var workbook = new XLWorkbook("Templates/Templates1.xlsx"))
            {
                var ws = workbook.Worksheet(1);
                for (int i = 0; i < data.Count(); i++)
                {
                    //menuliskan setiap baris pada tabel produk ke file excel
                    string j = (i + 2).ToString();
                    ws.Cell("A" + j).SetValue(data[i].CompanyName);
                    ws.Cell("B" + j).SetValue(data[i].ContactName);
                    ws.Cell("C" + j).SetValue(data[i].City);
                    ws.Cell("D" + j).SetValue(data[i].Country);
                    ws.Cell("E" + j).SetValue(data[i].Phone);
                }
                workbook.SaveAs("Templates/" + filename);
            }
            return filename;
        }
    }
}
