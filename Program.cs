using AccountService.Behaviors;
using AccountService.Controllers;
using AccountService.Features.Accounts.Commands;
using AccountService.Filters;
using AccountService.Interfaces;
using AccountService.Models.Dto;
using AccountService.Repositories;
using AccountService.Services;
using AccountService.Validators;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ApiExceptionFilter>();
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
    c.EnableAnnotations();
});

//регистрация репозиторие и сервисов
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<IClientVerificationService, ClientVerificationServiceStub>();
builder.Services.AddSingleton<ICurrencyService, CurrencyServiceStub>();

// Регистрация валидаторов
builder.Services.AddScoped<IValidator<CreateAccountCommand>, CreateAccountCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AccountValidator>(ServiceLifetime.Scoped);

//builder.Services.AddScoped<IValidator<CreateAccountRequest>, CreateAccountRequestValidator>();
//builder.Services.AddScoped<IValidator<UpdateAccountRequest>, UpdateAccountRequestValidator>();
//builder.Services.AddScoped<IValidator<CreateTransactionRequest>, CreateTransactionRequestValidator>();
//builder.Services.AddScoped<IValidator<TransferRequest>, TransferRequestValidator>();

// Настройка MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

// Регистрация фильтров
builder.Services.AddScoped<ValidationFilter>();
builder.Services.AddScoped<ApiExceptionFilter>();

//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();


app.Run();
