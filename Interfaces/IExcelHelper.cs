using System.Data;

namespace TugasAsinkron2.Interfaces
{
    public interface IExcelHelper
    {
        public DataTable ReadExcel(IFormFile excel);
        public Task<string> ExportToExcel();

    }
}
