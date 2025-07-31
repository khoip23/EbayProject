using EbayProject.Api.models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//DI
//service controller
builder.Services.AddControllers();

//SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//ORM EF
string connectionEbay = builder.Configuration.GetConnectionString("ConnectionStringEbay");

builder.Services.AddDbContext<EbayContext>(options =>
    options.UseSqlServer(connectionEbay));


var app = builder.Build();

//MIDDLEWARE
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();