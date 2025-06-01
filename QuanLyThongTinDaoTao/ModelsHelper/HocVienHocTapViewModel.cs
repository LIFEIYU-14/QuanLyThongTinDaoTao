
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.ModelsHelper
{
    public class HocVienHocTapViewModel
    {
        public string HocVienId { get; set; }
        public string TenHocVien { get; set; }
        public DateTime NgaySinh { get; set; }         // Thêm ngày sinh
        public string Email { get; set; }               // Thêm email
        public int TongBuoiHoc { get; set; }
        public int SoBuoiCoMat { get; set; }
        public int SoBuoiVang { get; set; }
        public bool DaCapChungChi { get; set; }
        public List<ChungChiViewModel> ChungChis { get; set; } = new List<ChungChiViewModel>();
    }

}