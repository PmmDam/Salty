using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace TFG_Salty.Client
{
    /// <summary>
    /// Obtiene el token generado y almacenado en el LocalStorage para poder comprobar si el usuario está autenticado o no
    /// </summary>
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        //Refrencia al local storage del navegador
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _http;

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorageService = localStorage;
            _http = http;
        }
 
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //Obtenemos el token de autenticación del local storage
            string authToken = await _localStorageService.GetItemAsStringAsync("authToken");

            //representa una identidad en funcion de las notificaciones(claims)
            var identity = new ClaimsIdentity();
            _http.DefaultRequestHeaders.Authorization = null;

            //Si tenemos un token de autenticación en local storage lo parseamos a base64 y lo asignamos con el nombre Bearer.
            //Si algo va mal en este proceso, cuando se cree el estado, el usuario no estará autenticadp
            if(!string.IsNullOrEmpty(authToken) )
            {
                try
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
                }
                catch
                {
                    await _localStorageService.RemoveItemAsync(authToken);
                    identity = new ClaimsIdentity();
                }

            }

            var user = new ClaimsPrincipal(identity);

            var state = new AuthenticationState(user);
            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }


        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch(base64.Length % 4)
            {
                case 2:base64 += "==";break;
                case 3:base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string authToken)
        {
            var payload = authToken.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs= JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key,kvp.Value.ToString()));

            return claims;
        }
    }
}
