using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace BtlWebNangCao.ViewModels
{
    public class PhongChatViewModel
    {
        public int Pk_iMaPhong { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề phòng")]
        [Display(Name = "Tiêu đề")]
        public string sTieuDe { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả phòng")]
        [Display(Name = "Mô tả")]
        public string sMoTa { get; set; }

        [Display(Name = "Công khai")]
        public bool bLaCongKhai { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime dNgayTao { get; set; }

        [Display(Name = "Người tạo")]
        public string TenNguoiTao { get; set; }
    }
}
