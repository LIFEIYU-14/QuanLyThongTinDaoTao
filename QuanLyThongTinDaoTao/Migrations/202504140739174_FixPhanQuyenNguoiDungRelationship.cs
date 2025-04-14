namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixPhanQuyenNguoiDungRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NguoiDungPhanQuyens", "NguoiDung_NguoiDungId", "dbo.NguoiDungs");
            DropForeignKey("dbo.NguoiDungPhanQuyens", "PhanQuyen_PhanQuyenId", "dbo.PhanQuyens");
            DropIndex("dbo.NguoiDungPhanQuyens", new[] { "NguoiDung_NguoiDungId" });
            DropIndex("dbo.NguoiDungPhanQuyens", new[] { "PhanQuyen_PhanQuyenId" });
            CreateIndex("dbo.PhanQuyens", "NguoiDungId");
            AddForeignKey("dbo.PhanQuyens", "NguoiDungId", "dbo.NguoiDungs", "NguoiDungId", cascadeDelete: true);
            DropTable("dbo.NguoiDungPhanQuyens");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.NguoiDungPhanQuyens",
                c => new
                    {
                        NguoiDung_NguoiDungId = c.Guid(nullable: false),
                        PhanQuyen_PhanQuyenId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.NguoiDung_NguoiDungId, t.PhanQuyen_PhanQuyenId });
            
            DropForeignKey("dbo.PhanQuyens", "NguoiDungId", "dbo.NguoiDungs");
            DropIndex("dbo.PhanQuyens", new[] { "NguoiDungId" });
            CreateIndex("dbo.NguoiDungPhanQuyens", "PhanQuyen_PhanQuyenId");
            CreateIndex("dbo.NguoiDungPhanQuyens", "NguoiDung_NguoiDungId");
            AddForeignKey("dbo.NguoiDungPhanQuyens", "PhanQuyen_PhanQuyenId", "dbo.PhanQuyens", "PhanQuyenId", cascadeDelete: true);
            AddForeignKey("dbo.NguoiDungPhanQuyens", "NguoiDung_NguoiDungId", "dbo.NguoiDungs", "NguoiDungId", cascadeDelete: true);
        }
    }
}
