using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BE1_TKD_25dh110029.Models.ViewModel
{
    public class ProfileInfoVM
    {
        public string Username { get; set; }

        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }

        [StringLength(50, MinimumLength = 6,
            ErrorMessage = "Mật khẩu mới phải từ 6 đến 50 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [Compare("NewPassword",
            ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string ConfirmNewPassword { get; set; }
    }
}
