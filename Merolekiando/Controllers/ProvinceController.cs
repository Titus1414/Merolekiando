using Merolekiando.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Merolekiando.Controllers
{
    public class ProvinceController : Controller
    {
        private readonly MerolikandoDBContext _Context;
        public ProvinceController(MerolikandoDBContext Context)
        {
            _Context = Context;
        }
        public IActionResult Index()
        {
            ViewBag.Provices = _Context.Provinces.ToList();
            return View();
        }
    }
}
