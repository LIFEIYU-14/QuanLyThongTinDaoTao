namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2304 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "GiangVien_Id", "dbo.GiangViens");
            DropForeignKey("dbo.AspNetUsers", "HocVien_Id", "dbo.HocViens");
            DropIndex("dbo.AspNetUsers", new[] { "GiangVien_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "HocVien_Id" });
            DropColumn("dbo.AspNetUsers", "GiangVien_Id");
            DropColumn("dbo.AspNetUsers", "HocVien_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "HocVien_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "GiangVien_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "HocVien_Id");
            CreateIndex("dbo.AspNetUsers", "GiangVien_Id");
            AddForeignKey("dbo.AspNetUsers", "HocVien_Id", "dbo.HocViens", "Id");
            AddForeignKey("dbo.AspNetUsers", "GiangVien_Id", "dbo.GiangViens", "Id");
        }
    }
}
