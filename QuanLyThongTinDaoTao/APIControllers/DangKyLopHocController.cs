using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using QuanLyThongTinDaoTao.Services;
using QuanLyThongTinDaoTao.Helpers;
using System.Net;
using QuanLyThongTinDaoTao.Identity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace QuanLyThongTinDaoTao.APIControllers
{
    [RoutePrefix("api/dangky")]
    public class DangKyLopHocController : ApiController
    {
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        private readonly EmailService emailService = new EmailService();
        private readonly HocVienService hocVienService = new HocVienService(new DbContextThongTinDaoTao());
        private AppUserManager _userManager;
        public AppUserManager UserManager
        {
            get => _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            private set => _userManager = value;
        }
        // POST: api/dangky/start
        [HttpPost]
        [Route("start")]
        public async Task<IHttpActionResult> StartOtp([FromBody] DangKyHocRequest model)
        {
            if (!ModelState.IsValid || model.LopHocIds == null || !model.LopHocIds.Any())
                return BadRequest("Thông tin đăng ký không hợp lệ.");

            string otp = new Random().Next(100000, 999999).ToString();

            OtpCache.Set("OTP_" + model.Email, otp);
            OtpCache.Set("HocVienData_" + model.Email, model);

            await emailService.SendOtpEmail(model.Email, otp);
            return Ok(new { message = "Mã OTP đã được gửi đến email." });
        }

        // POST: api/dangky/verify
        [HttpPost]
        [Route("verify")]
        public async Task<IHttpActionResult> VerifyOtp([FromBody] VerifyOtpRequest model)
        {
            var email = model.Email?.Trim();
            var otp = model.Otp?.Trim();

            string storedOtp = OtpCache.Get("OTP_" + email)?.ToString();
            if (storedOtp == null || storedOtp != otp)
                return Content(HttpStatusCode.BadRequest, new { error = "Mã OTP không hợp lệ hoặc đã hết hạn." });

            var hocVienData = OtpCache.Get("HocVienData_" + email) as DangKyHocRequest;
            if (hocVienData == null)
                return Content(HttpStatusCode.BadRequest, new { error = "Dữ liệu học viên không còn hiệu lực." });

            OtpCache.Remove("OTP_" + email);
            OtpCache.Remove("HocVienData_" + email);

            var appUser = await UserManager.FindByEmailAsync(email);
            if (appUser == null)
            {
                appUser = new AppUser
                {
                    UserName = email,
                    Email = email
                };
                var result = await UserManager.CreateAsync(appUser, "123456"); // hoặc random + gửi reset
                if (!result.Succeeded)
                    return Content(HttpStatusCode.InternalServerError, new { error = string.Join("; ", result.Errors) });

                await UserManager.AddToRoleAsync(appUser.Id, "HocVien");
            }

            // Kiểm tra HocVien đã gắn với AppUser chưa
            var hocVien = db.HocViens.FirstOrDefault(h => h.AppUserId == appUser.Id);
            if (hocVien == null)
            {
                hocVien = new HocVien
                {
                    HocVienId = Guid.NewGuid().ToString(),
                    MaHocVien = DateTime.Now.Year + hocVienData.NgaySinh.Year.ToString() + new Random().Next(1000, 9999),
                    Email = email,
                    HoVaTen = hocVienData.HoVaTen,
                    NgaySinh = hocVienData.NgaySinh,
                    SoDienThoai = hocVienData.SoDienThoai,
                    CoQuanLamViec = hocVienData.CoQuanLamViec,
                    QR_Code_HV = Guid.NewGuid().ToString(),
                    IsConfirmed = true,
                    AppUserId = appUser.Id
                };

                db.HocViens.Add(hocVien);
                db.SaveChanges();
            }

            var errors = new List<string>();
            var successfulRegistrations = new List<string>();

            foreach (var lopHocId in hocVienData.LopHocIds)
            {
                var lopHoc = db.LopHocs.FirstOrDefault(l => l.LopHocId == lopHocId);
                if (lopHoc == null)
                {
                    errors.Add($"Lớp học {lopHocId} không tồn tại.");
                    continue;
                }

                bool daDangKy = db.DangKyHocs.Any(d => d.HocVienId == hocVien.HocVienId && d.LopHocId == lopHocId);
                if (daDangKy)
                {
                    errors.Add($"Bạn đã đăng ký lớp {lopHoc.TenLopHoc} trước đó.");
                    continue;
                }

                db.DangKyHocs.Add(new DangKyHoc
                {
                    DangKyId = Guid.NewGuid(),
                    LopHocId = lopHocId,
                    HocVienId = hocVien.HocVienId,
                    IsConfirmed = true,
                    NgayDangKy = DateTime.Now
                });
                successfulRegistrations.Add(lopHoc.TenLopHoc);
            }

            db.SaveChanges();

            string qrCode = hocVienService.GenerateQRCodeForStudent(hocVien.HocVienId);
            await emailService.SendQrCodeEmail(hocVien.Email, qrCode);

            var resultObj = new
            {
                message = "Đăng ký thành công.",
                redirectUrl = Url.Content("~/Courses/Index"),
                successfulRegistrations = successfulRegistrations,
                errors = errors
            };

            return Ok(resultObj);
        }
    }
}