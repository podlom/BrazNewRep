using System.Web.Mvc;

namespace Braz.Controllers
{
    public class AdminFilterAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["User"] == filterContext.HttpContext.Application["Admin"])
            {
                base.OnActionExecuting(filterContext);
            }
            else
                filterContext.Result = new RedirectResult("/");
        }
    }
}