using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using QuanLyThongTinDaoTao.Services;
using QuanLyThongTinDaoTao.Helpers;

namespace QuanLyThongTinDaoTao.APIControllers
{
    [RoutePrefix("api/dangky")]
    public class DangKyLopHocController : ApiController
    {
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        private readonly EmailService emailService = new EmailService();
        private readonly HocVienService hocVienService = new HocVienService(new DbContextThongTinDaoTao());

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
            string storedOtp = OtpCache.Get("OTP_" + model.Email)?.ToString();
            if (storedOtp == null || storedOtp != model.Otp)
                return Content(System.Net.HttpStatusCode.BadRequest, new { error = "Mã OTP không hợp lệ hoặc đã hết hạn." });

            var hocVienData = OtpCache.Get("HocVienData_" + model.Email) as DangKyHocRequest;
            if (hocVienData == null)
                return Content(System.Net.HttpStatusCode.BadRequest, new { error = "Dữ liệu học viên không còn hiệu lực." });

            OtpCache.Remove("OTP_" + model.Email);
            OtpCache.Remove("HocVienData_" + model.Email);

            var hocVien = db.HocViens.FirstOrDefault(h => h.Email == model.Email);
            if (hocVien == null)
            {
                hocVien = new HocVien
                {
                    NguoiDungId = Guid.NewGuid(),
                    MaHocVien = DateTime.Now.Year + hocVienData.NgaySinh.Year.ToString() + new Random().Next(1000, 9999),
                    HoVaTen = hocVienData.HoVaTen,
                    Email = hocVienData.Email,
                    NgaySinh = hocVienData.NgaySinh,
                    SoDienThoai = hocVienData.SoDienThoai,
                    CoQuanLamViec = hocVienData.CoQuanLamViec,
                    QR_Code_HV = Guid.NewGuid().ToString(),
                    IsConfirmed = true
                };
                db.HocViens.Add(hocVien);
                db.SaveChanges();
            }

            // Đăng ký cho từng lớp học
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

                bool daDangKy = db.DangKyHocs.Any(d => d.NguoiDungId == hocVien.NguoiDungId && d.LopHocId == lopHocId);
                if (daDangKy)
                {
                    errors.Add($"Bạn đã đăng ký lớp {lopHoc.TenLopHoc} trước đó.");
                    continue;
                }

                db.DangKyHocs.Add(new DangKyHoc
                {
                    DangKyId = Guid.NewGuid(),
                    LopHocId = lopHocId,
                    NguoiDungId = hocVien.NguoiDungId,
                    IsConfirmed = true,
                    NgayDangKy = DateTime.Now
                });
                successfulRegistrations.Add(lopHoc.TenLopHoc);
            }

            db.SaveChanges();

            // Gửi QR code
            string qrCode = hocVienService.GenerateQRCodeForStudent(hocVien.NguoiDungId);
            await emailService.SendQrCodeEmail(hocVien.Email, qrCode);

            var result = new
            {
                message = "Đăng ký thành công.",
                redirectUrl = Url.Content("~/Courses/Index"),
                successfulRegistrations = successfulRegistrations,
                errors = errors
            };

            return Ok(result);
        }
    }
}