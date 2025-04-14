namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpAad : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PhanQuyens", "NguoiDungId", "dbo.NguoiDungs");
            DropIndex("dbo.PhanQuyens", new[] { "NguoiDungId" });
            CreateTable(
                "dbo.NguoiDungPhanQuyens",
                c => new
                    {
                        NguoiDung_NguoiDungId = c.Guid(nullable: false),
                        PhanQuyen_PhanQuyenId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.NguoiDung_NguoiDungId, t.PhanQuyen_PhanQuyenId })
                .ForeignKey("dbo.NguoiDungs", t => t.NguoiDung_NguoiDungId, cascadeDelete: true)
                .ForeignKey("dbo.PhanQuyens", t => t.PhanQuyen_PhanQuyenId, cascadeDelete: true)
                .Index(t => t.NguoiDung_NguoiDungId)
                .Index(t => t.PhanQuyen_PhanQuyenId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NguoiDungPhanQuyens", "PhanQuyen_PhanQuyenId", "dbo.PhanQuyens");
            DropForeignKey("dbo.NguoiDungPhanQuyens", "NguoiDung_NguoiDungId", "dbo.NguoiDungs");
            DropIndex("dbo.NguoiDungPhanQuyens", new[] { "PhanQuyen_PhanQuyenId" });
            DropIndex("dbo.NguoiDungPhanQuyens", new[] { "NguoiDung_NguoiDungId" });
            DropTable("dbo.NguoiDungPhanQuyens");
            CreateIndex("dbo.PhanQuyens", "NguoiDungId");
            AddForeignKey("dbo.PhanQuyens", "NguoiDungId", "dbo.NguoiDungs", "NguoiDungId", cascadeDelete: true);
        }
    }
}
