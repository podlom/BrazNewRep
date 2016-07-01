using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;


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

            RegisterGlobalVariables();
            ReadGlobalSetting();
            ReadLocalization();
        }
        protected void Session_Start()
        {
            //Increment session counter
            Application["ActiveSessionCount"] = ((int)Application["ActiveSessionCount"]) + 1;

            //Language cookies
            if ((Request.Cookies["lang"] == null)||(Request.Cookies["lang"].Value==null))
            {
                Session["lang"] = "Русский";
                Response.Cookies.Add(new HttpCookie("lang", "Русский"));
            }
            else
            {
                if (((List<string>)Application["Languages"]).Contains(Request.Cookies["lang"].Value))
                {
                    Session["lang"] = Request.Cookies["lang"].Value;
                }
                else
                {
                    Session["lang"] = "Русский";
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

        void RegisterGlobalVariables()
        {
            //Log file
            Application["LogFile"] = Server.MapPath("~/log.txt");

            //Active session counter
            Application["ActiveSessionCount"] = 0;

            //Available languages
            Application["Languages"] = new List<string>() { "Русский", "English" };

            //Set Admin
            Application["Admin"] = new Models.User(true);

            //Catalog Categories
            Application["ParentCategories"] = new List<string>() {
                "Полоса", //0
                "Трубы алюминиевые", //1
                "Уголок", //2
                "Пруток", //3
                "Прочие профили", //4
                "Профиль электротехнический", //5
                "Профиль для декоративных работ", //6
                "Профиль для светопрозрачных конструкций", //7
                "Строительный профиль", //8
                "Профиль для натяжных потолков", //9
                "Специализированные профили", //10
                "Мебельный профиль", //11
                "Торгово-выставочные системы", //11
                "Решетки алюминиевые", //12
                "Профиль для конструкций теплиц", //13
                "Профили для рекламных конструкций", //14
                "Отлив алюминиевый", //15
                "Соединительные профили" //16
            };

            Application["ProductionCategories"] = new Dictionary<string, string>()
            {
                { "litejnoe_proizvodstvo","Литейное производство" },
                { "ekstruzionnoe_proizvodstvo", "Экструзионное производство" },
                { "cex_anodnogo_pokrytiya", "Цех анодного покрытия" },
                { "cex_polimernogo_pokrytiya","Цех полимерного покрытия" },
                { "cex_mexanicheskoj_obrabotki","Цех механической обработки" },
                { "cex_obrabotki_poverxnosti","Цех обработки поверхности" },
                { "instrumentalnyj_cex","Инструментальный цех" },
                { "laboratoriya","Лаборатория" },
                { "uchastok_upakovki","Участок упаковки" }
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
    }

    public struct GeneralSetting
    {
        public string Identifier;
        public string Value;
        public string Display;
    }

}