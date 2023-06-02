using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace TFG_Salty.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
        {
            var result = await _productService.GetProductsAsync();

            return Ok(result);
        }


        [HttpGet("{productId}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int productId)
        {
            var result = await _productService.GetProductAsync(productId);
            return Ok(result);
        }

        [HttpGet("category/{categoryUrl}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProdyuctsByCategory(string categoryUrl)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryUrl);
            return Ok(result);
        }


        [HttpGet("search/{searchText}/{page}")]
        public async Task<ActionResult<ServiceResponse<ProductSearchResultDTO>>> SearchProduct(string searchText,int page = 1)
        {
            var result = await _productService.SearchProductsAsync(searchText,page);
            return Ok(result);
        }
        [HttpGet("searchsuggestions/{searchText}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductSearchSuggestions(string searchText)
        {
            var result = await _productService.GetProductSearchSuggestionsAsync(searchText);
            return Ok(result);
        }

        [HttpGet("featured")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetFeaturedProduct()
        {
            var result = await _productService.GetFeaturedProductsAsync();
            return Ok(result);
        }
        [HttpGet("admin"),Authorize(Roles ="Admin")]
        public async Task<ActionResult<ServiceResponse<ProductSearchResultDTO>>> GetAdminProducts()
        {
            var result = await _productService.GetAdminProductsAsync();
            return Ok(result);
        }
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<ProductSearchResultDTO>>> CreateProduct(Product product)
        {
            var result = await _productService.CreateProductAsync(product);
            return Ok(result);
        }
        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<ProductSearchResultDTO>>> UpdateProduct(Product product)
        {
            var result = await _productService.UpdateProductAsync(product);
            return Ok(result);
        }
        [HttpDelete("{productId}"),Authorize(Roles ="Admin")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteProduct(int productId)        {
            var result = await _productService.DeleteProductAsync(productId);
            return Ok(result);
        }
    }
}
