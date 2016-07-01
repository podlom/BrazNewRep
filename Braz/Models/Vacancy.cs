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
        public static int Create(string head,string descr,string salary,List<string> req, List<string> duty,string url,int type)
        {
            string requirements = "", duties = "";
            foreach(string data in req)
            {
                requirements += data + "|";
            }
            requirements = requirements.Substring(0,requirements.Length - 1);
            foreach(string data in duty)
            {
                duties += data + "|";
            }
            duties = duties.Substring(0,duties.Length - 1);
            string query = "INSERT INTO vacancies(Header,Description,Salary,Url,Type,Requirements,Duties) VALUES('"+head+"','"+descr+"','"+salary+"','"+url+"',"+type.ToString()+",'"+requirements+"','"+duties+"');";
            using (DbConnect db = new DbConnect())
            {
                return db.Insert(query);
            }
        }

        public static void Update(int id,string head,string descr,string url,string salary,int type,List<string> req,List<string> dut)
        {
            string require = "", duty = "";
            foreach (string data in req)
            {
                require += data + "|";
            }
            require = require.Substring(0, require.Length - 1);
            foreach (string data in dut)
            {
                duty += data + "|";
            }
            duty = duty.Substring(0, duty.Length - 1);
            string query = "UPDATE vacancies SET Header='" + head + "',Description='" + descr + "',Salary='" + salary + "',Url='" + url + "',Type=" + type.ToString() + ",Requirements='" + require + "',Duties='" + duty + "' WHERE Id=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Update(query);
            }
        }
        
        public static void DeleteVacancy(int id)
        {
            string query = "DELETE FROM vacancies WHERE Id=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Delete(query);
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