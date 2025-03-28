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

        // Hiển thị toàn bộ danh sách buổi học
        public ActionResult Index()
        {
            var buoiHocs = db.BuoiHocs.Include(b => b.LopHoc).ToList();
            return View(buoiHocs);
        }

        // Tạo buổi học (GET)
        public ActionResult Create()
        {
            ViewBag.LopHocList = db.LopHocs.ToList();
            return View();
        }

        // Tạo buổi học (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BuoiHoc model, Guid LopHocId, HttpPostedFileBase[] attachments)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LopHocList = db.LopHocs.ToList();
                return View(model);
            }

            var lopHoc = db.LopHocs.Find(LopHocId);
            if (lopHoc == null)
            {
                ModelState.AddModelError("LopHocId", "Lớp học không tồn tại.");
                ViewBag.LopHocList = db.LopHocs.ToList();
                return View(model);
            }

            // Kiểm tra NgayHoc có nằm trong khoảng NgayBatDau - NgayKetThuc của lớp không?
            if (model.NgayHoc < lopHoc.NgayBatDau || model.NgayHoc > lopHoc.NgayKetThuc)
            {
                ModelState.AddModelError("NgayHoc", "Ngày học phải nằm trong khoảng từ " + lopHoc.NgayBatDau.ToShortDateString() + " đến " + lopHoc.NgayKetThuc.ToShortDateString());
                ViewBag.LopHocList = db.LopHocs.ToList();
                return View(model);
            }

            // Kiểm tra trùng thời gian với buổi học khác của cùng lớp
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
                return View(model);
            }

            model.BuoiHocId = Guid.NewGuid();
            model.LopHoc = lopHoc;

            db.BuoiHocs.Add(model);
            db.SaveChanges();

            if (attachments != null)
            {
                UploadAttachments(model.BuoiHocId, attachments);
            }

            TempData["SuccessMessage"] = "Buổi học đã được tạo thành công!";
            return RedirectToAction("Index");
        }

        // Chỉnh sửa buổi học (GET)
        public ActionResult Edit(Guid id)
        {
            var buoiHoc = db.BuoiHocs
                .Include(k => k.BuoiHocAttachments.Select(a => a.Attachment))
                .FirstOrDefault(k => k.BuoiHocId == id);
            if (buoiHoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.LopHocList = db.LopHocs.ToList();
            return View(buoiHoc);
        }

        // Chỉnh sửa buổi học (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BuoiHoc model, Guid LopHocId, HttpPostedFileBase[] attachments)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LopHocList = db.LopHocs.ToList();
                return View(model);
            }

            var buoiHoc = db.BuoiHocs.Find(model.BuoiHocId);
            if (buoiHoc == null)
            {
                return HttpNotFound();
            }

            var lopHoc = db.LopHocs.Find(LopHocId);
            if (lopHoc == null)
            {
                ModelState.AddModelError("LopHocId", "Lớp học không tồn tại.");
                ViewBag.LopHocList = db.LopHocs.ToList();
                return View(model);
            }

            // Kiểm tra NgayHoc có nằm trong khoảng hợp lệ không?
            if (model.NgayHoc < lopHoc.NgayBatDau || model.NgayHoc > lopHoc.NgayKetThuc)
            {
                ModelState.AddModelError("NgayHoc", "Ngày học phải nằm trong khoảng từ " + lopHoc.NgayBatDau.ToShortDateString() + " đến " + lopHoc.NgayKetThuc.ToShortDateString());
                ViewBag.LopHocList = db.LopHocs.ToList();
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
                return View(model);
            }

            // Cập nhật thông tin buổi học
            buoiHoc.NgayHoc = model.NgayHoc;
            buoiHoc.GioBatDau = model.GioBatDau;
            buoiHoc.GioKetThuc = model.GioKetThuc;
            buoiHoc.TrangThai = model.TrangThai;
            buoiHoc.GhiChu = model.GhiChu;
            buoiHoc.LopHoc = lopHoc;

            db.SaveChanges();

            if (attachments != null)
            {
                UploadAttachments(buoiHoc.BuoiHocId, attachments);
            }

            TempData["SuccessMessage"] = "Cập nhật buổi học thành công!";
            return RedirectToAction("Index");
        }

        // Xóa buổi học (GET)
        public ActionResult Delete(Guid id)
        {
            var buoiHoc = db.BuoiHocs.Find(id);
            if (buoiHoc == null)
                return HttpNotFound();
            return View(buoiHoc);
        }

        // Xóa buổi học (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var buoiHoc = db.BuoiHocs.Find(id);
            if (buoiHoc != null)
            {
                db.BuoiHocs.Remove(buoiHoc);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Buổi học đã được xóa thành công!";
            }
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

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".ppt", ".pptx", ".txt" };

            foreach (var file in attachments)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                        continue;

                    string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
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
                    db.Attachments.Add(attachment);
                    db.SaveChanges();

                    var buoiHocAttachment = new BuoiHocAttachment
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
