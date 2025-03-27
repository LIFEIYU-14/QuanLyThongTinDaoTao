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
    public class BuoiHocController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        // Phương thức kiểm tra lỗi
        private ActionResult LopHocNotFound(Guid id)
        {
            return HttpNotFound($"Lớp học với ID {id} không tồn tại.");
        }
        // Hiển thị toàn bộ danh sách buổi học
        public ActionResult Index(Guid id)
        {
            var lopHoc = db.LopHocs.Include(l => l.BuoiHocs).FirstOrDefault(l => l.LopHocId == id);
            if (lopHoc == null)
            {
                return LopHocNotFound(id);
            }
            ViewBag.LopHoc = lopHoc;
            return View(lopHoc.BuoiHocs.ToList());
        }

        // Tạo buổi học
        public ActionResult Create()
        {
            ViewBag.LopHocLists = db.LopHocs.ToList();
            return View();
        }

        // Tạo buổi học mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BuoiHoc model, Guid LopHocId, HttpPostedFileBase[] attachments)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LopHocLists = db.LopHocs.ToList();
                return View(model);
            }
            model.BuoiHocId = Guid.NewGuid();
            // Gán LopHoc từ LopHocId
            var lopHoc = db.LopHocs.Find(LopHocId);
            if (lopHoc == null)
            {
                ModelState.AddModelError("LopHocId", "Lớp học không tồn tại.");
                ViewBag.LopHocLists = db.LopHocs.ToList();
                return View(model);
            }
            model.LopHoc = lopHoc;

            db.BuoiHocs.Add(model);
            db.SaveChanges();

            if (attachments != null)
            {
                UploadAttachments(model.BuoiHocId, attachments);
            }

            TempData["SuccessMessage"] = "Lớp học đã được tạo thành công!";
            return RedirectToAction("Index");
        }

        // Chỉnh sửa buổi học (GET)
        public ActionResult Edit(Guid id)
        {
            var buoiHoc = db.BuoiHocs.Find(id);
            if (buoiHoc == null)
                return HttpNotFound();

            ViewBag.BuoiHocList = db.BuoiHocs.Include("LopHoc").ToList();
            return View(buoiHoc);
        }

        // Chỉnh sửa buổi học (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BuoiHoc buoiHoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(buoiHoc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = buoiHoc.LopHoc.LopHocId });
            }
            return View(buoiHoc);
        }

        // Xóa buổi học (GET)
        public ActionResult Delete(Guid id)
        {
            var buoiHoc = db.BuoiHocs.Find(id);
            if (buoiHoc == null)
                return HttpNotFound();
            return View(buoiHoc);
        }

        // Xác nhận xóa buổi học (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var buoiHoc = db.BuoiHocs.Find(id);
            if (buoiHoc == null)
            {
                return HttpNotFound();
            }

            Guid lopHocId = buoiHoc.LopHoc.LopHocId;
            db.BuoiHocs.Remove(buoiHoc);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = lopHocId });
        }

        // Xem chi tiết buổi học
        public ActionResult Details(Guid id)
        {
            var buoiHoc = db.BuoiHocs.Find(id);
            if (buoiHoc == null)
                return HttpNotFound();
            return View(buoiHoc);
        }

        private void UploadAttachments(Guid buoiHocId, HttpPostedFileBase[] attachments)
        {
            if (attachments == null || attachments.Length == 0) return;

            string uploadPath = Server.MapPath("~/Upload/BuoiHoc/");
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

                    var buoiHocAttachment = new LopHocAttachment
                    {
                        BuoiHocId = buoiHocId,
                        AttachmentId = attachment.AttachmentId
                    };
                    db.BuoiHocAttachments.Add(buoiHocAttachment);
                    db.SaveChanges();
                }
            }
        }
    }
}
