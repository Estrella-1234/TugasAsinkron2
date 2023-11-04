using TugasAsinkron2.Interfaces;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace TugasAsinkron2.Controllers
{
    public class ExcelController : Controller
    {
        private readonly IExcelHelper _excelHelper;
        private readonly IWebHostEnvironment _host;

        public ExcelController(IExcelHelper excelHelper, IWebHostEnvironment host)
        {
            _excelHelper = excelHelper;
            _host = host;
        }

        public IActionResult Index()
        {
            ViewBag.Data = null;
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile excel)
        {
            DataTable data = _excelHelper.ReadExcel(excel);
            ViewBag.Data = data;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Export()
        {
            string filename = await _excelHelper.ExportToExcel();
            var path = _host.ContentRootPath + "/Templates/" + filename;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            //File yang kita buat tadi sementara disimpan pada folder templates, setelah sukses dibaca di sistem, hapus file tersebut
            //sehingga tidak memenuhi directory project
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Product " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx");
        }
    }
}
