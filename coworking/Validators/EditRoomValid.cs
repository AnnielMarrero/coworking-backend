using coworking.Dtos.Drones;
using coworking.UnitOfWork.Interfaces.Base;
using FluentValidation;

namespace Application.Validators
{
    public class EditRoomValid : AbstractValidator<EditRoomDto>
    {
        private readonly IServiceProvider _services;

        public EditRoomValid(IServiceProvider services)
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
