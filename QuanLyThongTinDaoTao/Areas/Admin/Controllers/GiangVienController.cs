using QuanLyThongTinDaoTao.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class GiangVienController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // GET: Admin/GiangVien
        public ActionResult Index()
        {
            var dsGiangVien = db.GiangViens.ToList();
            return View(dsGiangVien);
        }

        // GET: Admin/GiangVien/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/GiangVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GiangVien gv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra email đã tồn tại chưa
                    if (db.GiangViens.Any(g => g.Email == gv.Email))
                    {
                        ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");
                        return View(gv);
                    }

                    // Lấy mã giảng viên cuối cùng và tạo mã mới
                    var lastGV = db.GiangViens.OrderByDescending(g => g.MaGiangVien).FirstOrDefault();
                    int lastNumber = 0;

                    if (lastGV != null && lastGV.MaGiangVien.Length > 2)
                    {
                        string numberPart = lastGV.MaGiangVien.Substring(2);
                        int.TryParse(numberPart, out lastNumber);
                    }

                    // Nếu chưa có giảng viên nào, bắt đầu từ "GV001"
                    gv.MaGiangVien = "GV" + (lastNumber + 1).ToString("D3");

                    // Mặc định tài khoản là mã giảng viên
                    gv.TaiKhoan = gv.MaGiangVien;

                    // Xử lý mật khẩu mặc định
                    gv.MatKhau = PasswordHelper.HashPassword(
                        string.IsNullOrEmpty(gv.MatKhau) ? gv.MaGiangVien + "123456" : gv.MatKhau
                    );

                    gv.NguoiDungId = Guid.NewGuid();
                    gv.NgayTao = DateTime.Now;
                    gv.NgayCapNhat = DateTime.Now;

                    // Thêm vào database
                    db.GiangViens.Add(gv);
                    db.SaveChanges();

                    TempData["Success"] = "Thêm giảng viên thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm giảng viên: " + ex.Message);
                }
            }
            return View(gv);
        }


        // GET: Admin/GiangVien/Edit/{id}
        public ActionResult Edit(Guid id)
        {
            if (id == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            GiangVien gv = db.GiangViens.Find(id);
            if (gv == null) return HttpNotFound();
            return View(gv);
        }
        // GET: Admin/GiangVien/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GiangVien gv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingGV = db.GiangViens.Find(gv.NguoiDungId);
                    if (existingGV == null)
                    {
                        return HttpNotFound();
                    }

                    // Cập nhật thông tin (trừ mật khẩu nếu không nhập mới)
                    existingGV.HoVaTen = gv.HoVaTen;
                    existingGV.NgaySinh = gv.NgaySinh;
                    existingGV.SoDienThoai = gv.SoDienThoai;
                    existingGV.Email = gv.Email;
                    existingGV.ChuyenMon = gv.ChuyenMon;
                    existingGV.HocHam = gv.HocHam;
                    existingGV.NgayCapNhat = DateTime.Now;

                    if (!string.IsNullOrEmpty(gv.MatKhau))
                    {
                        existingGV.MatKhau = PasswordHelper.HashPassword(gv.MatKhau);
                    }

                    db.Entry(existingGV).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Success"] = "Cập nhật giảng viên thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật giảng viên: " + ex.Message);
                }
            }
            return View(gv);
        }


        // GET: Admin/GiangVien/Delete/{id}
        public ActionResult Delete(Guid id)
        {
            if (id == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            GiangVien gv = db.GiangViens.Find(id);
            if (gv == null) return HttpNotFound();
            return View(gv);
        }

        // POST: Admin/GiangVien/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                GiangVien gv = db.GiangViens.Find(id);
                if (gv != null)
                {
                    db.GiangViens.Remove(gv);
                    db.SaveChanges();
                    TempData["Success"] = "Xóa giảng viên thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa giảng viên: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}