using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRY.Models;

namespace TRY.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult productIndex(string result)
        {
            string rs = result;
            using (DBEntities db = new DBEntities())
            {
                var item = (from d in db.Products where d.Category==rs.ToString() select d).ToList();
                
                return View(item);
            }
        }
    }
}