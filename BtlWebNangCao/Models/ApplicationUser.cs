using Microsoft.AspNetCore.Identity;

namespace BtlWebNangCao.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgaySua { get; set; } = DateTime.Now;

        // Thêm thuộc tính LastActiveDate để lưu trữ thời gian người dùng hoạt động cuối cùng
        public DateTime? LastActiveDate { get; set; }

        // Quan hệ 1-N: Một người dùng có thể tạo nhiều phòng chat
        public ICollection<PhongChat> DanhSachPhongTao { get; set; }

        // Quan hệ N-N: Người dùng có thể tham gia nhiều phòng chat
        public ICollection<ThanhVienPhong> DanhSachPhongThamGia { get; set; }

        // Quan hệ 1-N: Một người dùng có thể gửi nhiều tin nhắn
        public ICollection<TinNhan> DanhSachTinNhan { get; set; }
    }
}