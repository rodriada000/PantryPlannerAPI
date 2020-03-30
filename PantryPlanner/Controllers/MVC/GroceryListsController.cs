using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PantryPlanner.Controllers.MVC
{
    public class GroceryListsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}