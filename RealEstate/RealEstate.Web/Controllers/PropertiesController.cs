using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IPropertiesService propertiesService;

        public PropertiesController(IPropertiesService propertiesService)
        {
            this.propertiesService = propertiesService;
        }

        public IActionResult Search()
        {
            return View();
        }
        public IActionResult DoSearch(int minPrice, int maxPrice)
        {
            var result = propertiesService.Search(minPrice, maxPrice, 0, int.MaxValue);
            return View(result);
        }
    }
}
