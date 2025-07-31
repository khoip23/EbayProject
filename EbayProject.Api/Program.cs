var builder = WebApplication.CreateBuilder(args);

//service controller
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();