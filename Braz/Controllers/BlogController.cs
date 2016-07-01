using System.Web.Mvc;
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
            return View();
        }
        public ActionResult News()
        {
            int postid = 0;
            int.TryParse(Request.QueryString["post"], out postid);
            ViewData["Post"] = Models.Post.GetPost(postid);
            return View();
        }
    }
}
