using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DocumentDBWebApp.Models;
using DotNet.Highcharts;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;


namespace DocumentDBWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var helper = new HighchartsHelper();
            var model = new HomeIndexViewModel();

            model.Chart =new[]
            {
                helper.GetTemperatureChart(),
                helper.GetEnergyChart(),
                helper.GetLightChart(),
                helper.GetHumidityChart()
            };

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}