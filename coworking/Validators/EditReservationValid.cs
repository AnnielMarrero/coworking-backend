using coworking.Dtos;
using coworking.UnitOfWork.Interfaces.Base;
using FluentValidation;


namespace coworking.Validators
{
    public class EditReservationValid : AbstractValidator<EditReservationDto>
    {
        private readonly IServiceProvider _services;
        public EditReservationValid(IServiceProvider services)
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
        public async Task<bool> AnyAsync(string code, int id)
        {
            using var scope = _services.CreateScope(); //acceder via service because Iuw is scopped and validator in singleton
            var repositorios = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            return await repositorios.Medications.AnyAsync(m => m.Code.ToLower() == code.ToLower() && m.Id != id);
        }
        */
    }
}
