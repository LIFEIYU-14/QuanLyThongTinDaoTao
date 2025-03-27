using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class CoursesController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao(); // Khai báo DbContext

        // GET: Courses
        public ActionResult Index(int? page)
        {
            int pageSize = 12; // Số lượng khóa học trên mỗi trang
            int pageNumber = (page ?? 1); // Trang mặc định là trang 1

            var khoaHocs = db.KhoaHocs.Include(k => k.KhoaHocAttachments)
                                       .OrderByDescending(k => k.NgayTao)
                                       .ToPagedList(pageNumber, pageSize);

            return View(khoaHocs);
        }
    }
}
