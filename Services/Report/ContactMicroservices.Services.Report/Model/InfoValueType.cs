using System.ComponentModel.DataAnnotations;

namespace ContactMicroservices.Services.Report.Model
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
