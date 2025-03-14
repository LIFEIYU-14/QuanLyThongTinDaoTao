namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1403 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DangKyHocs", "KhoaHoc_KhoaHocId", c => c.Guid());
            CreateIndex("dbo.DangKyHocs", "KhoaHoc_KhoaHocId");
            AddForeignKey("dbo.DangKyHocs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs", "KhoaHocId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DangKyHocs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropIndex("dbo.DangKyHocs", new[] { "KhoaHoc_KhoaHocId" });
            DropColumn("dbo.DangKyHocs", "KhoaHoc_KhoaHocId");
        }
    }
}
