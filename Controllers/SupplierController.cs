using TugasAsinkron2.Interfaces;
using TugasAsinkron2.Models;
using TugasAsinkron2.Models.DataContext;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TugasAsinkron2.Controllers
{
    public class SupplierController : Controller
    {
        private readonly IDataFunctions _dataFunctions;
        private readonly IExcelHelper _excelHelper;

        public SupplierController(IDataFunctions dataFunctions, IExcelHelper excelHelper)
        {
            _dataFunctions = dataFunctions;
            _excelHelper = excelHelper;
        }

        public async Task<IActionResult> Index()
        {
            List<SupplierViewModel> data = await _dataFunctions.ReadSuppliers();
            List<SupplierViewModel> model = new List<SupplierViewModel>();
            int totalPage = 1;
            int page = 1;

            if (data.Count() > 0)
            {
                totalPage = (data.Count() + 10) / 10;
                int start = ((page - 1) * 10);
                if (page != totalPage)
                    model = data.GetRange(start, 10);
                else
                    model = data.GetRange(start, data.Count() - start);
            }

            ViewBag.TotalPage = totalPage;
            ViewBag.CurrentPage = page;

            return View(model);
        }

        public IActionResult CreateSupplier()
        {
            return View();
        }

        public IActionResult AddCreateView()
        {
            return PartialView("~/Views/Supplier/_Partial/_SupplierForm.cshtml");
        }

        //Update fungsi CreateSupplier
        [HttpPost]
        public async Task<IActionResult> CreateSupplier(List<Supplier> suppliers)
        {
            try
            {
                foreach (var supplier in suppliers)
                {
                    await _dataFunctions.CreateSupplier(supplier);
                }
                return Json(new { success = true, redirectToUri = Url.Action("Index", "Supplier") });
            }
            catch (Exception e)
            {
                //Error handling
                return Json(new { success = false });
            }
        }


        //Jika fungsi dipanggil dari tombol btn-search, secara otomatis akan kembali ke halaman pertama.
        [HttpPost]
        public async Task<IActionResult> Search(string? kota, string? supplier, int page = 1)
        {
            // bagian pertama (Search)
            List<SupplierViewModel> data = await _dataFunctions.ReadSuppliers();

            // Check for null or empty kota and supplier before filtering
            if (!string.IsNullOrEmpty(kota))
            {
                data = data.Where(x => x.City.Contains(kota, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(supplier))
            {
                data = data.Where(x => x.CompanyName.Contains(supplier, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // bagian kedua (Pagination)
            List<SupplierViewModel> model = new List<SupplierViewModel>();
            int totalPage = 1;
            if (data.Count() > 0)
            {
                totalPage = (data.Count + 10 - 1) / 10;
                int start = (page - 1) * 10;
                if (page != totalPage)
                {
                    model = data.Skip(start).Take(10).ToList();
                }
                else
                {
                    model = data.Skip(start).ToList();
                }
            }

            ViewBag.TotalPage = totalPage;
            ViewBag.CurrentPage = page;

            return PartialView("~/Views/Supplier/_Partial/_Table.cshtml", model);
        }

        public async Task<IActionResult> Update(int Id)
        {
            List<SupplierViewModel> models = await _dataFunctions.ReadSupplier(Id);
            SupplierViewModel model = models.FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Supplier supplier)
        {
            await _dataFunctions.UpdateSupplier(supplier);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string kota, string supplier)
        {
            int page = 1;
            await _dataFunctions.DeleteSupplier(id);
            List<SupplierViewModel> data = await _dataFunctions.ReadSuppliers();

            // Check for null or empty kota and supplier before filtering
            if (!string.IsNullOrEmpty(kota))
            {
                data = data.Where(x => x.City.Contains(kota, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(supplier))
            {
                data = data.Where(x => x.CompanyName.Contains(supplier, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // bagian kedua (Pagination)
            List<SupplierViewModel> model = new List<SupplierViewModel>();
            int totalPage = 1;
            if (data.Count() > 0)
            {
                totalPage = (data.Count + 10 - 1) / 10;
                int start = (page - 1) * 10;
                if (page != totalPage)
                {
                    model = data.Skip(start).Take(10).ToList();
                }
                else
                {
                    model = data.Skip(start).ToList();
                }
            }

            ViewBag.TotalPage = totalPage;
            ViewBag.CurrentPage = page;

            return PartialView("~/Views/Supplier/_Partial/_Table.cshtml", model);
        }



    }
}
