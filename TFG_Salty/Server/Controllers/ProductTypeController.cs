using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TFG_Salty.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;

        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> GetProductTypes()
        {
            var response = await _productTypeService.GetProductTypesAsync();
            return Ok(response);
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> AddProductTypes(ProductType productType)
        {
            var response = await _productTypeService.AddProductTypeAsync(productType);
            return Ok(response);
        }
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> UpdateProductTypes(ProductType productType)
        {
            var response = await _productTypeService.UpdateProductTypeAsync(productType);
            return Ok(response);
        }
    }
}
