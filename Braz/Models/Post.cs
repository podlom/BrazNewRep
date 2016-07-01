using System;
using System.Collections.Generic;

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
        public static int Add(string header, string text, DateTime time,List<System.Web.HttpPostedFileBase> FileList)
        {
            int id;
            using (DbConnect db = new DbConnect())
            {
                string query = "INSERT INTO news (`Header`,`Text`,`Date`) VALUES ('" + header.Replace("`", "[0]") + "','" + text.Replace("'", "[0]").Replace("\r\n", "<br>") + "','" + time.ToString("yyyy-M-d") + "')";
                id = db.Insert(query);
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
        public static void Update(int id,string head,string text, List<System.Web.HttpPostedFileBase> FileList, DateTime? NewDate)
        {
            string query = "UPDATE news SET Header='" + head + "', Text='" + text.Replace("`", "[0]") + "'";
            if (NewDate != null)
                query += ", Date='" + NewDate.Value.ToString("yyyy-M-d") + "'";
            query += " Where Id=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Update(query);
            }
            for(int i=0;i<FileList.Count;i++)
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
            using (DbConnect db = new DbConnect())
            {
                db.Delete(query);
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