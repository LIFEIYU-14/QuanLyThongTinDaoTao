using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class CoursesController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao(); // Khai báo DbContext

        // GET: Courses
        public ActionResult Index()
        {
            var khoaHocs = db.KhoaHocs.ToList();
            return View(khoaHocs); 
        }
    }
}
