using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class HocVienController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // Hiển thị danh sách học viên
        public ActionResult Index(string searchString)
        {
            var hocViens = db.HocViens.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                hocViens = hocViens.Where(hv => hv.HoVaTen.Contains(searchString) || hv.Email.Contains(searchString));
            }

            return View(hocViens.ToList());
        }

        // Hiển thị form thêm học viên
        public ActionResult Create()
        {
            return View();
        }

        // Xử lý thêm học viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HocVien hocVien)
        {
            if (ModelState.IsValid)
            {
                hocVien.NguoiDungId = Guid.NewGuid();
                hocVien.MatKhau = PasswordHelper.HashPassword(hocVien.MatKhau); // Mã hóa mật khẩu
                hocVien.NgayTao = DateTime.Now;
                hocVien.NgayCapNhat = DateTime.Now;
                db.HocViens.Add(hocVien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hocVien);
        }

        // Hiển thị form sửa thông tin học viên
        public ActionResult Edit(Guid id)
        {
            var hocVien = db.HocViens.Find(id);
            if (hocVien == null)
            {
                return HttpNotFound();
            }
            return View(hocVien);
        }

        // Xử lý cập nhật học viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HocVien hocVien)
        {
            if (ModelState.IsValid)
            {
                hocVien.NgayCapNhat = DateTime.Now;
                db.Entry(hocVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hocVien);
        }

        // Xác nhận xóa học viên
        public ActionResult Delete(Guid id)
        {
            var hocVien = db.HocViens.Find(id);
            if (hocVien == null)
            {
                return HttpNotFound();
            }
            return View(hocVien);
        }

        // Xử lý xóa học viên
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var hocVien = db.HocViens.Find(id);
            if (hocVien != null)
            {
                db.HocViens.Remove(hocVien);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
