using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace TFG_Salty.Client.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navManager;

        public OrderService(HttpClient http,AuthenticationStateProvider authStateProvider,NavigationManager navManager)
        {
            _http = http;
            _authStateProvider = authStateProvider;
            _navManager = navManager;
        }

        public async Task<OrderDetailsResponseDTO> GetOrderDetails(int orderId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<OrderDetailsResponseDTO>>($"api/order/{orderId}");
            return result.Data;
        }

        public async Task<List<OrderOverviewResponseDTO>> GetOrders()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<OrderOverviewResponseDTO>>>("api/order");
            return result.Data;
        }

        public async Task<string> PlaceOrder()
        {
            if(await IsUserAuthenticatedAsync())
            {
                var result = await _http.PostAsync("api/payment/checkout", null);
                var url = await result.Content.ReadAsStringAsync();
                return url;
            }
            else
            {
               return "login";
            }
        }
        private async Task<bool> IsUserAuthenticatedAsync()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

    }
}
