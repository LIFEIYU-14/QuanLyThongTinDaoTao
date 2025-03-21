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
    public class KhoaHocController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // Hiển thị danh sách Khóa học
        public ActionResult Index()
        {
            var khoaHocs = db.KhoaHocs.Include(k => k.KhoaHocAttachments).ToList();
            return View(khoaHocs);
        }
        public ActionResult Details(Guid id)
        {
            var khoaHoc = db.KhoaHocs.Include(k => k.KhoaHocAttachments.Select(a => a.Attachment))
                                       .FirstOrDefault(k => k.KhoaHocId == id);
            if (khoaHoc == null)
            {
                return HttpNotFound();
            }
            return View(khoaHoc);
        }

        // GET: Tạo mới Khóa học
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tạo mới Khóa học
        [HttpPost]
        public ActionResult Create(KhoaHoc model, HttpPostedFileBase[] attachments)
        {
            if (ModelState.IsValid)
            {
                model.KhoaHocId = Guid.NewGuid();
                model.NgayTao = DateTime.Now;
                model.NgayCapNhat = DateTime.Now;
                db.KhoaHocs.Add(model);
                db.SaveChanges();

                UploadAttachments(model.KhoaHocId, attachments);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Chỉnh sửa Khóa học
        public ActionResult Edit(Guid id)
        {
            var khoaHoc = db.KhoaHocs.Include(k => k.KhoaHocAttachments).FirstOrDefault(k => k.KhoaHocId == id);
            if (khoaHoc == null)
            {
                return HttpNotFound();
            }
            return View(khoaHoc);
        }

        // POST: Chỉnh sửa Khóa học
        [HttpPost]
        public ActionResult Edit(KhoaHoc model, HttpPostedFileBase[] attachments)
        {
            if (ModelState.IsValid)
            {
                var khoaHoc = db.KhoaHocs.Find(model.KhoaHocId);
                if (khoaHoc == null)
                {
                    return HttpNotFound();
                }

                khoaHoc.TenKhoaHoc = model.TenKhoaHoc;
                khoaHoc.MoTa = model.MoTa;
                khoaHoc.ThoiLuong = model.ThoiLuong;
                khoaHoc.NgayCapNhat = DateTime.Now;
                db.SaveChanges();

                UploadAttachments(khoaHoc.KhoaHocId, attachments);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Xóa Khóa học
        public ActionResult Delete(Guid id)
        {
            var khoaHoc = db.KhoaHocs.Include(k => k.KhoaHocAttachments.Select(a => a.Attachment)).FirstOrDefault(k => k.KhoaHocId == id);
            if (khoaHoc == null)
            {
                return HttpNotFound();
            }

            foreach (var attachment in khoaHoc.KhoaHocAttachments)
            {
                string filePath = Server.MapPath(attachment.Attachment.FilePath);
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                db.Attachments.Remove(attachment.Attachment);
            }

            db.KhoaHocAttachments.RemoveRange(khoaHoc.KhoaHocAttachments);
            db.KhoaHocs.Remove(khoaHoc);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Upload tệp đính kèm
        private void UploadAttachments(Guid khoaHocId, HttpPostedFileBase[] attachments)
        {
            if (attachments == null || !attachments.Any())
            {
                return;
            }

            string uploadPath = Server.MapPath("~/Upload/KhoaHoc/");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };
            var khoaHoc = db.KhoaHocs.Find(khoaHocId);

            if (khoaHoc == null)
            {
                return;
            }

            foreach (var file in attachments)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Định dạng tệp không hợp lệ!");
                        return;
                    }

                    // Tạo tên tệp mới: KH_TenKhoaHoc_Timestamp.extension
                    string sanitizedTenKhoaHoc = string.Join("_", khoaHoc.TenKhoaHoc.Split(Path.GetInvalidFileNameChars()));
                    string fileName = $"KH_{sanitizedTenKhoaHoc}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    string filePath = Path.Combine(uploadPath, fileName);
                    file.SaveAs(filePath);

                    var attachment = new Attachment
                    {
                        FileName = fileName,
                        FileType = extension.TrimStart('.'),
                        FilePath = "/Upload/KhoaHoc/" + fileName,
                        UploadDate = DateTime.Now
                    };
                    db.Attachments.Add(attachment);
                    db.SaveChanges();

                    var khoaHocAttachment = new KhoaHocAttachment
                    {
                        KhoaHocId = khoaHocId,
                        AttachmentId = attachment.AttachmentId
                    };
                    db.KhoaHocAttachments.Add(khoaHocAttachment);
                }
            }
            db.SaveChanges();
        }

    }
}
