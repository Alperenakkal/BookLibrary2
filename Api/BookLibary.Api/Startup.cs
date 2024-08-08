using BookLibary.Api.Data.Context;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using BookLibary.Api.Services.AuthServices.BookServices;
using BookLibary.Api.Services.AuthServices.BorrowServices;
using BookLibary.Api.Services.AuthServices.LoginServices;
using BookLibary.Api.Services.AuthServices.RegisterServices;
using BookLibary.Api.Services.AuthServices.TokenHelperServices;
using BookLibary.Api.Services.AuthServices.TokenServices;
using BookLibary.Api.Services.AuthServices.UpdateServices;
using BookLibary.Api.Services.AuthServices.EmailServices;
using BookLibary.Api.Services.AuthServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // MongoDB ve diğer servislerin konfigürasyonu
        services.Configure<MongoSettings>(Configuration.GetSection("MongoSettings"));
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IUserRepository<User>, LoginRepository>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IUpdateService, UpdateService>();
        services.AddScoped<IBookRepository<Book>, BookRepository>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITokenHelperService, TokenHelperService>();
        services.AddScoped<IBorrowService, BorrowService>();
        services.AddScoped<IRepository<User>, MongoRepositoryBase<User>>();
        services.AddScoped<IRegisterRepository<User>, RegisterRepository>();
        services.AddScoped<IRegisterService, RegisterService>();
        services.AddScoped<EmailService>();

        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        // JWT Authentication yapılandırması
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

        // CORS politikası
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // İzin verilen köken
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Bu satır önemli
                });
        });

        services.AddControllers();

        // Swagger yapılandırması
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookLibrary API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your token."
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Veritabanı veya başlangıç verileri
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;
            SeedData.Initialize(services);
        }

        // CORS politikalarını uygulama
        app.UseCors("AllowSpecificOrigin");

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookLibrary API v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

}
