using System.Text.Json.Serialization;
using InssApi.Data;
using InssApi.Middleware;
using InssApi.Repositories;
using InssApi.Services;
using Microsoft.EntityFrameworkCore;

// Dia 2 · tarde · M8 — mesmas rotas da manhã, agora em camadas + log + correlação

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IContribuinteRepository, ContribuinteRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IContribuinteService, ContribuinteService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

var app = builder.Build();

app.UseMiddleware<ErrosMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
