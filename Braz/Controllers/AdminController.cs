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
            int id = Models.Post.Add(data["title"], data["text"], System.DateTime.Now, FileList);
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
            string title = data["title"];
            string text = data["text"];
            System.Collections.Generic.List<System.Web.HttpPostedFileBase> FileList = new System.Collections.Generic.List<System.Web.HttpPostedFileBase>();
            FileList.Add(Request.Files["index_image"]);
            FileList.Add(Request.Files["list_image"]);
            FileList.Add(Request.Files["post_image"]);
            Models.Post.Update(int.Parse(Request.QueryString["post"]), title, text.Replace("\n", "<br>").Replace("'", "[0]"), FileList, NewDate);
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
            return View(Models.Vacancy.GetVacancies());
        }
        [AdminFilter]
        public ActionResult AddVacancy(FormCollection data)
        {
            List<string> require = new List<string>();
            List<string> duty = new List<string>();
            int req = int.Parse(Request.QueryString["req"]);
            int dut = int.Parse(Request.QueryString["dut"]);
            string header = data["header"];
            string descr = data["descr"];
            string url = data["url"];
            string salary = data["salary"];
            int type = data["type"] == "Офис" ? 0 : 1;
            for(int i=0;i<req;i++)
                require.Add(data["req" + i.ToString()]);
            for (int i = 0; i < dut; i++)
                duty.Add(data["dut" + i.ToString()]);
            Braz.Models.Vacancy.Create(header, descr, salary, require, duty, url, type);
            return Redirect("/Admin/Vacancies");
        }
        [AdminFilter]
        public ActionResult UpdateVacancy(FormCollection data)
        {
            string header = data["header"];
            string descr = data["descr"];
            string url = data["url"];
            string salary = data["salary"];
            int type = data["type"] == "Офис" ? 0 : 1;

            List<string> require = new List<string>();
            List<string> duty = new List<string>();
            int id = int.Parse(Request.QueryString["vac"]);
            int req = int.Parse(Request.QueryString["req"]);
            int dut = int.Parse(Request.QueryString["dut"]);
            for (int i = 0; i < req; i++)
                require.Add(data["req" + i.ToString()]);
            for (int i = 0; i < dut; i++)
                duty.Add(data["dut" + i.ToString()]);
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
