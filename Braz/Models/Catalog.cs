using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Braz.Models
{
    public class Item
    {

        /// Fields
        
        public int Id { get; }
        public SubCategory Category { get; set; }
        public List<DataSet> Data { get; set; }

        //  Methods

        //Empty Container 
        public Item(int id)
        {
            Id = id;
        }

        //Get Item from DB
        public static Item GetItem(int id)
        {
            string query = "SELECT items.Id AS ItemID, items.DataSet, categories.Id AS CategoryID, categories.catid, categories.Name, categories.ParamSet FROM items INNER JOIN categories ON items.Category=categories.Id where items.Id=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                return db.GetItem(query);
            }
        }
        
        //Create Item
        public static int Insert(SubCategory SubCat,List<DataSet> set, List<HttpPostedFileBase> FileList)
        {
            int id;
            string get = "INSERT INTO items(Category, DataSet) VALUES (" + SubCat.Id.ToString() + ",'";
            string param_list = "";
            foreach (DataSet param in set)
            {
                param_list += param.Quantity;
                if(param.Type.Key!=set.Last().Type.Key)
                    param_list += "|";
            }
            get += param_list + "')";
            using (DbConnect db = new DbConnect())
            {
                id = db.Insert(get);
            }
            int i = 1;
            foreach (HttpPostedFileBase file in FileList)
            {
                if (file.ContentLength > 0)
                {
                    string foldercontents = SubCat.Parent.Id < 5 ? i++.ToString() : id.ToString()+"."+i++.ToString();
                    string path = HttpContext.Current.Server.MapPath("~/Content/img/catalog/cat-"+SubCat.Id.ToString()+"/" + foldercontents + ".jpg");
                    if(!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Content/img/catalog/cat-"+SubCat.Id.ToString())))
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Content/img/catalog/cat-" + SubCat.Id.ToString()));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    file.SaveAs(path);
                }
            }
            return id;
        }

        //Update Item
        public static void Update(Item item,List<DataSet> newset, List<HttpPostedFileBase> FileList)
        {
            string query = "UPDATE items SET DataSet='";
            string dataset = "";
            foreach(DataSet set in newset)
            {
                dataset += set.Quantity;
                if (set.Type.Key != newset.Last().Type.Key)
                    dataset += "|";
            }
            query += dataset + "' WHERE Id=" + item.Id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Update(query);
            }
            int i = 1;
            foreach (HttpPostedFileBase file in FileList)
            {
                if (file.ContentLength > 0)
                {
                    string foldercontents = item.Category.Parent.Id < 5 ? i++.ToString() : item.Id.ToString() + "." + i++.ToString();
                    string path = HttpContext.Current.Server.MapPath("~/Content/img/catalog/cat-" + item.Category.Id.ToString() + "/" + foldercontents + ".jpg");
                    if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Content/img/catalog/cat-" + item.Category.Id.ToString())))
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Content/img/catalog/cat-" + item.Category.Id.ToString()));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    file.SaveAs(path);
                }
            }
            return;
        }

        //Delete Item
        public static void Delete(int id, SubCategory category)
        {
            string query = "DELETE FROM items WHERE Id=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Delete(query);
            }
            if (category.Parent.Id < 5)
            {
                for (int i = 1; i < 3; i++)
                {
                    string foldercontents = id.ToString() + "." + i.ToString();
                    string path = HttpContext.Current.Server.MapPath("~/Content/img/catalog/cat-" + category.Id.ToString() + "/" + foldercontents + ".jpg");
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
            }
        }

        static public Item Search(string article)
        {
            string query = "SELECT items.Id AS ItemID, items.DataSet, categories.Id AS CategoryID, categories.catid, categories.Name, categories.ParamSet FROM items INNER JOIN categories ON items.Category=categories.Id where items.DataSet LIKE '" + article + "|%'";
            using (DbConnect db = new DbConnect())
            {
                return db.GetItem(query);
            }
        }
    }

    public class SubCategory
    {
        
        //Fields

        public int Id { get; }
        public List<Parameter> Params { get; set; }
        public string Name { get; set; }
        public Category Parent { get; set; }
        //public string Url { get; set; }
        
        //Methods

        //Empty constructor
        public SubCategory(int id)
        {
            Id = id;
        }

        //Get full item list
        public static List<SubCategory> GetCatalog()
        {
            string query = "SELECT Id,Name,catid FROM categories";
            using (DbConnect db = new DbConnect())
                return db.GetCatalog(query).OrderBy(x => x.Parent.Id).ToList();
        }
        
        //Get item list for concrete subcategory
        public static List<Item> GetCatList(int id)
        {
            string query = "SELECT items.Id AS ItemID, items.DataSet, categories.Id AS CategoryID, categories.catid, categories.Name, categories.ParamSet FROM items INNER JOIN categories ON items.Category=categories.Id WHERE items.Category=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                return db.GetItemList(query);
            }
        }
        
        //Get concrete subcategory object
        public static SubCategory GetSubCategory(int id)
        {
            string query = "SELECT * FROM categories WHERE Id=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                return db.GetSubCategory(query);
            }
        }
        public static SubCategory GetSubCategory(string url)
        {
            string query = "SELECT * FROM categories WHERE Url='" + url+"'";
            using (DbConnect db = new DbConnect())
            {
                return db.GetSubCategory(query);
            }
        }

        //Get filter list
        public static List<Filter> GetFilters(int id)
        {
            string query = "SELECT FilterSet FROM categories WHERE Id="+id.ToString();
            using (DbConnect db = new DbConnect())
            {
                return db.GetFilterList(query);
            }
        }

        //Apply filter and get filtered list of items
        internal static List<Item> ApplyFilters(List<FilterValue> input, int cat)
        {
            string separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            List<Item> data = GetCatList(cat);
            List<Item> workset = data.ToList();
            foreach(Item item in workset)
            {
                List<DataSet> sets = item.Data.Where(x => input.Select(y => y.Filter.Parameter.Id).Contains(x.Type.Id)).ToList();
                foreach (DataSet set in sets)
                {
                    string Quant = set.Quantity.Replace(',', separator[0]).Replace('.', separator[0]);
                    string Max = input.Where(x => x.Filter.Parameter.Id == set.Type.Id).First().Max.Replace(',', separator[0]).Replace('.', separator[0]);
                    string Min = input.Where(x => x.Filter.Parameter.Id == set.Type.Id).First().Min.Replace(',', separator[0]).Replace('.', separator[0]);
                    if (Convert.ToDouble(Quant)>Convert.ToDouble(Max) || Convert.ToDouble(Quant) < Convert.ToDouble(Min))
                    {
                        data.Remove(item);
                        break;
                    }
                }
            }
            return data;
        }
    }

    public class Category
    {
        
        //Fields

        public int Id { get; }
        public string Name { get; set; }

        //Methods

        //Filling constructor
        public Category(int id)
        {
            Id = id;
            Name = ((List<string>)HttpContext.Current.Application["ParentCategories"])[id];
        }
        public static List<SubCategory> GetSubCategories(int id)
        {
            string query = "SELECT Id,Name FROM categories Where catid = " + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                return db.GetSubCategories(query);
            }
        }
    }
    
    //Data containers

    public struct Parameter
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
    public struct Filter
    {
        public int Id { get; set; }
        public Parameter Parameter { get; set; }
        public double Step { get; set; }
        public double Max { get; set; }
    }
    public struct DataSet
    {
        private Parameter _param;
        public int Id { get; set; }
        public Parameter Type { get { return _param; } set { _param = value; } }
        public string Quantity { get; set; }
    }

    public class CategoryEqualityComparer:IEqualityComparer<Category>
    {
        public bool Equals(Category A,Category B)
        {
            return A.Id == B.Id;
        }
        public int GetHashCode(Category cat)
        {
            return cat.Id.GetHashCode();
        }
    }
}
