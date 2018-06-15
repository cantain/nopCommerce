using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using System.Linq;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        private readonly IWorkContext _workContext;

        public HomeController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Index()
        {
            //return View();
            if (_workContext.CurrentCustomer != null
                && (_workContext.IsAdmin
                || _workContext.CurrentCustomer.CustomerRoles?.Any(r => r.SystemName == "TenantAdmin") == true
                || _workContext.CurrentCustomer.CustomerRoles?.Any(r => r.SystemName == "Administrators") == true
                ))
            {
                if (_workContext.CurrentCustomer.CustomerRoles?.Any(r => r.SystemName == "TenantAdmin") == true)
                {
                    return Redirect("/EntranceGuard/User/List");
                }
                return Redirect("/admin");
            }
            return RedirectToAction("Login", "Customer");
        }
    }
}