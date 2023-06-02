namespace TFG_Salty.Client.Services.OrderService
{
    public interface IOrderService
    {
        /// <summary>
        /// Realiza el pedido
        /// </summary>
        /// <returns></returns>
        Task<string> PlaceOrder();

        /// <summary>
        /// Obitene todos los pedidos 
        /// </summary>
        /// <returns></returns>
        Task<List<OrderOverviewResponseDTO>> GetOrders();

        /// <summary>
        /// Obtiene todos los detalles de los productos de un pedido
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<OrderDetailsResponseDTO> GetOrderDetails(int orderId);
    }
}
