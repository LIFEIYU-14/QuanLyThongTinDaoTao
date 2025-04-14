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
            var nguoiDungList = db.NguoiDungs
         .Include("PhanQuyens")
         .ToList();

            var nguoiDung = nguoiDungList
                .FirstOrDefault(n => n.TaiKhoan == model.TaiKhoanOrEmail ||
                                     (n is GiangVien gv && gv.Email == model.TaiKhoanOrEmail));


            if (nguoiDung != null && PasswordHelper.VerifyPassword(model.MatKhau, nguoiDung.MatKhau))
            {
                var quyen = nguoiDung.PhanQuyens.FirstOrDefault()?.TenQuyen;
                Session["TaiKhoan"] = nguoiDung.TaiKhoan;
                Session["TenQuyen"] = quyen;
                Session["NguoiDungId"] = nguoiDung.NguoiDungId;
                // Chuyển hướng tùy theo quyền
                if (quyen == "Admin" || quyen == "GiangVien")
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