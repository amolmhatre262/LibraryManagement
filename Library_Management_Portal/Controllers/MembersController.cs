using Library.Models;
using Library.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_Portal.Controllers
{
    public class MembersController : Controller
    {

        private readonly MembersRepository _membersRepo;

        public MembersController()
        {
            // You can read connection string from appsettings.json if preferred
            string conn = "Server=AMOLHP\\MSSQLSERVER01;Database=LMS_Lite;Trusted_Connection=True;TrustServerCertificate=True;";
            _membersRepo = new MembersRepository(conn);
        }

        public IActionResult Index()
        {
            List<Members> members = _membersRepo.GetAllMembers();
            return View(members);
        }

        public ActionResult Details(int id)
        {
            Members member = _membersRepo.GetMemberByid(id);
            return View(member);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Members member)
        {
            if (ModelState.IsValid)
            {
                member.IsActive = true; // New members are active by default
                member.CreatedDate = DateTime.Now;
                _membersRepo.AddMember(member);
                return RedirectToAction("Index");
            }
            return View(member);
        }

        public ActionResult Edit(int id)
        {
            Members member = _membersRepo.GetMemberByid(id);
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Members member)
        {
            if (ModelState.IsValid)
            {
                _membersRepo.UpdateMember(member);
                return RedirectToAction("Index");
            }
            return View(member);
        }

        public ActionResult Delete(int id)
        {
            Members member = _membersRepo.GetMemberByid(id);
            return View(member);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _membersRepo.DeleteMember(id);
            return RedirectToAction("Index");
        }
    }
}
