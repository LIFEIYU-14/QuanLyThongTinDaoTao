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

        public ActionResult Create()
        {
            ViewBag.KhoaHocList = db.KhoaHocs.ToList();
            return View();
        }

        // POST: Xử lý tạo lớp học
        [HttpPost]
        [ValidateAntiForgeryToken]
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



        // Xử lý upload tệp đính kèm
        private void UploadAttachments(Guid lopHocId, HttpPostedFileBase[] attachments)
        {
            if (attachments == null || attachments.Length == 0) return;

            string uploadPath = Server.MapPath("~/Upload/LopHoc/");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };

            foreach (var file in attachments)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension)) continue;

                    string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
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
