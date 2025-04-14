using QuanLyThongTinDaoTao.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Services;
using System.Threading.Tasks;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    [RoleAuthorize("Admin")]
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
        public async Task<ActionResult> Create(GiangVien gv)
        {
            if (!ModelState.IsValid)
            {
                return View(gv);
            }

            try
            {
                // Kiểm tra email trùng
                if (db.GiangViens.Any(g => g.Email == gv.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");
                    return View(gv);
                }

                // Sinh mã giảng viên
                var lastGV = db.GiangViens.OrderByDescending(g => g.MaGiangVien).FirstOrDefault();
                int lastNumber = 0;
                if (lastGV != null && int.TryParse(lastGV.MaGiangVien.Substring(2), out int result))
                {
                    lastNumber = result;
                }
                gv.MaGiangVien = "GV" + (lastNumber + 1).ToString("D3");

                // Thiết lập thông tin giảng viên
                gv.TaiKhoan = gv.MaGiangVien;
                gv.MatKhau = PasswordHelper.HashPassword(gv.MaGiangVien + "123456");
                gv.NguoiDungId = Guid.NewGuid();
                gv.NgayTao = DateTime.Now;
                gv.NgayCapNhat = DateTime.Now;

                db.GiangViens.Add(gv);
                gv.PhanQuyens.Add(new PhanQuyen { TenQuyen = "GiangVien" });

                db.SaveChanges();
                // Gửi email chứa tài khoản và mật khẩu
                try
                {
                    var emailService = new EmailService();
                    string matKhauGoc = gv.MaGiangVien + "123456";
                    await emailService.SendAccountInfoEmail(gv.Email, gv.TaiKhoan, matKhauGoc);
                }
                catch (Exception emailEx)
                {
                    TempData["Warning"] = "Thêm giảng viên thành công nhưng gửi email thất bại: " + emailEx.Message;
                }

                TempData["Success"] = "Thêm giảng viên thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm giảng viên: " + ex.Message);
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

        // POST: Admin/GiangVien/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GiangVien gv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingGV = db.GiangViens.Find(gv.NguoiDungId);
                    if (existingGV == null) return HttpNotFound();

                    // Cập nhật thông tin
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
            if (id == Guid.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GiangVien gv = db.GiangViens.Find(id);
            if (gv == null)
            {
                TempData["Error"] = "Giảng viên không tồn tại!";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa các bản ghi liên quan trong bảng DiemDanh_GV
                var diemDanhs = db.DiemDanhs_GVs.Where(d => d.GiangVien.NguoiDungId == gv.NguoiDungId).ToList();
                db.DiemDanhs_GVs.RemoveRange(diemDanhs);

                // Xóa các bản ghi liên quan trong bảng GiangVien_BuoiHoc
                var giangVienBuoiHocs = db.GiangVien_BuoiHoc.Where(gvh => gvh.GiangVien.NguoiDungId == gv.NguoiDungId).ToList();
                db.GiangVien_BuoiHoc.RemoveRange(giangVienBuoiHocs);

                // Xóa các bản ghi liên quan trong bảng ThongBao
                var thongBaos = db.ThongBaos.Where(t => t.GiangViens.Any(gvItem => gvItem.NguoiDungId == gv.NguoiDungId)).ToList();
                foreach (var thongBao in thongBaos)
                {
                    thongBao.GiangViens.Remove(gv); // Xóa liên kết giảng viên khỏi thông báo
                    if (!thongBao.GiangViens.Any()) // Nếu không còn giảng viên nào liên kết với thông báo, xóa thông báo
                    {
                        db.ThongBaos.Remove(thongBao);
                    }
                }

                // Sau khi xóa các bản ghi liên quan, xóa giảng viên
                db.GiangViens.Remove(gv);
                db.SaveChanges();

                TempData["Success"] = "Xóa giảng viên thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa giảng viên: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
