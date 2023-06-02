namespace TFG_Salty.Server.Services.CartService
{
    public interface ICartService
    {
        /// <summary>
        /// Devuelve una ServiceResponse de todos los productos que están en el carrito
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<CartProductResponseDTO>>> GetCartProductsAsync(List<CartItem> cartItems);

        /// <summary>
        /// Guarda los productos de la cesta en la base de datos 
        /// </summary>
        /// <param name="cartItems"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<CartProductResponseDTO>>> StoreCartItemsAsync(List<CartItem> cartItems);

        /// <summary>
        /// Recuperamos de la base de datos el total de items en la cesta de la compra para un usuario
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<int>> GetCartItemsCountAsync();

        /// <summary>
        /// Obtiene los productos de la cesta de la base de datos en función del id del usuario. Por defecto su valor es nullo 
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<CartProductResponseDTO>>> GetDbCartProductsAsync(int? userId =null);

        /// <summary>
        /// Añade los productos de la cesta a la base de datos
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddToCartAsync(CartItem cartItem);
        
        /// <summary>
        /// Actualiza la cantidad del item
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateQuantityAsync(CartItem cartItem);

        /// <summary>
        ///  Borra un item de la cesta
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RemoveItemFromCartAsync(int productId,int productTypeId);
    }
}
