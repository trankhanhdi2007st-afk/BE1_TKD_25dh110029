using BE1_TKD_25dh110029.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE1_TKD_25dh110029.Models.ViewModel
{
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; } = 6;

        // Danh sách sản phẩm nổi bật
        public List<Product> Products { get; set; }

        // Danh sách sản phẩm mới có phân trang
        public IPagedList<Product> NewProducts { get; set; }
    }
}
