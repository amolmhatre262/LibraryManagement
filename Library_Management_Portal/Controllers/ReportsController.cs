using Library.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace Library_Management_Portal.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportsRepository _repo;

        public ReportsController(IConfiguration configuration)
        {
            string conn = configuration.GetConnectionString("DefaultConnection");
            _repo = new ReportsRepository(conn);
        }

        public IActionResult OverdueReport()
        {
            var report = _repo.GetOverdueMembers();
            return View(report);
        }

        public IActionResult BookHistory(int id)
        {
            var history = _repo.GetBookHistory(id);
            return View(history);
        }
    }
}
