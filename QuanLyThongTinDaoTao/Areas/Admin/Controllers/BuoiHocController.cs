using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
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
            // Cập nhật trạng thái của tất cả các buổi học theo thời gian thực
            UpdateTrangThaiBuoiHocs();
            var buoiHocs = db.BuoiHocs.Include(b => b.LopHoc).ToList();

            return View(buoiHocs);
        }

        // Tạo buổi học (GET)
        public ActionResult Create()
        {
            ViewBag.LopHocList = db.LopHocs.ToList();
            ViewBag.GiangVienList = db.GiangViens.ToList();
            return View();
        }
        // Tạo buổi học (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BuoiHoc model, Guid LopHocId, HttpPostedFileBase[] attachments, Guid[] selectedGiangViens)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LopHocList = db.LopHocs.ToList();
                ViewBag.GiangVienList = db.GiangViens.ToList();
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
                            NguoiDungId = giangVienId
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
            ViewBag.GiangVienDaChon = buoiHoc.GiangVien_BuoiHocs?.Select(g => g.NguoiDungId).ToList();
            return View(buoiHoc);
        }

        // Chỉnh sửa buổi học (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BuoiHoc model, Guid LopHocId, HttpPostedFileBase[] attachments, Guid[] selectedGiangViens)
        {
            if (!ModelState.IsValid)
            {
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
                return View(model);
            }

            // Kiểm tra NgayHoc có nằm trong khoảng hợp lệ không?
            if (model.NgayHoc < lopHoc.NgayBatDau || model.NgayHoc > lopHoc.NgayKetThuc)
            {
                ModelState.AddModelError("NgayHoc", "Ngày học phải nằm trong khoảng từ " + lopHoc.NgayBatDau.ToShortDateString() + " đến " + lopHoc.NgayKetThuc.ToShortDateString());
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

            //// --- XỬ LÝ GIẢNG VIÊN ---
            //// Xóa hết danh sách giảng viên đã có cho buổi học hiện tại
            //var existingGiangVienBuoiHoc = db.GiangVien_BuoiHoc.Where(gv => gv.BuoiHocId == model.BuoiHocId).ToList();
            //foreach (var gv in existingGiangVienBuoiHoc)
            //{
            //    db.GiangVien_BuoiHoc.Remove(gv);
            //}
            //db.SaveChanges();

            //// Thêm lại giảng viên theo danh sách được chọn
            //if (selectedGiangViens != null && selectedGiangViens.Any())
            //{
            //    foreach (var giangVienId in selectedGiangViens)
            //    {
            //        var giangVien = db.GiangViens.Find(giangVienId);
            //        if (giangVien != null)
            //        {
            //            db.GiangVien_BuoiHoc.Add(new GiangVien_BuoiHoc
            //            {
            //                Id = Guid.NewGuid(),
            //                BuoiHocId = model.BuoiHocId,
            //                NguoiDungId = giangVienId
            //            });
            //        }
            //    }
            //    db.SaveChanges();
            //}
            // --- XỬ LÝ GIẢNG VIÊN ---
            // Lấy danh sách giảng viên hiện tại của buổi học
            var existingGiangVienBuoiHoc = db.GiangVien_BuoiHoc
                .Where(gv => gv.BuoiHocId == model.BuoiHocId)
                .ToList();

            // Lấy danh sách ID giảng viên sẽ bị xóa (có trong DB nhưng không có trong danh sách chọn)
            var giangVienIdsToRemove = existingGiangVienBuoiHoc
                .Where(gv => selectedGiangViens == null || !selectedGiangViens.Contains(gv.NguoiDungId))
                .Select(gv => gv.NguoiDungId)
                .ToList();

            // Xóa điểm danh của các giảng viên bị xóa khỏi buổi học
            if (giangVienIdsToRemove.Any())
            {
                var diemDanhToRemove = db.DiemDanhs_GVs
                    .Where(d => d.BuoiHocId == model.BuoiHocId &&
                           giangVienIdsToRemove.Contains(d.NguoiDungId))
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
                            NguoiDungId = giangVienId
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
            return RedirectToAction("Index");
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

    }
}
