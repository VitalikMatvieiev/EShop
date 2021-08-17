using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).ToList();
            }
            List<int> prodInCart = shoppingCartsList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));
            return View(prodList);
        }
        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).ToList();
            }

            shoppingCartsList.Remove(shoppingCartsList.FirstOrDefault(p => p.ProductId == id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCartsList);

            List<int> prodInCart = shoppingCartsList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));
            return View(nameof(Index),prodList);
        }
    }
}
