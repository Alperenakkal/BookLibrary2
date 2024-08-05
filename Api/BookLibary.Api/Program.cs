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
using BookLibary.Api.Services.AuthServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IUserRepository<User>, LoginRepository>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUpdateService, UpdateService>();

builder.Services.AddScoped<IUpdateService, UpdateService>();

builder.Services.AddScoped<IBookRepository<Book>, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenHelperService, TokenHelperService>();

builder.Services.AddScoped<IBorrowService, BorrowService>();      //this service is not able to be constructed (Error while validating the service descriptor 
builder.Services.AddScoped<IRepository<User>, MongoRepositoryBase<User>>();
//this service is not able to be constructed (Error while validating the service descriptor 

builder.Services.AddScoped<IRegisterRepository<User>, RegisterRepository>();
builder.Services.AddScoped<IRegisterService, RegisterService>();



builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")  // İzin verilen köken
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}
app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
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

app.MapControllers();

app.Run();
