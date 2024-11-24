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
        public async Task<IActionResult> GetContactDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Geçersiz ID.");

            try
            {
                var contact = await _context.Contacts
                    .Find(c => c.Id == id)
                    .FirstOrDefaultAsync();

                if (contact == null || contact.InfoTypes == null)
                    return NotFound("Bilgi bulunamadı.");

                // InfoTypes bilgisini döndür
                var details = contact.InfoTypes.Select(info => new
                {
                    type = info.Type.ToString(),
                    value = info.Value
                }).ToList();

                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
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

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Geçersiz ID.");

            var contact = await _context.Contacts.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (contact == null)
                return NotFound("Kişi bulunamadı.");

            return View(contact);
        }

        [HttpPost]
        public async Task<IActionResult> EditContact([FromBody] Contact updatedContact)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedContact);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");

            // API'ye POST isteği gönder
            var response = await client.PutAsJsonAsync("Contact/UpdateContact", updatedContact);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Contact");
            }

            // Hata durumunda mesaj döndür
            ModelState.AddModelError("", "Güncelleme başarısız. Lütfen tekrar deneyin.");
            return View(updatedContact);

        }

        [HttpGet]
        public async Task <IActionResult> AddDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Geçersiz ID.");
            InfoTypeDetailModel infoTypeDetailModel = new InfoTypeDetailModel();
            var contact = await _context.Contacts.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (contact == null)
                return NotFound("Kişi bulunamadı.");

            infoTypeDetailModel.FirstName = contact.FirstName;
            infoTypeDetailModel.LastName = contact. LastName;
            infoTypeDetailModel.Company = contact.Company;

            ViewBag.ContactId = id; // İlgili kişi ID'si ViewBag ile gönderiliyor
            return View(infoTypeDetailModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddDetail([FromBody] InfoTypeDetailModel detail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Gönderilen veri geçersiz.");
            }

            var client = _httpClientFactory.CreateClient("ApiClient");

            // API'ye POST isteği gönder
            var response = await client.PostAsJsonAsync("Contact/add-detail", detail);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Contact");
            }

            return NoContent(); // Başarılı ama içerik dönmüyor
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
