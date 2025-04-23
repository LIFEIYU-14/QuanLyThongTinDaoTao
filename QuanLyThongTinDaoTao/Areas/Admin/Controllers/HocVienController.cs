using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuanLyThongTinDaoTao.Identity;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.Services;

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

        // Hiển thị danh sách học viên
        public ActionResult Index(string searchString)
        {
            var hocViens = db.HocViens.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                hocViens = hocViens.Where(hv => hv.HoVaTen.Contains(searchString) || hv.Email.Contains(searchString));
            }

            return View(hocViens.ToList());
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
            {
                return View(model);
            }

            // Kiểm tra email trùng
            var existingUser = await UserManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");
                return View(model);
            }

            try
            {
                // Tạo mã học viên
                model.MaHocVien = DateTime.Now.Year + model.NgaySinh.Year.ToString() + new Random().Next(1000, 9999);

                // Gán username (tài khoản) là mã học viên
                model.UserName = model.MaHocVien;

                // Tạo mật khẩu mặc định
                string defaultPassword = model.MaHocVien + "123456";

                // Thiết lập các ngày tạo, cập nhật
                model.NgayTao = DateTime.Now;
                model.NgayCapNhat = DateTime.Now;

                // Reset QR_Code_HV (tạo sau)
                model.QR_Code_HV = "";

                // Tạo user bằng UserManager
                var result = await UserManager.CreateAsync(model, defaultPassword);
                if (result.Succeeded)
                {
                    // Gán role "HocVien"
                    await UserManager.AddToRoleAsync(model.Id, "HocVien");

                    // Tạo QR code cho học viên bằng Id (string)
                    var hocVienService = new HocVienService(db);
                    string qrBase64 = hocVienService.GenerateQRCodeForStudent(model.Id);

                    // Cập nhật mã QR code trong database
                    // Truy vấn lại HocVien từ DbContext để update (không dùng createdUser.HocVien)
                    var hocVienEntity = db.HocViens.FirstOrDefault(hv => hv.Id == model.Id);
                    if (hocVienEntity != null)
                    {
                        hocVienEntity.QR_Code_HV = qrBase64;
                        db.Entry(hocVienEntity).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                    }

                    // Gửi email chứa QR code
                    var emailService = new EmailService();
                    await emailService.SendQrCodeEmail(model.Email, qrBase64);

                    TempData["Success"] = "Thêm học viên thành công và đã gửi email!";
                    return RedirectToAction("Index");
                }

                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm học viên: " + ex.Message);
            }

            return View(model);
        }

        // GET: Admin/HocVien/Edit/{id}
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var hocVien = await UserManager.FindByIdAsync(id);
            if (hocVien == null) return HttpNotFound();

            return View(hocVien);
        }

        // POST: Admin/HocVien/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HocVien model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var existingHV = await db.HocViens.FindAsync(model.Id);
                if (existingHV == null)
                    return HttpNotFound();

                // Cập nhật thông tin
                existingHV.HoVaTen = model.HoVaTen;
                existingHV.NgaySinh = model.NgaySinh;
                existingHV.SoDienThoai = model.SoDienThoai;
                existingHV.Email = model.Email;
                existingHV.UserName = model.Email; // hoặc giữ nguyên UserName? tùy bạn
                existingHV.CoQuanLamViec = model.CoQuanLamViec;
                existingHV.NgayCapNhat = DateTime.Now;

                var updateResult = await UserManager.UpdateAsync(existingHV);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }

                TempData["Success"] = "Cập nhật học viên thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật học viên: " + ex.Message);
            }

            return View(model);
        }

        // GET: Admin/HocVien/Delete/{id}
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var hocVien = await UserManager.FindByIdAsync(id);
            if (hocVien == null)
            {
                TempData["Error"] = "Học viên không tồn tại!";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa các bản ghi liên quan trong bảng DiemDanh_HV
                var diemDanhs = db.DiemDanhs_HVs.Where(d => d.HocVien.Id == hocVien.Id).ToList();
                db.DiemDanhs_HVs.RemoveRange(diemDanhs);

                // Xóa các bản ghi liên quan trong bảng DangKyHoc
                var hocVienDangKy = db.DangKyHocs.Where(gvh => gvh.HocVien.Id == hocVien.Id).ToList();
                db.DangKyHocs.RemoveRange(hocVienDangKy);

                await db.SaveChangesAsync();

                // Xóa user qua UserManager
                var result = await UserManager.DeleteAsync(hocVien);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Xóa học viên thành công!";
                }
                else
                {
                    TempData["Error"] = "Lỗi khi xóa học viên: " + string.Join(", ", result.Errors);
                }
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
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
