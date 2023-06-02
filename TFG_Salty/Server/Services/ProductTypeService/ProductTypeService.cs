namespace TFG_Salty.Server.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly DataContext _context;

        public ProductTypeService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<ProductType>>> AddProductTypeAsync(ProductType productType)
        {
            productType.Editing = false;
            productType.IsNew= false;


            //Guardamos en el repository correspondiente el tipo de producto
            _context.ProductTypes.Add(productType);

            //Commiteamos los cambios
            await _context.SaveChangesAsync();

            //Devolvemos un service response con todos los tipos de productos
            return await GetProductTypesAsync();
        }

        public async Task<ServiceResponse<List<ProductType>>> GetProductTypesAsync()
        {
            //Obtenemos todos los tipos de producto de la base de datos
            var productTypes = await _context.ProductTypes.ToListAsync();

            //Devolvemos un ServiceResponse con los tipos de productos 
            return new ServiceResponse<List<ProductType>> { Data = productTypes };
        }

        public async Task<ServiceResponse<List<ProductType>>> UpdateProductTypeAsync(ProductType productType)
        {
            //Obtenemos el tipo de producto de la base de datos
            var dbProductType = await _context.ProductTypes.FindAsync(productType.Id);

            //Si no existe, devolvemos una respuesta indicando el error
            if(dbProductType == null)
            {
                return new ServiceResponse<List<ProductType>>
                {
                    Success = false,
                    Message = "El tipo de producto no ha sido encontrado :("

                };
            }
            //Si extiste, Cambiamos el nombre
            dbProductType.Name = productType.Name;

            //Commiteamos los cabmios
            await _context.SaveChangesAsync();

            //Devolvemos una respuesta válida con todos los tipos de producto
            return await GetProductTypesAsync();
        }
    }
}
