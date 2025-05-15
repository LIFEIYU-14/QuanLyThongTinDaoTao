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
    [Authorize(Roles = "Admin")]
    public class GiangVienController : Controller
    {
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        private AppUserManager _userManager;
        public AppUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            private set => _userManager = value;
        }

        public ActionResult Index()
        {
            var list = db.GiangViens.Include(g => g.AppUser).ToList();
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(GiangVien model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra email đã tồn tại
            var existingUser = await UserManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");
                return View(model);
            }

            try
            {
                // Sinh mã giảng viên duy nhất
                var lastGiangVien = db.GiangViens
                       .OrderByDescending(g => g.MaGiangVien)
                       .FirstOrDefault();

                int number = 1;
                if (lastGiangVien != null && lastGiangVien.MaGiangVien.StartsWith("GV"))
                {
                    string numberPart = lastGiangVien.MaGiangVien.Substring(2); 
                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        number = lastNumber + 1;
                    }
                }

                string maGiangVien = "GV" + number.ToString("D3"); // GV001, GV002, ...
                model.MaGiangVien = maGiangVien;

                // Tạo AppUser
                var appUser = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                string defaultPassword = model.MaGiangVien + "123456";
                var result = await UserManager.CreateAsync(appUser, defaultPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                    return View(model);
                }

                // Gán role Giảng Viên
                await UserManager.AddToRoleAsync(appUser.Id, "GiangVien");

                // Gán dữ liệu giảng viên
                model.GiangVienId = Guid.NewGuid().ToString();
                model.AppUserId = appUser.Id;

                db.GiangViens.Add(model);
                await db.SaveChangesAsync();

                try
                {
                    // Sinh mã QR và gửi email
                    var giangVienService = new GiangVienService(db);
                    string qrBase64 = giangVienService.GenerateQRCodeForTeacher(model.GiangVienId);
                    model.QR_Code_GV = qrBase64;
                    await db.SaveChangesAsync();

                    var emailService = new EmailService();
                    await emailService.SendTeacherAccountWithQrEmail(model.Email, model.Email, defaultPassword, qrBase64);

                    TempData["Success"] = "Tạo giảng viên và gửi email QR thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Warning"] = "Tạo giảng viên thành công, nhưng lỗi khi gửi email: " + ex.Message;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm giảng viên: " + ex.Message);
                return View(model);
            }
        }




        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var gv = await db.GiangViens.FindAsync(id);
            if (gv == null) return HttpNotFound();

            return View(gv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(GiangVien model, string newPassword)
        {
            if (!ModelState.IsValid) return View(model);

            var gv = await db.GiangViens.FindAsync(model.GiangVienId);
            if (gv == null) return HttpNotFound();

            gv.HoVaTen = model.HoVaTen;
            gv.NgaySinh = model.NgaySinh;
            gv.Email = model.Email;
            gv.SoDienThoai = model.SoDienThoai;
            gv.ChuyenMon = model.ChuyenMon;
            gv.HocHam = model.HocHam;

            if (!string.IsNullOrEmpty(newPassword))
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(gv.AppUserId);
                var resetResult = await UserManager.ResetPasswordAsync(gv.AppUserId, token, newPassword);
                if (!resetResult.Succeeded)
                {
                    foreach (var e in resetResult.Errors)
                        ModelState.AddModelError("", e);
                    return View(model);
                }
            }
            var user = await UserManager.FindByIdAsync(model.AppUserId);
            if (user != null)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                var updateResult = await UserManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError("", error);
                    return View(model);
                }
            }
            db.Entry(gv).State = EntityState.Modified;
            await db.SaveChangesAsync();

            TempData["Success"] = "Cập nhật giảng viên thành công.";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var gv = await db.GiangViens
                .Include(g => g.ThongBaos)
                .FirstOrDefaultAsync(g => g.GiangVienId == id);

            if (gv == null)
            {
                TempData["Error"] = "Không tìm thấy giảng viên.";
                return RedirectToAction("Index");
            }

            // Xóa quan hệ phụ
            db.DiemDanhs_GVs.RemoveRange(db.DiemDanhs_GVs.Where(d => d.GiangVienId == id));
            db.GiangVien_BuoiHoc.RemoveRange(db.GiangVien_BuoiHoc.Where(b => b.GiangVienId == id));

            foreach (var tb in gv.ThongBaos.ToList())
            {
                tb.GiangViens.Remove(gv);
                if (!tb.GiangViens.Any())
                {
                    db.ThongBaos.Remove(tb);
                }
            }

            // Lưu AppUserId để xử lý sau khi xóa GV
            var appUserId = gv.AppUserId;

            // Xóa giảng viên
            db.GiangViens.Remove(gv);
            await db.SaveChangesAsync();

            // Xóa tài khoản người dùng nếu có
            if (!string.IsNullOrEmpty(appUserId))
            {
                var user = await UserManager.FindByIdAsync(appUserId);
                if (user != null)
                {
                    var deleteResult = await UserManager.DeleteAsync(user);
                    if (!deleteResult.Succeeded)
                    {
                        TempData["Error"] = "Đã xóa giảng viên nhưng không xóa được tài khoản người dùng: " 
                                            + string.Join(", ", deleteResult.Errors);
                        return RedirectToAction("Index");
                    }
                }
            }

            TempData["Success"] = "Xóa giảng viên và tài khoản thành công.";
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
