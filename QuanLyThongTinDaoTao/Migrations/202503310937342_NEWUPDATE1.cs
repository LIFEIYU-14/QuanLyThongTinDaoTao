namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NEWUPDATE1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KhoaHocs", "MoTa", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KhoaHocs", "MoTa", c => c.String(nullable: false, maxLength: 255));
        }
    }
}
