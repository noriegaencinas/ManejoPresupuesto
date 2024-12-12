using ManejoPresupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayuscula(ErrorMessage = "La primera letra debe ser mayuscula")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

    }
}

/* 
   [StringLength(maximumLength:50, MinimumLength =3, ErrorMessage = "La longitud del campo {0} debe estar entre {1} y {2}")]
   [Required(ErrorMessage = "El campo {0} es requerido")]
   [Display(Name = "Nombre del tipo cuenta")]
   [Required(ErrorMessage = "El campo {0} es requerido")]
   [EmailAddress(ErrorMessage = "El campo debe ser un email valido")]
   [Range(minimum:18, maximum:100, ErrorMessage = "El valor debe estar entre {1} {2}")]
   [Url(ErrorMessage = "El campo debe ser una URL valida")]
   [CreditCard(ErrorMessage = "La tarjeta de credito no es valida")]
 */