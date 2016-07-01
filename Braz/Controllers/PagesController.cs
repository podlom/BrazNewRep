using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Braz.Controllers
{
    public class PagesController : Controller
    {
        //MAIN VIEW
        public ActionResult Index()
        {
            ViewData["Translation"] = ((Dictionary<string,Dictionary<int,Dictionary<string,string>>>)HttpContext.Application["Localization"])[(string)Session["lang"]][0];
            ViewData["Posts"] = Models.Post.GetPosts();
            return View();
        }
        //PROFILE VIEW 
        public ActionResult Profile()
        {
            if(!(bool)Session["Logged"])
                return Redirect("~/");
            ViewData["User"] = Session["User"];
            ViewData["History"] = Models.Order.GetUserOrders(((Models.User)Session["User"]).Id);
            return View("Profile");
        }
        public ActionResult Story()
        {
            return View("Story");
        }
        public ActionResult Services()
        {
            return View("Services");
        }
        public ActionResult Production(string income)
        {
            string k = Request.Path.Substring(Request.Path.LastIndexOf('/') + 1);
            ViewData["Page"] = k;
            return View("Production");
        }
        public ActionResult Statistics()
        {
            return View();
        }
        public ActionResult Contacts()
        {
            return View();
        }
        public ActionResult Group()
        {
            string path = Request.RawUrl;
            path = path.Substring(path.LastIndexOf('/') + 1);
            ViewData["Section"] = path;
            return View();
        }
        public ActionResult chasto_zadavaemye_voprosy()
        {
            return View();
        }
        //404 VIEW
        public ActionResult NotFound()
        {
            return View("~/Views/Shared/404.cshtml");
        }
        //LANGUAGE CHANGE
        public ActionResult ChangeLang()
        {
            Session["lang"] = Request.QueryString["Lang"];
            HttpCookie cookie = Request.Cookies["lang"];
            if (cookie != null)
                Response.Cookies["lang"].Value = Request.QueryString["Lang"];   
            else
            {
                cookie = new HttpCookie("lang");
                cookie.HttpOnly = false;
                cookie.Value = Request.QueryString["Lang"];
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return Redirect("/");
        }

        
        //CHANGE VIDEO
        /*public ActionResult UploadVideo()
        {
            HttpPostedFileBase file = Request.Files["index-video"];
            if (file.ContentLength == 0)
            {
                return Redirect("/");
            }
            string path=Server.MapPath("/Content/images/index-video.mp4");
            if(System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            file.SaveAs(path);
            return Redirect("/");
        }*/

        //LOGIN ATTEMPT
        public ActionResult CheckUser(FormCollection data)
        {
            Models.User User = Models.User.GetUser(data["email"], data["password"]);
            if(User!=null)
            {
                Session["Logged"] = true;
                Session["User"] = User;
                Response.Cookies.Add(new HttpCookie("id", ((Braz.Models.User)Session["User"]).Id.ToString()));
                return JavaScript("location.replace('pages/profile')");
            }
            return PartialView("LogFail");
        }
        //REGISTER NEW USER
        public ActionResult AddUser(FormCollection data)
        {
            Braz.Models.User toAdd = Models.User.Insert(data["name"], data["password"], data["phone"], data["email"]);
            Session["User"] = toAdd;
            Session["Logged"] = true;
            Response.Cookies.Add(new HttpCookie("id", toAdd.Id.ToString()));
            return Redirect("/Pages/Profile");
        }
        //EXIT
        public ActionResult Exit()
        {
            Session["Logged"]=false;
            Session["User"] = null;
            Response.Cookies.Remove("id");
            return Redirect("/");
        }
        

        public ActionResult ChangeUserData(FormCollection data)
        {
            ((Models.User)Session["User"]).Update(data["name"], data["email"], data["phone"], data["password"]);
            return Redirect("/Pages/Profile");
        }
       
        public ActionResult SubmitOrder(FormCollection data)
        {
            /*ViewData["IsConfirmed"] =*/ ((Models.User)Session["User"]).SubmitOrder(data["comment"]);
            
            return Redirect("/Pages/Profile");
        }
        public ActionResult SendFeedback(FormCollection data)
        {
            string message = "Имя - " + data["name"] + Environment.NewLine;
            message += "Телефон - " + data["phone"] + Environment.NewLine;
            message += "Почта - " + data["email"] + Environment.NewLine;
            message += "Сообщение - " + data["message"];
            System.Net.Mail.SmtpClient client= new System.Net.Mail.SmtpClient("mail.ukraine.com.ua", 25);
            string reciever = ((List<GeneralSetting>)System.Web.HttpContext.Current.Application["GeneralSettings"]).Find(x => x.Identifier == "FeedbackReciever").Value;
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage("site@braz.com.ua", reciever);
            msg.Subject = "Новый отзыв";
            msg.Body = message;
            client.Port = 2525;
            client.EnableSsl = false;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("site@braz.com.ua", "3xxu9LY7S4Um");
            ViewData["IsSend"] = true;
            try
            {
                client.Send(msg);
            }
            catch(Exception e)
            {
                string path = (string)HttpContext.Application["LogFile"];
                System.IO.StreamWriter writer = System.IO.File.AppendText(path);
                System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("ru-RU");
                writer.WriteLine("[ERROR] "+e.Message);
                writer.Close();
                ViewData["IsSend"] = false;
            }
            return PartialView("FeedbackSended");
        }







        
    }
}
