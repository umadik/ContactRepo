using System.ComponentModel.DataAnnotations;

namespace ContactMicroservices.Web.Models
{
    public enum InfoValueType
    {
        [Display(Name = "Telefon Numarası")]
        TelNo,

        [Display(Name = "Email")]
        Email,

        [Display(Name = "Konum")]
        Konum
    }
}
