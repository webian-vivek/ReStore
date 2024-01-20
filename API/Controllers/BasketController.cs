using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;

        public BasketController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            Basket basket = await RetrieveBasket(GetBuyerId());
            if (basket is null) return NotFound();
            return basket.MapBasketToDto();
        }

        
        [HttpPost] //api/basket?productId=3&quantity=4
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            //get basket || if not then create basket
            var basket = await RetrieveBasket(GetBuyerId());
            if (basket == null) basket = CreateBasket();
            //get product
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return BadRequest(new ProblemDetails{Title ="Product Not Found"});
            //add item
            basket.AddItem(product, quantity);
            //save changes
            var result = await _context.SaveChangesAsync() > 0;
            //if (result) return StatusCode(201);
            if (result) return CreatedAtRoute("GetBasket",basket.MapBasketToDto());
            return BadRequest(new ProblemDetails { Title = "Problem saving item to basket." });
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveItemToBasket(int productId, int quantity)
        {
            //get basket
            var basket = await RetrieveBasket(GetBuyerId());
            if (basket is null) return NotFound();

            //remove item or reduce quantity
            basket.RemovedItem(productId, quantity);
            //save changes
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok();
            return BadRequest(new ProblemDetails { Title = "Problem to removing item from basket." });
        }

        [HttpGet]
        private async Task<Basket> RetrieveBasket(string buyerId)
        {
            if(string.IsNullOrEmpty(buyerId)){
                Response.Cookies.Delete("buyerId");
                return null;
            }
            return await _context.Baskets.Include(itm => itm.Items)
                                .ThenInclude(p => p.Product)
                                .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
        }

        private string GetBuyerId()
        {
            return User.Identity?.Name ?? Request.Cookies["buyerId"];
        }

        [HttpPost]
        private Basket CreateBasket()
        {
            //var buyerId = Guid.NewGuid().ToString();
            var buyerId = User.Identity?.Name;
            if(string.IsNullOrEmpty(buyerId))
            {
                buyerId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
                Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            }
            var basket = new Basket { BuyerId = buyerId };
            _context.Baskets.Add(basket);
            return basket;
        }

    }
}