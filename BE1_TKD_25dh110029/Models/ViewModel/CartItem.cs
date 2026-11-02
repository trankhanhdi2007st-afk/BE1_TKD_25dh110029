using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE1_TKD_25dh110029.Models.ViewModel
{
    public class CartItem
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductImage { get; set; }

        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }

        // Dùng để nhóm sản phẩm theo danh mục
        public string Category { get; set; }

        // Đánh dấu sản phẩm được chọn mua
        public bool IsSelected { get; set; }

        public decimal Total
        {
            get { return ProductPrice * Quantity; }
        }
    }
}