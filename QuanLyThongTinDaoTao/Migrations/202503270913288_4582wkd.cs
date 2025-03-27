namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4582wkd : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DangKyHocs", new[] { "HocVien_NguoiDungId" });
            RenameColumn(table: "dbo.DangKyHocs", name: "HocVien_NguoiDungId", newName: "NguoiDungId");
            AlterColumn("dbo.DangKyHocs", "NguoiDungId", c => c.Guid(nullable: false));
            CreateIndex("dbo.DangKyHocs", "NguoiDungId");
            DropColumn("dbo.DangKyHocs", "HocVienId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DangKyHocs", "HocVienId", c => c.Guid(nullable: false));
            DropIndex("dbo.DangKyHocs", new[] { "NguoiDungId" });
            AlterColumn("dbo.DangKyHocs", "NguoiDungId", c => c.Guid());
            RenameColumn(table: "dbo.DangKyHocs", name: "NguoiDungId", newName: "HocVien_NguoiDungId");
            CreateIndex("dbo.DangKyHocs", "HocVien_NguoiDungId");
        }
    }
}
