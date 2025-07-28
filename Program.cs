using AccountService.Controllers;
using AccountService.Interfaces;
using AccountService.Models.Configs;
using AccountService.Models.Dto;
using AccountService.Models.Enums;
using AccountService.Repositories;
using AccountService.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
//using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Account Service", 
        Version = "v1",
        Description = "ћикросервис дл€ управлени€ банковскими счетами"
    });
    c.EnableAnnotations();
});

//регистраци€ сервисов
builder.Services.AddValidatorsFromAssemblyContaining<AccountValidator>(ServiceLifetime.Scoped);
builder.Services.AddScoped<IValidator<CreateAccountRequest>, CreateAccountRequestValidator>();
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
builder.Services.AddMediatR(cfg =>  cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
// –егистраци€ валидаторов
builder.Services.AddScoped<IValidator<UpdateAccountRequest>, UpdateAccountRequestValidator>();
builder.Services.AddScoped<IValidator<CreateTransactionRequest>, CreateTransactionRequestValidator>();
builder.Services.AddScoped<IValidator<TransferRequest>, TransferRequestValidator>();

// –егистраци€ контроллеров (если не используетс€ AddControllers())
builder.Services.AddControllers()
    .AddApplicationPart(typeof(AccountsController).Assembly);

builder.Services.AddTransient(typeOf(IPipelineBehavior<,>), typeOf(ValidationBehavior<,>));

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
