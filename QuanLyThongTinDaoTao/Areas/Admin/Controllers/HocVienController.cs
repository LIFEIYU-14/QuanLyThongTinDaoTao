﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.Services;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    [RoleAuthorize("Admin")]
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
        public async Task<ActionResult> Create(HocVien hv)
        {
            if (!ModelState.IsValid)
            {
                return View(hv);
            }

            try
            {
                // Kiểm tra email trùng
                if (db.HocViens.Any(h => h.Email == hv.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");
                    return View(hv);
                }

                // Tạo mã học viên
                hv.MaHocVien = DateTime.Now.Year + hv.NgaySinh.Year.ToString() + new Random().Next(1000, 9999);
                // Thiết lập thông tin học viên
                hv.TaiKhoan = hv.MaHocVien;
                hv.MatKhau = PasswordHelper.HashPassword(hv.MaHocVien + "123456");
                hv.NguoiDungId = Guid.NewGuid();
                hv.NgayTao = DateTime.Now;
                hv.NgayCapNhat = DateTime.Now;
                hv.QR_Code_HV = ""; // Chưa có mã QR tại thời điểm tạo, sẽ tạo sau

                db.HocViens.Add(hv);
                hv.PhanQuyens.Add(new PhanQuyen { TenQuyen = "HocVien" });

                db.SaveChanges();

                // Tạo QR code cho học viên
                var hocVienService = new HocVienService(db);
                string qrBase64 = hocVienService.GenerateQRCodeForStudent(hv.NguoiDungId);

                // Gửi email chứa QR code
                var emailService = new EmailService();
                await emailService.SendQrCodeEmail(hv.Email, qrBase64);

                // Cập nhật lại mã QR vào database
                hv.QR_Code_HV = qrBase64;
                db.SaveChanges();

                TempData["Success"] = "Thêm học viên thành công và đã gửi email!";
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
        public ActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HocVien hv = db.HocViens.Find(id);
            if (hv == null)
            {
                TempData["Error"] = "Học viên không tồn tại!";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa các bản ghi liên quan trong bảng DiemDanh_hV
                var diemDanhs = db.DiemDanhs_HVs.Where(d => d.HocVien.NguoiDungId == hv.NguoiDungId).ToList();
                db.DiemDanhs_HVs.RemoveRange(diemDanhs);

                // Xóa các bản ghi liên quan trong bảng DangKyHoc
                var hocVienDangKy = db.DangKyHocs.Where(gvh => gvh.HocVien.NguoiDungId == hv.NguoiDungId).ToList();
                db.DangKyHocs.RemoveRange(hocVienDangKy);

                // Xóa các bản ghi liên quan trong bảng ThongBao
                //var thongBaos = db.ThongBaos.Where(t => t.GiangViens.Any(gvItem => gvItem.NguoiDungId == gv.NguoiDungId)).ToList();
                //foreach (var thongBao in thongBaos)
                //{
                //    thongBao.GiangViens.Remove(gv); // Xóa liên kết giảng viên khỏi thông báo
                //    if (!thongBao.GiangViens.Any()) // Nếu không còn giảng viên nào liên kết với thông báo, xóa thông báo
                //    {
                //        db.ThongBaos.Remove(thongBao);
                //    }
                //}

                // Sau khi xóa các bản ghi liên quan, xóa giảng viên
                db.HocViens.Remove(hv);
                db.SaveChanges();

                TempData["Success"] = "Xóa học viên thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa học viên: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

    }
}
