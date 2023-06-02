using System.Net.Http.Json;

namespace TFG_Salty.Client.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly HttpClient _http;

        public AddressService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<Address> AddOrUpdate(Address address)
        {
            var response = await _http.PostAsJsonAsync("api/address", address);
            return response.Content.ReadFromJsonAsync<ServiceResponse<Address>>().Result.Data;
        }

        public async Task<Address> GetAddress()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<Address>>("api/address");
            return response.Data;
        }
    }
}
