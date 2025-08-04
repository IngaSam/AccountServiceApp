using AccountService.Behaviors;
using AccountService.Documentation;
using AccountService.Filters;
using AccountService.Interfaces;
using AccountService.Models.Configs;
using AccountService.Models.Results;
using AccountService.Repositories;
using AccountService.Services;
using AccountService.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using AccountService.Features.Accounts.Commands;
using AccountService.Features.Accounts.Handlers;
using AccountService.Features.Accounts.Validators;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;


var builder = WebApplication.CreateBuilder(args);

// Конфигурация JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
    });

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


// Добавление сервисов
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ApiExceptionFilter>();
})

.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Сохраняем оригинальные имена свойств
});

// Настройка Swagger с JWT поддержкой
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Account Service", 
        Version = "v1",
        Description = "Микросервис для управления банковскими счетами"
    });

    // Включаем XML-комментарии
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

    // Поддержка примеров запросов
    c.ExampleFilters();

    // Отображение enum как строк
    c.UseInlineDefinitionsForEnums();
    c.SchemaFilter<EnumSchemaFilter>();

    // Добавляем поддержку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

});


//builder.Services.AddSwaggerExamplesFromAssemblyOf<AccountExamples>();
//builder.Services.AddSwaggerExamplesFromAssemblyOf<TransactionExamples>();
//builder.Services.AddSwaggerExamplesFromAssemblyOf<TransferExamples>();

//регистрация репозиторие и сервисов
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<IClientVerificationService, ClientVerificationServiceStub>();
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();

// Регистрация валидаторов
builder.Services.AddScoped<IValidator<CreateAccountCommand>, CreateAccountCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AccountValidator>();
builder.Services.AddScoped<IValidator<UpdateAccountCommand>, UpdateAccountCommandValidator>();
builder.Services.AddScoped<IRequestHandler<DeleteAccountCommand, bool>, DeleteAccountHandler>();

//builder.Services.AddScoped<IValidator<CreateAccountRequest>, CreateAccountRequestValidator>();
//builder.Services.AddScoped<IValidator<UpdateAccountRequest>, UpdateAccountRequestValidator>();
//builder.Services.AddScoped<IValidator<CreateTransactionRequest>, CreateTransactionRequestValidator>();
//builder.Services.AddScoped<IValidator<TransferRequest>, TransferRequestValidator>();

// Регистрация всех обработчиков MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    //cfg.RegisterServicesFromAssembly(typeof(TransferCommandHandler).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    //cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
   // cfg.RegisterServicesFromAssembly(typeof(DeleteAccountHandler).Assembly));
});

// Регистрация фильтров
builder.Services.AddScoped<ValidationFilter>();
builder.Services.AddScoped<ApiExceptionFilter>();

// Настройка логирования
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Чтение конфигурации
builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection("CurrencySettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Настройка FluentValidation
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
    config.ImplicitlyValidateChildProperties = false;
});

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Account Service v1");
        c.RoutePrefix = string.Empty;
    });

// Редирект с корневого URL на Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();


// Класс для фильтра enum 
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

// Класс настроек JWT
public class JwtSettings
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiryMinutes { get; set; }
}