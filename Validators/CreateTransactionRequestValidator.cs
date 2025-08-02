using AccountService.Models.Dto;
using FluentValidation;

namespace AccountService.Validators
{
    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionRequestValidator()
        {
        
            RuleFor(x => x.AccountId)
                .NotEmpty()
                .WithMessage("Идентификатор счёта обязателен");
                

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Сумма должна быть положительной");
                

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Допустимые типы транзакций: Credit, Debit");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Описание обязательно")
                .MaximumLength(500)
                .WithMessage("Описание не должно превышать 500 символов")
                .Matches(@"^[a-zA-Zа-яА-Я0-9\s.,!?-]+$")
                .WithMessage("Описание содержит недопустимые символы");
        }
    }
    
    
}
