using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TRY.Models;


namespace TRY.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Main()
        {
            
            return View();
        }

        public JsonResult GetSearchValue(string search)
        {
            using (DBEntities db = new DBEntities())
            {
                List<ProductModel> allsearch = db.Products.Where(x => x.Name.Contains(search) || x.Category.Contains(search)).Select(x => new ProductModel {
                    
                    Name = x.Name,
                    
                    Category = x.Category
                }).ToList();
                return new JsonResult { Data = allsearch, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        public JsonResult login(User user)
        {
            var result = 0;
            using (DBEntities db = new DBEntities())
            {
                if(db.Users.Any(x => x.Username == user.Username && x.Password == user.Password) == true)
                {
                    result = 1;
                    Session["username"] = user.Username;
                    return Json(result+" "+Session["username"], JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult saveData(User user)
        {
            var result = 0;
            
            using (DBEntities db = new DBEntities())
            {
                
                db.Users.Add(user);
                db.SaveChanges();
                //BuildEmailTemplate(user.Id);
                result = 1;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Confirm(int regId)
        {
            ViewBag.regID = regId;
            return View();
        }

        public JsonResult RegisterConfirm(int regId)
        {
            using (DBEntities db = new DBEntities())
            {
                User Data = db.Users.Where(x => x.Id == regId).FirstOrDefault();
                db.SaveChanges();
                var msg = "your email is verified";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public void BuildEmailTemplate(int regID)
        {
            using (DBEntities db = new DBEntities())
            {
                string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml");
                var regInfo = db.Users.Where(x=>x.Id == regID).FirstOrDefault();
                var url = "http://localhost:64809/" + "Home/Confirm?regId=" + regID;
                body = body.Replace("@ViewBag.ConfirmationLink", url);
                body = body.ToString();
                BuildEmailTemplate("your account is successfully created", body, regInfo.Email);
            }
        }
             
        public static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "sandeep.padigapati@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if(!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if(!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }

        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 2177;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("sandeep.padigapati@gmail.com","Sandeep@123");
            try
            {
                client.Send(mail);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult productIndex(string result)
        {
            string rs = result;
            Session["result"] = result;
            using (DBEntities db = new DBEntities())
            {
                //var item = (from d in db.Products where d.Category == rs.ToString() || d.Name==rs.ToString() select d).ToList();
                var item = (from d in db.Products where d.Category.Contains(rs) || d.Name.Contains(rs) select d).ToList();
                return View(item);
            }
        }
        
        public ActionResult AddProduct(int id, int qty)
        {
            using (DBEntities db = new DBEntities())
            {
                var item = (from a in db.Products where a.Id == id select a).FirstOrDefault();
                Product2 product2 = new Product2();
                product2.Name = item.Name;
                product2.Price = item.Price;
                product2.Count = qty.ToString();

                //byte[] data = new byte[item.File.ContentLength];
                //item.File.InputStream.Read(data, 0, item.File.ContentLength);
                product2.Img = item.Img;
                product2.Category = item.Category;
                db.Product2.Add(product2);
                db.SaveChanges();
                var result = Session["result"];
                return RedirectToAction("Cart");
            }
        }

        public ActionResult RemoveProduct(int id)
        {
            using (DBEntities db = new DBEntities())
            {
                var item = (from d in db.Product2 where d.Id == id select d).FirstOrDefault();
                db.Product2.Remove(item);
                db.SaveChanges();
                return RedirectToAction("Cart");
            }
        }

        public ActionResult Cart()
        {
            using (DBEntities db = new DBEntities())
            {
                var item = (from d in db.Product2 select d).ToList();
                return View(item);
            }
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Checkout(string username)
        {
            if (username == null)
                return View("Index");
            else
                return View("Checkout2");
        }

        public ActionResult Checkout2()
        {
            return View();
        }

        public ActionResult ClearCart()
        {
            using (DBEntities db = new DBEntities())
            {
                //var item = (from d in db.Product2 select d).ToList();
                //db.Product2.Remove(item);
                var item = db.Product2.ToList();
                var count = item.Count;
                for (var i = 0; i < count; i++)
                {
                    db.Product2.Remove(item[i]);
                }
                db.SaveChanges();
                return View("Main");
            }
        }
        public ActionResult Logout()
        {
            Session["username"] = "";
            return View("Index");
        }
    }
}