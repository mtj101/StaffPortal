using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using StaffPortal.Models;

namespace StaffPortal.Controllers
{
    public class Account2Controller : Controller
    {
        [ChildActionOnly]
        public ActionResult LoginMenu()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userResult = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindByNameAsync(User.Identity.Name);
                userResult.Wait();

                var db = new ApplicationDbContext();
                var staffMember = db.StaffMember.SingleOrDefault(d => d.Id == userResult.Result.StaffMemberId);

                var viewModel = new LoginPartialViewModel
                {
                    Id = userResult.Result.Id,
                    Name = staffMember.FirstNames + " " + staffMember.Surname
                };
                return PartialView("_LoginPartial", viewModel);
            }           
            return PartialView("_LoginPartial");
        }
    }

    public class LoginPartialViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}