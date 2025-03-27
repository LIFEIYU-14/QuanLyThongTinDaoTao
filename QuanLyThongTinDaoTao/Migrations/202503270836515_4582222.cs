namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4582222 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DangKyHocs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.DangKyHocs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.DangKyHocs", new[] { "KhoaHoc_KhoaHocId" });
            DropIndex("dbo.DangKyHocs", new[] { "LopHoc_LopHocId" });
            RenameColumn(table: "dbo.DangKyHocs", name: "KhoaHoc_KhoaHocId", newName: "KhoaHocId");
            RenameColumn(table: "dbo.DangKyHocs", name: "LopHoc_LopHocId", newName: "LopHocId");
            AddColumn("dbo.DangKyHocs", "HocVienId", c => c.Guid(nullable: false));
            AlterColumn("dbo.DangKyHocs", "KhoaHocId", c => c.Guid(nullable: false));
            AlterColumn("dbo.DangKyHocs", "LopHocId", c => c.Guid(nullable: false));
            CreateIndex("dbo.DangKyHocs", "LopHocId");
            CreateIndex("dbo.DangKyHocs", "KhoaHocId");
            AddForeignKey("dbo.DangKyHocs", "KhoaHocId", "dbo.KhoaHocs", "KhoaHocId", cascadeDelete: true);
            AddForeignKey("dbo.DangKyHocs", "LopHocId", "dbo.LopHocs", "LopHocId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DangKyHocs", "LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.DangKyHocs", "KhoaHocId", "dbo.KhoaHocs");
            DropIndex("dbo.DangKyHocs", new[] { "KhoaHocId" });
            DropIndex("dbo.DangKyHocs", new[] { "LopHocId" });
            AlterColumn("dbo.DangKyHocs", "LopHocId", c => c.Guid());
            AlterColumn("dbo.DangKyHocs", "KhoaHocId", c => c.Guid());
            DropColumn("dbo.DangKyHocs", "HocVienId");
            RenameColumn(table: "dbo.DangKyHocs", name: "LopHocId", newName: "LopHoc_LopHocId");
            RenameColumn(table: "dbo.DangKyHocs", name: "KhoaHocId", newName: "KhoaHoc_KhoaHocId");
            CreateIndex("dbo.DangKyHocs", "LopHoc_LopHocId");
            CreateIndex("dbo.DangKyHocs", "KhoaHoc_KhoaHocId");
            AddForeignKey("dbo.DangKyHocs", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
            AddForeignKey("dbo.DangKyHocs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs", "KhoaHocId");
        }
    }
}
