using TugasAsinkron2.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TugasAsinkron2.Controllers
{
    public class OrderController : Controller
    {
        private IDataFunctions _dataFunctions;
        public OrderController(IDataFunctions dataFunctions)
        {
            _dataFunctions = dataFunctions;
        }

        public IActionResult Index(string? msg)
        {
            ViewBag.msg = msg;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int Id)
        {
            string msg = await _dataFunctions.DeleteOrderTransact(Id);
            return RedirectToAction("Index", new { msg });
        }
    }
}
