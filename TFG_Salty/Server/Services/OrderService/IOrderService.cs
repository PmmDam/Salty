namespace TFG_Salty.Server.Services.OrderService
{
    public interface IOrderService
    {
        /// <summary>
        /// Obtiene los productos de la orden almacenados en la db, calculamos el precio total y crea los OrderItems y lo amacena en la base de datos
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<bool>> PlaceOrderAsync(int userId);

        /// <summary>
        /// Obtiene todos los pedidos del usuario
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<OrderOverviewResponseDTO>>> GetOrdersAsync();

        /// <summary>
        /// Obtiene los detalles de los productos de un pedido
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ServiceResponse<OrderDetailsResponseDTO>> GetOrderDetailsAsync(int orderId);
    }
}
