using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using System;
using System.Linq;
using System.Web.Http;

namespace QuanLyThongTinDaoTao.ApiControllers
{
    [RoutePrefix("api/chungchi")]
    public class ChungChiApiController : ApiController
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        [HttpGet]
        [Route("tra-cuu/email")]
        public IHttpActionResult TraCuuBangEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Vui lòng nhập email.");

            var hocVien = db.HocViens.FirstOrDefault(h => h.Email == email);
            if (hocVien == null)
                return NotFound();

            var chungChis = db.ChungChiHocTaps
                              .Where(c => c.HocVienId == hocVien.HocVienId)
                              .ToList();

            if (!chungChis.Any())
                return Ok(new { DaCapChungChi = false });

            var model = new HocVienHocTapViewModel
            {
                HocVienId = hocVien.MaHocVien,
                TenHocVien = hocVien.HoVaTen,
                Email = hocVien.Email,
                NgaySinh = hocVien.NgaySinh,
                DaCapChungChi = true,
                ChungChis = chungChis.Select(c => new ChungChiViewModel
                {
                    TenKhoaHoc = c.KhoaHoc?.TenKhoaHoc,
                    NgayCap = c.NgayCap,
                    NgayHetHan = c.NgayHetHan
                }).ToList()
            };

            return Ok(model);
        }

        [HttpPost]
        [Route("quet-qr")]
        public IHttpActionResult QuetQR([FromBody] QRRequest data)
        {
            try
            {
                var hocVien = db.HocViens.FirstOrDefault(h => h.HocVienId == data.HocVienId);
                if (hocVien == null)
                    return NotFound();

                var chungChis = db.ChungChiHocTaps.Where(c => c.HocVienId == hocVien.HocVienId).ToList();
                if (!chungChis.Any())
                    return Ok(new { success = false, message = "Học viên chưa được cấp chứng chỉ." });

                return Ok(new
                {
                    success = true,
                    message = "✅ Tra cứu thành công!",
                    data = new { hocVien.HocVienId }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class QRRequest
    {
        public string HocVienId { get; set; }
    }
}
