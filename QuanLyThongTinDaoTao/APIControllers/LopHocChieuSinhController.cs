using QuanLyThongTinDaoTao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace QuanLyThongTinDaoTao.APIControllers
{
    public class LopHocChieuSinhController : ApiController
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        [HttpGet]
        [Route("api/lophoc/chieusinh")]
        public IHttpActionResult GetLopHocChieuSinh()
        {
            var result = db.LopHocs
                .Where(lh => lh.TrangThai == LopHoc.TrangThaiLopHoc.SapMo)
                .Select(lh => new
                {
                    lh.LopHocId,
                    lh.TenLopHoc,
                    lh.MaLopHoc,
                    lh.SoTinChi,
                    lh.NgayBatDau,
                    lh.NgayKetThuc,
                    lh.SoLuongToiDa,
                    MoTa = string.IsNullOrEmpty(lh.MoTa) ? "Không có mô tả" : lh.MoTa,
                    KhoaHoc = lh.KhoaHoc.TenKhoaHoc
                })
                .ToList();

            return Ok(result);
        }
    }
}
