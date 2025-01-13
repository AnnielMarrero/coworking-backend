
using coworking.Dtos;
using coworking.UnitOfWork.Interfaces.Base;
using FluentValidation;


namespace minimalApiIglesia.Validators
{
    public class CreateRoomValid : AbstractValidator<CreateRoomDto>
    {
        private readonly IServiceProvider _services;
        public CreateRoomValid(IServiceProvider services)
        {
            _services = services;

            RuleFor(dto => dto.Location)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(dto => dto.Capacity)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0);

        }


    }
}
