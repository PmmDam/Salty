namespace TFG_Salty.Server.Services.AuthService
{
    public interface IAuthService
    {
        /// <summary>
        /// Creamos un usuario en la base de datos. Primero comprobamos si existe y en caso contrario, hasheamos la contraseña para guardarla en la base de datos
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ServiceResponse<int>> RegisterAsync(User user,string password);

        /// <summary>
        /// Comprobamos si el email existe en la plataforma
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> UserExistsAsync(string email);

        /// <summary>
        /// Login del usuario utilizando JsonWebTokens
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> LoginAsync(string email, string password);

        Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, string newPassword);

        int GetUserId();
        string GetUserEmail();
        Task<User> GetUserByEmailAsync(string email);

    }
}
