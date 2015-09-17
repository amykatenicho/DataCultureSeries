using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNet.Highcharts;

namespace DocumentDBWebApp.Models
{
    public class HomeIndexViewModel
    {
        public HomeIndexViewModel()
        {
          
        }

        public Highcharts[] Chart { get; set; }
    }
}