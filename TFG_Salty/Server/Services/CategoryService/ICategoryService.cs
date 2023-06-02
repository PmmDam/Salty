namespace TFG_Salty.Server.Services.CategoryService
{
    public interface ICategoryService
    {
        /// <summary>
        /// Devuelve todas las categorias visibles de la base de datos
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<Category>>> GetCategoriesAsync();

        /// <summary>
        /// Devuelve todas las categorias, incluso las que no están visibles
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<Category>>> GetAdminCategoriesAsync();
        
        /// <summary>
        /// Añade una categoriía a la db
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Category>>> AddCategoryAsync(Category category);
        /// <summary>
        /// Actualiza una categoría en la db
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Category>>> UpdateCategoryAsync(Category category);

        /// <summary>
        /// Borra una categoría de la db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Category>>> DeleteCategoryAsync(int id);

        /// <summary>
        /// Obtiene una categoría en función de su ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Category>GetCategoryByIdAsync(int id);

    }
}
