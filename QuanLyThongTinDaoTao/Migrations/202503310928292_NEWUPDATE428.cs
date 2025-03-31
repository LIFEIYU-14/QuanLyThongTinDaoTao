namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NEWUPDATE428 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DiemDanhs", newName: "DiemDanhs_HV");
            DropForeignKey("dbo.ThongBaoHocViens", "ThongBao_ThongBaoId", "dbo.ThongBaos");
            DropForeignKey("dbo.ThongBaoHocViens", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.DangKyHocs", "KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.Admins", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.DiemDanhs", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs");
            DropIndex("dbo.NguoiDungs", new[] { "Email" });
            DropIndex("dbo.DangKyHocs", new[] { "KhoaHocId" });
            DropIndex("dbo.DiemDanhs_HV", new[] { "BuoiHoc_BuoiHocId" });
            DropIndex("dbo.ThongBaoHocViens", new[] { "ThongBao_ThongBaoId" });
            DropIndex("dbo.ThongBaoHocViens", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.Admins", new[] { "NguoiDungId" });
            RenameColumn(table: "dbo.DiemDanhs_HV", name: "BuoiHoc_BuoiHocId", newName: "BuoiHocId");
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
            
            AddColumn("dbo.GiangViens", "HoVaTen", c => c.String(nullable: false));
            AddColumn("dbo.GiangViens", "NgaySinh", c => c.DateTime(nullable: false));
            AddColumn("dbo.GiangViens", "SoDienThoai", c => c.String(nullable: false, maxLength: 15));
            AddColumn("dbo.GiangViens", "Email", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.HocViens", "HoVaTen", c => c.String(nullable: false));
            AddColumn("dbo.HocViens", "NgaySinh", c => c.DateTime(nullable: false));
            AddColumn("dbo.HocViens", "SoDienThoai", c => c.String(nullable: false, maxLength: 15));
            AddColumn("dbo.HocViens", "Email", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.NguoiDungs", "TaiKhoan", c => c.String(nullable: false));
            AddColumn("dbo.NguoiDungs", "MatKhau", c => c.String(nullable: false));
            AddColumn("dbo.BuoiHocs", "HocVien_NguoiDungId", c => c.Guid());
            AddColumn("dbo.LopHocs", "MaLopHoc", c => c.String(nullable: false));
            AddColumn("dbo.KhoaHocs", "MaKhoaHoc", c => c.String(nullable: false));
            AlterColumn("dbo.DiemDanhs_HV", "BuoiHocId", c => c.Guid(nullable: false));
            CreateIndex("dbo.BuoiHocs", "HocVien_NguoiDungId");
            CreateIndex("dbo.DiemDanhs_HV", "BuoiHocId");
            CreateIndex("dbo.GiangViens", "Email", unique: true);
            CreateIndex("dbo.HocViens", "Email", unique: true);
            AddForeignKey("dbo.BuoiHocs", "HocVien_NguoiDungId", "dbo.HocViens", "NguoiDungId");
            AddForeignKey("dbo.DiemDanhs_HV", "BuoiHocId", "dbo.BuoiHocs", "BuoiHocId", cascadeDelete: true);
            DropColumn("dbo.GiangViens", "MatKhau");
            DropColumn("dbo.NguoiDungs", "HoVaTen");
            DropColumn("dbo.NguoiDungs", "VaiTro");
            DropColumn("dbo.NguoiDungs", "NgaySinh");
            DropColumn("dbo.NguoiDungs", "SoDienThoai");
            DropColumn("dbo.NguoiDungs", "Email");
            DropColumn("dbo.DangKyHocs", "KhoaHocId");
            DropTable("dbo.ThongBaoHocViens");
            DropTable("dbo.Admins");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        NguoiDungId = c.Guid(nullable: false),
                        MaNhanVien = c.String(),
                    })
                .PrimaryKey(t => t.NguoiDungId);
            
            CreateTable(
                "dbo.ThongBaoHocViens",
                c => new
                    {
                        ThongBao_ThongBaoId = c.Guid(nullable: false),
                        HocVien_NguoiDungId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ThongBao_ThongBaoId, t.HocVien_NguoiDungId });
            
            AddColumn("dbo.DangKyHocs", "KhoaHocId", c => c.Guid(nullable: false));
            AddColumn("dbo.NguoiDungs", "Email", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.NguoiDungs", "SoDienThoai", c => c.String(nullable: false, maxLength: 15));
            AddColumn("dbo.NguoiDungs", "NgaySinh", c => c.DateTime(nullable: false));
            AddColumn("dbo.NguoiDungs", "VaiTro", c => c.Int(nullable: false));
            AddColumn("dbo.NguoiDungs", "HoVaTen", c => c.String(nullable: false));
            AddColumn("dbo.GiangViens", "MatKhau", c => c.String(nullable: false));
            DropForeignKey("dbo.DiemDanhs_HV", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.DiemDanhs_GV", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.DiemDanhs_GV", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.GiangVien_BuoiHoc", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.PhanQuyens", "NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.BuoiHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.GiangVien_BuoiHoc", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs");
            DropIndex("dbo.HocViens", new[] { "Email" });
            DropIndex("dbo.GiangViens", new[] { "Email" });
            DropIndex("dbo.DiemDanhs_GV", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.DiemDanhs_GV", new[] { "BuoiHocId" });
            DropIndex("dbo.PhanQuyens", new[] { "NguoiDungId" });
            DropIndex("dbo.DiemDanhs_HV", new[] { "BuoiHocId" });
            DropIndex("dbo.GiangVien_BuoiHoc", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.GiangVien_BuoiHoc", new[] { "BuoiHoc_BuoiHocId" });
            DropIndex("dbo.BuoiHocs", new[] { "HocVien_NguoiDungId" });
            AlterColumn("dbo.DiemDanhs_HV", "BuoiHocId", c => c.Guid());
            DropColumn("dbo.KhoaHocs", "MaKhoaHoc");
            DropColumn("dbo.LopHocs", "MaLopHoc");
            DropColumn("dbo.BuoiHocs", "HocVien_NguoiDungId");
            DropColumn("dbo.NguoiDungs", "MatKhau");
            DropColumn("dbo.NguoiDungs", "TaiKhoan");
            DropColumn("dbo.HocViens", "Email");
            DropColumn("dbo.HocViens", "SoDienThoai");
            DropColumn("dbo.HocViens", "NgaySinh");
            DropColumn("dbo.HocViens", "HoVaTen");
            DropColumn("dbo.GiangViens", "Email");
            DropColumn("dbo.GiangViens", "SoDienThoai");
            DropColumn("dbo.GiangViens", "NgaySinh");
            DropColumn("dbo.GiangViens", "HoVaTen");
            DropTable("dbo.DiemDanhs_GV");
            DropTable("dbo.PhanQuyens");
            DropTable("dbo.GiangVien_BuoiHoc");
            RenameColumn(table: "dbo.DiemDanhs_HV", name: "BuoiHocId", newName: "BuoiHoc_BuoiHocId");
            CreateIndex("dbo.Admins", "NguoiDungId");
            CreateIndex("dbo.ThongBaoHocViens", "HocVien_NguoiDungId");
            CreateIndex("dbo.ThongBaoHocViens", "ThongBao_ThongBaoId");
            CreateIndex("dbo.DiemDanhs_HV", "BuoiHoc_BuoiHocId");
            CreateIndex("dbo.DangKyHocs", "KhoaHocId");
            CreateIndex("dbo.NguoiDungs", "Email", unique: true);
            AddForeignKey("dbo.DiemDanhs", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs", "BuoiHocId");
            AddForeignKey("dbo.Admins", "NguoiDungId", "dbo.NguoiDungs", "NguoiDungId");
            AddForeignKey("dbo.DangKyHocs", "KhoaHocId", "dbo.KhoaHocs", "KhoaHocId", cascadeDelete: true);
            AddForeignKey("dbo.ThongBaoHocViens", "HocVien_NguoiDungId", "dbo.HocViens", "NguoiDungId", cascadeDelete: true);
            AddForeignKey("dbo.ThongBaoHocViens", "ThongBao_ThongBaoId", "dbo.ThongBaos", "ThongBaoId", cascadeDelete: true);
            RenameTable(name: "dbo.DiemDanhs_HV", newName: "DiemDanhs");
        }
    }
}
