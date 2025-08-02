using AccountService.Behaviors;
using AccountService.Documentation;
using AccountService.Features.Accounts.Commands;
using AccountService.Features.Accounts.Handlers;
using AccountService.Features.Accounts.Validators;
using AccountService.Features.Transfers.Commands;
using AccountService.Filters;
using AccountService.Interfaces;
using AccountService.Models.Configs;
using AccountService.Models.Enums;
using AccountService.Repositories;
using AccountService.Services;
using AccountService.Validators;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ApiExceptionFilter>();
})
//.AddNewtonsoftJson()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

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
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Поддержка примеров запросов
    c.ExampleFilters();

    // Отображение enum как строк
    c.UseInlineDefinitionsForEnums();
    c.SchemaFilter<EnumSchemaFilter>();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<AccountExamples>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<TransactionExamples>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<TransferExamples>();

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

builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Чтение конфигурации
builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection("CurrencySettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Account Service v1");
        c.RoutePrefix = string.Empty;
    });
//}
// Редирект с корневого URL на Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.MapControllers();


app.Run();
