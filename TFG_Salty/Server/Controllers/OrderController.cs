using Microsoft.AspNetCore.Mvc;

namespace TFG_Salty.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

     

        //Método Get por defecto
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<OrderOverviewResponseDTO>>>> GetOrders()
        {
            var result = await _orderService.GetOrdersAsync();
            return Ok(result);
        }
        [HttpGet("{orderId}")]
        public async Task<ActionResult<ServiceResponse<OrderDetailsResponseDTO>>> GetOrdersDetails(int orderId)
        {
            var result = await _orderService.GetOrderDetailsAsync(orderId);
            return Ok(result);
        }

    }
}
