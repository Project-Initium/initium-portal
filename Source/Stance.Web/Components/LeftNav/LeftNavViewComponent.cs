using Microsoft.AspNetCore.Mvc;

namespace Stance.Web.Components.LeftNav
{
    public class LeftNavViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return this.View();
        }
    }
}