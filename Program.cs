using api.Data;
using api.Services;
using Microsoft.EntityFrameworkCore;

namespace api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);   

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<UserService>();
            builder.Services.AddSingleton<JwtTokenService>();

            builder.Services.AddAuthorization();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
