using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.ModelsHelper
{
    public class LopHocThongKeViewModel
    {
        public string TenLopHoc { get; set; }
        public int SoHocVien { get; set; }
        public int SoBuoiHoc { get; set; }
        public double TiLeDiemDanh { get; set; }
        public string TrangThai { get; set; }
        public string GiangVienPhuTrach { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        // Thêm các trường điểm danh giảng viên
        public int SoLanDiemDanhGiangVienCoMat { get; set; }
        public int TongSoLanDiemDanhGiangVien { get; set; }
        public double TiLeDiemDanhGiangVien { get; set; }
    }

}