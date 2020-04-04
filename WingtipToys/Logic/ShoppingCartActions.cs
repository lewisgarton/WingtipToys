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
            // Get product key from database
            ShoppingCartId = GetCartId();

            var cartItem = _db.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == ShoppingCartId &&
                c.ProductId == id);

            // create new cart if none exsists
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
        
        public string GetCartId()
        {
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