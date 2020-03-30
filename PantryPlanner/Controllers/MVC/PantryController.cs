using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PantryPlanner.Controllers.MVC
{
    public class PantryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}