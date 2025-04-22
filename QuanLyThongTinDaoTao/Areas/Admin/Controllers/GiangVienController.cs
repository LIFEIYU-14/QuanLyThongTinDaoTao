using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuanLyThongTinDaoTao.Identity;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.Services;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class GiangVienController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        private AppUserManager _userManager;
        public AppUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            private set => _userManager = value;
        }

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
                return View(gv);

            if (db.Users.Any(u => u.Email == gv.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");
                return View(gv);
            }

            // Tạo mã giảng viên
            var lastGV = db.GiangViens
                .OrderByDescending(g => g.MaGiangVien)
                .FirstOrDefault();

            int lastNumber = 0;
            if (lastGV != null && int.TryParse(lastGV.MaGiangVien.Substring(2), out int number))
            {
                lastNumber = number;
            }

            gv.MaGiangVien = "GV" + (lastNumber + 1).ToString("D3");
            gv.UserName = gv.Email;
            gv.NgayTao = DateTime.Now;
            gv.NgayCapNhat = DateTime.Now;
            gv.QR_Code_GV = Guid.NewGuid().ToString();

            string matKhauGoc = gv.MaGiangVien + "123456";

            var result = await UserManager.CreateAsync(gv, matKhauGoc);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err);
                return View(gv);
            }

            await UserManager.AddToRoleAsync(gv.Id, "GiangVien");

            // Tạo QR code
            var giangVienService = new GiangVienService(db);
            string qrCodeBase64 = giangVienService.GenerateQRCodeForTeacher(gv.Id);

            // Cập nhật QR code
            var gvInDb = await db.GiangViens.FindAsync(gv.Id);
            if (gvInDb != null)
            {
                gvInDb.QR_Code_GV = qrCodeBase64;
                await db.SaveChangesAsync();
            }

            // Gửi email
            var emailService = new EmailService();
            await emailService.SendTeacherAccountWithQrEmail(gv.Email, gv.UserName, matKhauGoc, qrCodeBase64);

            TempData["Success"] = "Thêm giảng viên thành công và đã gửi Email!";
            return RedirectToAction("Index");
        }


        // GET: Admin/GiangVien/Edit/{id}
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var gv = await db.GiangViens.FindAsync(id);
            if (gv == null)
                return HttpNotFound();

            return View(gv);
        }

        // POST: Admin/GiangVien/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(GiangVien gv, string newPassword)
        {
            if (!ModelState.IsValid)
                return View(gv);

            try
            {
                var existingGV = await db.GiangViens.FindAsync(gv.Id);
                if (existingGV == null)
                    return HttpNotFound();

                // Cập nhật thông tin cá nhân
                existingGV.HoVaTen = gv.HoVaTen;
                existingGV.NgaySinh = gv.NgaySinh;
                existingGV.SoDienThoai = gv.SoDienThoai;
                existingGV.Email = gv.Email;
                existingGV.ChuyenMon = gv.ChuyenMon;
                existingGV.HocHam = gv.HocHam;
                existingGV.NgayCapNhat = DateTime.Now;
                existingGV.UserName = gv.Email;

                // Cập nhật mật khẩu nếu nhập
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(existingGV.Id);
                    var resetResult = await UserManager.ResetPasswordAsync(existingGV.Id, token, newPassword);
                    if (!resetResult.Succeeded)
                    {
                        foreach (var err in resetResult.Errors)
                            ModelState.AddModelError("", err);
                        return View(gv);
                    }
                }

                db.Entry(existingGV).State = EntityState.Modified;
                await db.SaveChangesAsync();

                TempData["Success"] = "Cập nhật giảng viên thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật giảng viên: " + ex.Message);
                return View(gv);
            }
        }

        // GET: Admin/GiangVien/Delete/{id}
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var gv = await db.GiangViens.FindAsync(id);
            if (gv == null)
            {
                TempData["Error"] = "Giảng viên không tồn tại!";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa dữ liệu liên quan
                var diemDanhs = db.DiemDanhs_GVs.Where(d => d.AppUserId == gv.Id);
                db.DiemDanhs_GVs.RemoveRange(diemDanhs);

                var giangVienBuoiHocs = db.GiangVien_BuoiHoc.Where(gvh => gvh.AppUserId == gv.Id);
                db.GiangVien_BuoiHoc.RemoveRange(giangVienBuoiHocs);

                var thongBaos = db.ThongBaos
                    .Where(t => t.GiangViens.Any(gvItem => gvItem.Id == gv.Id))
                    .ToList();

                foreach (var thongBao in thongBaos)
                {
                    thongBao.GiangViens.Remove(gv);
                    if (!thongBao.GiangViens.Any())
                    {
                        db.ThongBaos.Remove(thongBao);
                    }
                }

                db.GiangViens.Remove(gv); // Xóa khỏi bảng GiangVien
                await db.SaveChangesAsync(); // Lưu thay đổi DB đầu tiên

                // Xóa user khỏi Identity
                var appUser = await UserManager.FindByIdAsync(gv.Id);
                if (appUser != null)
                {
                    var resultDelete = await UserManager.DeleteAsync(appUser);
                    if (!resultDelete.Succeeded)
                    {
                        TempData["Error"] = "Lỗi khi xóa giảng viên: " + string.Join(", ", resultDelete.Errors);
                        return RedirectToAction("Index");
                    }
                }

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
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
