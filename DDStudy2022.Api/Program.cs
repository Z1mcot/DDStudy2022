using AspNetCoreRateLimit;
using AutoMapper.Configuration.Annotations;
using DDStudy2022.Api.Configs;
using DDStudy2022.Api.Mapper;
using DDStudy2022.Api.Middleware;
using DDStudy2022.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DDStudy2022.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Регистрация сервисов
            var authSection = builder.Configuration.GetSection(AuthConfig.ConfigPosition);
            var authConfig = authSection.Get<AuthConfig>();

            builder.Services.Configure<AuthConfig>(authSection);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "Enter your token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme,
                            },
                            Scheme = "oauth2",
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                c.SwaggerDoc("Auth", new OpenApiInfo { Title = "Auth" });
                c.SwaggerDoc("Api", new OpenApiInfo { Title = "Api" });
            });

            // Регистрация сессии с нашей ДБ 
            builder.Services.AddDbContext<DAL.DataContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostreSQL"), sql => { });
            });

            // Анти дудос
            builder.Services.AddOptions();
            builder.Services.AddMemoryCache();
            builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
            builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
            
            // Наши кастомные сервисы
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<AuthService>();

            builder.Services.AddTransient<AttachmentService>();
            builder.Services.AddScoped<LinkGeneratorService>();
            
            builder.Services.AddScoped<PostService>();
            builder.Services.AddScoped<CommentService>();
            
            builder.Services.AddScoped<SubscriptionService>();

            //builder.Services.AddSingleton<DdosGuard>();

            // Аутентификация и авторизация
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authConfig.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authConfig.SymmetricSecurityKey,
                    ClockSkew = TimeSpan.Zero,
                };
            });
            builder.Services.AddAuthorization(o =>
            {
                o.AddPolicy("ValidAccessToken", p =>
                {
                    p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    p.RequireAuthenticatedUser();
                });
            });

            var app = builder.Build();


            // Описание логики API

            // Автоматическая миграция. Вызывается при каждом запуске приложения. Это scoped сервис, который живё только во время запроса
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
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("Api/swagger.json", "Api");
                    c.SwaggerEndpoint("Auth/swagger.json", "Auth");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseClientRateLimiting();
            //app.UseCustomDdosProtection();
            
            app.UseAuthorization();
            app.UseTokenValidator();
            app.UseGlobalErrorWrapper();
            app.MapControllers();

            app.Run();
        }
    }
}