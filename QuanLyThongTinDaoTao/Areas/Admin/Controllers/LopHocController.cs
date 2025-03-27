// LopHocController.cs
using System;
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
        public ActionResult Index()
        {
            var lopHocs = db.LopHocs.Include(l => l.KhoaHoc).ToList();
            return View(lopHocs);
        }
        public ActionResult Details(Guid id)
        {
            var lopHoc = db.LopHocs.Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                                       .FirstOrDefault(k => k.LopHocId == id);
            if (lopHoc == null)
            {
                return HttpNotFound();
            }
            return View(lopHoc);
        }
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
            if (!ModelState.IsValid)
            {
                ViewBag.KhoaHocList = db.KhoaHocs.ToList();
                return View(model);
            }

            model.LopHocId = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.NgayCapNhat = DateTime.Now;

            // Gán KhoaHoc từ KhoaHocId
            var khoaHoc = db.KhoaHocs.Find(KhoaHocId);
            if (khoaHoc == null)
            {
                ModelState.AddModelError("KhoaHocId", "Khóa học không tồn tại.");
                ViewBag.KhoaHocList = db.KhoaHocs.ToList();
                return View(model);
            }

            model.KhoaHoc = khoaHoc;

            db.LopHocs.Add(model);
            db.SaveChanges();

            if (attachments != null)
            {
                UploadAttachments(model.LopHocId, attachments);
            }

            TempData["SuccessMessage"] = "Lớp học đã được tạo thành công!";
            return RedirectToAction("Index");
        }
        public ActionResult Delete(Guid id)
        {
            var lopHoc = db.LopHocs
                .Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                .FirstOrDefault(k => k.LopHocId == id);

            if (lopHoc == null)
            {
                return HttpNotFound();
            }

            // Tạo bản sao để tránh lỗi collection modified
            var lopHocAttachments = lopHoc.LopHocAttachments.ToList();

            // Xóa các file đính kèm nếu tồn tại
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

            // Xóa các liên kết trong bảng trung gian
            db.LopHocAttachments.RemoveRange(lopHocAttachments);

            // Xóa khóa học
            db.LopHocs.Remove(lopHoc);

            // Lưu thay đổi
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        // GET: Chỉnh sửa Lớp học
        public ActionResult Edit(Guid id)
        {
            var lopHoc = db.LopHocs.Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                                     .FirstOrDefault(k => k.LopHocId == id);
            if (lopHoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.KhoaHocList = db.KhoaHocs.ToList();
            return View(lopHoc);
        }

        // POST: Cập nhật thông tin Lớp học
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(LopHoc model, Guid KhoaHocId, HttpPostedFileBase[] attachments)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.KhoaHocList = db.KhoaHocs.ToList();
                return View(model);
            }

            var lopHoc = db.LopHocs.Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                                     .FirstOrDefault(k => k.LopHocId == model.LopHocId);

            if (lopHoc == null)
            {
                return HttpNotFound();
            }

            // Cập nhật thông tin lớp học
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
            if (attachments != null)
            {
                UploadAttachments(lopHoc.LopHocId, attachments);
            }

            TempData["SuccessMessage"] = "Cập nhật lớp học thành công!";
            return RedirectToAction("Index");
        }


        private void UploadAttachments(Guid lopHocId, HttpPostedFileBase[] attachments)
        {
            if (attachments == null || attachments.Length == 0) return;

            string uploadPath = Server.MapPath("~/Upload/LopHoc/");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            // Thêm các định dạng ppt, pptx, txt
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".ppt", ".pptx", ".txt" };

            foreach (var file in attachments)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension)) continue;

                    string fileName = Path.GetFileNameWithoutExtension(file.FileName)
                                       + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                                       + extension;
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

    }
}
