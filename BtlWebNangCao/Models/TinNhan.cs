using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BtlWebNangCao.Models
{
    public class TinNhan
    {
        [Key]
        public int MaTinNhan { get; set; }

        [Required]
        public int MaPhong { get; set; }
        [ForeignKey("MaPhong")]
        public PhongChat PhongChat { get; set; }

        [Required]
        public string MaNguoiGui { get; set; }
        [ForeignKey("MaNguoiGui")]
        public ApplicationUser NguoiGui { get; set; }

        [Required]
        public string NoiDung { get; set; }

        public DateTime NgayGui { get; set; } = DateTime.Now;
    }
}
