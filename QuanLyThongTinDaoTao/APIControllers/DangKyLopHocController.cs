using System;
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
            if (!ModelState.IsValid || model.LopHocId == Guid.Empty)
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
                return BadRequest("Mã OTP không hợp lệ hoặc đã hết hạn.");

            var hocVienData = OtpCache.Get("HocVienData_" + model.Email) as DangKyHocRequest;
            if (hocVienData == null)
                return BadRequest("Dữ liệu học viên không còn hiệu lực.");

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

            var lopHoc = db.LopHocs.FirstOrDefault(l => l.LopHocId == hocVienData.LopHocId);
            if (lopHoc == null)
                return BadRequest("Lớp học không tồn tại.");

            bool daDangKy = db.DangKyHocs.Any(d => d.NguoiDungId == hocVien.NguoiDungId && d.LopHocId == hocVienData.LopHocId);
            if (daDangKy)
                return BadRequest("Bạn đã đăng ký lớp học này trước đó.");

            db.DangKyHocs.Add(new DangKyHoc
            {
                DangKyId = Guid.NewGuid(),
                LopHocId = hocVienData.LopHocId,
                NguoiDungId = hocVien.NguoiDungId,
                IsConfirmed = true,
                NgayDangKy = DateTime.Now
            });
            db.SaveChanges();

            string qrCode = hocVienService.GenerateQRCodeForStudent(hocVien.NguoiDungId);
            await emailService.SendQrCodeEmail(hocVien.Email, qrCode);

            return Ok(new { message = "Đăng ký thành công. Mã QR đã được gửi đến email.",
                redirectUrl = Url.Content("~/Courses/Index")
            });

        }
    }
}
