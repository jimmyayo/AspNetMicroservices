using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
   [Route("api/v1/[controller]")]
   [ApiController]
   public class BasketController : ControllerBase
   {
      private readonly IBasketRepository basketRepository;
      private readonly DiscountGrpcService _discountGrpcService;

      public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService)
      {
         this.basketRepository = basketRepository;
         _discountGrpcService = discountGrpcService;
      }

      [HttpGet("{userName}", Name ="GetBasket")]
      [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
      public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
      {
         var basket = await basketRepository.GetBasket(userName);
         return Ok(basket ?? new ShoppingCart(userName));
      }

      [HttpPost]
      [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
      public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
      {
         // consume Discount.Grpc service
         // get most recent prices of basket items & update basket 
         foreach (var item in basket.Items)
         {
            var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
            item.Price -= coupon.Amount;
         }

         return Ok(await basketRepository.UpdateBasket(basket));
      }

      [HttpDelete("{userName}", Name ="DeleteBasket")]
      [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]

      public async Task<IActionResult> DeleteBasket(string userName)
      {
         await basketRepository.DeleteBasket(userName);
         return Ok();
      }
   }
}
