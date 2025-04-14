using QuanLyThongTinDaoTao.LoginModelView;
using QuanLyThongTinDaoTao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class TaiKhoanController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Tìm theo tài khoản hoặc email
            var nguoiDung = db.NguoiDungs
                .Include("PhanQuyens")
                .FirstOrDefault(n => n.TaiKhoan == model.TaiKhoanOrEmail ||
                                     (n is GiangVien && ((GiangVien)n).Email == model.TaiKhoanOrEmail));

            if (nguoiDung != null && PasswordHelper.VerifyPassword(model.MatKhau, nguoiDung.MatKhau))
            {
                var quyen = nguoiDung.PhanQuyens.FirstOrDefault()?.TenQuyen;
                Session["TaiKhoan"] = nguoiDung.TaiKhoan;
                Session["TenQuyen"] = quyen;

                // Điều hướng dựa vào quyền
                if (quyen == "Admin")
                    return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
                else if (quyen == "GiangVien")
                    return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
         
         
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
            return View(model);
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap");
        }
    }

}