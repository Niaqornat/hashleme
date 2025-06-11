using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace hashleme.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
