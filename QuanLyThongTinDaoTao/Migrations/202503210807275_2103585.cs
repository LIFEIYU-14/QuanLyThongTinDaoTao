namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2103585 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.LopHocs", "MaLopHoc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LopHocs", "MaLopHoc", c => c.String(nullable: false));
        }
    }
}
