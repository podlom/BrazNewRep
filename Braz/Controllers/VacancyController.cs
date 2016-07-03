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
            if (Request.Path.ToLower() == "/vacancy"||Request.Path.ToLower()=="/vacancy/")
            {
                ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string,string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][12];
                ViewData["Vacancies"] = Vacancy.GetVacancies();
                ViewData["VacTrans"]= ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][13];
                return View();
            }
            Vacancy result = Vacancy.GetVacancy(path);
            if (result != null)
            {
                ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][13];
                return View("Vacancy", result);
            }
            else
                return View("404");
        }

    }
}
