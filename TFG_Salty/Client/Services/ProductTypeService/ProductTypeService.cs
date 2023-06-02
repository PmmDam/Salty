using System.Net.Http.Json;

namespace TFG_Salty.Client.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly HttpClient _http;

        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>();

        public event Action OnChange;


        public ProductTypeService(HttpClient http)
        {
            _http = http;
        }

        public async Task GetProductTypes()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<ProductType>>>("api/producttype");
            ProductTypes = response.Data;
        }

        public async Task AddProductTypes(ProductType productType)
        {

            var response = await _http.PostAsJsonAsync("api/producttype", productType);
            ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
            OnChange.Invoke();
        }

        public async Task UpdateProductTypes(ProductType productType)
        {
            var response = await _http.PutAsJsonAsync("api/producttype", productType);
            ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
            OnChange.Invoke();
        }

        public ProductType CreateNewProductType()
        {
            var productType = new ProductType
            {
                IsNew = true,
                Editing = true,
            };
            ProductTypes.Add(productType);
            OnChange.Invoke();
            return productType;
        }
    }
}
