using QuanLyThongTinDaoTao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        DbContextThongTinDaoTao db = new DbContextThongTinDaoTao(); 
        // GET: Admin/HomeAdmin
        public ActionResult Index()
        {
            return View();
        }
     
    }
}