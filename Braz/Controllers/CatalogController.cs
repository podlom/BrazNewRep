using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Braz.Controllers
{
    public class CatalogController : Controller
    {
        //
        // GET: /Catalog/

        public ActionResult Index()
        {
            //localization
            ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][4];

            ViewData["CatList"] = Models.SubCategory.GetCatalog();
            string path = Request.RawUrl;
            path = path.Substring(path.LastIndexOf('/')+1);
            if(path!=null)
            {
                Models.SubCategory result = Models.SubCategory.GetSubCategory(path);
                ViewData["WithCat"] = result!=null;
                ViewData["InitCat"] = result;
            }
            return View();
        }
        //get list view subcategory
        public ActionResult ListView()
        {  
            //localization
            ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][4];

            ViewData["Items"] = Models.SubCategory.GetCatList(Int32.Parse(Request.QueryString["catid"]));
            ViewData["SubCategory"] = Models.SubCategory.GetSubCategory(Int32.Parse(Request.QueryString["catid"]));
            return PartialView("ListView");
        }
        //get table view subcategory
        public ActionResult TableView()
        {
            //localization
            ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][4];

            ViewData["Items"] = Models.SubCategory.GetCatList(Int32.Parse(Request.QueryString["catid"]));
            ViewData["SubCategory"] = Models.SubCategory.GetSubCategory(Int32.Parse(Request.QueryString["catid"]));
            return PartialView("TableView");
        }
        //update aside bar
        public ActionResult UpdateAside()
        {
            ViewData["CatList"] = Models.Category.GetSubCategories(Int32.Parse(Request.QueryString["catid"]));
            ViewData["Parent"] = new Models.Category(Int32.Parse(Request.QueryString["catid"]));
            ViewData["SubName"] = Request.QueryString["sub"].Replace('_', ' ');
            return PartialView("CategoryView");
        }
        //update filters bar
        public ActionResult UpdateFilters()
        {
            int catid = Int32.Parse(Request.QueryString["catid"]);
            ViewData["FilterSet"] = Models.SubCategory.GetFilters(catid);
            ViewData["ParameterSet"] = Models.SubCategory.GetSubCategory(catid).Params;
            ViewData["SubCat"] = catid;
            return PartialView("Filters");
        }
        //LOAD MODALS
        public ActionResult GetBuyProduct()
        {
            //localization
            ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][4];

            ViewData["ItemId"] = Request.QueryString["itemid"];
            ViewData["CatId"] = Models.Item.GetItem(Int32.Parse(Request.QueryString["itemid"]));
            ViewData["IsLoggedIn"] = Session["Logged"];
            return View("BuyModal");
        }
        [AdminFilter]
        public ActionResult GetAddInlineProduct()
        {
            ViewData["Category"] = Models.SubCategory.GetSubCategory(Int32.Parse(Request.QueryString["cat"]));
            return PartialView("InlineModal");
        }
        [AdminFilter]
        public ActionResult GetAddGroupProduct()
        {
            ViewData["Category"] = Models.SubCategory.GetSubCategory(Int32.Parse(Request.QueryString["cat"]));
            return View("GroupModal");
        }
        [AdminFilter]
        public ActionResult GetEditProduct()
        {
            ViewData["Item"] = Models.Item.GetItem(Int32.Parse(Request.QueryString["ItemId"]));
            return PartialView("EditModal");
        }
        [AdminFilter]
        public ActionResult GetDeleteProduct()
        {
            ViewData["Item"] = Request.QueryString["itemid"];
            return View("DeleteModal");
        }



        //Buy data processing
        public ActionResult BuyProduct(FormCollection data)
        {
            Models.OrderedItem Item = new Models.OrderedItem();
            Item.Item = Models.Item.GetItem(Int32.Parse(Request.QueryString["ItemId"]));
            //Alloy mark && Thermal Treatment
            Item.AlloyMark = data["mark"];
            if (Item.AlloyMark == "АД31")
                Item.ThermalTreatment = data["termo1"];
            else if (Item.AlloyMark == "6060 экспорт" || Item.AlloyMark == "6063 экспорт" || Item.AlloyMark == "6005 экспорт")
                Item.ThermalTreatment = data["termo2"];
            else
                Item.ThermalTreatment = "Значение не выбрано";
            //Length && Deflection
            Item.Length = data["length"];
            Item.Deflection = data["accLength"] == "yes" ? true : false;
            //Cover
            //Anod
            Item.Cover = data["cover"];
            if (Item.Cover == "Анодирование")
            {
                Item.Color = data["chooseColor"];
                Item.Width = data["chooseWeight"];
                Item.ContactSpot = data["anodBorders"] == "yes" ? true : false;
            }
            //Paint
            else if (Item.Cover== "Покраска")
            {
                Item.Color = data["color"];
                Item.PaintType = data["paintType"];
                Item.Shine = data["shineType"];
            }
            //addition
            Item.AdditionalProcessing = data["mechanicWork"] == "yes" ? true : false;
            Item.Comment = data["commentary"];
            ((Models.User)Session["User"]).CurrOrder.AddToCart(Item);
            return Redirect("/Catalog?cat="+Request.QueryString["catid"]);
        }
        //add product to subcategory
        [AdminFilter]
        public ActionResult AddProduct(FormCollection data)
        {
            Models.SubCategory SubCat = Models.SubCategory.GetSubCategory(Int32.Parse(Request.QueryString["catid"]));
            List<Models.DataSet> DataList = new List<Models.DataSet>();
            for(int i=0;i<SubCat.Params.Count;i++)
            {
                Models.DataSet TSet = new Models.DataSet();
                TSet.Type = SubCat.Params[i];
                TSet.Quantity = data[SubCat.Params[i].Key + ParamType(SubCat.Params[i].Key)];
                DataList.Add(TSet);
            }
            List<HttpPostedFileBase> Filelist = new List<HttpPostedFileBase>();
            Filelist.Add(Request.Files.Get("image"));
            Filelist.Add(Request.Files.Get("image2"));
            Models.Item.Insert(SubCat, DataList, Filelist);
            return Redirect("/Catalog?cat=" + SubCat.Id.ToString());
        }
        //Add inline product
        [AdminFilter]
        public ActionResult AddInlineProduct(FormCollection data)
        {
            Models.SubCategory SubCat = Models.SubCategory.GetSubCategory(Int32.Parse(Request.QueryString["catid"]));
            List<Models.DataSet> DataList = new List<Models.DataSet>();
            for (int i = 0; i < SubCat.Params.Count; i++)
            {
                Models.DataSet TSet = new Models.DataSet();
                TSet.Type = SubCat.Params[i];
                TSet.Quantity = data[SubCat.Params[i].Key + ParamType(SubCat.Params[i].Key)];
                DataList.Add(TSet);
            }
            List<HttpPostedFileBase> Filelist = new List<HttpPostedFileBase>();
            Filelist.Add(Request.Files.Get("image"));
            Filelist.Add(Request.Files.Get("image2"));
            Models.Item.Insert(SubCat, DataList, Filelist);
            return Redirect("/Catalog?cat="+SubCat.Id.ToString());
        }
        //edit product
        [AdminFilter]
        public ActionResult EditProduct(FormCollection data)
        {
            Models.Item item = Models.Item.GetItem(Int32.Parse(Request.QueryString["itemid"]));
            Models.SubCategory category = Models.SubCategory.GetSubCategory(item.Category.Id);
            List<Models.DataSet> datalist = new List<Models.DataSet>();
            for(int i=0;i<category.Params.Count;i++)
            {
                datalist.Add(new Models.DataSet() { Type = category.Params[i], Quantity = data[category.Params[i].Key+ ParamType(category.Params[i].Key)] });
            }
            List<HttpPostedFileBase> Filelist = new List<HttpPostedFileBase>();
            Filelist.Add(Request.Files.Get("image"));
            Filelist.Add(Request.Files.Get("image2"));
            Models.Item.Update(item, datalist, Filelist);
            return Redirect("/Catalog?cat=" + category.Id.ToString());
        }
        [AdminFilter]
        public ActionResult DeleteProduct()
        {
            Models.SubCategory subcategory = Models.Item.GetItem(Int32.Parse(Request.QueryString["itemid"])).Category;
            Models.Item.Delete(Int32.Parse(Request.QueryString["itemid"]),subcategory);
            return Redirect("/Catalog?cat="+subcategory.Id.ToString());
        }

        public ActionResult Search()
        {
            if(Request.QueryString["q"]=="")
                return PartialView("NotFound");
            Models.Item item = Models.Item.Search(Request.QueryString["q"]);
            if (item == null)
                return PartialView("NotFound");
            ViewData["Items"] = new List<Models.Item>() { item };
            ViewData["SubCategory"] = Models.SubCategory.GetSubCategory(item.Category.Id);
            if (item.Category.Parent.Id > 4)
                return PartialView("TableView");
            return PartialView("ListView");
        }
        public ActionResult ApplyFilters()
        {
            List<Models.Filter> list = Models.SubCategory.GetFilters(Int32.Parse(Request.QueryString["cat"]));
            List<Models.FilterValue> Get = new List<Models.FilterValue>();
            foreach(Models.Filter filter in list)
            {
                Get.Add(new Models.FilterValue() { Max = Request.QueryString[filter.Id.ToString()+"-max"], Min= Request.QueryString[filter.Id.ToString() + "-min"], Filter=filter });
            }
            List<Models.Item> result = Models.SubCategory.ApplyFilters(Get,Int32.Parse(Request.QueryString["cat"]));
            ViewData["Items"] = result;
            ViewData["SubCategory"] = Models.SubCategory.GetSubCategory(Int32.Parse(Request.QueryString["cat"]));
            return Models.SubCategory.GetSubCategory(Int32.Parse(Request.QueryString["cat"])).Parent.Id > 4 ? PartialView("TableView") : PartialView("ListView");
        }

        public static string ParamType(string input)
        {
            if(input.Length>0)
                if (Char.IsLetter(input[0]) && Char.IsUpper(input[0]))
                    return "B";
            return "L";
        }
    }
}
