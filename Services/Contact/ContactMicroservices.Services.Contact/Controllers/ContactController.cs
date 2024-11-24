using ContactMicroservices.Services.Contact.Data;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ContactMicroservices.Services.Contact.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly ICapPublisher _capPublisher;

        public ContactController(MongoDbContext context,ICapPublisher capPublisher)
        {
            _context = context;
            _capPublisher = capPublisher;
        }

        [HttpPost("producer-transaction")]
        public async Task<IActionResult> ProducerTransaction()
        {
            var date = DateTime.Now;
            await _capPublisher.PublishAsync<DateTime>("producer.transaction", date);
            return Ok(date);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await _context.Contacts.Find(_ => true).ToListAsync();
            return Ok(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] Model.Contact contact)
        {
            await _context.Contacts.InsertOneAsync(contact);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(string id)
        {
            var contact = await _context.Contacts.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(string id, [FromBody] Model.Contact updatedContact)
        {
            var result = await _context.Contacts.ReplaceOneAsync(c => c.Id == id, updatedContact);
            if (result.MatchedCount == 0)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(string id)
        {
            var result = await _context.Contacts.DeleteOneAsync(c => c.Id == id);
            if (result.DeletedCount == 0)
                return NotFound();

            return NoContent();
        }

    }
}
