using VAAO_BE.Data;
using VAAO_BE.Repositories;
using VAAO_BE.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("all", policy =>
    {
       policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod(); 
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<VAAOContext>();
builder.Services.AddScoped<IUsersRepository, UserRepository>();
builder.Services.AddScoped<IClientesRepository, ClientesRepository>();
builder.Services.AddScoped<IRepartidoresRepository, RepartidoresRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidosRepository>();
builder.Services.AddScoped<IEntregasRepository, EntregasRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
var app = builder.Build();
app.UseCors("all"); 
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
