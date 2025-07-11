using System.Text;
using BookFlix.API;
using BookFlix.API.Data;
using BookFlix.API.Mappings;
using BookFlix.API.Middlewares;
using BookFlix.API.Repositories;
using BookFlix.API.Repositories.Interfaces;
using BookFlix.API.Services;
using BookFlix.API.Services.Interfaces;
using DotNetEnv;
using GroqApiLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pgvector;
using Python.Runtime;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

//add logging functionality (implementing both writeTo console and writeTo txt file)
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/BookFlix_Log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BookFlix API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "OAuth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

//singleton services
//Register the background queue interface
builder.Services.AddSingleton<EmbeddingQueue>();
builder.Services.AddSingleton<IEmbeddingQueue>(sp => sp.GetRequiredService<EmbeddingQueue>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<EmbeddingQueue>());

//Scoped and short-lived, new instance of dbContext is created for each http request
builder.Services.AddDbContext<BookFlixDbContext>(options =>
options.UseNpgsql(Environment.GetEnvironmentVariable("BookFlixConnectionString"), o => o.UseVector()));

builder.Services.AddDbContext<BookFlixAuthDbContext>(options =>
options.UseSqlServer(Environment.GetEnvironmentVariable("BookFlixAuthConnectionString")));

//service layer
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IEmbeddingService, PythonEmbeddingService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<ILLMService, GroqLLMService>();

// Register Groq API client with injected API key
builder.Services.AddScoped<GroqApiClient>(sp =>
    new GroqApiClient(Environment.GetEnvironmentVariable("GroqApiKey")));

//repository layer
builder.Services.AddScoped<IRatingRepository, SQLRatingRepository>();
builder.Services.AddScoped<ICategoryRepository, SQLCategoryRepository>();
builder.Services.AddScoped<IBookRepository, SQLBookRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("BookFlix")
    .AddEntityFrameworkStores<BookFlixAuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 9;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_Issuer"),
        ValidAudience = Environment.GetEnvironmentVariable("Jwt_Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt_Key")))
    });

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Runtime.PythonDLL = Environment.GetEnvironmentVariable("PythonDLLPath");
var pythonInstance = PythonEngineSingleton.Instance;

app.Lifetime.ApplicationStopping.Register(() =>
{
    pythonInstance.Shutdown();
});

app.Run();
