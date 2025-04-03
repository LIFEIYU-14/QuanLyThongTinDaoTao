namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sss : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.GiangVien_BuoiHoc");
            AddColumn("dbo.GiangVien_BuoiHoc", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.GiangVien_BuoiHoc", "Id");
            DropColumn("dbo.GiangVien_BuoiHoc", "GiangVien_BuoiHocId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GiangVien_BuoiHoc", "GiangVien_BuoiHocId", c => c.Guid(nullable: false));
            DropPrimaryKey("dbo.GiangVien_BuoiHoc");
            DropColumn("dbo.GiangVien_BuoiHoc", "Id");
            AddPrimaryKey("dbo.GiangVien_BuoiHoc", "GiangVien_BuoiHocId");
        }
    }
}
