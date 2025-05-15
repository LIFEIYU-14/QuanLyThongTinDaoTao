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
    public class HocVienController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        private AppUserManager _userManager;
        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set { _userManager = value; }
        }

        // GET: Admin/HocVien
        public ActionResult Index(string searchString)
        {
            var hocViens = db.HocViens.Include(hv => hv.AppUser).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                hocViens = hocViens.Where(hv => hv.HoVaTen.Contains(searchString) || hv.Email.Contains(searchString));
            }

            return View(hocViens.ToList());
        }

        // GET: Admin/HocVien/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var hocVien = db.HocViens
                            .Include(h => h.DangKyHocs.Select(d => d.LopHoc))
                            .Include(h => h.AppUser)
                            .FirstOrDefault(h => h.HocVienId == id);

            if (hocVien == null)
                return HttpNotFound();

            return View(hocVien);
        }

        // GET: Admin/HocVien/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/HocVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HocVien model)
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
                // Tạo AppUser
                var appUser = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                string defaultPassword = model.NgaySinh.ToString("yyyyMMdd") + "@" + new Random().Next(1000, 9999);
                var result = await UserManager.CreateAsync(appUser, defaultPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                    return View(model);
                }

                // Gán role học viên
                await UserManager.AddToRoleAsync(appUser.Id, "HocVien");

                // Sinh mã học viên
                string maHocVien;
                do
                {
                    maHocVien = DateTime.Now.Year + model.NgaySinh.Year.ToString() + new Random().Next(1000, 9999);
                }
                while (db.HocViens.Any(h => h.MaHocVien == maHocVien));

                model.MaHocVien = maHocVien;
                model.HocVienId = Guid.NewGuid().ToString();
                model.AppUserId = appUser.Id;
                model.IsConfirmed = true;

                // Thêm học viên vào DB
                db.HocViens.Add(model);
                await db.SaveChangesAsync();

                try
                {
                    // Gửi email QR code
                    var hocVienService = new HocVienService(db);
                    string qrBase64 = hocVienService.GenerateQRCodeForStudent(model.HocVienId);

                    var emailService = new EmailService();
                    await emailService.SendQrCodeEmail(model.Email, qrBase64);

                    TempData["Success"] = "Thêm học viên và gửi email QR thành công!";
                }
                catch (Exception ex)
                {
                    // Vẫn thêm học viên, chỉ là lỗi khi gửi email
                    TempData["Warning"] = "Thêm học viên thành công, nhưng lỗi khi gửi email: " + ex.Message;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm học viên: " + ex.Message);
                return View(model);
            }
        }

        // GET: Admin/HocVien/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var hocVien = await db.HocViens.FindAsync(id);
            if (hocVien == null)
                return HttpNotFound();

            return View(hocVien);

        }

        // POST: Admin/HocVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HocVien model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hocVien = db.HocViens.FirstOrDefault(h => h.HocVienId == model.HocVienId);
            if (hocVien == null)
            {
                ModelState.AddModelError("", "Học viên không tồn tại.");
                return View(model);
            }
            try
            {
                // Cập nhật thông tin HocVien
                hocVien.HoVaTen = model.HoVaTen;
                hocVien.Email = model.Email;
                hocVien.NgaySinh = model.NgaySinh;
                hocVien.SoDienThoai = model.SoDienThoai;
                hocVien.CoQuanLamViec = model.CoQuanLamViec;

                var user = await UserManager.FindByIdAsync(hocVien.AppUserId);
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

                db.Entry(hocVien).State = EntityState.Modified;
                await db.SaveChangesAsync();

                TempData["Success"] = "Cập nhật học viên thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật học viên: " + ex.Message);
                return View(model);
            }
        }


        // GET: Admin/HocVien/Delete/{id}
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var hocVien = await db.HocViens
                .Include(h => h.DiemDanh_HVs)
                .Include(h => h.DangKyHocs)
                .FirstOrDefaultAsync(h => h.HocVienId == id);

            if (hocVien == null)
            {
                TempData["Error"] = "Không tìm thấy học viên.";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa các bản ghi liên quan
                db.DiemDanhs_HVs.RemoveRange(hocVien.DiemDanh_HVs);
                db.DangKyHocs.RemoveRange(hocVien.DangKyHocs);

                // Lưu AppUserId để xử lý sau khi xóa học viên
                var appUserId = hocVien.AppUserId;

                // Xóa học viên
                db.HocViens.Remove(hocVien);
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
                            TempData["Error"] = "Đã xóa học viên nhưng không xóa được tài khoản người dùng: "
                                                + string.Join(", ", deleteResult.Errors);
                            return RedirectToAction("Index");
                        }
                    }
                }

                TempData["Success"] = "Xóa học viên và tài khoản thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa học viên: " + ex.Message;
            }

            return RedirectToAction("Index");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                _userManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
