using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class GiangVienController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // 🔹 Hiển thị danh sách giảng viên
        public ActionResult Index()
        {
            var giangViens = db.NguoiDungs.OfType<GiangVien>().ToList();
            return View(giangViens);
        }

        // 🔹 Hiển thị form thêm mới
        public ActionResult Create()
        {
            return View();
        }

        // 🔹 Xử lý thêm giảng viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GiangVien giangVien)
        {
            if (ModelState.IsValid)
            {
                giangVien.NguoiDungId = Guid.NewGuid();
                giangVien.MaGiangVien = "GV" + DateTime.Now.Ticks.ToString(); // Sinh mã tự động
                giangVien.QR_Code_GV = Guid.NewGuid().ToString();
                giangVien.VaiTro = VaiTroNguoiDung.GiangVien; // Đảm bảo vai trò đúng
                giangVien.NgayTao = DateTime.Now;
                giangVien.NgayCapNhat = DateTime.Now;

                db.NguoiDungs.Add(giangVien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(giangVien);
        }

        // 🔹 Hiển thị form chỉnh sửa
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GiangVien giangVien = db.NguoiDungs.OfType<GiangVien>().FirstOrDefault(gv => gv.NguoiDungId == id);
            if (giangVien == null)
                return HttpNotFound();

            return View(giangVien);
        }

        // 🔹 Xử lý chỉnh sửa giảng viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GiangVien giangVien)
        {
            if (ModelState.IsValid)
            {
                var existingGiangVien = db.NguoiDungs.OfType<GiangVien>().FirstOrDefault(gv => gv.NguoiDungId == giangVien.NguoiDungId);
                if (existingGiangVien != null)
                {
                    existingGiangVien.HoVaTen = giangVien.HoVaTen;
                    existingGiangVien.NgaySinh = giangVien.NgaySinh;
                    existingGiangVien.SoDienThoai = giangVien.SoDienThoai;
                    existingGiangVien.ChuyenMon = giangVien.ChuyenMon;
                    existingGiangVien.HocHam = giangVien.HocHam;
                    existingGiangVien.NgayCapNhat = DateTime.Now;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(giangVien);
        }

        // 🔹 Xác nhận xóa giảng viên
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GiangVien giangVien = db.NguoiDungs.OfType<GiangVien>().FirstOrDefault(gv => gv.NguoiDungId == id);
            if (giangVien == null)
                return HttpNotFound();

            return View(giangVien);
        }

        // 🔹 Xử lý xóa giảng viên
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            GiangVien giangVien = db.NguoiDungs.OfType<GiangVien>().FirstOrDefault(gv => gv.NguoiDungId == id);
            if (giangVien != null)
            {
                db.NguoiDungs.Remove(giangVien);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
