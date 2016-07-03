using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Braz.Controllers
{
    public class BlogController : Controller
    {
        //
        // GET: /Blog/
        
        //Views
        public ActionResult Index()
        {
            ViewData["PostList"] = Models.Post.GetPosts();
            ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][6];
            ViewData["News"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][7];
            return View();
        }
        public ActionResult News()
        {
            int postid = 0;
            int.TryParse(Request.QueryString["post"], out postid);
            var post = Models.Post.GetPost(postid);
            ViewData["Post"] = post;
            ViewData["Local"] = ((Dictionary<string, Dictionary<int, Dictionary<string, string>>>)HttpContext.Application["Localization"])[(string)Session["Lang"]][7];
            return View();
        }
    }
}
