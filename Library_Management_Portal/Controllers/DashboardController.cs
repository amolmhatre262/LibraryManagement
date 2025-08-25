using Library.Models;
using Library.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_Portal.Controllers
{
    public class DashboardController : Controller
    {
        private readonly BookIssueRepository _repo;

        public DashboardController(IConfiguration configuration)
        {
            string conn = configuration.GetConnectionString("DefaultConnection");
            _repo = new BookIssueRepository(conn);
        }

        public IActionResult Index()
        {
            var issuesLast7Days = _repo.GetBookIssuesLast7Days();
            var categoryData = _repo.GetBooksByCategory();

            ViewBag.IssuesLast7Days = issuesLast7Days;
            ViewBag.CategoryData = categoryData;

            return View();
        }
    }
}
