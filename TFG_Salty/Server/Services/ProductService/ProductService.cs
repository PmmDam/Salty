using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;

namespace TFG_Salty.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Devuelve una lista con todos los productos y sus variants que sean visibles y no estén en el estado "borrado"
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                .Where(product => product.Visible && !product.Deleted)
                .Include(product => product.Variants
                                    .Where(variant => variant.Visible && !variant.Deleted))
                .Include(p => p.Images)
                .ToListAsync()
            };
            return response;
        }

        /// <summary>
        /// Devuelve un ServiceResponse con un producto en función del ID
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();
            Product product = null;

            //Comprobamos si el usuario está autenticado como Admin para devolver todos los productos que no estén borrados
            if (_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                product = await _context.Products
               .Include(p => p.Variants.Where(variant => !variant.Deleted))
               .ThenInclude(v => v.ProductType)
               .Include(p=>p.Images)
               .FirstOrDefaultAsync(product => product.Id == productId && !product.Deleted);

            }
            else //Si no, devuelve los productos que son visible y no están borrados
            {
                product = await _context.Products
               .Include(p => p.Variants.Where(variant => variant.Visible && !variant.Deleted))
               .ThenInclude(v => v.ProductType)
               .Include(p => p.Images)
               .FirstOrDefaultAsync(product => product.Id == productId && !product.Deleted && product.Visible);

            }

            if (product == null)
            {
                response.Success = false;
                response.Message = "Sorry, but this p does not exist.";
            }
            else
            {
                response.Data = product;
            }
            return response;
        }
        /// <summary>
        /// Devuelve un ServiceResponse con una lista de productos en función de su categoría
        /// </summary>
        /// <param name="categoryUrl"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Product>>> GetProductsByCategoryAsync(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                .Where(product => product.Category.Url.ToLower().Equals(categoryUrl.ToLower()) && product.Visible && !product.Deleted)
                .Include(product => product.Variants
                                .Where(variant => variant.Visible && !variant.Deleted))
                 .Include(p => p.Images)
                .ToListAsync()
            };
            return response;
        }

        /// <summary>
        /// Devuelve una lista de productos que contienen el texto de busqueda en el título o en la descripción
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        private async Task<List<Product>> FindProductBySearchTextAsync(string searchText)
        {
            return await _context.Products
                            .Where(product => product.Title.ToLower().Contains(searchText.ToLower()) ||
                            product.Description.ToLower().Contains(searchText.ToLower()) && product.Visible && !product.Deleted)
                            .Include(P => P.Variants.Where(variant => variant.Visible && !variant.Deleted))
                            .ToListAsync();
        }
        /// <summary>
        /// Devuelve un ServiceResponse con una lista de productos en función del parámetro de búsqueda
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<ProductSearchResultDTO>> SearchProductsAsync(string searchText, int page)
        {

            var pageResults = 2f;
            var pageCount = Math.Ceiling((await FindProductBySearchTextAsync(searchText)).Count / pageResults);

            var products = await _context.Products
                            .Where(product => product.Title.ToLower().Contains(searchText.ToLower()) ||
                            product.Description.ToLower().Contains(searchText.ToLower()) && product.Visible && !product.Deleted)
                            .Include(product => product.Variants.Where(variant => variant.Visible && !variant.Deleted))
                            .Include(p => p.Images)
                            .Skip((page - 1) * (int)pageResults)
                            .Take((int)pageResults)
                            .ToListAsync();


            var response = new ServiceResponse<ProductSearchResultDTO>()
            {
                Data = new ProductSearchResultDTO
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pageCount
                }
            };
            return response;
        }

        /// <summary>
        /// Devuelve un ServiceResponse con una lista de titulos de productos como sugerencias en función del parámetro de búsqueda.
        /// No solo sugiere una lista de titulos, tambien comprueba si la descripción contiene las palabras que estás introduciendo y te lo sugiere también.
        /// Es una lógica algo compleja y estaría bien refactorizarlo para dejarlo algo más limpio, pero bueno, de momento funciona
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestionsAsync(string searchText)
        {
            var products = await FindProductBySearchTextAsync(searchText);

            List<string> result = new List<string>();

            foreach (var product in products)
            {
                //Comprobamps si el titulo del producto contiene el searchText para añadirlo a la lista de sugerencias
                if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }
                //Comprobamos si la descripción contiene las palabras que vamos introduciendo para añadirlo como sugerencia
                if (!string.IsNullOrEmpty(product.Description))
                {
                    var punctutation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();

                    var words = product.Description.Split().Select(s => s.Trim(punctutation));

                    foreach (var word in words)
                    {
                        if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }

            return new ServiceResponse<List<string>> { Data = result };
        }

        //Obtiene los productos destacados
        public async Task<ServiceResponse<List<Product>>> GetFeaturedProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _context.Products
                .Where(product => product.Featured && product.Visible && !product.Deleted)
                .Include(product => product.Variants.
                                    Where(variant => variant.Visible && !variant.Deleted))
                .Include(p => p.Images)
                .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetAdminProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _context.Products
                .Where(product => !product.Deleted)
                .Include(product => product.Variants.Where(variant => !variant.Deleted))
                .ThenInclude(variant => variant.ProductType)
                .Include(p => p.Images)
                .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<Product>> CreateProductAsync(Product product)
        {
            foreach(var variant in product.Variants)
            {
                //Si no lo dejamos a null, Entity framework intentará crear de nuevo el variant y podría duplicarnos registros
                variant.ProductType = null;
            }


            product.Category= null;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Product> { Data = product };
        }

        /// <summary>
        /// Actualiza un producto 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Product>> UpdateProductAsync(Product product)
        {
            //Buscamos el producto en la base de datos
            var dbProduct = await _context.Products.Include(p=> p.Images).FirstOrDefaultAsync(p=>p.Id == product.Id);
            if (dbProduct is null)
            {
                return new ServiceResponse<Product>
                {
                    Success = false,
                    Message = "Producto no encontrado"
                };
            }
            //Si lo encontramos, actuallizamos sus propiedades
            dbProduct.Title = product.Title;
            dbProduct.Description = product.Description;
            dbProduct.ImageUrl = product.ImageUrl;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.Visible = product.Visible;
            dbProduct.Featured = product.Featured;
            
            //Borramos todos las imagenes del producto y guardamos las nuevas
            var productImages = dbProduct.Images;
            _context.Images.RemoveRange(productImages);

            //Guardamos en el producto de base de datos las imagenes recibidas como parametro
            dbProduct.Images = product.Images;

            //Y actualizamos tambien cada uno de sus variantes si las tuviera
            foreach (var variant in product.Variants)
            {
                //Recuperamos la variante actual de la base de datos
                var dbVariant = await _context.ProductVariants
                              .SingleOrDefaultAsync(v => v.ProductId == variant.ProductId && v.ProductTypeId == variant.ProductTypeId);

                //Si no existe, la añadimos directamente pero antes poniendo el productType a null para que entity framework no nos duplique registros
                if (dbVariant is null)
                {
                    variant.ProductType = null;
                    _context.ProductVariants.Add(variant);
                }
                //En caso contrario, actualizamos las propiedades de la varienta en la base de datos
                else
                {
                    dbVariant.ProductTypeId = variant.ProductTypeId;
                    dbVariant.Price = variant.Price;
                    dbVariant.OriginalPrice = variant.OriginalPrice;
                    dbVariant.Visible = variant.Visible;
                    dbVariant.Deleted = variant.Deleted;

                }
            }

            //Commiteamos los cambios
            await _context.SaveChangesAsync();

            //Devolvemos una respuesta con el producto actualizado 
            return new ServiceResponse<Product> { Data = dbProduct };
        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(int productId)
        {
            var dbProduct = await _context.Products.FindAsync(productId);
            if (dbProduct is null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Producto no encontrado"
                };
            }
            dbProduct.Deleted = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<bool> {  Data = true};
        }
    }
}
