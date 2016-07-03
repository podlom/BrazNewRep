using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;


namespace Braz
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            PreRegisterGlobalVariables();
            ReadGlobalSetting();
            ReadLocalization();
            PostRegisterGlobalVariables();
        }
        protected void Session_Start()
        {
            //Crawler protect
            List<string> ips = new List<string>() { "66.249.79.145" };
            if (ips.Contains(Request.UserHostAddress))
                Session.Timeout = 1;
            //Increment session counter
            Application["ActiveSessionCount"] = ((int)Application["ActiveSessionCount"]) + 1;

            //Language cookies
            if ((Request.Cookies["lang"] == null)||(Request.Cookies["lang"].Value==null))
            {
                Session["lang"] = "Русский";
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
                Response.Cookies.Add(new HttpCookie("lang", "Русский"));
            }
            else
            {
                if (((List<string>)Application["Languages"]).Contains(Request.Cookies["lang"].Value))
                {
                    Session["lang"] = Request.Cookies["lang"].Value;
                    if (Request.QueryString["Lang"] == "English") System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    else System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
                }
                else
                {
                    Session["lang"] = "Русский";
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
                }
            }
            //Login cookies
            if (Request.Cookies["id"] != null && Request.Cookies["id"].Value != null)
            {
                if ((Session["User"] = Models.User.GetUser(int.Parse(Request.Cookies["id"].Value))) == null)
                {
                    Session["Logged"] = false;
                }
                else
                {
                    Session["Logged"] = true;
                }
            }
            else
            {
                Session["Logged"] = false;
            }

            //Logger
            string path = (string)Application["LogFile"];
            try
            {
                System.IO.StreamWriter writer = System.IO.File.AppendText(path);
                System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("ru-RU");
                writer.WriteLine(Request.UserHostAddress + " - " + System.DateTime.UtcNow.AddHours(3).ToString(info)+" Active sessions: "+((int)Application["ActiveSessionCount"]).ToString());
                writer.Close();
            }
            catch(System.Exception) { }
        }
        
        protected void Session_End()
        {
            //Decrement session counter
            
            Application["ActiveSessionCount"] = ((int)Application["ActiveSessionCount"]) - 1;
        }

        void PreRegisterGlobalVariables()
        {
            //Log file
            Application["LogFile"] = Server.MapPath("~/log.txt");

            //Active session counter
            Application["ActiveSessionCount"] = 0;

            //Available languages
            Application["Languages"] = new List<string>() { "Русский", "English" };

            //Set Admin
            Application["Admin"] = new Models.User(true);

            Application["ProductionCategories"] = new List<string>()
            {
                { "litejnoe_proizvodstvo" },
                { "ekstruzionnoe_proizvodstvo"},
                { "cex_anodnogo_pokrytiya"},
                { "cex_polimernogo_pokrytiya"},
                { "cex_mexanicheskoj_obrabotki" },
                { "cex_obrabotki_poverxnosti" },
                { "instrumentalnyj_cex" },
                { "laboratoriya"},
                { "uchastok_upakovki" }
            };
        }
        void ReadGlobalSetting()
        {
            string query = "SELECT * FROM ApplicationData";
            using (Braz.Models.DbConnect db = new Models.DbConnect())
            {
                Application["GeneralSettings"] = db.ReadSettings(query);
            }
        }
        void ReadLocalization()
        {
            string query = "SELECT * FROM Localization";
            using (Braz.Models.DbConnect db = new Models.DbConnect())
            {
                Application["Localization"] = db.ReadLocalization(query);
            }
        }
        void PostRegisterGlobalVariables()
        {
            //Catalog Categories
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach(KeyValuePair<string,Dictionary<int,Dictionary<string,string>>> pair in (Dictionary<string, Dictionary<int, Dictionary<string, string>>>)Application["Localization"])
            {
                List<string> tlist = new List<string>();
                foreach(KeyValuePair<int,Dictionary<string,string>> pair2 in pair.Value.Where(x=>x.Key==4))
                {
                    foreach (KeyValuePair<string, string> pair3 in pair2.Value.Where(x => x.Key.Substring(0, System.Math.Min(8,x.Key.Length)) == "SuperCat"))
                    {
                        tlist.Add(pair3.Value);
                    }
                }
                result.Add(pair.Key, tlist);
            }
            Application["ParentCategories"] = result;
        }
    }

    public struct GeneralSetting
    {
        public string Identifier;
        public string Value;
        public string Display;
    }

}