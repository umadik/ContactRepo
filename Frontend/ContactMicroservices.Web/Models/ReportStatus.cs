using System.ComponentModel.DataAnnotations;

namespace ContactMicroservices.Web.Models
{
    public enum ReportStatus
    {
        [Display(Name = "Hazırlanıyor")]
        Preparing,
        [Display(Name = "Tamamlandı")]
        Completed
    }
}
