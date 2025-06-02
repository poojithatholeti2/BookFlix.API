using System.Text;
using BookFlix.API.Data;
using BookFlix.API.Mappings;
using BookFlix.API.Middlewares;
using BookFlix.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Pgvector;
using BookFlix.API.Repositories.Interfaces;
using BookFlix.API.Services.Interfaces;
using BookFlix.API.Services;
using Python.Runtime;
using BookFlix.API;

var builder = WebApplication.CreateBuilder(args);

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
options.UseNpgsql(builder.Configuration.GetConnectionString("BookFlixConnectionString"), o => o.UseVector()));

builder.Services.AddDbContext<BookFlixAuthDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("BookFlixAuthConnectionString")));

//service layer
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IEmbeddingService, PythonEmbeddingService>();

//repository layer
builder.Services.AddScoped<IRatingRepository, SQLRatingRepository>();
builder.Services.AddScoped<ICategoryRepository, SQLCategoryRepository>();
builder.Services.AddScoped<IBookRepository, SQLBookRepository>();
builder.Services.AddScoped<ITokenRepository, SQLTokenRepository>();

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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

Runtime.PythonDLL = "C:\\Users\\POOJITHA\\AppData\\Local\\Programs\\Python\\Python313\\python313.dll";
var pythonInstance = PythonEngineSingleton.Instance;

app.Lifetime.ApplicationStopping.Register(() =>
{
    pythonInstance.Shutdown();
});

app.Run();
