namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _903 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DiemDanhs_HV", new[] { "HocVien_NguoiDungId" });
            RenameColumn(table: "dbo.DiemDanhs_HV", name: "HocVien_NguoiDungId", newName: "NguoiDungId");
            AlterColumn("dbo.DiemDanhs_HV", "NguoiDungId", c => c.Guid(nullable: false));
            CreateIndex("dbo.DiemDanhs_HV", "NguoiDungId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DiemDanhs_HV", new[] { "NguoiDungId" });
            AlterColumn("dbo.DiemDanhs_HV", "NguoiDungId", c => c.Guid());
            RenameColumn(table: "dbo.DiemDanhs_HV", name: "NguoiDungId", newName: "HocVien_NguoiDungId");
            CreateIndex("dbo.DiemDanhs_HV", "HocVien_NguoiDungId");
        }
    }
}
