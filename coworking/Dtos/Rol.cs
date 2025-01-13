using System.ComponentModel.DataAnnotations;

namespace coworking.Dtos
{
    public enum RolEnum
    {
        [Display(Name = "Admin")]
        ADMIN,
        [Display(Name = "Estandar")]
        ESTANDAR
    }
}
