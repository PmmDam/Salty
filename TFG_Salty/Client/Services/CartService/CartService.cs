using TFG_Salty.Shared;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace TFG_Salty.Client.Services.CartService
{
    public class CartService : ICartService
    {
        //Referencia al localStorage del navegador donde almacenamos temporalmente el carrito en caso de que el usuario no esté logueado
        private readonly ILocalStorageService _localStorage;

        private readonly HttpClient _http;
        
        private readonly IAuthService _authService;
        
        public event Action OnChange;

        public CartService(ILocalStorageService localStorage, HttpClient http,IAuthService authService)
        {
            _localStorage = localStorage;
            _http = http;
            _authService = authService;
            
        }

        /// <summary>
        /// Devuelve una lista de CartItems que se almacenan utilizando las funcionalidades del paquete NuGet Blarozed.LocalStorage
        /// para almacenar el carrito de la compra sin tener que estar logueado de manera local con blazor
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<List<CartItem>> InitializeLocalStorageAsync(string key)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>(key);
            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            return cart;
        }

        /// <summary>
        /// Añade un producto al carrito, si el usuario está autenticado lo cogerá de la base de datos, si no, del LocalStorage
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        public async Task AddToCart(CartItem cartItem)
        {
            if (await _authService.IsUserAuthenticated())
            {
                await _http.PostAsJsonAsync("api/cart/add", cartItem);
            }
            else
            {
                List<CartItem> cart = await InitializeLocalStorageAsync("cart");
                if (cart == null)
                {
                    cart = new List<CartItem> { };
                }

                //Comprobamos si el item que queremos añadir está ya en la cesta
                var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);

                //Si sameItem no está en la cesta, será igual a null
                if (sameItem == null)
                {
                    cart.Add(cartItem);
                }
                //Si está en la cesta, incrementamos la cantidad
                else
                {
                    sameItem.Quantity += cartItem.Quantity;
                }

                await _localStorage.SetItemAsync("cart", cart);
            }
          
            await GetCartItemsCount();
        }

      
      

        public async Task<List<CartProductResponseDTO>> GetCartProducts()
        {
            if(await _authService.IsUserAuthenticated())
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<CartProductResponseDTO>>>("api/cart");
                return response.Data;
            }
            else
            {
                var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if(cartItems == null)
                {
                    return new List<CartProductResponseDTO>();
                }
                var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);
                var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponseDTO>>>().ConfigureAwait(false);

                return cartProducts.Data;
            }
            

        }


        /// <summary>
        /// Comprobamos si el usuario está autenticado para borrarlo de base de datos y si no, lo borramos del local storage
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            if(await _authService.IsUserAuthenticated())
            {
                await _http.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart != null)
                {
                    var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);

                    if (cartItem != null)
                    {
                        cart.Remove(cartItem);
                        await _localStorage.SetItemAsync("cart", cart);

                    }
                }
            }
   

        }


        public async Task UpdateQuantity(CartProductResponseDTO product)
        {
            //Si el usuario está autenticado, recuperamos el objeto de la base de datos
            if(await _authService.IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };
                await _http.PutAsJsonAsync("api/cart/update-quantity", request).ConfigureAwait(false);
            }
            else //Si no, obtenemos 
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart != null)
                {
                    var cartItem = cart.Find(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);

                    if (cartItem != null)
                    {
                        cartItem.Quantity = product.Quantity;
                        await _localStorage.SetItemAsync("cart", cart);

                    }
                }
            }
         
        }

        public async Task StoreCartItems(bool emptyLocalCart)
        {
           var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            
            //Clausula guarda para evitar niveles de indentación y que el código quede lo más claro posible
            if(localCart == null)
            {
                return;
            }

            await _http.PostAsJsonAsync("api/cart", localCart);

            if (emptyLocalCart)
            {
                await _localStorage.RemoveItemAsync("cart");
            }

        }
        /// <summary>
        /// Método que comprueba si el usuario está autenticado
        /// </summary>
        /// <returns></returns>
     

        public async Task GetCartItemsCount()
        {
            if(await _authService.IsUserAuthenticated())
            {
                var result = await _http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;
                await _localStorage.SetItemAsync<int>("cartItemsCount", count);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                await _localStorage.SetItemAsync<int>("cartItemsCount",cart != null? cart.Count : 0);
            }
            OnChange.Invoke();
        }
    }
}
