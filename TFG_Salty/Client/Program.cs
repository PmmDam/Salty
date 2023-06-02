//Proyecto compartido (modelos)
global using TFG_Salty.Shared;

//Servicios
global using TFG_Salty.Client.Services.CategoryService;
global using TFG_Salty.Client.Services.ProductService;
global using TFG_Salty.Client.Services.CartService;
global using TFG_Salty.Client.Services.AuthService;
global using TFG_Salty.Client.Services.OrderService;
global using TFG_Salty.Client.Services.AddressService;
global using TFG_Salty.Client.Services.ProductTypeService;

//Autenticación y autorización
global using Microsoft.AspNetCore.Components.Authorization;

//Por defecto
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TFG_Salty.Client;
using Blazored.LocalStorage;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


//registramos las implementaciones concretas a las interfaces correspondientes en el contenedor de dependencias
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();

//Configuracion de autenticación
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();