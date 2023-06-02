using Microsoft.AspNetCore.Mvc;

namespace TFG_Salty.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponseDTO>>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = await _cartService.GetCartProductsAsync(cartItems);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponseDTO>>>> StoreProducts(List<CartItem> cartItems)
        {
            var result = await _cartService.StoreCartItemsAsync(cartItems);
            return Ok(result);
        }
        [HttpGet("count")]
        public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCount()
        {
            return await _cartService.GetCartItemsCountAsync();
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponseDTO>>>> GetDbCarTProducts()
        {
            var result = await _cartService.GetDbCartProductsAsync();
            return Ok(result);
        }
        [HttpPost("add")]
        public async Task<ActionResult<ServiceResponse<bool>>> AddToCart(CartItem cartItem)
        {
            var result = await _cartService.AddToCartAsync(cartItem);
            return Ok(result);
        }

        [HttpPut("update-quantity")]
        public async Task<ActionResult<ServiceResponse<bool>>>UpdateQuantity(CartItem cartItem)
        {
            var result = await _cartService.UpdateQuantityAsync(cartItem);
            return Ok(result);
        }

        [HttpDelete("{productId}/{productTypeId}")]
        public async Task<ActionResult<ServiceResponse<bool>>> RemoveItemFromCart(int productId,int productTypeId)
        {
            var result = await _cartService.RemoveItemFromCartAsync(productId,productTypeId);
            return Ok(result);
        }

    }
}
