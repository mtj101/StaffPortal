using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StaffPortal.Models;
using StaffPortal.Models.ViewModels;
using StaffPortal.Service;
using StaffPortal.Business;

namespace StaffPortal.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        private BookingService _bookingService = new BookingService();
        private AlertService _alertService = new AlertService();
        private MessageService _messageService = new MessageService();
        private StaffService _staffService = new StaffService();


        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        [Route("")]
        public ActionResult Index()
        {
            return RedirectToAction("Calendar");
        }

        [Route("calendar")]
        public async Task<ActionResult> Calendar()
        {
            var user = new LoggedInStaffMember();
            if (User.Identity.IsAuthenticated)
            {
                user.Id = (await UserManager.FindByNameAsync(User.Identity.Name)).StaffMemberId;
            }
            var db = new ApplicationDbContext();

            var totals = _bookingService.GetHolidayTotalsForUser(user.Id);
            user.HolidaysBooked = totals.Booked;
            user.HolidaysPending = totals.Pending;
            user.MaximumHolidays = int.Parse(db.ApplicationSettings.Find("Holidays Per Member").Value);

            return View(user);
        }

        [Authorize]
        [Route("userpanel")]
        public async Task<ActionResult> UserPanel()
        {
            var user = new LoggedInStaffMember();
            if (User.Identity.IsAuthenticated)
            {
                user.Id = (await UserManager.FindByNameAsync(User.Identity.Name)).StaffMemberId;
            }

            var totals = _bookingService.GetHolidayTotalsForUser(user.Id);
            user.HolidaysBooked = totals.Booked;
            user.HolidaysPending = totals.Pending;

            return View(user);
        }

        [Authorize]
        [Route("messages/{section}/{page:int=1}", Name = "Messages")]
        public async Task<ActionResult> Messages(string section, int page)
        {
            var user = new LoggedInStaffMember();
            if (User.Identity.IsAuthenticated)
            {
                user.Id = (await UserManager.FindByNameAsync(User.Identity.Name)).StaffMemberId;
            }

            MessageType messageType = section == "inbox" ? MessageType.Received : MessageType.Sent;

            List<Message> messages = _messageService.GetAllMessagesForUser(User.Identity.GetUserId(), messageType);
            int pages = (messages.Count + 10 - 1) / 10;
            messages = messages.Skip((page - 1) * 10).Take(10).ToList();

            var allStaff = _staffService.GetAllStaff();

            var viewModel = new MessagesViewModel
            {
                Messages = messages,
                Staff = allStaff,
                Section = messageType,
                TotalPages = pages,
                CurrentPage = page
            };

            return View(viewName: "Messages", model: viewModel);
        }

        [HttpPost]
        public ActionResult SendMessage(SendMessageDto dto)
        {
            var senderId = User.Identity.GetUserId();

            _messageService.SendMessage(senderId, dto.ReceiverId.ToString(), dto.Subject, dto.Body);

            return RedirectToAction("Messages", new { Section = "inbox" });
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult DeleteMessages()
        {
            var messageIds = Request.Form.AllKeys.Select(k => int.Parse(k)).ToArray();
            var senderId = User.Identity.GetUserId();

            _messageService.DeleteMessages(messageIds, senderId);

            return RedirectToAction("Messages", new { Section = "inbox" });
        }

        [ChildActionOnly]
        public ActionResult SideBar()
        {
            var viewModel = new SideBarViewModel();

            if (User.IsInRole("Admin"))
            {
                viewModel.PendingHolidaysForSupervisor = new SupervisorService().GetAllPendingHolidays().Count;
            }
            if (User.IsInRole("Supervisor"))
            {
                int userId = UserManager.FindById(User.Identity.GetUserId()).StaffMemberId;
                viewModel.PendingHolidaysForSupervisor = new SupervisorService().GetPendingHolidaysForSupervisor(userId).Count;
            }
            if (User.Identity.IsAuthenticated)
            {
                viewModel.UnreadMessages = _messageService.GetUnreadTotalForUser(User.Identity.GetUserId());
            }

            return View("_SideBar", viewModel);
        }

        [ChildActionOnly]
        public ActionResult GetAlerts()
        {
            var alerts = _alertService.GetAlertsForUser(User.Identity.GetUserId()).OrderByDescending(a => a.Created).ToList();
            return View("_GetAlerts", alerts);
        }
    }

    public class SendMessageDto
    {
        public Guid ReceiverId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class MessagesViewModel
    {
        public List<Message> Messages { get; set; }
        public List<ApplicationUser> Staff { get; set; }
        public MessageType Section { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}