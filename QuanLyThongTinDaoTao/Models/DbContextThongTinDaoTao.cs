﻿using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace QuanLyThongTinDaoTao.Models
{
    public class DbContextThongTinDaoTao : IdentityDbContext<AppUser>
    {
        public DbContextThongTinDaoTao() : base("DbContextThongTinDaoTao")
        {
        }

        public static DbContextThongTinDaoTao Create()
        {
            return new DbContextThongTinDaoTao();
        }

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
        public DbSet<ChungChiHocTap> ChungChiHocTaps { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<LopHocAttachment> LopHocAttachments { get; set; }
        public DbSet<KhoaHocAttachment> KhoaHocAttachments { get; set; }
        public DbSet<BuoiHocAttachment> BuoiHocAttachments { get; set; }
    }
}
