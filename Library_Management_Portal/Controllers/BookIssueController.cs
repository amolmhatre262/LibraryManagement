using Library.Models;
using Library.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_Portal.Controllers
{
    public class BookIssueController : Controller
    {
        private readonly BookIssueRepository _repo;

        public BookIssueController(IConfiguration configuration)
        {
            string conn = configuration.GetConnectionString("DefaultConnection");
            _repo = new BookIssueRepository(conn);
        }

        public IActionResult Issue()
        {
            // Load books and members for dropdowns
            ViewBag.Books = _repo.GetAvailableBooks();
            ViewBag.Members = _repo.GetAllMembers();
            var issuedBooks = _repo.GetActiveIssues();
            return View(issuedBooks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Issue(BookIssue issue)
        {
            //if (ModelState.IsValid)
            //{

            //}

            try
            {
                _repo.IssueBook(issue);
                TempData["SuccessMessage"] = "Book issued successfully!";
                return RedirectToAction("Issue");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            // Re-populate dropdowns if validation fails
            ViewBag.Books = _repo.GetAvailableBooks();
            ViewBag.Members = _repo.GetAllMembers();
            var issuedBooks = _repo.GetActiveIssues();
            return View(issue);
        }

        public IActionResult Return()
        {
            var activeIssues = _repo.GetActiveIssues(); // Method to get all books not yet returned
            return View(activeIssues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReturnBook(int issueId)
        {
            try
            {
                _repo.ReturnBook(issueId);
                TempData["SuccessMessage"] = "Book returned successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Return");
        }

        public IActionResult OverdueMembers()
        {
            var overdueList = _repo.GetOverdueMembersReport();
            return View(overdueList);
        }




        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
