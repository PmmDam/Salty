namespace TFG_Salty.Server.Services.ProductService
{
    public interface IProductService
    {
        /// <summary>
        /// Devuelve una lista de productos con todos los productos de la BD
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<Product>>> GetProductsAsync();

        /// <summary>
        /// Obtiene un producto en función de su ID
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<ServiceResponse<Product>> GetProductAsync(int productId);

        /// <summary>
        /// Obtiene todos los productos que pertenencen a una categoría
        /// </summary>
        /// <param name="categoryUrl"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Product>>> GetProductsByCategoryAsync(string categoryUrl);

        /// <summary>
        /// Busca productos que contienen en el título o en la descripción el texto de búsqueda que se pasa como parámetro
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        Task<ServiceResponse<ProductSearchResultDTO>> SearchProductsAsync(string searchText, int page);

        Task<ServiceResponse<List<Product>>> SearchProductsAsync(string searchText);


        /// <summary>
        /// Devuelve una lista de productos sugeridos/recomendados en función del parámetro de búsqueda
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<string>>> GetProductSearchSuggestionsAsync(string searchText);


        /// <summary>
        /// Devuelve una Response con los productos destacados
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<Product>>> GetFeaturedProductsAsync();
        /// <summary>
        /// Devuelve un service response con todos los productos
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<Product>>> GetAdminProductsAsync();

        Task<ServiceResponse<Product>> CreateProductAsync(Product product);
        Task<ServiceResponse<Product>> UpdateProductAsync(Product product);
        Task<ServiceResponse<bool>> DeleteProductAsync(int productId);

    }
}