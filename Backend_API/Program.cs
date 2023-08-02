using ASP_API.Helpers;
using ASP_API.Mapper;
using Backend_API.Asbtract;
using Backend_API.Data;
using Backend_API.Data.Entities.Identity;
using Backend_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<AppEFContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MyConnectionDB")));


builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Stores.MaxLengthForKeys = 128;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<AppEFContext>()
    .AddDefaultTokenProviders();

var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<String>("JwtSecretKey")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = signinKey,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
builder.Services.AddSwaggerGen(c =>
{
    var fileDoc = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");
    c.IncludeXmlComments(fileDoc);
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Description = "Jwt Auth header using the Bearer scheme",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Id="Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            }, new List<string>()
        }
    });
});

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddAutoMapper(typeof(AppMapProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(conf =>
    conf.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var dir = Path.Combine(Directory.GetCurrentDirectory(), "images");
if (!Directory.Exists(dir))
    Directory.CreateDirectory(dir);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dir),
    RequestPath = "/images"
});

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.SeedData();

app.Run();
