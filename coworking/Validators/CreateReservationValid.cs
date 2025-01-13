using coworking.Dtos;

using coworking.UnitOfWork.Interfaces.Base;
using FluentValidation;


namespace minimalApiIglesia.Validators
{
    public class CreateReservationValid : AbstractValidator<CreateReservationDto>
    {
        private readonly IServiceProvider _services;
        public CreateReservationValid(IServiceProvider services)
        {
            _services = services;

            RuleFor(dto => dto.RoomId)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0);

          
            RuleFor(dto => dto.Date)
                .NotNull()
                .NotEmpty();

        }
        /*
        public async Task<bool> AnyAsync(string code)
        {
            using var scope = _services.CreateScope(); //acceder via service because Iuw is scopped and validator in singleton
            var repositorios = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            return await repositorios.Medications.AnyAsync(m => m.Code.ToLower() == code.ToLower());
        }
        */
    }
}
