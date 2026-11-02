using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BE1_TKD_25dh110029.Models.ViewModel
{
    public class CartService
    {
        private const string CartSessionKey = "ShoppingCart";

        private HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        public Cart GetCart()
        {
            var cart = Session[CartSessionKey] as Cart;

            if (cart == null)
            {
                cart = new Cart();
                Session[CartSessionKey] = cart;
            }

            return cart;
        }

        public void SaveCart(Cart cart)
        {
            Session[CartSessionKey] = cart;
        }

        public void ClearCart()
        {
            var cart = GetCart();
            cart.Clear();
            SaveCart(cart);
        }
    }
}
