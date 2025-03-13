namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Uo : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.NguoiDungs", "TenDangNhap");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NguoiDungs", "TenDangNhap", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
