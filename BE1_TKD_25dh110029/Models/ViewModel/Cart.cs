using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE1_TKD_25dh110029.Models.ViewModel
{
    public class Cart
    {
        public List<CartItem> Items { get; set; }

        public Cart()
        {
            Items = new List<CartItem>();
        }

        // Thêm sản phẩm vào giỏ
        public void AddItem(CartItem item)
        {
            var existingItem = Items
                .FirstOrDefault(x => x.ProductID == item.ProductID);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        // Xóa sản phẩm
        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(
                x => x.ProductID == productId);

            if (item != null)
            {
                Items.Remove(item);
            }
        }

        // Cập nhật số lượng
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(
                x => x.ProductID == productId);

            if (item != null)
            {
                if (quantity <= 0)
                    Items.Remove(item);
                else
                    item.Quantity = quantity;
            }
        }

        // Tổng tiền
        public decimal Total
        {
            get
            {
                return Items.Sum(x => x.Total);
            }
        }

        // Làm trống giỏ
        public void Clear()
        {
            Items.Clear();
        }
    }
}
