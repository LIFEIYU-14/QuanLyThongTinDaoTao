namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _213 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LopHocs", "TrangThai", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LopHocs", "TrangThai", c => c.String(nullable: false));
        }
    }
}
