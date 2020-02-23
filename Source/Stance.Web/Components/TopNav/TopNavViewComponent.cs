using Microsoft.AspNetCore.Mvc;

namespace Stance.Web.Components.TopNav
{
    public class TopNavViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return this.View();
        }
    }
}