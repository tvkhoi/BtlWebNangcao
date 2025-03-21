using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BtlWebNangCao.Models
{
    public class ThanhVienPhong
    {
        [Key, Column(Order = 0)]
        public int MaNguoiDung { get; set; }

        [ForeignKey("MaNguoiDung")]
        public NguoiDung NguoiDung { get; set; }

        [Key, Column(Order = 1)]
        public int MaPhong { get; set; }

        [ForeignKey("MaPhong")]
        public PhongChat PhongChat { get; set; }

        public DateTime NgayThamGia { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string VaiTroPhong { get; set; } // Vai trò trong phòng: "Thành viên", "Quản trị viên"
    }
}
