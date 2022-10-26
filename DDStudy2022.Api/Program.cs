using DDStudy2022.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace DDStudy2022.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Регистрация сервисов

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DAL.DataContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostreSQL"), sql => { });
            });

            builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

            builder.Services.AddScoped<UserService>();

            var app = builder.Build();
            
            // Описание логики API

            // Автоматическая миграция. Вызывается при каждом запуске приложения
            using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                if (serviceScope != null)
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<DAL.DataContext>();
                    context.Database.Migrate();
                }
            }

            // Configure the HTTP request pipeline.
            // if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}