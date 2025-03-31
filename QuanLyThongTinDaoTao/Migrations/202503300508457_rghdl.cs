namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rghdl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GiangViens", "MatKhau", c => c.String(nullable: false));
            AlterColumn("dbo.GiangViens", "QR_Code_GV", c => c.String());
            AlterColumn("dbo.HocViens", "QR_Code_HV", c => c.String());
            DropColumn("dbo.NguoiDungs", "MatKhau");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NguoiDungs", "MatKhau", c => c.String(nullable: false));
            AlterColumn("dbo.HocViens", "QR_Code_HV", c => c.String(nullable: false));
            AlterColumn("dbo.GiangViens", "QR_Code_GV", c => c.String(nullable: false));
            DropColumn("dbo.GiangViens", "MatKhau");
        }
    }
}
