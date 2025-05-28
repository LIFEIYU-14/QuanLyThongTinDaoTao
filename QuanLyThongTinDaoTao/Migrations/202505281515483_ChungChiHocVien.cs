namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChungChiHocVien : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChungChiHocTaps",
                c => new
                    {
                        ChungChiId = c.Guid(nullable: false),
                        HocVienId = c.String(nullable: false, maxLength: 128),
                        KhoaHocId = c.Guid(nullable: false),
                        NgayCap = c.DateTime(nullable: false),
                        NgayHetHan = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChungChiId)
                .ForeignKey("dbo.HocViens", t => t.HocVienId, cascadeDelete: true)
                .ForeignKey("dbo.KhoaHocs", t => t.KhoaHocId, cascadeDelete: true)
                .Index(t => t.HocVienId)
                .Index(t => t.KhoaHocId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChungChiHocTaps", "KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.ChungChiHocTaps", "HocVienId", "dbo.HocViens");
            DropIndex("dbo.ChungChiHocTaps", new[] { "KhoaHocId" });
            DropIndex("dbo.ChungChiHocTaps", new[] { "HocVienId" });
            DropTable("dbo.ChungChiHocTaps");
        }
    }
}
