using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ?? Clave secreta para firmar los tokens JWT
var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyWithAtLeast32Characters");

// ? Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ? Versionado de la API
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// ? Autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// ? Swagger con múltiples versiones y JWT
builder.Services.AddSwaggerGen(options =>
{
    var provider = builder.Services.BuildServiceProvider()
        .GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = $"ApiPokemonLista {description.ApiVersion}",
            Version = description.GroupName
        });
    }

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Introduce tu token JWT con el prefijo 'Bearer'. Ejemplo: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new List<string>()
        }
    });
});

var app = builder.Build();

// ? Middleware
if (app.Environment.IsDevelopment())
{
    var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

