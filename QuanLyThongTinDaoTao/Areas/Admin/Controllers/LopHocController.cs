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
    [Authorize(Roles = "Admin")]
    public class LopHocController : Controller
    {
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".ppt", ".pptx", ".txt" };

        public ActionResult Index(Guid? KhoaHocId)
        {
            var lopHocs = db.LopHocs.Include(lh => lh.KhoaHoc).ToList();
            foreach (var lopHoc in lopHocs)
            {
                CapNhatTrangThaiLopHoc(lopHoc);
            }
            db.SaveChanges(); // lưu thay đổi trạng thái
            ViewBag.KhoaHocList = db.KhoaHocs.ToList();
            return View(lopHocs);
        }

        public ActionResult FilterByKhoaHoc(Guid? khoaHocId)
        {
            if (khoaHocId.HasValue)
            {
                var danhSachLopHoc = db.LopHocs
                    .Include(lh => lh.KhoaHoc)
                    .Where(lh => lh.KhoaHoc != null && lh.KhoaHoc.KhoaHocId == khoaHocId.Value)
                    .ToList();

                return PartialView("_LopHocTablePartial", danhSachLopHoc);
            }

            return PartialView("_LopHocTablePartial", db.LopHocs.Include(lh => lh.KhoaHoc).ToList());
        }

        public ActionResult Details(Guid id)
        {
            var lopHoc = db.LopHocs
                .Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                .Include(k => k.BuoiHocs.Select(b => b.GiangVien_BuoiHocs))
                .FirstOrDefault(k => k.LopHocId == id);

            if (lopHoc == null)
            {
                return HttpNotFound();
            }

            ViewBag.TenLopHoc = lopHoc.TenLopHoc;
            ViewBag.LopHocId = lopHoc.LopHocId;

            // Sắp xếp buổi học theo ngày và giờ
            ViewBag.BuoiHocs = lopHoc.BuoiHocs
                .OrderBy(b => b.NgayHoc)
                .ThenBy(b => b.GioBatDau)
                .ToList();

            return View(lopHoc);
        }



        [HttpGet]
        public ActionResult Create(Guid? khoaHocId)
        {
            // Tạo mã lớp học tự động
            var lastLopHoc = db.LopHocs
                              .OrderByDescending(l => l.MaLopHoc)
                              .FirstOrDefault();
          
            int newNumber = 1;
            if (lastLopHoc != null && lastLopHoc.MaLopHoc.StartsWith("MH"))
            {
                if (int.TryParse(lastLopHoc.MaLopHoc.Substring(2), out int lastNumber))
                {
                    newNumber = lastNumber + 1;
                }
            }
            var model = new LopHoc
            {
                MaLopHoc = $"MH{newNumber.ToString("D3")}", // Định dạng 3 chữ số
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today.AddMonths(3) // Mặc định 3 tháng
            };
            if (khoaHocId.HasValue)
            {
                var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.KhoaHocId == khoaHocId.Value);
                ViewBag.TenKhoaHoc = khoaHoc?.TenKhoaHoc;
                model.KhoaHocId = khoaHocId.Value;
            }

            // Nếu không có KhoaHocId thì hiển thị dropdown
            if (!khoaHocId.HasValue)
            {
                ViewBag.KhoaHocList = db.KhoaHocs.ToList();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(LopHoc model, Guid KhoaHocId, HttpPostedFileBase[] attachments)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.KhoaHocList = db.KhoaHocs.ToList();

                var khoaHocEntity = db.KhoaHocs.FirstOrDefault(k => k.KhoaHocId == KhoaHocId);
                if (khoaHocEntity != null)
                {
                    ViewBag.TenKhoaHoc = khoaHocEntity.TenKhoaHoc;
                }

                return View(model);
            }
            // Kiểm tra trùng mã lớp học
            if (db.LopHocs.Any(l => l.MaLopHoc == model.MaLopHoc))
            {
                ModelState.AddModelError("MaLopHoc", "Mã lớp học đã tồn tại");
                ViewBag.KhoaHocList = db.KhoaHocs.ToList();
                return View(model);
            }
            // Kiểm tra trùng Tên lớp học
            if (db.LopHocs.Any(l => l.TenLopHoc == model.TenLopHoc))
            {
                ModelState.AddModelError("TenLopHoc", "Tên lớp học đã tồn tại.");
                ViewBag.KhoaHocList = db.KhoaHocs.ToList();
                return View(model);
            }
            model.LopHocId = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.NgayCapNhat = DateTime.Now;

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

            if (attachments != null && attachments.Any())
            {
                UploadAttachments(model.LopHocId, attachments);
            }

            TempData["Success"] = "Lớp học đã được tạo thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid id)
        {
            var lopHoc = db.LopHocs
                .Include(k => k.KhoaHoc)
                .Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                .FirstOrDefault(k => k.LopHocId == id);

            if (lopHoc == null)
            {
                return HttpNotFound();
            }

            ViewBag.KhoaHocList = db.KhoaHocs.ToList();
            return View(lopHoc);
        }

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

            var lopHoc = db.LopHocs
                .Include(k => k.KhoaHoc)
                .Include(k => k.LopHocAttachments.Select(a => a.Attachment))
                .FirstOrDefault(k => k.LopHocId == model.LopHocId);

            if (lopHoc == null)
            {
                return HttpNotFound();
            }

            // Update properties
            lopHoc.MaLopHoc = model.MaLopHoc;
            lopHoc.TenLopHoc = model.TenLopHoc;
            lopHoc.SoTinChi = model.SoTinChi;
            lopHoc.NgayBatDau = model.NgayBatDau;
            lopHoc.NgayKetThuc = model.NgayKetThuc;
            lopHoc.TrangThai = model.TrangThai;
            lopHoc.SoLuongToiDa = model.SoLuongToiDa;
            lopHoc.MoTa = model.MoTa;
            lopHoc.NgayCapNhat = DateTime.Now;

            var khoaHoc = db.KhoaHocs.Find(KhoaHocId);
            if (khoaHoc != null)
            {
                lopHoc.KhoaHoc = khoaHoc;
            }
            else
            {
                ModelState.AddModelError("KhoaHocId", "Khóa học không tồn tại");
                ViewBag.KhoaHocList = db.KhoaHocs.ToList();
                return View(model);
            }
            db.SaveChanges();

            if (attachments != null && attachments.Any())
            {
                UploadAttachments(lopHoc.LopHocId, attachments);
            }

            TempData["Success"] = "Cập nhật lớp học thành công!";
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

            // Delete related BuoiHocs
            if (lopHoc.BuoiHocs != null)
            {
                foreach (var buoiHoc in lopHoc.BuoiHocs.ToList())
                {
                    db.BuoiHocs.Remove(buoiHoc);
                }
            }

            // Delete attachments
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

            db.LopHocAttachments.RemoveRange(lopHocAttachments);
            db.LopHocs.Remove(lopHoc);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult DeleteAttachment(Guid attachmentId, Guid lopHocId)
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

            // Xóa khỏi bảng Attachment và LopHocAttachment
            var lopHocAttachment = db.LopHocAttachments.FirstOrDefault(l => l.AttachmentId == attachmentId);
            if (lopHocAttachment != null)
            {
                db.LopHocAttachments.Remove(lopHocAttachment);
            }

            db.Attachments.Remove(attachment);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = lopHocId });
        }

        [HttpGet]
        public ActionResult DeleteAllAttachments(Guid lopHocId)
        {
            var attachments = db.LopHocAttachments.Where(l => l.LopHocId == lopHocId).ToList();
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


        private void UploadAttachments(Guid lopHocId, HttpPostedFileBase[] attachments)
        {
            if (attachments == null || attachments.Length == 0)
                return;

            string uploadPath = Server.MapPath("~/Upload/LopHoc/");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var newAttachments = new List<Attachment>();
            var newLopHocAttachments = new List<LopHocAttachment>();

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
                        FilePath = "/Upload/LopHoc/" + fileName,
                        UploadDate = DateTime.Now
                    };

                    newAttachments.Add(attachment);

                    newLopHocAttachments.Add(new LopHocAttachment
                    {
                        LopHocId = lopHocId,
                        AttachmentId = attachment.AttachmentId
                    });
                }
            }

            if (newAttachments.Any())
            {
                db.Attachments.AddRange(newAttachments);
                db.LopHocAttachments.AddRange(newLopHocAttachments);
                db.SaveChanges();
            }
        }

        private void CapNhatTrangThaiLopHoc(LopHoc lopHoc)
        {
            var now = DateTime.Now;

            if (lopHoc.NgayBatDau > now)
            {
                lopHoc.TrangThai = LopHoc.TrangThaiLopHoc.SapMo;
            }
            else if (lopHoc.NgayBatDau <= now && lopHoc.NgayKetThuc >= now)
            {
                lopHoc.TrangThai = LopHoc.TrangThaiLopHoc.DaBatDau;
            }
            else if (lopHoc.NgayKetThuc < now)
            {
                lopHoc.TrangThai = LopHoc.TrangThaiLopHoc.DaKetThuc;
            }
        }

        [HttpGet]
        public JsonResult GetTrangThaiLopHoc()
        {
            var lopHocs = db.LopHocs.ToList();
            foreach (var lopHoc in lopHocs)
            {
                CapNhatTrangThaiLopHoc(lopHoc);
            }
            db.SaveChanges();

            var data = lopHocs.Select(lh => new
            {
                LopHocId = lh.LopHocId,
                TrangThai = GetTrangThaiText(lh.TrangThai),
                BadgeClass = GetBadgeClass(lh.TrangThai)
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private string GetTrangThaiText(LopHoc.TrangThaiLopHoc trangThai)
        {
            switch (trangThai)
            {
                case LopHoc.TrangThaiLopHoc.SapMo:
                    return "Sắp mở";
                case LopHoc.TrangThaiLopHoc.DaBatDau:
                    return "Đã Bắt Đầu";
                case LopHoc.TrangThaiLopHoc.DaKetThuc:
                    return "Kết thúc";
                default:
                    return "Không xác định";
            }
        }

        private string GetBadgeClass(LopHoc.TrangThaiLopHoc trangThai)
        {
            switch (trangThai)
            {
                case LopHoc.TrangThaiLopHoc.SapMo:
                    return "bg-primary";
                case LopHoc.TrangThaiLopHoc.DaBatDau:
                    return "bg-success";
                case LopHoc.TrangThaiLopHoc.DaKetThuc:
                    return "bg-secondary";
                default:
                    return "bg-dark";
            }
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
                var uploadPath = Server.MapPath("~/Upload/LopHoc/CKEditorImages");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                upload.SaveAs(filePath);

                var fileUrl = Url.Content("~/Upload/LopHoc/CKEditorImages/" + fileName);
                return Json(new { uploaded = 1, fileName = fileName, url = fileUrl });
            }
            catch (Exception ex)
            {
                return Json(new { uploaded = 0, error = new { message = "Lỗi: " + ex.Message } });
            }
        }

    }
}
