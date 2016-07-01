using System;
using System.Collections.Generic;
using System.Linq;
using Braz.Models;
using System.Web.Mvc;

namespace Braz.Controllers
{
    public class VacancyController : Controller
    {
        //
        // GET: /Vacancy/

        public ActionResult Index()
        {
            string path = Request.Path.Substring(Request.Path.LastIndexOf('/')+1);
            if (Request.Path.ToLower() == "/vacancy")
            {
                ViewData["Vacancies"] = Vacancy.GetVacancies();
                return View();
            }
            Vacancy result = Vacancy.GetVacancy(path);
            if (result != null)
                return View("Vacancy", result);
            else
                return View("404");
        }

    }
}
