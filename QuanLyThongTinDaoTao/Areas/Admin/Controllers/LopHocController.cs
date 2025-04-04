﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class LopHocController : Controller
    {
        public readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // Hiển thị danh sách lớp học
        public ActionResult Index(Guid? KhoaHocId)
        {

            var lopHocs = db.LopHocs.Include(lh => lh.KhoaHoc).ToList();
            var danhSachKhoaHoc = db.KhoaHocs.ToList();
            ViewBag.KhoaHocList = danhSachKhoaHoc; // Truyền danh sách khóa học vào ViewBag
            return View(lopHocs);
        }
        public ActionResult FilterByKhoaHoc(Guid? khoaHocId)
        {
            if (khoaHocId.HasValue)
            {
                var danhSachLopHoc = db.LopHocs
                    .Include(lh => lh.KhoaHoc) // Đảm bảo load dữ liệu KhoaHoc
                    .Where(lh => lh.KhoaHoc != null && lh.KhoaHoc.KhoaHocId == khoaHocId.Value)
                    .ToList();

                return PartialView("_LopHocTablePartial", danhSachLopHoc);
            }

            // Nếu không lọc theo khóa học, trả về toàn bộ danh sách lớp học
            return PartialView("_LopHocTablePartial", db.LopHocs.Include(lh => lh.KhoaHoc).ToList());
        }

        // Hiển thị chi tiết lớp học
        public ActionResult Details(Guid id)
        {
            var lopHoc = db.LopHocs
                .Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                .FirstOrDefault(k => k.LopHocId == id);
            if (lopHoc == null)
            {
                return HttpNotFound();
            }
            return View(lopHoc);
        }

        // Hiển thị danh sách buổi học của lớp học
        public ActionResult DanhSachBuoiHoc(Guid id)
        {
            var lopHoc = db.LopHocs.Include(l => l.BuoiHocs).FirstOrDefault(k => k.LopHocId == id);
            if (lopHoc == null)
            {
                return HttpNotFound();
            }

            ViewBag.TenLopHoc = lopHoc.TenLopHoc;
            return View(lopHoc.BuoiHocs.ToList());
        }

        // GET: Tạo lớp học
        public ActionResult Create()
        {
            ViewBag.KhoaHocList = db.KhoaHocs.ToList();
            return View();
        }

        // POST: Xử lý tạo lớp học
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(LopHoc model, Guid KhoaHocId, HttpPostedFileBase[] attachments)
        {
            // Nếu ModelState không hợp lệ (bao gồm ràng buộc ngày qua CustomValidation)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Thiết lập ID và thời gian tạo, cập nhật
            model.LopHocId = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.NgayCapNhat = DateTime.Now;

            // Gán KhoaHoc từ KhoaHocId
            var khoaHoc = db.KhoaHocs.Find(KhoaHocId);
            if (khoaHoc == null)
            {
                ModelState.AddModelError("KhoaHocId", "Khóa học không tồn tại.");
                return View(model);
            }
            model.KhoaHoc = khoaHoc;

            db.LopHocs.Add(model);
            db.SaveChanges();

            // Xử lý file đính kèm nếu có
            if (attachments != null && attachments.Any())
            {
                UploadAttachments(model.LopHocId, attachments);
            }

            TempData["Success"] = "Lớp học đã được tạo thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            var lopHoc = db.LopHocs
                .Include(l => l.LopHocAttachments.Select(a => a.Attachment))
                .Include(l => l.BuoiHocs)
                .FirstOrDefault(l => l.LopHocId == id);

            if (lopHoc == null)
            {
                return HttpNotFound();
            }

            // Xóa các buổi học liên quan đến lớp học này
            if (lopHoc.BuoiHocs != null)
            {
                foreach (var buoiHoc in lopHoc.BuoiHocs.ToList())
                {
                    db.BuoiHocs.Remove(buoiHoc);
                }
            }

            // Xóa các file đính kèm của lớp học
            var lopHocAttachments = lopHoc.LopHocAttachments.ToList();
            foreach (var lopHocAttachment in lopHocAttachments)
            {
                var attachment = lopHocAttachment.Attachment;
                if (attachment != null)
                {
                    string filePath = Server.MapPath(attachment.FilePath);
                    if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    db.Attachments.Remove(attachment);
                }
            }

            // Xóa các liên kết giữa Lớp học và Attachment
            db.LopHocAttachments.RemoveRange(lopHocAttachments);

            // Xóa lớp học
            db.LopHocs.Remove(lopHoc);

            // Lưu thay đổi
            db.SaveChanges();

            return RedirectToAction("Index");
        }



        // GET: Chỉnh sửa lớp học
        public ActionResult Edit(Guid id)
        {
            var lopHoc = db.LopHocs
                .Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                .FirstOrDefault(k => k.LopHocId == id);
            if (lopHoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.KhoaHocList = db.KhoaHocs.ToList();
            return View(lopHoc);
        }

        // POST: Cập nhật thông tin lớp học
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(LopHoc model, Guid KhoaHocId, HttpPostedFileBase[] attachments)
        {
            // Kiểm tra ràng buộc ngày (CustomValidation trong model sẽ tự chạy, nhưng bạn có thể kiểm tra thêm nếu cần)
            if (!ModelState.IsValid)
            {
                ViewBag.KhoaHocList = db.KhoaHocs.ToList();
                return View(model);
            }

            var lopHoc = db.LopHocs
                .Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                .FirstOrDefault(k => k.LopHocId == model.LopHocId);

            if (lopHoc == null)
            {
                return HttpNotFound();
            }

            // Cập nhật các thông tin
            lopHoc.MaLopHoc = model.MaLopHoc;

            lopHoc.TenLopHoc = model.TenLopHoc;
            lopHoc.SoTinChi = model.SoTinChi;
            lopHoc.NgayBatDau = model.NgayBatDau;
            lopHoc.NgayKetThuc = model.NgayKetThuc;
            lopHoc.TrangThai = model.TrangThai;
            lopHoc.SoLuongToiDa = model.SoLuongToiDa;
            lopHoc.MoTa = model.MoTa;
            lopHoc.NgayCapNhat = DateTime.Now;

            // Cập nhật KhoaHoc
            var khoaHoc = db.KhoaHocs.Find(KhoaHocId);
            if (khoaHoc != null)
            {
                lopHoc.KhoaHoc = khoaHoc;
            }

            db.SaveChanges();

            // Xử lý file đính kèm nếu có
            if (attachments != null && attachments.Any())
            {
                UploadAttachments(lopHoc.LopHocId, attachments);
            }

            TempData["Success"] = "Cập nhật lớp học thành công!";
            return RedirectToAction("Index");
        }

        // Phương thức xử lý upload file đính kèm
        private void UploadAttachments(Guid lopHocId, HttpPostedFileBase[] attachments)
        {
            if (attachments == null || attachments.Length == 0)
                return;

            string uploadPath = Server.MapPath("~/Upload/LopHoc/");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Cho phép các định dạng file: ppt, pptx, txt, jpg, jpeg, png, pdf, docx
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".ppt", ".pptx", ".txt" };

            foreach (var file in attachments)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                        continue;

                    string fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    string filePath = Path.Combine(uploadPath, fileName);
                    file.SaveAs(filePath);

                    var attachment = new Attachment
                    {
                        AttachmentId = Guid.NewGuid(),
                        FileName = fileName,
                        FileType = extension.TrimStart('.'),
                        FilePath = "/Upload/LopHoc/" + fileName,
                        UploadDate = DateTime.Now
                    };
                    db.Attachments.Add(attachment);
                    db.SaveChanges();

                    var lopHocAttachment = new LopHocAttachment
                    {
                        LopHocId = lopHocId,
                        AttachmentId = attachment.AttachmentId
                    };
                    db.LopHocAttachments.Add(lopHocAttachment);
                    db.SaveChanges();
                }
            }
        }

        [HttpGet]
        public ActionResult DeleteAllAttachments(Guid lopHocId)
        {
            // Lấy danh sách tất cả các attachment liên quan đến lớp học
            var attachments = db.LopHocAttachments.Where(k => k.LopHocId == lopHocId).ToList();
            if (!attachments.Any())
            {
                return HttpNotFound("Không có tài liệu nào để xóa.");
            }

            foreach (var item in attachments)
            {
                string filePath = Server.MapPath(item.Attachment.FilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                db.Attachments.Remove(item.Attachment);
                db.LopHocAttachments.Remove(item);
            }

            db.SaveChanges();
            return RedirectToAction("Edit", new { id = lopHocId });
        }

       
    }
}
