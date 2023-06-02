namespace TFG_Salty.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;

        public CategoryService(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Añadimos una categoría a su repositorio correspondiente
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Category>>> AddCategoryAsync(Category category)
        {
            //Actualizamos las flags de esta categoría para no tener problemas en el front
            category.Editing = false;
            category.IsNew = false;

            //Añadimos la categoría al repository
            _context.Categories.Add(category);

            //Commiteamos los cambios a la sb
            await _context.SaveChangesAsync();

            //Devolvemos un service response con todas las categorías
            return await GetAdminCategoriesAsync();
        }


        /// <summary>
        /// Borramos una categoría en función del ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ServiceResponse<List<Category>>> DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryByIdAsync(id);

            //Clausula guarda
            if (category == null)
            {
                return CategoryNotFoundResponse();

            }
            category.Deleted = true;
            await _context.SaveChangesAsync();

            //Por ahorrar codigo, esto al final devuelve un service response con todas las categorías
            return await GetAdminCategoriesAsync();
        }

        private static ServiceResponse<List<Category>> CategoryNotFoundResponse()
        {
            return new ServiceResponse<List<Category>>
            {
                Success = false,
                Message = "Categoría no encontrada"
            };
        }

        /// <summary>
        /// Devuelve una categoría en función de su ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(category => category.Id == id);
        }

        public async Task<ServiceResponse<List<Category>>> GetAdminCategoriesAsync()
        {
            var categories = await _context.Categories.Where(category => !category.Deleted).ToListAsync();
            return new ServiceResponse<List<Category>>()
            {
                Data = categories
            };
        }

        public async Task<ServiceResponse<List<Category>>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.Where(category => !category.Deleted && category.Visible).ToListAsync();
            return new ServiceResponse<List<Category>>()
            {
                Data = categories
            };
        }

        public async Task<ServiceResponse<List<Category>>> UpdateCategoryAsync(Category category)
        {
            var dbCategory = await GetCategoryByIdAsync(category.Id);
            if(category == null)
            {
                return CategoryNotFoundResponse();
            }

            dbCategory.Name = category.Name;
            dbCategory.Url= category.Url;
            dbCategory.Visible = category.Visible;

            await _context.SaveChangesAsync();

            return await GetAdminCategoriesAsync();
        }
    }
}
