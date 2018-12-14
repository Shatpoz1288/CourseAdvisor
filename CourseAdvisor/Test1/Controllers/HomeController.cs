using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Test1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About() => View();
       

        public ActionResult Contact()
        {
            ViewBag.Message = "If more information about the page is required, you can contact me at my email in the support section below.";

            return View();
        }
    }
}