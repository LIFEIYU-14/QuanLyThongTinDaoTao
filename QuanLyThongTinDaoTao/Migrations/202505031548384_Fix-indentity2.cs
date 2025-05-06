namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fixindentity2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GiangViens", "AppUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.HocViens", "AppUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GiangViens", new[] { "AppUserId" });
            DropIndex("dbo.HocViens", new[] { "AppUserId" });
            AlterColumn("dbo.GiangViens", "AppUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.HocViens", "AppUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.GiangViens", "AppUserId");
            CreateIndex("dbo.HocViens", "AppUserId");
            AddForeignKey("dbo.GiangViens", "AppUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.HocViens", "AppUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HocViens", "AppUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GiangViens", "AppUserId", "dbo.AspNetUsers");
            DropIndex("dbo.HocViens", new[] { "AppUserId" });
            DropIndex("dbo.GiangViens", new[] { "AppUserId" });
            AlterColumn("dbo.HocViens", "AppUserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.GiangViens", "AppUserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.HocViens", "AppUserId");
            CreateIndex("dbo.GiangViens", "AppUserId");
            AddForeignKey("dbo.HocViens", "AppUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GiangViens", "AppUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
