using TugasAsinkron2.Interfaces;
using TugasAsinkron2.Models;
using TugasAsinkron2.Models.DataContext;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TugasAsinkron2.Controllers
{
	public class ProductController : Controller
	{
		private readonly IDataFunctions _dataFunctions;
		private readonly IExcelHelper _excelHelper;

		public ProductController(IDataFunctions dataFunctions, IExcelHelper excelHelper)
		{
			_dataFunctions = dataFunctions;
			_excelHelper = excelHelper;
		}

		public async Task<IActionResult> Index()
		{
			List<ProductViewModel> data = await _dataFunctions.ReadProducts();
			List<ProductViewModel> model = new List<ProductViewModel>();
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

		[HttpPost]
		public async Task<IActionResult> Index(IFormFile excel)
		{
			DataTable data = _excelHelper.ReadExcel(excel);
			string result = await _dataFunctions.BulkInsertProduct(data);

			if (result == "Success")
			{
				return RedirectToAction("Index");
			}
			else
			{
				return NoContent();
			}
		}

		public IActionResult CreateProduct()
		{
			return View();
		}


		//Update fungsi CreateProduct
		[HttpPost]
		public async Task<IActionResult> CreateProduct(List<Product> products)
		{
			try
			{
				foreach (var product in products)
				{
					await _dataFunctions.CreateProduct(product);
				}
				return Json(new { success = true, redirectToUri = Url.Action("Index", "Product") });
			}
			catch (Exception e)
			{
				//Error handling
				return Json(new { success = false });
			}
		}

		public async Task<IActionResult> Update(int Id)
		{
			List<ProductViewModel> models = await _dataFunctions.ReadProduct(Id);
			ProductViewModel model = models.FirstOrDefault();
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Update(Product product)
		{
			await _dataFunctions.UpdateProduct(product);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int Id, string product, string supplier)
		{
			int page = 1;
			try
			{
				await _dataFunctions.DeleteProduct(Id);
                List<ProductViewModel> data = await _dataFunctions.ReadProducts();
                data = string.IsNullOrEmpty(product) ? data : data.Where(x => x.ProductName.Contains(product, StringComparison.OrdinalIgnoreCase)).ToList();
                data = string.IsNullOrEmpty(supplier) ? data : data.Where(x => x.CompanyName.Contains(supplier, StringComparison.OrdinalIgnoreCase)).ToList();

                List<ProductViewModel> model = new List<ProductViewModel>();
                int totalPage = 1;
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

                return PartialView("~/Views/Product/_Partial/_Table.cshtml", model);
			}
			catch (Exception e)
			{
				// Log the exception details or inspect it for debugging
				Console.WriteLine(e);
				return Json(new { success = false, message = e.Message });
			}

		}

		public IActionResult AddCreateView()
		{
			return PartialView("~/Views/Product/_Partial/_ProductForm.cshtml");
		}

		[HttpPost]
		public JsonResult SupplierAc(string prefix)
		{
			List<Supplier> supplier = new List<Supplier>();
			//Kita gunakan fungsi EF Core yang sudah pernah dibuat untuk mengambil data dari tabel Supplier
			using (var db = new DataContext())
			{
				supplier = db.Suppliers.ToList();
			}
			var suggestion = supplier.Where(x => x.CompanyName.Contains(prefix, StringComparison.OrdinalIgnoreCase)).Select(x => x.CompanyName).ToList();
			return Json(suggestion);
		}

		[HttpPost]
		//Jika fungsi dipanggil dari tombol btn-search, secara otomatis akan kembali ke halaman pertama.
		public async Task<IActionResult> Search(string product, string supplier, int page = 1)
		{
			// bagian pertama (Search)
			List<ProductViewModel> data = await _dataFunctions.ReadProducts();
			data = string.IsNullOrEmpty(product) ? data : data.Where(x => x.ProductName.Contains(product, StringComparison.OrdinalIgnoreCase)).ToList();
			data = string.IsNullOrEmpty(supplier) ? data : data.Where(x => x.CompanyName.Contains(supplier, StringComparison.OrdinalIgnoreCase)).ToList();

			// bagian kedua (Pagination)
			List<ProductViewModel> model = new List<ProductViewModel>();
			int totalPage = 1;
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

			return PartialView("~/Views/Product/_Partial/_Table.cshtml", model);
		}
	}
}
