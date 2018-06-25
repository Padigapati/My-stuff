using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRY.Models;

namespace TRY.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        public ActionResult bookIndex()
        {
            return View();
        }

        public ActionResult GetBooks()
        {
            using (DBEntities db = new DBEntities())
            {
                var bookList = db.Books.ToList<Book>();
                return Json(new { data=bookList}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult AddOrEdit(int id=0)
        {
            return View();
        }

        
    }
}