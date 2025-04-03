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
        // GET: Admin/HocVien/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HocVien hv)
        {
            if (!ModelState.IsValid)
            {
                return View(hv);
            }

            try
            {
                // Kiểm tra email trùng
                if (db.HocViens.Any(g => g.Email == hv.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");
                    return View(hv);
                }

                hv.MaHocVien = DateTime.Now.Year + hv.NgaySinh.Year.ToString() + new Random().Next(1000, 9999);
                // Thiết lập thông tin hocj viên
                hv.TaiKhoan = hv.MaHocVien;
                hv.MatKhau = PasswordHelper.HashPassword(hv.MaHocVien + "123456");
                hv.NguoiDungId = Guid.NewGuid();
                hv.NgayTao = DateTime.Now;
                hv.NgayCapNhat = DateTime.Now;

                db.HocViens.Add(hv);
                db.SaveChanges();
                TempData["Success"] = "Thêm học viên thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm học viên: " + ex.Message);
            }

            return View(hv);
        }

        // Xử lý cập nhật học viên
        public ActionResult Edit(Guid id)
        {
            if (id == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            HocVien hv = db.HocViens.Find(id);
            if (hv == null) return HttpNotFound();
            return View(hv);
        }
        // POST: Admin/GiangVien/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HocVien hv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingHV = db.HocViens.Find(hv.NguoiDungId);
                    if (existingHV == null) return HttpNotFound();

                    // Cập nhật thông tin
                    existingHV.HoVaTen = hv.HoVaTen;
                    existingHV.NgaySinh = hv.NgaySinh;
                    existingHV.SoDienThoai = hv.SoDienThoai;
                    existingHV.Email = hv.Email;
                    existingHV.CoQuanLamViec = hv.CoQuanLamViec;
                    existingHV.NgayCapNhat = DateTime.Now;

                    if (!string.IsNullOrEmpty(hv.MatKhau))
                    {
                        existingHV.MatKhau = PasswordHelper.HashPassword(hv.MatKhau);
                    }

                    db.Entry(existingHV).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật học viên thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật giảng viên: " + ex.Message);
                }
            }
            return View(hv);
        }

        
    }
}
