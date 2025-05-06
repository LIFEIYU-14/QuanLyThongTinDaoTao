namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fixindentity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GiangVien_BuoiHoc", "AppUserId", "dbo.GiangViens");
            DropForeignKey("dbo.DiemDanhs_GV", "AppUserId", "dbo.GiangViens");
            DropForeignKey("dbo.DangKyHocs", "AppUserId", "dbo.HocViens");
            DropForeignKey("dbo.DiemDanhs_HV", "AppUserId", "dbo.HocViens");
            DropForeignKey("dbo.GiangViens", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.HocViens", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ThongBaoGiangViens", "GiangVien_Id", "dbo.GiangViens");
            RenameColumn(table: "dbo.GiangVien_BuoiHoc", name: "AppUserId", newName: "GiangVienId");
            RenameColumn(table: "dbo.DiemDanhs_GV", name: "AppUserId", newName: "GiangVienId");
            RenameColumn(table: "dbo.ThongBaoGiangViens", name: "GiangVien_Id", newName: "GiangVien_GiangVienId");
            RenameColumn(table: "dbo.DangKyHocs", name: "AppUserId", newName: "HocVienId");
            RenameColumn(table: "dbo.DiemDanhs_HV", name: "AppUserId", newName: "HocVienId");
            RenameColumn(table: "dbo.GiangViens", name: "Id", newName: "AppUserId");
            RenameColumn(table: "dbo.HocViens", name: "Id", newName: "AppUserId");
            RenameIndex(table: "dbo.GiangVien_BuoiHoc", name: "IX_AppUserId", newName: "IX_GiangVienId");
            RenameIndex(table: "dbo.GiangViens", name: "IX_Id", newName: "IX_AppUserId");
            RenameIndex(table: "dbo.DiemDanhs_GV", name: "IX_AppUserId", newName: "IX_GiangVienId");
            RenameIndex(table: "dbo.DangKyHocs", name: "IX_AppUserId", newName: "IX_HocVienId");
            RenameIndex(table: "dbo.HocViens", name: "IX_Id", newName: "IX_AppUserId");
            RenameIndex(table: "dbo.DiemDanhs_HV", name: "IX_AppUserId", newName: "IX_HocVienId");
            RenameIndex(table: "dbo.ThongBaoGiangViens", name: "IX_GiangVien_Id", newName: "IX_GiangVien_GiangVienId");
            DropPrimaryKey("dbo.GiangViens");
            DropPrimaryKey("dbo.HocViens");
            AddColumn("dbo.GiangViens", "GiangVienId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.GiangViens", "Email", c => c.String(nullable: false));
            AddColumn("dbo.HocViens", "HocVienId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.HocViens", "Email", c => c.String(nullable: false));
            AddPrimaryKey("dbo.GiangViens", "GiangVienId");
            AddPrimaryKey("dbo.HocViens", "HocVienId");
            AddForeignKey("dbo.GiangVien_BuoiHoc", "GiangVienId", "dbo.GiangViens", "GiangVienId", cascadeDelete: true);
            AddForeignKey("dbo.DiemDanhs_GV", "GiangVienId", "dbo.GiangViens", "GiangVienId", cascadeDelete: true);
            AddForeignKey("dbo.DangKyHocs", "HocVienId", "dbo.HocViens", "HocVienId", cascadeDelete: true);
            AddForeignKey("dbo.DiemDanhs_HV", "HocVienId", "dbo.HocViens", "HocVienId", cascadeDelete: true);
            AddForeignKey("dbo.GiangViens", "AppUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.HocViens", "AppUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ThongBaoGiangViens", "GiangVien_GiangVienId", "dbo.GiangViens", "GiangVienId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThongBaoGiangViens", "GiangVien_GiangVienId", "dbo.GiangViens");
            DropForeignKey("dbo.HocViens", "AppUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GiangViens", "AppUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DiemDanhs_HV", "HocVienId", "dbo.HocViens");
            DropForeignKey("dbo.DangKyHocs", "HocVienId", "dbo.HocViens");
            DropForeignKey("dbo.DiemDanhs_GV", "GiangVienId", "dbo.GiangViens");
            DropForeignKey("dbo.GiangVien_BuoiHoc", "GiangVienId", "dbo.GiangViens");
            DropPrimaryKey("dbo.HocViens");
            DropPrimaryKey("dbo.GiangViens");
            DropColumn("dbo.HocViens", "Email");
            DropColumn("dbo.HocViens", "HocVienId");
            DropColumn("dbo.GiangViens", "Email");
            DropColumn("dbo.GiangViens", "GiangVienId");
            AddPrimaryKey("dbo.HocViens", "Id");
            AddPrimaryKey("dbo.GiangViens", "Id");
            RenameIndex(table: "dbo.ThongBaoGiangViens", name: "IX_GiangVien_GiangVienId", newName: "IX_GiangVien_Id");
            RenameIndex(table: "dbo.DiemDanhs_HV", name: "IX_HocVienId", newName: "IX_AppUserId");
            RenameIndex(table: "dbo.HocViens", name: "IX_AppUserId", newName: "IX_Id");
            RenameIndex(table: "dbo.DangKyHocs", name: "IX_HocVienId", newName: "IX_AppUserId");
            RenameIndex(table: "dbo.DiemDanhs_GV", name: "IX_GiangVienId", newName: "IX_AppUserId");
            RenameIndex(table: "dbo.GiangViens", name: "IX_AppUserId", newName: "IX_Id");
            RenameIndex(table: "dbo.GiangVien_BuoiHoc", name: "IX_GiangVienId", newName: "IX_AppUserId");
            RenameColumn(table: "dbo.HocViens", name: "AppUserId", newName: "Id");
            RenameColumn(table: "dbo.GiangViens", name: "AppUserId", newName: "Id");
            RenameColumn(table: "dbo.DiemDanhs_HV", name: "HocVienId", newName: "AppUserId");
            RenameColumn(table: "dbo.DangKyHocs", name: "HocVienId", newName: "AppUserId");
            RenameColumn(table: "dbo.ThongBaoGiangViens", name: "GiangVien_GiangVienId", newName: "GiangVien_Id");
            RenameColumn(table: "dbo.DiemDanhs_GV", name: "GiangVienId", newName: "AppUserId");
            RenameColumn(table: "dbo.GiangVien_BuoiHoc", name: "GiangVienId", newName: "AppUserId");
            AddForeignKey("dbo.ThongBaoGiangViens", "GiangVien_Id", "dbo.GiangViens", "Id", cascadeDelete: true);
            AddForeignKey("dbo.HocViens", "Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.GiangViens", "Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.DiemDanhs_HV", "AppUserId", "dbo.HocViens", "Id");
            AddForeignKey("dbo.DangKyHocs", "AppUserId", "dbo.HocViens", "Id");
            AddForeignKey("dbo.DiemDanhs_GV", "AppUserId", "dbo.GiangViens", "Id");
            AddForeignKey("dbo.GiangVien_BuoiHoc", "AppUserId", "dbo.GiangViens", "Id");
        }
    }
}
