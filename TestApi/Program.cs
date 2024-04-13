using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestApi.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TestApiContextConnection") ?? throw new InvalidOperationException("Connection string 'TestApiContextConnection' not found.");

builder.Services.AddDbContext<TestApiContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TestApiContext>();


builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "809969990663-ckdtdr658iijec0lcaa36e5ddqmq7l0r.apps.googleusercontent.com";
    googleOptions.ClientSecret = "IFD0rZ-Ix4YJaUYQj-aQJRaO";
});

builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("TestPolicy", b =>
    {
        b.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
builder.Services.AddControllers();

var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.UseAuthorization();
app.UseCors();

app.MapControllers();
app.MapRazorPages();
app.Run();
