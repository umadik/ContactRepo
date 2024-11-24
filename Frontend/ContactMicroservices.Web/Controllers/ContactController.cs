using Microsoft.AspNetCore.Mvc;
using DotNetCore.CAP;
using MongoDB.Driver;
using ContactMicroservices.Web.Data;
using ContactMicroservices.Web.Models;
using Amazon.Runtime;

namespace ContactMicroservices.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly MongoDbContext _context;
        private readonly ICapPublisher _capPublisher;
        private readonly IHttpClientFactory _httpClientFactory;


        public ContactController(IHttpClientFactory httpClientFactory,MongoDbContext context, ICapPublisher capPublisher)
        {
            _context = context;
            _capPublisher = capPublisher;
            _httpClientFactory = httpClientFactory;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // API'ye GET isteği gönder
            var response = await client.GetAsync("Contact");

            if (response.IsSuccessStatusCode)
            {
                // API'den gelen veriyi deserialize et
                var contacts = await response.Content.ReadFromJsonAsync<List<Contact>>();
                return View(contacts); // Veriyi View'e gönder
            }

            // Hata durumunda boş bir liste döndür
            return View(new List<Contact>());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await _context.Contacts.Find(_ => true).ToListAsync();
            return View(contacts);  
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] Contact contact)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            contact.Id = "";

            // API'ye POST isteği gönder
            var response = await client.PostAsJsonAsync("Contact", contact);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Contact"); 
            }

            // Hata durumunda mesaj döndür
            ModelState.AddModelError("", "Kayıt oluşturulamadı. Lütfen tekrar deneyin.");
            return View(contact);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Geçersiz ID");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.DeleteAsync($"Contact/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Kayıt başarıyla silindi.";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "Kayıt bulunamadı.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Silme işlemi başarısız oldu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Silme işlemi sırasında bir hata oluştu: {ex.Message}";
            }

            // Liste sayfasına yönlendirme
            return RedirectToAction("Index");
        }


        // Producer Transaction (RabbitMQ)
        [HttpPost("producer-transaction")]
        public async Task<IActionResult> ProducerTransaction()
        {
            var date = DateTime.Now;
            await _capPublisher.PublishAsync<DateTime>("producer.transaction", date);
            return Ok("Attım: " + date);
        }

        // Create view for the contact form
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }
    }
}
