namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConfirmationToDangKyHocdahj : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HocViens", "IsConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.HocViens", "XacNhanToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HocViens", "XacNhanToken");
            DropColumn("dbo.HocViens", "IsConfirmed");
        }
    }
}
