using Microsoft.EntityFrameworkCore;
using TWebApp.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ImageDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("userConnection")));
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
//
