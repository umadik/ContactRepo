using ContactMicroservices.Services.Report.Data;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Text;

namespace ContactMicroservices.Services.Report.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly MongoDbContext _context;
        private readonly HttpClient _httpClient;

        public ReportController(MongoDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _context.Reports.Find(_ => true).ToListAsync();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(string id)
        {
            var report = await _context.Reports.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (report == null)
                return NotFound();

            return Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] Model.Report report)
        {
            await _context.Reports.InsertOneAsync(report);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [CapSubscribe("producer.transaction")]
        public async Task Consumer(string reportId)
        {

            var report = await _context.Reports.Find(r => r.Id == reportId).FirstOrDefaultAsync();
            if (report == null)
            {
                throw new Exception("Rapor bulunamadı.");
            }

            var contacts = await _context.Contacts.Find(_ => true).ToListAsync();  // Tüm kontakları alıyoruz

            var locationGroup = contacts
                .SelectMany(c => c.InfoTypes)
                .Where(info => info.Type == Model.InfoValueType.Konum)
                .GroupBy(info => info.Value)  // Konum bazında grupla
                .ToList();

            foreach (var group in locationGroup)
            {
                var location = group.Key;
                var phoneNumbersInLocation = contacts
                    .Where(c => c.InfoTypes.Any(i => i.Type == Model.InfoValueType.Konum && i.Value == location))
                    .SelectMany(c => c.InfoTypes)
                    .Count(i => i.Type == Model.InfoValueType.TelNo);  // Telefon numarasını say

                var contactCountInLocation = contacts
                    .Where(c => c.InfoTypes.Any(i => i.Type == Model.InfoValueType.Konum && i.Value == location))
                    .Count();  // Konumdaki kişi sayısı

                // Raporu güncelle
                var update = Builders<Model.Report>.Update
                    .Set(r => r.ContactCount, contactCountInLocation)
                    .Set(r => r.PhoneNumberCount, phoneNumbersInLocation)
                    .Set(r => r.Status, Model.ReportStatus.Completed);  // Rapor durumunu tamamlandı olarak güncelle

                await _context.Reports.UpdateOneAsync(r => r.Id == report.Id, update);
            }


        }
    }
}
