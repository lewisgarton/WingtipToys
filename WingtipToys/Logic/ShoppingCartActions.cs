using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        public string ShoppingCartId { get; set; }

        private ProductContext _db = new ProductContext();

        public const string CartSessionKey = "CartId";

        public void AddToCart(int id)
        {
     
            ShoppingCartId = GetCartId();

            // check if item already in cart
            var cartItem = _db.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == ShoppingCartId &&
                c.ProductId == id);

            // add item to cart or increment
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    CartId = ShoppingCartId,
                    Quantity = 1,
                    DateCreated = DateTime.Now,
                    ProductId = id,
                    Product = _db.Products.SingleOrDefault(
                        p => p.ProductID == id)
                };

                _db.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }
            _db.SaveChanges();
        }
        
        public void RemoveFromCart(int id)
        {
            ShoppingCartId = GetCartId();

            // check if item is in the cart
            var cartItem = _db.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == ShoppingCartId &&
                c.ProductId == id);
            
            // remove item from cart if quantity is 1, else decrement quantity
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    _db.ShoppingCartItems.Remove(cartItem);
                }
            }
            _db.SaveChanges();
        }

        public string GetCartId()
        {
            // check for cart id in current session, if none create one
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return HttpContext.Current.Session[CartSessionKey].ToString();
        }

        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }

        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = GetCartId();
            return _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
        }
    }

}