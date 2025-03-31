namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NEWUPDATE : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        AttachmentId = c.Guid(nullable: false),
                        FileName = c.String(nullable: false, maxLength: 255),
                        FileType = c.String(nullable: false, maxLength: 100),
                        FilePath = c.String(nullable: false, maxLength: 500),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AttachmentId);
            
            CreateTable(
                "dbo.BuoiHoc_Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BuoiHocId = c.Guid(nullable: false),
                        AttachmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHocId, cascadeDelete: true)
                .Index(t => t.BuoiHocId)
                .Index(t => t.AttachmentId);
            
            CreateTable(
                "dbo.BuoiHocs",
                c => new
                    {
                        BuoiHocId = c.Guid(nullable: false),
                        NgayHoc = c.DateTime(nullable: false),
                        GioBatDau = c.Time(nullable: false, precision: 7),
                        GioKetThuc = c.Time(nullable: false, precision: 7),
                        TrangThai = c.Int(nullable: false),
                        GhiChu = c.String(),
                        GiangVien_NguoiDungId = c.Guid(),
                        LopHoc_LopHocId = c.Guid(),
                        HocVien_NguoiDungId = c.Guid(),
                    })
                .PrimaryKey(t => t.BuoiHocId)
                .ForeignKey("dbo.GiangViens", t => t.GiangVien_NguoiDungId)
                .ForeignKey("dbo.LopHocs", t => t.LopHoc_LopHocId)
                .ForeignKey("dbo.HocViens", t => t.HocVien_NguoiDungId)
                .Index(t => t.GiangVien_NguoiDungId)
                .Index(t => t.LopHoc_LopHocId)
                .Index(t => t.HocVien_NguoiDungId);
            
            CreateTable(
                "dbo.GiangVien_BuoiHoc",
                c => new
                    {
                        GiangVien_BuoiHocId = c.Guid(nullable: false),
                        BuoiHoc_BuoiHocId = c.Guid(),
                        GiangVien_NguoiDungId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.GiangVien_BuoiHocId)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHoc_BuoiHocId)
                .ForeignKey("dbo.GiangViens", t => t.GiangVien_NguoiDungId)
                .Index(t => t.BuoiHoc_BuoiHocId)
                .Index(t => t.GiangVien_NguoiDungId);
            
            CreateTable(
                "dbo.NguoiDungs",
                c => new
                    {
                        NguoiDungId = c.Guid(nullable: false),
                        TaiKhoan = c.String(nullable: false),
                        MatKhau = c.String(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NguoiDungId);
            
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
                        MoTa = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                        KhoaHoc_KhoaHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.LopHocId)
                .ForeignKey("dbo.KhoaHocs", t => t.KhoaHoc_KhoaHocId)
                .Index(t => t.KhoaHoc_KhoaHocId);
            
            CreateTable(
                "dbo.DangKyHocs",
                c => new
                    {
                        DangKyId = c.Guid(nullable: false),
                        NguoiDungId = c.Guid(nullable: false),
                        LopHocId = c.Guid(nullable: false),
                        NgayDangKy = c.DateTime(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        ConfirmationToken = c.String(),
                    })
                .PrimaryKey(t => t.DangKyId)
                .ForeignKey("dbo.HocViens", t => t.NguoiDungId)
                .ForeignKey("dbo.LopHocs", t => t.LopHocId, cascadeDelete: true)
                .Index(t => t.NguoiDungId)
                .Index(t => t.LopHocId);
            
            CreateTable(
                "dbo.DiemDanhs_HV",
                c => new
                    {
                        DiemDanhId = c.Guid(nullable: false),
                        BuoiHocId = c.Guid(nullable: false),
                        NgayDiemDanh = c.DateTime(nullable: false),
                        TrangThai = c.Int(nullable: false),
                        HocVien_NguoiDungId = c.Guid(),
                    })
                .PrimaryKey(t => t.DiemDanhId)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHocId, cascadeDelete: true)
                .ForeignKey("dbo.HocViens", t => t.HocVien_NguoiDungId)
                .Index(t => t.BuoiHocId)
                .Index(t => t.HocVien_NguoiDungId);
            
            CreateTable(
                "dbo.PhanQuyens",
                c => new
                    {
                        PhanQuyenId = c.Guid(nullable: false),
                        TenQuyen = c.String(nullable: false),
                        NguoiDungId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.PhanQuyenId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId, cascadeDelete: true)
                .Index(t => t.NguoiDungId);
            
            CreateTable(
                "dbo.KhoaHocs",
                c => new
                    {
                        KhoaHocId = c.Guid(nullable: false),
                        HinhDaiDienKhoaHoc = c.String(maxLength: 500),
                        MaKhoaHoc = c.String(nullable: false),
                        TenKhoaHoc = c.String(nullable: false, maxLength: 255),
                        MoTa = c.String(nullable: false, maxLength: 255),
                        ThoiLuong = c.Int(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.KhoaHocId);
            
            CreateTable(
                "dbo.KhoaHoc_Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        KhoaHocId = c.Guid(nullable: false),
                        AttachmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .ForeignKey("dbo.KhoaHocs", t => t.KhoaHocId, cascadeDelete: true)
                .Index(t => t.KhoaHocId)
                .Index(t => t.AttachmentId);
            
            CreateTable(
                "dbo.LopHoc_Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LopHocId = c.Guid(nullable: false),
                        AttachmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .ForeignKey("dbo.LopHocs", t => t.LopHocId, cascadeDelete: true)
                .Index(t => t.LopHocId)
                .Index(t => t.AttachmentId);
            
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
                "dbo.DiemDanhs_GV",
                c => new
                    {
                        DiemDanhId = c.Guid(nullable: false),
                        BuoiHocId = c.Guid(nullable: false),
                        NgayDiemDanh = c.DateTime(nullable: false),
                        TrangThai = c.Int(nullable: false),
                        GiangVien_NguoiDungId = c.Guid(),
                    })
                .PrimaryKey(t => t.DiemDanhId)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHocId, cascadeDelete: true)
                .ForeignKey("dbo.GiangViens", t => t.GiangVien_NguoiDungId)
                .Index(t => t.BuoiHocId)
                .Index(t => t.GiangVien_NguoiDungId);
            
            CreateTable(
                "dbo.LopHocGiangViens",
                c => new
                    {
                        LopHoc_LopHocId = c.Guid(nullable: false),
                        GiangVien_NguoiDungId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.LopHoc_LopHocId, t.GiangVien_NguoiDungId })
                .ForeignKey("dbo.LopHocs", t => t.LopHoc_LopHocId, cascadeDelete: true)
                .ForeignKey("dbo.GiangViens", t => t.GiangVien_NguoiDungId, cascadeDelete: true)
                .Index(t => t.LopHoc_LopHocId)
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
                "dbo.GiangViens",
                c => new
                    {
                        NguoiDungId = c.Guid(nullable: false),
                        MaGiangVien = c.String(nullable: false, maxLength: 20),
                        HoVaTen = c.String(nullable: false),
                        NgaySinh = c.DateTime(nullable: false),
                        SoDienThoai = c.String(nullable: false, maxLength: 15),
                        Email = c.String(nullable: false, maxLength: 100),
                        ChuyenMon = c.String(nullable: false, maxLength: 255),
                        HocHam = c.String(nullable: false, maxLength: 255),
                        QR_Code_GV = c.String(),
                    })
                .PrimaryKey(t => t.NguoiDungId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId)
                .Index(t => t.NguoiDungId)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "dbo.HocViens",
                c => new
                    {
                        NguoiDungId = c.Guid(nullable: false),
                        MaHocVien = c.String(nullable: false, maxLength: 20),
                        HoVaTen = c.String(nullable: false),
                        NgaySinh = c.DateTime(nullable: false),
                        SoDienThoai = c.String(nullable: false, maxLength: 15),
                        Email = c.String(nullable: false, maxLength: 100),
                        CoQuanLamViec = c.String(nullable: false, maxLength: 255),
                        QR_Code_HV = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                        XacNhanToken = c.String(),
                    })
                .PrimaryKey(t => t.NguoiDungId)
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDungId)
                .Index(t => t.NguoiDungId)
                .Index(t => t.Email, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HocViens", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.GiangViens", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.DiemDanhs_GV", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.DiemDanhs_GV", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.GiangVien_BuoiHoc", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.ThongBaoGiangViens", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.ThongBaoGiangViens", "ThongBao_ThongBaoId", "dbo.ThongBaos");
            DropForeignKey("dbo.LopHoc_Attachments", "LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.LopHoc_Attachments", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.LopHocs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.KhoaHoc_Attachments", "KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.KhoaHoc_Attachments", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.PhanQuyens", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.HocVienLopHocs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.HocVienLopHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.DiemDanhs_HV", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.DiemDanhs_HV", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.DangKyHocs", "LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.DangKyHocs", "NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.BuoiHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.LopHocGiangViens", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.LopHocGiangViens", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.BuoiHocs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.BuoiHocs", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.GiangVien_BuoiHoc", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.BuoiHoc_Attachments", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.BuoiHoc_Attachments", "AttachmentId", "dbo.Attachments");
            DropIndex("dbo.HocViens", new[] { "Email" });
            DropIndex("dbo.HocViens", new[] { "NguoiDungId" });
            DropIndex("dbo.GiangViens", new[] { "Email" });
            DropIndex("dbo.GiangViens", new[] { "NguoiDungId" });
            DropIndex("dbo.ThongBaoGiangViens", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.ThongBaoGiangViens", new[] { "ThongBao_ThongBaoId" });
            DropIndex("dbo.HocVienLopHocs", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.HocVienLopHocs", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.LopHocGiangViens", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.LopHocGiangViens", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.DiemDanhs_GV", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.DiemDanhs_GV", new[] { "BuoiHocId" });
            DropIndex("dbo.LopHoc_Attachments", new[] { "AttachmentId" });
            DropIndex("dbo.LopHoc_Attachments", new[] { "LopHocId" });
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "AttachmentId" });
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "KhoaHocId" });
            DropIndex("dbo.PhanQuyens", new[] { "NguoiDungId" });
            DropIndex("dbo.DiemDanhs_HV", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.DiemDanhs_HV", new[] { "BuoiHocId" });
            DropIndex("dbo.DangKyHocs", new[] { "LopHocId" });
            DropIndex("dbo.DangKyHocs", new[] { "NguoiDungId" });
            DropIndex("dbo.LopHocs", new[] { "KhoaHoc_KhoaHocId" });
            DropIndex("dbo.GiangVien_BuoiHoc", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.GiangVien_BuoiHoc", new[] { "BuoiHoc_BuoiHocId" });
            DropIndex("dbo.BuoiHocs", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.BuoiHocs", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.BuoiHocs", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "AttachmentId" });
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "BuoiHocId" });
            DropTable("dbo.HocViens");
            DropTable("dbo.GiangViens");
            DropTable("dbo.ThongBaoGiangViens");
            DropTable("dbo.HocVienLopHocs");
            DropTable("dbo.LopHocGiangViens");
            DropTable("dbo.DiemDanhs_GV");
            DropTable("dbo.ThongBaos");
            DropTable("dbo.LopHoc_Attachments");
            DropTable("dbo.KhoaHoc_Attachments");
            DropTable("dbo.KhoaHocs");
            DropTable("dbo.PhanQuyens");
            DropTable("dbo.DiemDanhs_HV");
            DropTable("dbo.DangKyHocs");
            DropTable("dbo.LopHocs");
            DropTable("dbo.NguoiDungs");
            DropTable("dbo.GiangVien_BuoiHoc");
            DropTable("dbo.BuoiHocs");
            DropTable("dbo.BuoiHoc_Attachments");
            DropTable("dbo.Attachments");
        }
    }
}
