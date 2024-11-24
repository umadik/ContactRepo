using Microsoft.AspNetCore.Mvc;
using DotNetCore.CAP;
using MongoDB.Driver;
using ContactMicroservices.Web.Data;
using ContactMicroservices.Web.Models;

namespace ContactMicroservices.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly MongoDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICapPublisher _capPublisher;

        public ReportController(MongoDbContext context, IHttpClientFactory httpClientFactory, ICapPublisher capPublisher)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _capPublisher = capPublisher;
        }

        // Raporları listeleyen sayfa
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient2");

            // API'ye GET isteği gönder
            var response = await client.GetAsync("Report/get-all");

            if (response.IsSuccessStatusCode)
            {

                // API'den gelen veriyi deserialize et
                var reports = await response.Content.ReadFromJsonAsync<List<Report>>();
                return View(reports); // Veriyi View'e gönder
            }
            return View(new List<Report>());
        }

        // Yeni rapor oluşturma
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] Report report)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            report.Id = "";

            // API'ye POST isteği gönder
            var response = await client.PostAsJsonAsync("Report", report);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Report");
            }

            // Hata durumunda mesaj döndür
            ModelState.AddModelError("", "Kayıt oluşturulamadı. Lütfen tekrar deneyin.");
            return View(report);
        }

        // Producer Transaction (RabbitMQ)
        [HttpPost("producer-transaction")]
        public async Task<IActionResult> ProducerTransaction(string City)
        {
            var city = City.ToLower();
            var report = new Report
            {
                Location = city,  // Örnek olarak İstanbul verildi, burayı kullanıcıdan alabilirsiniz.
                Status = ReportStatus.Preparing,
                CreatedAt = DateTime.UtcNow,
                ContactCount = 0,  // Başlangıçta 0, işleme başladıkça güncellenecek
                PhoneNumberCount = 0  // Başlangıçta 0, işleme başladıkça güncellenecek
            };

            await _context.Reports.InsertOneAsync(report);  // MongoDB'ye ekliyoruz

            await _capPublisher.PublishAsync<string>("producer.transaction", report.Id);
            return RedirectToAction("Index");
        }
    }
}
