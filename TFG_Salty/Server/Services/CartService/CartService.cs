using TFG_Salty.Server.Migrations;
using TFG_Salty.Shared;
using System.Security.Claims;

namespace TFG_Salty.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;

        
        public CartService(DataContext context,IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<ServiceResponse<List<CartProductResponseDTO>>> GetCartProductsAsync(List<CartItem> cartItems)
        {
            //Valor de retorno.
            var result = new ServiceResponse<List<CartProductResponseDTO>>
            {
                Data = new List<CartProductResponseDTO>()
            };

            //Recorremos la lista de cartItems 
            foreach(var item in cartItems)
            {
                //Recuperamos de la base de datos el item sobre el que estamos iterando
                var product = await _context.Products.Where(p=>p.Id == item.ProductId).FirstOrDefaultAsync();

                //Si existe el item, comprbamos y sus variants
                if(product != null)
                {
                    var productVariant = await _context.ProductVariants
                        .Where(v => v.ProductId == item.ProductId && v.ProductTypeId == item.ProductTypeId)
                        .Include(v=> v.ProductType)
                        .FirstOrDefaultAsync();


                    if (productVariant != null)
                    {
                        var cartProduct = new CartProductResponseDTO
                        {
                            ProductId = product.Id,
                            Title = product.Title,
                            ImageUrl = product.ImageUrl,
                            Price = productVariant.Price,
                            ProductType = productVariant.ProductType.Name,
                            ProductTypeId = productVariant.ProductTypeId,
                            Quantity = item.Quantity
                        };
                        result.Data.Add(cartProduct);
                    }
               
                }
                
            }
            return result;
        }

        public async Task<ServiceResponse<List<CartProductResponseDTO>>> StoreCartItemsAsync(List<CartItem> cartItems)
        {
            cartItems.ForEach(cartItem => cartItem.UserId = _authService.GetUserId());
            _context.CartItems.AddRange(cartItems);
            await _context.SaveChangesAsync();

            return await GetDbCartProductsAsync();
        }

        public async Task<ServiceResponse<int>> GetCartItemsCountAsync()
        {
           var count = (await _context.CartItems.Where(ci=> ci.UserId == _authService.GetUserId()).ToListAsync()).Count;
            return new ServiceResponse<int> { Data = count};
        }

        public async Task<ServiceResponse<List<CartProductResponseDTO>>> GetDbCartProductsAsync(int? userId = null)
        {
            if(userId == null)
            {
                userId = _authService.GetUserId();
            }
            return await GetCartProductsAsync(await _context.CartItems.Where(ci => ci.UserId == userId).ToListAsync());
        }

        public async Task<ServiceResponse<bool>> AddToCartAsync(CartItem cartItem)
        {
            cartItem.UserId= _authService.GetUserId();
            var sameItem = await _context.CartItems
                .FirstOrDefaultAsync(ci=>ci.ProductId == cartItem.ProductId && ci.ProductTypeId == cartItem.ProductTypeId && ci.UserId == cartItem.UserId);
            if(sameItem == null) { 
                _context.CartItems.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool> {  Data = true };
        }

        private async Task<CartItem?> GetCartItemFromDb(int cartItemProductId,int cartItemProductTypeId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == cartItemProductId && ci.ProductTypeId == cartItemProductTypeId&& ci.UserId == _authService.GetUserId());
        }

        public async Task<ServiceResponse<bool>> UpdateQuantityAsync(CartItem cartItem)
        {
            CartItem? dbCartItem = await GetCartItemFromDb(cartItem.ProductId,cartItem.ProductTypeId);

            if (dbCartItem is null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "El producto de la cesta no existe"
                };
            }
            dbCartItem.Quantity = cartItem.Quantity;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool> { Data = true };
        }

    

        public async Task<ServiceResponse<bool>> RemoveItemFromCartAsync(int productId, int productTypeId)
        {
            CartItem? dbCartItem = await GetCartItemFromDb(productId, productTypeId);

            if (dbCartItem is null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "El producto de la cesta no existe"
                };
            }
            _context.CartItems.Remove(dbCartItem);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> {  Data = true };
        }
    }
}
