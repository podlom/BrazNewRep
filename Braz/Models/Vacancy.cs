using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Braz.Models
{
    public class Vacancy
    {
        public int Id { get; }
        public string Header { get; set; }
        public string Description { get; set; }
        public string Salary { get; set; }
        public string Url { get; set; }
        public int Type { get; set; }
        public List<string> Requirements { get; set; }
        public List<string> Duties { get; set; }
        public Vacancy(int id) { Id = id; }
        public static int Create(Dictionary<string,string> head,Dictionary<string,string> descr,string salary,Dictionary<string,List<string>> req, Dictionary<string,List<string>> duty,string url,int type)
        {
            string requirements = "", duties = "";
            foreach (string data in req["Русский"])
            {
                requirements += data + "|";
            }
            requirements = requirements.Substring(0, requirements.Length - 1);
            foreach (string data in duty["Русский"])
            {
                duties += data + "|";
            }
            duties = duties.Substring(0, duties.Length - 1);
            string query2 = "INSERT INTO vacancies(Header,Description,Salary,Url,Type,Requirements,Duties) VALUES('" + head["Русский"] + "','" + descr["Русский"] + "','" + salary + "','" + url + "'," + type.ToString() + ",'" + requirements + "','" + duties + "');";
            int id = 0;
            using (DbConnect db = new DbConnect())
            {
                id= db.Insert(query2);
            }
            string query = "INSERT INTO localization(Anchor,PageID,Русский,English) VALUES('Header-"+id.ToString()+"',13,'";
            query += head["Русский"] + "','" + head["English"] + "');";
            query+= "INSERT INTO localization(Anchor,PageID,Русский,English) VALUES('Description-" + id.ToString() + "',13,'";
            query += descr["Русский"] + "','" + descr["English"] + "');";
            requirements = "";
            foreach (string data in req["Русский"])
            {
                requirements += data + "|";
            }
            requirements = requirements.Substring(0, requirements.Length - 1);
            query += "INSERT INTO localization(Anchor,PageID,Русский,English) VALUES('Requirements-" + id.ToString() + "',13,'";
            query += requirements+"','";
            requirements = "";
            foreach (string data in req["English"])
            {
                requirements += data + "|";
            }
            requirements = requirements.Substring(0, requirements.Length - 1);
            query += requirements + "');";

            query += "INSERT INTO localization(Anchor,PageID,Русский,English) VALUES('Duties-" + id.ToString() + "',13,'";
            duties = "";
            foreach (string data in duty["Русский"])
                duties += data + "|";
            duties = duties.Substring(0, duties.Length - 1);
            query += duties + "','";
            duties = "";
            foreach (string data in duty["English"])
            {
                duties += data + "|";
            }
            duties = duties.Substring(0, duties.Length - 1);
            query += duties + "');";
            using (DbConnect db = new DbConnect())
            {
                db.Insert(query);
                System.Web.HttpContext.Current.Application["Localization"] = db.ReadLocalization("SELECT * FROM localization");
            }
            return id;
        }

        public static void Update(int id,Dictionary<string,string> head,Dictionary<string,string> descr,string url,string salary,int type,Dictionary<string,List<string>> req, Dictionary<string,List<string>> dut)
        {
            string require = "", duty = "";
            foreach (string data in req["Русский"])
            {
                require += data + "|";
            }
            require = require.Substring(0, require.Length - 1);
            foreach (string data in dut["Русский"])
            {
                duty += data + "|";
            }
            duty = duty.Substring(0, duty.Length - 1);
            string query = "UPDATE vacancies SET Header='" + head + "',Description='" + descr + "',Salary='" + salary + "',Url='" + url + "',Type=" + type.ToString() + ",Requirements='" + require + "',Duties='" + duty + "' WHERE Id=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Update(query);
            }
            query = "UPDATE localization SET Русский='" + head["Русский"] + "', English='" + head["English"] + "' WHERE Anchor = 'Header-" + id.ToString() + "';";
            query += "UPDATE localization SET Русский='" + descr["Русский"] + "', English='" + descr["English"] + "' WHERE Anchor = 'Description-" + id.ToString() + "';";
            query += "UPDATE localization SET Русский='" + require + "', English='";
            require = "";
            foreach (string data in req["English"])
            {
                require += data + "|";
            }
            require = require.Substring(0, require.Length - 1);

            query += require + "' WHERE Anchor = 'Requirements-" + id.ToString() + "';";

            query += "UPDATE localization SET Русский='" + duty + "', English='";
            duty = "";
            foreach (string data in dut["English"])
            {
                duty += data + "|";
            }
            duty = duty.Substring(0, duty.Length - 1);

            query += duty + "' WHERE Anchor = 'Duties-" + id.ToString() + "';";
            using (DbConnect db = new DbConnect())
            {
                db.Update(query);
                System.Web.HttpContext.Current.Application["Localization"] = db.ReadLocalization("SELECT * FROM localization");
            }
        }

        public static void DeleteVacancy(int id)
        {
            string query = "DELETE FROM vacancies WHERE Id=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Delete(query);
                query = "DELETE FROM localization WHERE Anchor='Header-" + id.ToString() + "' or Anchor='Description-" + id.ToString() + "' or Anchor='Requirements-" + id.ToString() + "' or Anchor='Duties-" + id.ToString() + "';";
                db.Delete(query);
                System.Web.HttpContext.Current.Application["Localization"] = db.ReadLocalization("SELECT * FROM localization");
            }
        }

        public static Vacancy GetVacancy(string url)
        {
            string query = "SELECT * FROM vacancies WHERE Url='" + url+"';";
            using (DbConnect db = new DbConnect())
            {
                return db.GetVacancy(query);
            }
        }
        
        public static List<Vacancy> GetVacancies()
        {
            string query = "SELECT * FROM vacancies";
            using (DbConnect db = new DbConnect())
            {
                return db.GetVacancies(query);
            }
        }
    }
}