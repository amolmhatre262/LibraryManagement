using Library.Models;
using Library.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace Library_Management_Portal.Controllers
{
    public class BooksController : Controller
    {

        private readonly BooksRepository _repo;

        public BooksController(IConfiguration configuration)
        {
            string conn = configuration.GetConnectionString("DefaultConnection");
            _repo = new BooksRepository(conn);
        }
        public ActionResult Index()
        {
            List<Books> books = _repo.GetAllBooks();
            return View(books);
        }
        public ActionResult Details(int id)
        {
            Books book = _repo.GetBookById(id);
            return View(book);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Books book)
        {
            if (ModelState.IsValid)
            {
                book.AvailableCopies = book.TotalCopies;  // initially all copies available
                _repo.AddBook(book);
                return RedirectToAction("Index");
            }
            return View(book);
        }

        public ActionResult Edit(int id)
        {
            Books book = _repo.GetBookById(id);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Books book)
        {
            if (ModelState.IsValid)
            {
                _repo.UpdateBook(book);
                return RedirectToAction("Index");
            }
            return View(book);
        }

        public ActionResult Delete(int id)
        {
            Books book = _repo.GetBookById(id);
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _repo.DeleteBook(id);
            return RedirectToAction("Index");
        }

    }
}
