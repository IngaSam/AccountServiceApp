using AccountService.Behaviors;
using AccountService.Documentation;
using AccountService.Features.Accounts.Commands;
using AccountService.Features.Accounts.Handlers;
using AccountService.Features.Accounts.Validators;
using AccountService.Filters;
using AccountService.Interfaces;
using AccountService.Middleware;
using AccountService.Models.Configs;
using AccountService.Models.Errors;
using AccountService.Models.Results;
using AccountService.Repositories;
using AccountService.Services;
using AccountService.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);



// Настройка CORS (AllowAll)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Добавление контроллеров с фильтрами
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ApiExceptionFilter>();
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Account Service",
        Version = "v1",
        Description = "Микросервис для управления банковскими счетами"
    });

    // XML документация
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

    // JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
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

    // Enum как строки
    c.SchemaFilter<EnumSchemaFilter>();
});

// Регистрация сервисов
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<IClientVerificationService, ClientVerificationServiceStub>();
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();

// Регистрация MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

// Регистрация конфигураций
builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection("Keycloak"));
builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection("CurrencySettings"));

// Валидация конфигурации Keycloak
var keycloakSettings = builder.Configuration.GetSection("Keycloak").Get<KeycloakSettings>();
if (string.IsNullOrEmpty(keycloakSettings?.Authority))
    throw new ApplicationException("Keycloak Authority not configured");
if (string.IsNullOrEmpty(keycloakSettings.Audience))
    throw new ApplicationException("Keycloak Audience not configured");

// Настройка аутентификации (использует проверенную конфигурацию)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakSettings.Authority; // Используем уже проверенное значение
        options.Audience = keycloakSettings.Audience;
        options.RequireHttpsMetadata = false;
    });
// Валидация CurrencySettings
var currencySettings = builder.Configuration.GetSection("CurrencySettings").Get<CurrencySettings>();
if (currencySettings?.SupportedCurrencies == null || !currencySettings.SupportedCurrencies.Any())
    throw new ApplicationException("SupportedCurrencies not configured");

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>(lifetime: ServiceLifetime.Scoped);
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Account Service v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Custom middleware
app.UseMiddleware<ValidationExceptionMiddleware>();
app.UseMiddleware<ApiExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(
            MbResult<object>.Fail(new MbError("NOT_FOUND", "Resource not found"))
        ));
    }
});

app.Run();

// Фильтр для enum
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Type = "string";
            schema.Enum.Clear();
            Enum.GetNames(context.Type)
                .ToList()
                .ForEach(name => schema.Enum.Add(new OpenApiString(name)));
        }
    }
}