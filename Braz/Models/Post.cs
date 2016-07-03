using System;
using System.Collections.Generic;
using System.Linq;

namespace Braz.Models
{
    public class Post
    {
        private string _text;
        private DateTime _date;
        public int Id { get; }
        public string Header { get; set; }
        public string Text { get { return _text.Replace("[0]", "'"); } set { _text = value.Replace("'", "[0]"); } }
        public DateTime Date { get { return _date; } set { _date = value; } }
        public int? Previous { get; set; }
        public int? Next { get; set; }
        public Post(int id)
        {
            Id = id;
        }
        public static Post GetPost(int id)
        {
            using (DbConnect db = new DbConnect())
            {
                return db.GetPost(id.ToString());
            }
           
        }
        public static List<Post> GetPosts()
        {
            List<Post> result;
            using (DbConnect db = new DbConnect())
            {
                result = db.GetNews();
            }
            if (result == null)
                return null;
            result.Reverse();
            return result;
        }
        public static int Add(Dictionary<string,string> headers, Dictionary<string,string> texts, DateTime time,List<System.Web.HttpPostedFileBase> FileList)
        {
            int id;
            using (DbConnect db = new DbConnect())
            {
                string query = "INSERT INTO news (`Header`,`Text`,`Date`) VALUES ('" + headers.First().Value.Replace("`", "[0]") + "','" + texts.First().Value.Replace("'", "[0]").Replace("\r\n", "<br>") + "','" + time.ToString("yyyy-M-d") + "')";
                id = db.Insert(query);
                string query2 = "INSERT INTO localization(Anchor,PageID,";
                foreach(string lang in headers.Keys)
                {
                    query2 += lang + ",";
                }
                query2 = query2.Substring(0, query2.Length - 1);
                query2 += ") VALUES(";
                string query3 = query2;
                query2 += "'Header-" + id.ToString() + "',7,'";
                query3 += "'Text-" + id.ToString() + "',7,'";
                foreach(string lang in headers.Keys)
                {
                    query2 += headers[lang] + "','";
                    query3 += texts[lang] + "','";
                }
                query2 = query2.Substring(0, query2.Length - 2);
                query3 = query3.Substring(0, query3.Length - 2);
                query2 += ")";
                query3 += ")";
                db.Insert(query2);
                db.Insert(query3);
                System.Web.HttpContext.Current.Application["Localization"] = db.ReadLocalization("SELECT * FROM localization");
            }
            for (int i=0;i<FileList.Count;i++)
            {
                System.Web.HttpPostedFileBase file = FileList[i];
                if (file.ContentLength > 0)
                {
                    string dest;
                    switch (i)
                    {
                        case 0:
                            dest = "index"; break;
                        case 1:
                            dest = "list"; break;
                        case 2:
                            dest = "post"; break;
                        default:
                            dest = "unknown";break;
                    }
                    string path = "~/Content/img/news/newsmore/" + id.ToString() + "." + dest + ".jpg";
                    file.SaveAs(System.Web.HttpContext.Current.Server.MapPath(path));
                }
            }
            return id;
        }
        public static void Update(int id,Dictionary<string,string> head,Dictionary<string,string> text, List<System.Web.HttpPostedFileBase> FileList, DateTime? NewDate)
        {
            string query = "UPDATE news SET Header='" + head + "', Text='" + text + "'";
            if (NewDate != null)
                query += ", Date='" + NewDate.Value.ToString("yyyy-M-d") + "'";
            query += " Where Id=" + id.ToString();
            string query2 = "UPDATE localization SET ";
            string query3 = query2;
            foreach(string lang in (List<string>)System.Web.HttpContext.Current.Application["Languages"])
            {
                query2 += lang + "='" + head[lang] + "', ";
                query3 += lang + "='" + text[lang] + "', ";
            }
            query2 = query2.Substring(0, query2.Length - 2);
            query3 = query3.Substring(0, query3.Length - 2);
            query2 += " WHERE Anchor='";
            query3 += " WHERE Anchor='";
            query2 += "Header-" + id.ToString() + "';";
            query3 += "Text-" + id.ToString() + "';";
            using (DbConnect db = new DbConnect())
            {
                db.Update(query);
                db.Update(query2);
                db.Update(query3);
                System.Web.HttpContext.Current.Application["Localization"] = db.ReadLocalization("SELECT * FROM localization");
            }
            for (int i=0;i<FileList.Count;i++)
            {
                System.Web.HttpPostedFileBase file = FileList[i];
                if (file.ContentLength > 0)
                {
                    string dest;
                    switch (i)
                    {
                        case 0:
                            dest = "index"; break;
                        case 1:
                            dest = "list"; break;
                        case 2:
                            dest = "post"; break;
                        default:
                            dest = "unknown"; break;
                    }
                    string path = System.Web.HttpContext.Current.Server.MapPath("~/Content/img/news/newsmore/" + id.ToString() + "." + dest + ".jpg");
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    file.SaveAs(path);
                }
            }
        }
        public static void Delete(int id)
        {
            string query = "DELETE FROM news WHERE Id=" + id.ToString();
            string query2 = "DELETE FROM localization WHERE Anchor='Header-" + id.ToString() + "';DELETE FROM localization WHERE Anchor='Text-" + id.ToString() + "';";
            using (DbConnect db = new DbConnect())
            {
                db.Delete(query);
                db.Delete(query2);
                System.Web.HttpContext.Current.Application["Localization"] = db.ReadLocalization("SELECT * FROM localization");
            }
            for (int i = 0; i < 3; i++)
            {
                string dest;
                switch (i)
                {
                    case 0:
                        dest = "index"; break;
                    case 1:
                        dest = "list"; break;
                    case 2:
                        dest = "post"; break;
                    default:
                        dest = "unknown"; break;
                }
                string path = System.Web.HttpContext.Current.Server.MapPath("~/Content/img/news/newsmore/" +id.ToString()+"."+dest+ ".jpg");
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }


        }
    }
}