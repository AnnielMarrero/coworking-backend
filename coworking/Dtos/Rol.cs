using System.ComponentModel.DataAnnotations;

namespace coworking.Dtos
{
    public enum RolEnum
    {
        [Display(Name = "Admin")]
        ADMIN = 2,
        [Display(Name = "Estandar")]
        ESTANDAR =1
    }
}
