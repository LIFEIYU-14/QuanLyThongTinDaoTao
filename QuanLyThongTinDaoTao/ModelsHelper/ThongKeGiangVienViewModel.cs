using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.ModelsHelper
{
    public class ThongKeGiangVienViewModel
    {
        public string GiangVienId { get; set; }
        public string TenGiangVien { get; set; }
        public int SoBuoiDay { get; set; }
        public int SoBuoiCoMat { get; set; }
        public int SoBuoiVang { get; set; }
        public double TyLeChuyenCan { get; set; }
        public List<string> LopHocThamGia { get; set; }
    }



}