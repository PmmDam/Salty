namespace TFG_Salty.Client.Services.CategoryService
{
    public interface ICategoryService
    {
        /// <summary>
        /// Evento que invocamos para notificar que ha habido un cambio y poder suscribirnos a el para referescar el front end 
        /// </summary>
        event Action OnChange;

        List<Category> Categories { get; set; }
        List<Category> AdminCategories { get; set; }

        /// <summary>
        /// Hace una request HTTP para obtener las categorías en función del rol
        /// </summary>
        /// <returns></returns>
        Task GetCategories();
        /// <summary>
        /// Hace una request HTTP para obtener todas las categorías 
        /// </summary>
        /// <returns></returns>
        Task GetAdminCategories();
        
        /// <summary>
        /// Mandamos una categoría para que el servidor la añada a la base de datos
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task AddCategory(Category category);

        /// <summary>
        /// Actualizamos una categoría en función de lo modificado por un usuario/a administrador
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task UpdateCategory(Category category);

        /// <summary>
        /// Borramos una categoría
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task DeleteCategory(int categoryId);

        /// <summary>
        /// Creamos una categoría
        /// </summary>
        /// <returns></returns>
        Category CreateNewCategory();

       
    }
}
