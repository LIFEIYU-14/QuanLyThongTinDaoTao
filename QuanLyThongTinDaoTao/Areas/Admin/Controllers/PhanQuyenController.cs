using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class PhanQuyenController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // GET: Admin/PhanQuyen
        public ActionResult Index()
        {
            var nguoiDungKhongPhaiHocVien = db.NguoiDungs
                .Where(nd => !db.HocViens.Any(hv => hv.NguoiDungId == nd.NguoiDungId))
                .Include(nd => nd.PhanQuyens)
                .ToList();

            return View(nguoiDungKhongPhaiHocVien);
        }

        // GET: Admin/PhanQuyen/Edit/{nguoiDungId}
        public ActionResult Edit(Guid nguoiDungId)
        {
            var nguoiDung = db.NguoiDungs
                .Include(nd => nd.PhanQuyens)
                .FirstOrDefault(nd => nd.NguoiDungId == nguoiDungId);

            if (nguoiDung == null) return HttpNotFound();

            ViewBag.DanhSachQuyen = new SelectList(QuyenConstants.TenQuyenTiengViet, "Key", "Value");
            return View(nguoiDung);
        }

        // POST: Admin/PhanQuyen/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid nguoiDungId, string TenQuyen)
        {
            var nguoiDung = db.NguoiDungs
                .Include(nd => nd.PhanQuyens)
                .FirstOrDefault(nd => nd.NguoiDungId == nguoiDungId);

            if (nguoiDung == null) return HttpNotFound();

            var quyenCu = nguoiDung.PhanQuyens.FirstOrDefault();
            if (quyenCu != null) db.PhanQuyens.Remove(quyenCu);

            if (!string.IsNullOrEmpty(TenQuyen))
            {
                db.PhanQuyens.Add(new PhanQuyen
                {
                    PhanQuyenId = Guid.NewGuid(),
                    NguoiDungId = nguoiDungId,
                    TenQuyen = TenQuyen
                });
            }

            db.SaveChanges();
            TempData["Success"] = "Cập nhật quyền thành công!";
            return RedirectToAction("Index");
        }

        // POST: Admin/PhanQuyen/Delete/{phanQuyenId}
        [HttpPost]
        public ActionResult Delete(Guid phanQuyenId)
        {
            var quyen = db.PhanQuyens.Find(phanQuyenId);
            if (quyen == null) return HttpNotFound();

            db.PhanQuyens.Remove(quyen);
            db.SaveChanges();
            return Json(new { success = true, message = "Xóa quyền thành công!" });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
