using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NicolaPIermatteiWec.Models;
using NicolaPIermatteiWec.Models.ViewModels;
using NicolaPIermatteiWec.Services;

namespace NicolaPIermatteiWec.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataAccess _Da;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IDataAccess dataAccess)
        {
            _logger = logger;
            _Da = dataAccess;
        }

        public async Task<IActionResult> Tables()
        {
            try
            {
                var res = new FirstPageModel();
                res.DistanceByProvinces = await _Da.GetDistanceByProvinceTableData();
                res.TypeOfRelevations = await _Da.GetTypeOfRelevationsData();
                res.TopTenDevices = await _Da.GetTopTenDevicesData();
                res.TopTenByScores = await _Da.GetTopTenByScoresData();
                return View(res);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> DbTables()
        {
            try
            {
                var res = await _Da.ResultForTables();
                return View(res);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult Graph()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteData()
        {
            if (await _Da.ClearData())
            {
                return RedirectToAction(nameof(DbTables));
            }
            else
            {
                //Implementare pagina eccezione
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
