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
            // Lấy danh sách quyền người dùng từ bảng PhanQuyens
            var danhSach = db.PhanQuyens
                .Include(p => p.NguoiDung) // Liên kết với bảng NguoiDung
                .ToList(); // Lấy tất cả dữ liệu

            // Truyền danh sách vào ViewBag để sử dụng trong view
            ViewBag.DanhSach = danhSach;
            return View();
        }

        // GET: Admin/PhanQuyen/Delete/{id}
        public ActionResult Delete(Guid id)
        {
            var quyen = db.PhanQuyens.Include(p => p.NguoiDung).FirstOrDefault(p => p.PhanQuyenId == id);
            if (quyen == null) return HttpNotFound();

            // Truyền thông tin quyền vào ViewBag
            ViewBag.TenQuyenTiengViet = QuyenConstants.TenQuyenTiengViet.ContainsKey(quyen.TenQuyen)
                ? QuyenConstants.TenQuyenTiengViet[quyen.TenQuyen]
                : quyen.TenQuyen;

            return View(quyen);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var quyen = db.PhanQuyens.Find(id);
            if (quyen != null)
            {
                db.PhanQuyens.Remove(quyen);
                db.SaveChanges();
                TempData["Success"] = "Xóa quyền thành công!";
            }
            return RedirectToAction("Index");
        }

        // Dọn dẹp tài nguyên khi controller không sử dụng nữa
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
