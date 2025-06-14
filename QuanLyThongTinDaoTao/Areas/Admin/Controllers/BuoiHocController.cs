﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BuoiHocController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".ppt", ".pptx", ".txt" };


        // Hiển thị toàn bộ danh sách buổi học
        public ActionResult Index()
        {
            // Cập nhật trạng thái của tất cả các buổi học theo thời gian thực
            UpdateTrangThaiBuoiHocs();
            var buoiHocs = db.BuoiHocs
                 .Include(b => b.LopHoc.KhoaHoc)
                 .Include(b => b.GiangVien_BuoiHocs.Select(gvbh => gvbh.GiangVien))
                 .ToList();


            ViewBag.LopHocList = db.LopHocs.ToList();
            ViewBag.KhoaHocList = db.KhoaHocs.ToList();
            return View(buoiHocs);
        }
        public ActionResult GetLopHocByKhoaHoc(Guid? khoaHocId)
        {
            if (!khoaHocId.HasValue)
            {
                return Content("<option value=''>-- Không có lớp học --</option>");
            }

            var lopHocs = db.LopHocs.Where(l => l.KhoaHocId == khoaHocId).ToList();

            var options = "<option value=''>-- Chọn lớp học --</option>";
            foreach (var lop in lopHocs)
            {
                options += $"<option value='{lop.LopHocId}'>{lop.TenLopHoc}</option>";
            }

            return Content(options, "text/html");
        }

        public ActionResult FilterByLopHoc(Guid? lopHocId)
        {
            if (lopHocId.HasValue)
            {
                var danhSachBuoiHoc = db.BuoiHocs
                    .Include(lh => lh.LopHoc)
                    .Where(lh => lh.LopHoc != null && lh.LopHoc.LopHocId == lopHocId.Value)
                    .ToList();

                return PartialView("_BuoiHocPartial", danhSachBuoiHoc);
            }

            return PartialView("_BuoiHocPartial", db.BuoiHocs.Include(lh => lh.LopHoc).ToList());
        }
        // Hiển thị danh sách học viên của buổi học
        public ActionResult DanhSachHocVien(Guid id)
        {
            // Kiểm tra buổi học có tồn tại không
            var buoiHoc = db.BuoiHocs
                .Include(b => b.LopHoc)
                .FirstOrDefault(b => b.BuoiHocId == id);

            if (buoiHoc == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách học viên đã đăng ký và xác nhận

            var danhSachHocVien = db.DangKyHocs
                    .Include(dk => dk.HocVien)
                    .Where(dk => dk.LopHocId == buoiHoc.LopHoc.LopHocId)
                    .Select(dk => dk.HocVien) // Lấy ra học viên
                    .ToList();
            ViewBag.BuoiHoc = buoiHoc;
            return View(danhSachHocVien); // Trả đúng List<HocVien>
        }


        // Tạo buổi học (GET)


        [HttpGet]
        public ActionResult Create(Guid? lopHocId)
        {
            var model = new BuoiHoc();
            if (lopHocId.HasValue)
            {
                var lopHoc = db.LopHocs.FirstOrDefault(k => k.LopHocId == lopHocId.Value);
                ViewBag.TenLopHoc = lopHoc?.TenLopHoc;
                ViewBag.NgayBatDau = lopHoc?.NgayBatDau;
                ViewBag.NgayKetThuc = lopHoc?.NgayKetThuc;
                model.LopHocId = lopHocId.Value;
            }

            // Nếu không có LopHocId thì hiển thị dropdown
            if (!lopHocId.HasValue)
            {
                ViewBag.LopHocList = db.LopHocs.ToList();
            }
            return View(model);
        }

        // Tạo buổi học (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BuoiHoc model, Guid LopHocId, HttpPostedFileBase[] attachments, string[] selectedGiangViens)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LopHocList = db.LopHocs.ToList();

                if (model.LopHocId != Guid.Empty)
                {
                    var lopHocEntity = db.LopHocs.FirstOrDefault(k => k.LopHocId == model.LopHocId);
                    if (lopHocEntity != null)
                    {
                        ViewBag.LopHoc = lopHocEntity;
                    }
                }

                return View(model);
            }

            var lopHoc = db.LopHocs.Find(LopHocId);
            if (lopHoc == null)
            {
                ModelState.AddModelError("LopHocId", "Lớp học không tồn tại.");
                ViewBag.LopHocList = db.LopHocs.ToList();
                ViewBag.GiangVienList = db.GiangViens.ToList();
                return View(model);
            }

            // Kiểm tra thời gian hợp lệ
            if (model.NgayHoc < lopHoc.NgayBatDau || model.NgayHoc > lopHoc.NgayKetThuc)
            {
                ModelState.AddModelError("NgayHoc", "Ngày học phải nằm trong khoảng từ " + lopHoc.NgayBatDau.ToShortDateString() + " đến " + lopHoc.NgayKetThuc.ToShortDateString());
                ViewBag.LopHocList = db.LopHocs.ToList();
                ViewBag.GiangVienList = db.GiangViens.ToList();
                return View(model);
            }

            // Kiểm tra trùng thời gian
            bool isOverlap = db.BuoiHocs.Any(b =>
                b.LopHoc.LopHocId == LopHocId &&
                b.NgayHoc == model.NgayHoc &&
                ((model.GioBatDau >= b.GioBatDau && model.GioBatDau < b.GioKetThuc) ||
                 (model.GioKetThuc > b.GioBatDau && model.GioKetThuc <= b.GioKetThuc) ||
                 (model.GioBatDau <= b.GioBatDau && model.GioKetThuc >= b.GioKetThuc))
            );

            if (isOverlap)
            {
                ModelState.AddModelError("GioBatDau", "Thời gian buổi học bị trùng với một buổi học khác.");
                ViewBag.LopHocList = db.LopHocs.ToList();
                ViewBag.GiangVienList = db.GiangViens.ToList();
                return View(model);
            }

            // Kiểm tra trùng lịch giảng viên
            if (selectedGiangViens != null && selectedGiangViens.Any())
            {
                foreach (var giangVienId in selectedGiangViens)
                {
                    bool isGiangVienBusy = db.GiangVien_BuoiHoc
                        .Include(gv => gv.BuoiHoc)
                        .Any(gv => gv.GiangVienId == giangVienId &&
                              gv.BuoiHoc.NgayHoc == model.NgayHoc &&
                              ((model.GioBatDau >= gv.BuoiHoc.GioBatDau && model.GioBatDau < gv.BuoiHoc.GioKetThuc) ||
                               (model.GioKetThuc > gv.BuoiHoc.GioBatDau && model.GioKetThuc <= gv.BuoiHoc.GioKetThuc) ||
                               (model.GioBatDau <= gv.BuoiHoc.GioBatDau && model.GioKetThuc >= gv.BuoiHoc.GioKetThuc)));

                    if (isGiangVienBusy)
                    {
                        var busyGiangVien = db.GiangViens.Find(giangVienId);
                        ModelState.AddModelError("", $"Giảng viên {busyGiangVien.HoVaTen} đã có lịch dạy trùng thời gian này.");
                        ViewBag.LopHocList = db.LopHocs.ToList();
                        ViewBag.GiangVienList = db.GiangViens.ToList();
                        return View(model);
                    }
                }
            }

            // Thêm buổi học mới
            model.BuoiHocId = Guid.NewGuid();
            model.LopHoc = lopHoc;
            db.BuoiHocs.Add(model);
            db.SaveChanges();

            // Thêm giảng viên vào buổi học
            if (selectedGiangViens != null && selectedGiangViens.Any())
            {
                foreach (var giangVienId in selectedGiangViens)
                {
                    var giangVien = db.GiangViens.Find(giangVienId);
                    if (giangVien != null)
                    {
                        db.GiangVien_BuoiHoc.Add(new GiangVien_BuoiHoc
                        {
                            Id = Guid.NewGuid(),
                            BuoiHocId = model.BuoiHocId,
                            GiangVienId = giangVienId
                        });
                    }
                }
                db.SaveChanges();
            }

            // Xử lý tệp đính kèm
            if (attachments != null)
            {
                UploadAttachments(model.BuoiHocId, attachments);
            }

            TempData["Success"] = "Buổi học đã được tạo thành công!";
            return RedirectToAction("Index");
        }
        // Chỉnh sửa buổi học (GET)
        public ActionResult Edit(Guid id)
        {
            var buoiHoc = db.BuoiHocs
                .Include(k => k.LopHoc)
                .Include(k => k.BuoiHocAttachments.Select(a => a.Attachment))
                .FirstOrDefault(k => k.BuoiHocId == id);
            if (buoiHoc == null)
            {
                return HttpNotFound();
            }

            ViewBag.LopHocList = db.LopHocs.ToList();
            ViewBag.GiangVienList = db.GiangViens.ToList();
            ViewBag.GiangVienDaChon = buoiHoc.GiangVien_BuoiHocs?.Select(g => g.GiangVienId).ToList();
            return View(buoiHoc);
        }

        // Chỉnh sửa buổi học (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BuoiHoc model, Guid LopHocId, HttpPostedFileBase[] attachments, string[] selectedGiangViens)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LopHocList = db.LopHocs.ToList();
                ViewBag.GiangVienList = db.GiangViens.ToList();
                var lopHocEntity = db.LopHocs.FirstOrDefault(k => k.LopHocId == LopHocId);
                if (lopHocEntity != null)
                {
                    ViewBag.TenLopHoc = lopHocEntity.TenLopHoc;
                }

                return View(model);
            }
         

            var buoiHoc = db.BuoiHocs.Find(model.BuoiHocId);
            if (buoiHoc == null)
            {
                ViewBag.LopHocList = db.LopHocs.ToList();
                ViewBag.GiangVienList = db.GiangViens.ToList();
                return HttpNotFound();
            }

            var lopHoc = db.LopHocs.Find(LopHocId);
            if (lopHoc == null)
            {
                ModelState.AddModelError("LopHocId", "Lớp học không tồn tại.");
                ViewBag.LopHocList = db.LopHocs.ToList();
                ViewBag.GiangVienList = db.GiangViens.ToList();
                return View(model);
            }

            // Kiểm tra NgayHoc có nằm trong khoảng hợp lệ không?
            if (model.NgayHoc < lopHoc.NgayBatDau || model.NgayHoc > lopHoc.NgayKetThuc)
            {
                ModelState.AddModelError("NgayHoc", "Ngày học phải nằm trong khoảng từ " + lopHoc.NgayBatDau.ToShortDateString() + " đến " + lopHoc.NgayKetThuc.ToShortDateString());
                ViewBag.LopHocList = db.LopHocs.ToList();
                ViewBag.GiangVienList = db.GiangViens.ToList();
                return View(model);
            }

            // Kiểm tra trùng thời gian (trừ buổi học hiện tại)
            bool isOverlap = db.BuoiHocs.Any(b =>
                b.LopHoc.LopHocId == LopHocId &&
                b.NgayHoc == model.NgayHoc &&
                b.BuoiHocId != model.BuoiHocId &&
                ((model.GioBatDau >= b.GioBatDau && model.GioBatDau < b.GioKetThuc) ||
                 (model.GioKetThuc > b.GioBatDau && model.GioKetThuc <= b.GioKetThuc) ||
                 (model.GioBatDau <= b.GioBatDau && model.GioKetThuc >= b.GioKetThuc))
            );

            if (isOverlap)
            {
                ModelState.AddModelError("GioBatDau", "Thời gian buổi học bị trùng với một buổi học khác.");
                ViewBag.LopHocList = db.LopHocs.ToList();
                ViewBag.GiangVienList = db.GiangViens.ToList();
                return View(model);
            }
            // Kiểm tra trùng lịch giảng viên (trừ buổi học hiện tại)
            if (selectedGiangViens != null && selectedGiangViens.Any())
            {
                foreach (var giangVienId in selectedGiangViens)
                {
                    bool isGiangVienBusy = db.GiangVien_BuoiHoc
                        .Include(gv => gv.BuoiHoc)
                        .Any(gv => gv.GiangVienId == giangVienId &&
                              gv.BuoiHoc.NgayHoc == model.NgayHoc &&
                              gv.BuoiHoc.BuoiHocId != model.BuoiHocId && // Loại trừ buổi học hiện tại
                              ((model.GioBatDau >= gv.BuoiHoc.GioBatDau && model.GioBatDau < gv.BuoiHoc.GioKetThuc) ||
                               (model.GioKetThuc > gv.BuoiHoc.GioBatDau && model.GioKetThuc <= gv.BuoiHoc.GioKetThuc) ||
                               (model.GioBatDau <= gv.BuoiHoc.GioBatDau && model.GioKetThuc >= gv.BuoiHoc.GioKetThuc)));

                    if (isGiangVienBusy)
                    {
                        var busyGiangVien = db.GiangViens.Find(giangVienId);
                        ModelState.AddModelError("", $"Giảng viên {busyGiangVien.HoVaTen} đã có lịch dạy trùng thời gian này.");
                        ViewBag.LopHocList = db.LopHocs.ToList();
                        ViewBag.GiangVienList = db.GiangViens.ToList();
                        return View(model);
                    }
                }
            }
            // Cập nhật thông tin buổi học
            buoiHoc.NgayHoc = model.NgayHoc;
            buoiHoc.GioBatDau = model.GioBatDau;
            buoiHoc.GioKetThuc = model.GioKetThuc;
            buoiHoc.TrangThai = model.TrangThai;
            buoiHoc.GhiChu = model.GhiChu;
            buoiHoc.LopHoc = lopHoc;

          
            // --- XỬ LÝ GIẢNG VIÊN ---
            // Lấy danh sách giảng viên hiện tại của buổi học
            var existingGiangVienBuoiHoc = db.GiangVien_BuoiHoc
                .Where(gv => gv.BuoiHocId == model.BuoiHocId)
                .ToList();

            // Lấy danh sách ID giảng viên sẽ bị xóa (có trong DB nhưng không có trong danh sách chọn)
            var giangVienIdsToRemove = existingGiangVienBuoiHoc
                    .Where(gv => selectedGiangViens == null || !selectedGiangViens.Contains(gv.GiangVienId))
                    .Select(gv => gv.GiangVienId)
                    .ToList();


            // Xóa điểm danh của các giảng viên bị xóa khỏi buổi học
            if (giangVienIdsToRemove.Any())
            {
                var diemDanhToRemove = db.DiemDanhs_GVs
                    .Where(d => d.BuoiHocId == model.BuoiHocId &&
                           giangVienIdsToRemove.Contains(d.GiangVienId))
                    .ToList();

                db.DiemDanhs_GVs.RemoveRange(diemDanhToRemove);
            }

            // Xóa hết danh sách giảng viên đã có cho buổi học hiện tại
            db.GiangVien_BuoiHoc.RemoveRange(existingGiangVienBuoiHoc);
            db.SaveChanges();

            // Thêm lại giảng viên theo danh sách được chọn
            if (selectedGiangViens != null && selectedGiangViens.Any())
            {
                foreach (var giangVienId in selectedGiangViens)
                {
                    var giangVien = db.GiangViens.Find(giangVienId);
                    if (giangVien != null)
                    {
                        db.GiangVien_BuoiHoc.Add(new GiangVien_BuoiHoc
                        {
                            Id = Guid.NewGuid(),
                            BuoiHocId = model.BuoiHocId,
                            GiangVienId = giangVienId
                        });
                    }
                }
                db.SaveChanges();
            }
            // --- KẾT THÚC XỬ LÝ GIẢNG VIÊN ---

            if (attachments != null)
            {
                UploadAttachments(buoiHoc.BuoiHocId, attachments);
            }

            TempData["Success"] = "Cập nhật buổi học thành công!";
            return RedirectToAction("Edit", new { id = buoiHoc.BuoiHocId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BuoiHoc buoiHoc = db.BuoiHocs.Find(id);
            if (buoiHoc == null)
            {
                TempData["Error"] = "Buổi học không tồn tại!";
                return RedirectToAction("Index");
            }

            var giangVienBuoiHocs = db.GiangVien_BuoiHoc.Where(gv => gv.BuoiHocId == id).ToList();
            db.GiangVien_BuoiHoc.RemoveRange(giangVienBuoiHocs);

            var diemDanhHV = db.DiemDanhs_HVs.Where(dh => dh.BuoiHocId == id).ToList();
            db.DiemDanhs_HVs.RemoveRange(diemDanhHV);

            var diemDanhGV = db.DiemDanhs_GVs.Where(dg => dg.BuoiHocId == id).ToList();
            db.DiemDanhs_GVs.RemoveRange(diemDanhGV);

            var buoiHocAttachments = db.BuoiHocAttachments.Where(bha => bha.BuoiHocId == id).ToList();
            foreach (var attachment in buoiHocAttachments)
            {
                var filePath = Server.MapPath(attachment.Attachment.FilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                db.Attachments.Remove(attachment.Attachment);
            }
            db.BuoiHocAttachments.RemoveRange(buoiHocAttachments);

            // Xóa buổi học khỏi bảng BuoiHocs
            db.BuoiHocs.Remove(buoiHoc);
            db.SaveChanges();

            TempData["Success"] = "Xóa buổi học thành công!";
            return RedirectToAction("Index");
        }
        // Xem chi tiết buổi học
        public ActionResult Details(Guid id)
        {
            var buoiHoc = db.BuoiHocs
                .Include(b => b.BuoiHocAttachments.Select(a => a.Attachment))
                .FirstOrDefault(b => b.BuoiHocId == id);
            if (buoiHoc == null)
                return HttpNotFound();
            return View(buoiHoc);
        }

        // Upload tệp đính kèm
        private void UploadAttachments(Guid buoiHocId, HttpPostedFileBase[] attachments)
        {
            if (attachments == null || attachments.Length == 0)
                return;

            string uploadPath = Server.MapPath("~/Upload/BuoiHoc/");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var newAttachments = new List<Attachment>();
            var newBuoiHocAttachments = new List<BuoiHocAttachment>();

            foreach (var file in attachments)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                        continue;

                    string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
                    string fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMddHHmmss}_{uniqueSuffix}{extension}";
                    string filePath = Path.Combine(uploadPath, fileName);
                    file.SaveAs(filePath);

                    var attachment = new Attachment
                    {
                        AttachmentId = Guid.NewGuid(),
                        FileName = fileName,
                        FileType = extension.TrimStart('.'),
                        FilePath = "/Upload/BuoiHoc/" + fileName,
                        UploadDate = DateTime.Now
                    };

                    newAttachments.Add(attachment);

                    newBuoiHocAttachments.Add(new BuoiHocAttachment
                    {
                        BuoiHocId = buoiHocId,
                        AttachmentId = attachment.AttachmentId
                    });
                }
            }

            if (newAttachments.Any())
            {
                db.Attachments.AddRange(newAttachments);
                db.BuoiHocAttachments.AddRange(newBuoiHocAttachments);
                db.SaveChanges();
            }
        }

        public ActionResult DeleteAttachment(Guid attachmentId, Guid buoiHocId)
        {
            var attachment = db.Attachments.Find(attachmentId);
            if (attachment == null)
            {
                return HttpNotFound();
            }

            // Xóa tệp từ thư mục lưu trữ
            string filePath = Server.MapPath(attachment.FilePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Xóa khỏi bảng Attachment và BuoiHocAttachment
            var buoiHocAttachment = db.BuoiHocAttachments.FirstOrDefault(k => k.AttachmentId == attachmentId);
            if (buoiHocAttachment != null)
            {
                db.BuoiHocAttachments.Remove(buoiHocAttachment);
            }

            db.Attachments.Remove(attachment);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = buoiHocId });
        }
        public void UpdateTrangThaiBuoiHocs()
        {
            var buoiHocs = db.BuoiHocs.ToList();
            DateTime now = DateTime.Now;
            foreach (var buoiHoc in buoiHocs)
            {
                DateTime startDateTime = buoiHoc.NgayHoc.Date.Add(buoiHoc.GioBatDau);
                DateTime endDateTime = buoiHoc.NgayHoc.Date.Add(buoiHoc.GioKetThuc);

                if (now < startDateTime && buoiHoc.TrangThai != TrangThaiBuoiHoc.SapDienRa)
                {
                    buoiHoc.TrangThai = TrangThaiBuoiHoc.SapDienRa;
                }
                else if (now >= startDateTime && now < endDateTime && buoiHoc.TrangThai != TrangThaiBuoiHoc.DangDienRa)
                {
                    buoiHoc.TrangThai = TrangThaiBuoiHoc.DangDienRa;
                }
                else if (now >= endDateTime && buoiHoc.TrangThai != TrangThaiBuoiHoc.DaKetThuc)
                {
                    buoiHoc.TrangThai = TrangThaiBuoiHoc.DaKetThuc;
                }
            }
            db.SaveChanges();
        }
        [HttpPost]
        public JsonResult GetAvailableGiangViens(DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, Guid? buoiHocId = null)
        {
            var allGVs = db.GiangViens.ToList();

            var selectedGVIds = buoiHocId != null
                ? db.GiangVien_BuoiHoc
                    .Where(x => x.BuoiHocId == buoiHocId)
                    .Select(x => x.GiangVienId)
                    .ToList()
                : new List<string>();

            var danhSach = allGVs.Select(gv => new
            {
                gv.GiangVienId,
                gv.HoVaTen,
                IsSelected = selectedGVIds.Contains(gv.GiangVienId),
                IsBusy = db.BuoiHocs.Any(b =>
                    b.BuoiHocId != buoiHocId && // bỏ qua chính nó
                    b.NgayHoc == ngayHoc &&
                    (
                        (gioBatDau >= b.GioBatDau && gioBatDau < b.GioKetThuc) ||
                        (gioKetThuc > b.GioBatDau && gioKetThuc <= b.GioKetThuc) ||
                        (gioBatDau <= b.GioBatDau && gioKetThuc >= b.GioKetThuc)
                    ) &&
                    b.GiangVien_BuoiHocs.Any(g => g.GiangVienId == gv.GiangVienId)
                )
            });

            return Json(danhSach);
        }
        

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase upload)
        {
            try
            {
                if (upload == null || upload.ContentLength <= 0)
                {
                    return Json(new { uploaded = 0, error = new { message = "Không có file được tải lên" } });
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(upload.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { uploaded = 0, error = new { message = "Định dạng file không hợp lệ" } });
                }

                var fileName = Guid.NewGuid() + fileExtension;
                var uploadPath = Server.MapPath("~/Upload/BuoiHoc/CKEditorImages");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                upload.SaveAs(filePath);

                var fileUrl = Url.Content("~/Upload/BuoiHoc/CKEditorImages/" + fileName);
                return Json(new { uploaded = 1, fileName = fileName, url = fileUrl });
            }
            catch (Exception ex)
            {
                return Json(new { uploaded = 0, error = new { message = "Lỗi: " + ex.Message } });
            }
        }


    }
}
