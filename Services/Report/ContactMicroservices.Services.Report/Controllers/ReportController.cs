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

        [HttpGet]
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

        [ApiExplorerSettings(IgnoreApi = true)]
        [CapSubscribe("producer.transaction")]
        public void Consumer(DateTime date)
        {
            Console.WriteLine(date);
        }
    }
}
