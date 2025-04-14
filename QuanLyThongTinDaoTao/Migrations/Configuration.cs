namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<QuanLyThongTinDaoTao.Models.DbContextThongTinDaoTao>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(QuanLyThongTinDaoTao.Models.DbContextThongTinDaoTao context)
        {
            // Kiểm tra nếu chưa có admin nào
            if (!context.NguoiDungs.Any(n => n.TaiKhoan == "admin"))
            {
                var admin = new QuanLyThongTinDaoTao.Models.NguoiDung
                {
                    NguoiDungId = Guid.NewGuid(),
                    TaiKhoan = "admin",
                    MatKhau = QuanLyThongTinDaoTao.Models.PasswordHelper.HashPassword("123456"),
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                };

                // Thêm quyền Admin
                admin.PhanQuyens.Add(new QuanLyThongTinDaoTao.Models.PhanQuyen
                {
                    TenQuyen = "Admin",
                    NguoiDungId = admin.NguoiDungId
                });

                context.NguoiDungs.Add(admin);
                context.SaveChanges();
            }
        }

    }
}
