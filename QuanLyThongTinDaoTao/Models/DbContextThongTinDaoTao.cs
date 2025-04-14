using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Models
{
    public class DbContextThongTinDaoTao : DbContext
    {
        public DbContextThongTinDaoTao() : base("DbContextThongTinDaoTao")
        {

        }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<HocVien> HocViens { get; set; }
        public DbSet<GiangVien> GiangViens { get; set; }
        public DbSet<KhoaHoc> KhoaHocs { get; set; }
        public DbSet<LopHoc> LopHocs { get; set; }
        public DbSet<DangKyHoc> DangKyHocs { get; set; }
        public DbSet<DiemDanh_HV> DiemDanhs_HVs { get; set; }
        public DbSet<DiemDanh_GV> DiemDanhs_GVs { get; set; }
        public DbSet<GiangVien_BuoiHoc> GiangVien_BuoiHoc { get; set; }
        public DbSet<BuoiHoc> BuoiHocs { get; set; }
        public DbSet<ThongBao> ThongBaos { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<LopHocAttachment> LopHocAttachments { get; set; }
        public DbSet<KhoaHocAttachment> KhoaHocAttachments { get; set; }
        public DbSet<BuoiHocAttachment> BuoiHocAttachments { get; set; }
        public DbSet<PhanQuyen> PhanQuyens { get; set; }
      
    }

}