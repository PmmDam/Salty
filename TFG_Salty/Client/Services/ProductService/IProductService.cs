namespace TFG_Salty.Client.Services.ProductService
{
    public interface IProductService
    {
        /// <summary>
        /// Evento que actualizará el renderizado de los productos cuando se haga una petición
        /// </summary>
        event Action ProductsChanged;

        /// <summary>
        /// Lista con los productos que puede ver un usuario normal
        /// </summary>
        List<Product> Products { get; set; }
        /// <summary>
        /// Lista con todos los productos
        /// </summary>
        List<Product> AdminProducts { get; set; }

        /// <summary>
        /// Obtiene todos los productos en función a la categoria. 
        /// Por defecto la categoria es un null así que si no se le especifica, 
        /// devolverá todos los productos
        /// </summary>
        /// <param name="categoryUrl"></param>
        /// <returns></returns>
        Task GetProductsAsync(string? categoryUrl = null);

        /// <summary>
        /// Obtiene un producto en función del ID
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<ServiceResponse<Product>> GetProductAsync(int productId);

        /// <summary>
        /// Centralizamos los mensajes que le podemos dar al usuario en funcion de las casuisticas con el producto.
        /// Producto no encontrado, Etc...
        /// </summary>
        string Message { get; set; }


        /// <summary>
        /// Almacenamos la página actual en la que estamos
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        /// El total de páginas
        /// </summary>
        int PageCount { get; set; }

        /// <summary>
        /// Tenemos que guardar cual ha sido la busqueda para que al paginar no perdamos esa información
        /// </summary>
        string LastSearchText { get; set; }

        /// <summary>
        /// Hace una petición de la lista de productos filtrada por el parámetro de busqueda
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        Task SearchProductsAsync(string searchText, int page);

        /// <summary>
        /// Hace una petición de la lista de sugerencias en función del parámetro de busqueda
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        Task<List<string>> GetProductSearchSuggestionsAsync(string searchText);

        /// <summary>
        /// añade a la lista AdminProducts todos los productos de la base de datos
        /// </summary>
        /// <returns></returns>
        Task GetAdminProducts();
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
    }
}
