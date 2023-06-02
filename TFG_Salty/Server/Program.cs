//Persistence
global using TFG_Salty.Shared;
global using Microsoft.EntityFrameworkCore;
global using TFG_Salty.Server.Data;

//Services 
global using TFG_Salty.Server.Services.ProductService;
global using TFG_Salty.Server.Services.CategoryService;
global using TFG_Salty.Server.Services.AuthService;
global using TFG_Salty.Server.Services.CartService;
global using TFG_Salty.Server.Services.OrderService;
global using TFG_Salty.Server.Services.PaymentService;
global using TFG_Salty.Server.Services.AddressService;
global using TFG_Salty.Server.Services.ProductTypeService;

//Default
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//Añadimos las dependencias de Swagger para poder añadir una interfaz de usuario a nuestras APIs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Inyectamos las dependencias de los servicios creados en el proyecto
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();

//Con este servicio podremos extraer y validar el JSON Web Token para
//que el usuario pueda realizar operaciones como el cambio de contraseña unicamente si está autenticado y autorizado
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddHttpContextAccessor();


var app = builder.Build();


//Le decimos a la aplicación que utilice la UI de Swagger
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Le decimos a la aplicación que utilice Swagger
app.UseSwagger();


app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();