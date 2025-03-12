namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NguoiDungs",
                c => new
                    {
                        NguoiDungId = c.Guid(nullable: false),
                        TenDangNhap = c.String(nullable: false, maxLength: 50),
                        HoVaTen = c.String(nullable: false),
                        MatKhau = c.String(nullable: false),
                        VaiTro = c.Int(nullable: false),
                        NgaySinh = c.DateTime(nullable: false),
                        SoDienThoai = c.String(nullable: false, maxLength: 15),
                        Email = c.String(nullable: false, maxLength: 100),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NguoiDungId);
            
            CreateTable(
                "dbo.BuoiHocs",
                c => new
                    {
                        BuoiHocId = c.Guid(nullable: false),
                        NgayHoc = c.DateTime(nullable: false),
                        GioBatDau = c.Time(nullable: false, precision: 7),
                        GioKetThuc = c.Time(nullable: false, precision: 7),
                        TrangThai = c.String(nullable: false, maxLength: 20),
                        GhiChu = c.String(),
                        TaiLieuDinhKem = c.String(),
                        LopHoc_LopHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.BuoiHocId)
                .ForeignKey("dbo.LopHocs", t => t.LopHoc_LopHocId)
                .Index(t => t.LopHoc_LopHocId);
            
            CreateTable(
                "dbo.LopHocs",
                c => new
                    {
                        LopHocId = c.Guid(nullable: false),
                        MaLopHoc = c.String(nullable: false),
                        TenLopHoc = c.String(nullable: false),
                        SoTinChi = c.Int(nullable: false),
                        NgayBatDau = c.DateTime(nullable: false),
                        NgayKetThuc = c.DateTime(nullable: false),
                        TrangThai = c.String(nullable: false),
                        SoLuongToiDa = c.Int(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                        GiangVien_NguoiDungId = c.Guid(),
                        KhoaHoc_KhoaHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.LopHocId)
                .ForeignKey("dbo.GiangViens", t => t.GiangVien_NguoiDungId)
                .ForeignKey("dbo.KhoaHocs", t => t.KhoaHoc_KhoaHocId)
                .Index(t => t.GiangVien_NguoiDungId)
                .Index(t => t.KhoaHoc_KhoaHocId);
            
            CreateTable(
                "dbo.ThongBaos",
                c => new
                    {
                        ThongBaoId = c.Guid(nullable: false),
                        TieuDe = c.String(nullable: false),
                        NoiDung = c.String(nullable: false),
                        NgayGui = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ThongBaoId);
            
            CreateTable(
                "dbo.KhoaHocs",
                c => new
                    {
                        KhoaHocId = c.Guid(nullable: false),
                        TenKhoaHoc = c.String(nullable: false, maxLength: 255),
                        MoTa = c.String(nullable: false, maxLength: 255),
                        HinhAnh = c.String(),
                        ThoiLuong = c.Int(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.KhoaHocId);
            
            CreateTable(
                "dbo.DangKyHoc",
                c => new
                    {
                        DangKyId = c.Guid(nullable: false),
                        NgayDangKy = c.DateTime(nullable: false),
                        HocVien_NguoiDungId = c.Guid(),
                        LopHoc_LopHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.DangKyId)
                .ForeignKey("dbo.HocViens", t => t.HocVien_NguoiDungId)
                .ForeignKey("dbo.LopHocs", t => t.LopHoc_LopHocId)
                .Index(t => t.HocVien_NguoiDungId)
                .Index(t => t.LopHoc_LopHocId);
            
            CreateTable(
                "dbo.DiemDanhs",
                c => new
                    {
                        DiemDanhId = c.Guid(nullable: false),
                        NgayDiemDanh = c.DateTime(nullable: false),
                        TrangThai = c.Int(nullable: false),
                        BuoiHoc_BuoiHocId = c.Guid(),
                        HocVien_NguoiDungId = c.Guid(),
                    })
                .PrimaryKey(t => t.DiemDanhId)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHoc_BuoiHocId)
                .ForeignKey("dbo.HocViens", t => t.HocVien_NguoiDungId)
                .Index(t => t.BuoiHoc_BuoiHocId)
                .Index(t => t.HocVien_NguoiDungId);
            
            CreateTable(
                "dbo.ThongBaoGiangViens",
                c => new
                    {
                        ThongBao_ThongBaoId = c.Guid(nullable: false),
                        GiangVien_NguoiDungId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ThongBao_ThongBaoId, t.GiangVien_NguoiDungId })
                .ForeignKey("dbo.ThongBaos", t => t.ThongBao_ThongBaoId, cascadeDelete: true)
                .ForeignKey("dbo.GiangViens", t => t.GiangVien_NguoiDungId, cascadeDelete: true)
                .Index(t => t.ThongBao_ThongBaoId)
                .Index(t => t.GiangVien_NguoiDungId);
            
            CreateTable(
                "dbo.HocVienLopHocs",
                c => new
                    {
                        HocVien_NguoiDungId = c.Guid(nullable: false),
                        LopHoc_LopHocId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.HocVien_NguoiDungId, t.LopHoc_LopHocId })
                .ForeignKey("dbo.HocViens", t => t.HocVien_NguoiDungId, cascadeDelete: true)
                .ForeignKey("dbo.LopHocs", t => t.LopHoc_LopHocId, cascadeDelete: true)
                .Index(t => t.HocVien_NguoiDungId)
                .Index(t => t.LopHoc_LopHocId);
            
            CreateTable(
                "dbo.HocVienThongBaos",
                c => new
                    {
                        HocVien_NguoiDungId = c.Guid(nullable: false),
                        ThongBao_ThongBaoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.HocVien_NguoiDungId, t.ThongBao_ThongBaoId })
                .ForeignKey("dbo.HocViens", t => t.HocVien_NguoiDungId, cascadeDelete: true)
                .ForeignKey("dbo.ThongBaos", t => t.ThongBao_ThongBaoId, cascadeDelete: true)
                .Index(t => t.HocVien_NguoiDungId)
                .Index(t => t.ThongBao_ThongBaoId);
            
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        NguoiDungId = c.Guid(nullable: false),
                        MaNhanVien = c.String(),
                    })
                .PrimaryKey(t => t.NguoiDungId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId)
                .Index(t => t.NguoiDungId);
            
            CreateTable(
                "dbo.GiangViens",
                c => new
                    {
                        NguoiDungId = c.Guid(nullable: false),
                        MaGiangVien = c.String(nullable: false, maxLength: 20),
                        ChuyenMon = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.NguoiDungId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId)
                .Index(t => t.NguoiDungId);
            
            CreateTable(
                "dbo.HocViens",
                c => new
                    {
                        NguoiDungId = c.Guid(nullable: false),
                        MaHocVien = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.NguoiDungId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId)
                .Index(t => t.NguoiDungId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HocViens", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.GiangViens", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.Admins", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.DiemDanhs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.DiemDanhs", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.DangKyHoc", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.DangKyHoc", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.LopHocs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.HocVienThongBaos", "ThongBao_ThongBaoId", "dbo.ThongBaos");
            DropForeignKey("dbo.HocVienThongBaos", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.HocVienLopHocs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.HocVienLopHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.ThongBaoGiangViens", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.ThongBaoGiangViens", "ThongBao_ThongBaoId", "dbo.ThongBaos");
            DropForeignKey("dbo.LopHocs", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.BuoiHocs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.HocViens", new[] { "NguoiDungId" });
            DropIndex("dbo.GiangViens", new[] { "NguoiDungId" });
            DropIndex("dbo.Admins", new[] { "NguoiDungId" });
            DropIndex("dbo.HocVienThongBaos", new[] { "ThongBao_ThongBaoId" });
            DropIndex("dbo.HocVienThongBaos", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.HocVienLopHocs", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.HocVienLopHocs", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.ThongBaoGiangViens", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.ThongBaoGiangViens", new[] { "ThongBao_ThongBaoId" });
            DropIndex("dbo.DiemDanhs", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.DiemDanhs", new[] { "BuoiHoc_BuoiHocId" });
            DropIndex("dbo.DangKyHoc", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.DangKyHoc", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.LopHocs", new[] { "KhoaHoc_KhoaHocId" });
            DropIndex("dbo.LopHocs", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.BuoiHocs", new[] { "LopHoc_LopHocId" });
            DropTable("dbo.HocViens");
            DropTable("dbo.GiangViens");
            DropTable("dbo.Admins");
            DropTable("dbo.HocVienThongBaos");
            DropTable("dbo.HocVienLopHocs");
            DropTable("dbo.ThongBaoGiangViens");
            DropTable("dbo.DiemDanhs");
            DropTable("dbo.DangKyHoc");
            DropTable("dbo.KhoaHocs");
            DropTable("dbo.ThongBaos");
            DropTable("dbo.LopHocs");
            DropTable("dbo.BuoiHocs");
            DropTable("dbo.NguoiDungs");
        }
    }
}
