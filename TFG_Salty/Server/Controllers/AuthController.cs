using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TFG_Salty.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegister request)
        {
            // Aquí tengo que tomar una decisión. Debido a como implemento el Servico de autenticación tengo que crear un UserModel en un estado no válido es decir,
            // que solo tenga un email. He leído varias cosas al respecto y no parece haber un consenso claro. Por un lado es una mala práctica pero por otro para determinadas cosas
            // relacionadas con el funcionamiento de los ORMs es necesario que los modelos tengan un constructor por defecto al que no se le pasan parámetros. Diría que me falta todavía 
            // experiencia para determinar cual es la decisión más óptima aunque, como diría Paco, Depende.
            var response = await _authService.RegisterAsync(new User { Email = request.Email }, request.Password);

            //Clausual Guarda
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLogin request)
        {
            var response = await _authService.LoginAsync(request.Email, request.Password);

            //Clausual Guarda
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        //La etiqueta Auhotirize obliga al usuario a estar autorizado para navegar a esta url
        [HttpPost("change-password"),Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword([FromBody] string password)
        {
            //Obtenemos el user id de los Claims gracias a JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _authService.ChangePasswordAsync(int.Parse(userId), password);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
               return Ok(response);
            }
            
        }

    }
}
