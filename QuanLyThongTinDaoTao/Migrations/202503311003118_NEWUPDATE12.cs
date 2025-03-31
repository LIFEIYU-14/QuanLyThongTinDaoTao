namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NEWUPDATE12 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.NguoiDungs", "TaiKhoan", c => c.String());
            AlterColumn("dbo.NguoiDungs", "MatKhau", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.NguoiDungs", "MatKhau", c => c.String(nullable: false));
            AlterColumn("dbo.NguoiDungs", "TaiKhoan", c => c.String(nullable: false));
        }
    }
}
