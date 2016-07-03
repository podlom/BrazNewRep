using System.Web.Mvc;
using System.Collections.Generic;

namespace Braz.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        //
        public ActionResult Index()
        {
            return View("Index");
        }
        //Check Admin login attempt
        public ActionResult CheckAdmin(FormCollection data)
        {
            if ((data["email"] == ((Braz.Models.User)HttpContext.Application["Admin"]).Email) && (data["password"] == ((Braz.Models.User)HttpContext.Application["Admin"]).Password))
            {
                Session["User"] = (Braz.Models.User)HttpContext.Application["Admin"];
                return Redirect("/Admin");
            }
            return PartialView("LogFail");
        }
        [AdminFilter]
        public ActionResult Posts()
        {
            ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"]);
            ViewData["PostList"] = Models.Post.GetPosts();
            return View();
        }
        [AdminFilter]
        public ActionResult Log()
        {
            byte[] data = System.IO.File.ReadAllBytes((string)HttpContext.Application["LogFile"]);
            ViewData["Logs"] = System.Text.UnicodeEncoding.UTF8.GetString(data);
            return View();
        }
        [AdminFilter]
        public ActionResult Settings()
        {
            ViewData["Settings"] = (System.Collections.Generic.List<GeneralSetting>)HttpContext.Application["GeneralSettings"];
            return View();
        }
        [AdminFilter]
        public ActionResult SaveSettings(FormCollection data)
        {
            System.Collections.Generic.List<GeneralSetting> result = new System.Collections.Generic.List<GeneralSetting>();
            string query = "";
            foreach (GeneralSetting set in (System.Collections.Generic.List<GeneralSetting>)HttpContext.Application["GeneralSettings"])
            {
                query += "UPDATE ApplicationData SET Value = '" + data[set.Identifier] + "' WHERE Identifier='" + set.Identifier + "';";
                result.Add(new GeneralSetting() { Identifier = set.Identifier, Value = data[set.Identifier], Display = set.Display });
            }
            HttpContext.Application["GeneralSettings"] = result;
            using (Braz.Models.DbConnect db = new Models.DbConnect())
            {
                db.Update(query);
            }
            return Redirect("/Admin");
        }
        [AdminFilter]
        [HttpPost, ValidateInput(false)]
        public ActionResult AddPost(FormCollection data)
        {
            List<System.Web.HttpPostedFileBase> FileList = new List<System.Web.HttpPostedFileBase>();
            FileList.Add(Request.Files["index_image"]);
            FileList.Add(Request.Files["list_image"]);
            FileList.Add(Request.Files["post_image"]);
            Dictionary<string, string> titles = new Dictionary<string, string>();
            Dictionary<string, string> texts = new Dictionary<string, string>();
            foreach (string lang in (List<string>)HttpContext.Application["Languages"])
            {
                titles.Add(lang, data[lang + "-title"]);
                texts.Add(lang, data[lang + "-text"]);
            }
            int id = Models.Post.Add(titles, texts, System.DateTime.Now, FileList);
            return Redirect("/Admin/Posts");
        }
        [AdminFilter]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditPost(FormCollection data)
        {
            System.DateTime? NewDate = new System.DateTime(System.Int32.Parse(data["year"]), System.Int32.Parse(data["month"]), System.Int32.Parse(data["day"]));
            System.DateTime OldDate = System.DateTime.Parse(Request.QueryString["olddate"]);
            if (OldDate == NewDate)
                NewDate = null;
            Dictionary<string, string> titles = new Dictionary<string, string>();
            Dictionary<string, string> texts = new Dictionary<string, string>();
            foreach (string lang in (List<string>)HttpContext.Application["Languages"])
            {
                titles.Add(lang, data[lang + "-title"]);
                texts.Add(lang, data[lang + "-text"]);
            }
            System.Collections.Generic.List<System.Web.HttpPostedFileBase> FileList = new System.Collections.Generic.List<System.Web.HttpPostedFileBase>();
            FileList.Add(Request.Files["index_image"]);
            FileList.Add(Request.Files["list_image"]);
            FileList.Add(Request.Files["post_image"]);
            Models.Post.Update(int.Parse(Request.QueryString["post"]), titles, texts, FileList, NewDate);
            return Redirect("/admin/posts");
        }
        [AdminFilter]
        public ActionResult DeletePost()
        {
            int id = int.Parse(Request.QueryString["post"]);
            Models.Post.Delete(id);
            return Redirect("/Admin/Posts");
        }
        [AdminFilter]
        public ActionResult Vacancies()
        {
            ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"]);
            return View(Models.Vacancy.GetVacancies());
        }
        [AdminFilter]
        public ActionResult AddVacancy(FormCollection data)
        {
            Dictionary<string,List<string>> require = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> duty = new Dictionary<string, List<string>>();
            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, string> descr = new Dictionary<string, string>();
            int req = int.Parse(Request.QueryString["req"]);
            int dut = int.Parse(Request.QueryString["dut"]);
            foreach(string lang in (List<string>)HttpContext.Application["Languages"])
            {
                header.Add(lang,data[lang+"-header"]);
                descr.Add(lang,data[lang+"-descr"]);
                List<string> temp = new List<string>();
                for (int i = 0; i < req; i++)
                    temp.Add(data[lang+"-req" + i.ToString()]);
                require.Add(lang, temp);
                temp = new List<string>();
                for (int i = 0; i < dut; i++)
                    temp.Add(data[lang+"-dut" + i.ToString()]);
                duty.Add(lang, temp);
            }
            string url = data["url"];
            string salary = data["salary"];
            int type = data["type"] == "Офис" ? 0 : 1;
            Braz.Models.Vacancy.Create(header, descr, salary, require, duty, url, type);
            return Redirect("/Admin/Vacancies");
        }
        [AdminFilter]
        public ActionResult UpdateVacancy(FormCollection data)
        {
            Dictionary<string, List<string>> require = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> duty = new Dictionary<string, List<string>>();
            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, string> descr = new Dictionary<string, string>();
            string url = data["url"];
            string salary = data["salary"];
            int type = data["type"] == "Офис" ? 0 : 1;
            int id = int.Parse(Request.QueryString["vac"]);
            int req = int.Parse(Request.QueryString["req"]);
            int dut = int.Parse(Request.QueryString["dut"]);
            foreach(string lang in (List<string>)HttpContext.Application["Languages"])
            {
                header.Add(lang, data[lang + "-header"]);
                descr.Add(lang, data[lang + "-descr"]);
                List<string> result = new List<string>();
                for (int i = 0; i < req; i++)
                    result.Add(data[lang+"-req" + i.ToString()]);
                require.Add(lang, result);
                result = new List<string>();
                for (int i = 0; i < dut; i++)
                    result.Add(data[lang+"-dut" + i.ToString()]);
                duty.Add(lang, result);
            }
            Braz.Models.Vacancy.Update(id, header, descr, url, salary, type, require, duty);
            return Redirect("/Admin/Vacancies");
        }

        [AdminFilter]
        public ActionResult DeleteVacancy()
        {
            int id = int.Parse(Request.QueryString["vacid"]);
            Braz.Models.Vacancy.DeleteVacancy(id);
            return Redirect("/Admin/Vacancies");
        }
    }
}
