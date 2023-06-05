using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;

namespace TFG_Salty.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        //Representa a la sesion que tenemos abierta con la base de datos
        private readonly DataContext _context;

        //Referencia a la configuración de la aplicación guardada en el appsettings.json
        private readonly IConfiguration _configuration;

        private readonly IHttpContextAccessor _httpContextAccessor;

        //Constructor al que se le pasan por inyección de dependencias las propiedades anteriores
        public AuthService(DataContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<string>> LoginAsync(string email, string password)
        {
            var response = new ServiceResponse<string>();
            var user  = await _context.Users.Include(user => user.Role).FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));

            if(user == null)
            {
                response.Success = false;
                response.Message = "Usuario no encontrado.";
            }

            else if(!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt)) 
            {
                response.Success = false;
                response.Message = "Contraseña incorrecta.";
            }
            else
            {
                response.Data = CreateToken(user);
            }
            
            
            return response;
        }

        public async Task<ServiceResponse<int>> RegisterAsync(User user, string password)
        {
            //Clausula guarda
            if (await UserExistsAsync(user.Email))
            {
                return new ServiceResponse<int> { Success = false, Message = "El email está en uso" };
            }

            //Hasheamos la contraseña
            CreatePasswordHash(password,out byte[] passwordHash,out byte[] salt); 
            
            //asignamos el salt y el hash al usuario
            user.PasswordSalt = salt;
            user.PasswordHash = passwordHash;

            //Lo añadimos al contexto/sesion de la base de datos
            _context.Users.Add(user);

            //Actualizamos la base de datos
            await _context.SaveChangesAsync();

            //Devolvemos la respuesta con el id del usuario
            return new ServiceResponse<int> { Data = user.Id,Message="Registro completado!" };
        }
        public async Task<bool> UserExistsAsync(string email)
        {
            bool result = false;
            if(await _context.Users.AnyAsync(x => x.Email.ToLower().Equals(email.ToLower())))
            {
                result = true;
            }
            return result;
        }

       private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),

            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims:claims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials:creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true, Message = "La contraseña ha sido cambiada." };
        }

        public int GetUserId()
        {
            var nameIdentifierClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public string GetUserEmail()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }
    }
}
