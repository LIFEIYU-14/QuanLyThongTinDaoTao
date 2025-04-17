using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    [RoleAuthorize("Admin")]
    public class KhoaHocController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".txt", ".pptx", ".xlsx" };
        private readonly string[] hinhDaiDienExtensions = { ".jpg", ".jpeg", ".png" };

        // Hiển thị danh sách Khóa học
        public ActionResult Index(int? page)
        {
            int pageSize = 12; // Số lượng khóa học trên mỗi trang
            int pageNumber = (page ?? 1); // Trang mặc định là trang 1

            var khoaHocs = db.KhoaHocs.Include(k => k.KhoaHocAttachments)
                                       .OrderByDescending(k => k.NgayTao)
                                       .ToPagedList(pageNumber, pageSize);

            return View(khoaHocs);
        }
        public ActionResult Details(Guid id)
        {
            var khoaHoc = db.KhoaHocs
                .Include(k => k.KhoaHocAttachments.Select(a => a.Attachment))
                .Include(k => k.LopHocs.Select(l => l.BuoiHocs))
                .FirstOrDefault(k => k.KhoaHocId == id);

            if (khoaHoc == null)
            {
                return HttpNotFound();
            }

            ViewBag.TenKhoaHoc = khoaHoc.TenKhoaHoc;
            ViewBag.KhoaHocId = khoaHoc.KhoaHocId;

            return View(khoaHoc);
        }



        // GET: Tạo mới Khóa học
        [HttpGet]
        public ActionResult Create()
        {
            // Tạo mã khóa học tự động
            var lastKhoaHoc = db.KhoaHocs
                                .OrderByDescending(k => k.MaKhoaHoc)
                                .FirstOrDefault();

            int newNumber = 1;
            if (lastKhoaHoc != null && lastKhoaHoc.MaKhoaHoc.StartsWith("KH"))
            {
                if (int.TryParse(lastKhoaHoc.MaKhoaHoc.Substring(2), out int lastNumber))
                {
                    newNumber = lastNumber + 1;
                }
            }

            var model = new KhoaHoc
            {
                MaKhoaHoc = $"KH{newNumber.ToString("D3")}" // Định dạng 3 chữ số
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhoaHoc model, HttpPostedFileBase hinhDaiDien, HttpPostedFileBase[] attachments)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng mã khóa học (phòng trường hợp có nhiều người cùng thêm)
                if (db.KhoaHocs.Any(k => k.MaKhoaHoc == model.MaKhoaHoc))
                {
                    ModelState.AddModelError("MaKhoaHoc", "Mã khóa học đã tồn tại");
                    return View(model);
                }

                model.KhoaHocId = Guid.NewGuid();
                model.NgayTao = DateTime.Now;
                model.NgayCapNhat = DateTime.Now;

                // Xử lý upload hình đại diện
                if (hinhDaiDien != null && hinhDaiDien.ContentLength > 0)
                {
                    string extension = Path.GetExtension(hinhDaiDien.FileName).ToLower();
                    if (!hinhDaiDienExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Định dạng hình đại diện không hợp lệ!");
                        return View(model);
                    }

                    string uploadPath = Server.MapPath("~/Upload/KhoaHoc/HinhDaiDien/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string fileName = $"{model.KhoaHocId}_HinhDaiDien{extension}";
                    string filePath = Path.Combine(uploadPath, fileName);
                    hinhDaiDien.SaveAs(filePath);
                    model.HinhDaiDienKhoaHoc = $"/Upload/KhoaHoc/HinhDaiDien/{fileName}";
                }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KhoaHoc model, HttpPostedFileBase hinhDaiDien, HttpPostedFileBase[] attachments)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var khoaHoc = db.KhoaHocs.Find(model.KhoaHocId);
            if (khoaHoc == null)
            {
                return HttpNotFound();
            }

            // Xử lý lưu hình ảnh nếu có upload file mới
            if (hinhDaiDien != null && hinhDaiDien.ContentLength > 0)
            {
                string extension = Path.GetExtension(hinhDaiDien.FileName).ToLower();
                // Kiểm tra định dạng hợp lệ (chỉ cho phép jpg, jpeg, png)
                if (!new string[] { ".jpg", ".jpeg", ".png" }.Contains(extension))
                {
                    ModelState.AddModelError("", "Định dạng hình đại diện không hợp lệ! Vui lòng chọn file jpg, jpeg hoặc png.");
                    return View(model);
                }

                try
                {
                    // Xóa hình cũ nếu có
                    if (!string.IsNullOrEmpty(khoaHoc.HinhDaiDienKhoaHoc))
                    {
                        string oldFilePath = Server.MapPath(khoaHoc.HinhDaiDienKhoaHoc);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Đảm bảo thư mục upload tồn tại
                    string folder = "~/Upload/KhoaHoc/HinhDaiDien/";
                    string directoryPath = Server.MapPath(folder);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Tạo tên file mới theo KhoaHocId và thời gian hiện tại để tránh trùng lặp
                    string newFileName = $"{khoaHoc.KhoaHocId}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    string filePath = Path.Combine(directoryPath, newFileName);
                    hinhDaiDien.SaveAs(filePath);

                    // Cập nhật đường dẫn hình đại diện
                    khoaHoc.HinhDaiDienKhoaHoc = folder + newFileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi lưu hình ảnh: {ex.Message}");
                    return View(model);
                }
            }

            // Cập nhật thông tin khác của khóa học
            khoaHoc.TenKhoaHoc = model.TenKhoaHoc;
            khoaHoc.MaKhoaHoc = model.MaKhoaHoc;
            khoaHoc.MoTa = model.MoTa;
            khoaHoc.ThoiLuong = model.ThoiLuong;
            khoaHoc.NgayCapNhat = DateTime.Now;

            db.SaveChanges();

            // Xử lý upload các tệp đính kèm (nếu có)
            UploadAttachments(khoaHoc.KhoaHocId, attachments);

            return RedirectToAction("Index");
        }


        public ActionResult Delete(Guid id)
        {
            var khoaHoc = db.KhoaHocs
                .Include(k => k.KhoaHocAttachments.Select(a => a.Attachment))
                .Include(k => k.LopHocs.Select(l => l.BuoiHocs))
                .FirstOrDefault(k => k.KhoaHocId == id);

            if (khoaHoc == null)
            {
                return HttpNotFound();
            }

            // Xóa các lớp học và các buổi học liên quan
            if (khoaHoc.LopHocs != null)
            {
                foreach (var lopHoc in khoaHoc.LopHocs.ToList())
                {
                    if (lopHoc.BuoiHocs != null)
                    {
                        foreach (var buoiHoc in lopHoc.BuoiHocs.ToList())
                        {
                            db.BuoiHocs.Remove(buoiHoc);
                        }
                    }
                    db.LopHocs.Remove(lopHoc);
                }
            }

            // Xóa các file đính kèm của Khóa học
            var khoaHocAttachments = khoaHoc.KhoaHocAttachments.ToList();
            foreach (var khoaHocAttachment in khoaHocAttachments)
            {
                var attachment = khoaHocAttachment.Attachment;
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

            // Xóa các liên kết giữa Khóa học và Attachment
            db.KhoaHocAttachments.RemoveRange(khoaHocAttachments);

            // Xóa Khóa học
            db.KhoaHocs.Remove(khoaHoc);

            // Lưu thay đổi
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
                        ModelState.AddModelError("", $"Định dạng tệp '{extension}' không được hỗ trợ!");
                        return;
                    }

                    if (file.ContentLength > 1073741824) // 1GB
                    {
                        ModelState.AddModelError("", "Kích thước tệp không được vượt quá 1GB!");
                        return;
                    }

                    string sanitizedTenKhoaHoc = string.Join("_", khoaHoc.TenKhoaHoc.Split(Path.GetInvalidFileNameChars()));
                    string fileName = $"KH_{sanitizedTenKhoaHoc}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    string filePath = Path.Combine(uploadPath, fileName);
                    file.SaveAs(filePath);

                    var attachment = new Attachment
                    {
                        AttachmentId = Guid.NewGuid(),
                        FileName = fileName,
                        FileType = extension.TrimStart('.'),
                        FilePath = $"/Upload/KhoaHoc/{fileName}",
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
        public ActionResult DeleteAttachment(Guid attachmentId, Guid khoaHocId)
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

            // Xóa khỏi bảng Attachment và KhoaHocAttachment
            var khoaHocAttachment = db.KhoaHocAttachments.FirstOrDefault(k => k.AttachmentId == attachmentId);
            if (khoaHocAttachment != null)
            {
                db.KhoaHocAttachments.Remove(khoaHocAttachment);
            }

            db.Attachments.Remove(attachment);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = khoaHocId });
        }
        [HttpGet]
        public ActionResult DeleteAllAttachments(Guid khoaHocId)
        {
            // Lấy danh sách tất cả attachment liên quan đến khóa học
            var attachments = db.KhoaHocAttachments.Where(k => k.KhoaHocId == khoaHocId).ToList();
            if (!attachments.Any())
            {
                return HttpNotFound("Không có tài liệu nào để xóa.");
            }

            foreach (var item in attachments)
            {
                // Xóa tệp vật lý
                string filePath = Server.MapPath(item.Attachment.FilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Xóa bản ghi trong bảng Attachment và KhoaHocAttachment
                db.Attachments.Remove(item.Attachment);
                db.KhoaHocAttachments.Remove(item);
            }

            db.SaveChanges();
            return RedirectToAction("Edit", new { id = khoaHocId });
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
                var uploadPath = Server.MapPath("~/Upload/KhoaHoc/CKEditorImages");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                upload.SaveAs(filePath);

                var fileUrl = Url.Content("~/Upload/KhoaHoc/CKEditorImages/" + fileName);
                return Json(new { uploaded = 1, fileName = fileName, url = fileUrl });
            }
            catch (Exception ex)
            {
                return Json(new { uploaded = 0, error = new { message = "Lỗi: " + ex.Message } });
            }
        }
    }
}
