using BtlWebNangCao.Models;
using Microsoft.EntityFrameworkCore;
namespace BtlWebNangCao.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<PhongChat> PhongChats { get; set; }
        public DbSet<ThanhVienPhong> ThanhVienPhongs { get; set; }
        public DbSet<TinNhan> TinNhans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính cho bảng ThanhVienPhong (bảng trung gian)
            modelBuilder.Entity<ThanhVienPhong>()
                .HasKey(tv => new { tv.MaNguoiDung, tv.MaPhong });

            // Đảm bảo Tên Đăng Nhập và Email là duy nhất
            modelBuilder.Entity<NguoiDung>()
                .HasIndex(nd => nd.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(nd => nd.Email)
                .IsUnique();
        }
    }
}
