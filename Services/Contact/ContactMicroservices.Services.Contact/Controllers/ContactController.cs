using ContactMicroservices.Services.Contact.Data;
using ContactMicroservices.Services.Contact.Model;
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
        public async Task<IActionResult> UpdateContact([FromBody] Model.Contact updatedContact)
        {
            var result = await _context.Contacts.ReplaceOneAsync(c => c.Id == updatedContact.Id, updatedContact);
            if (result.MatchedCount == 0)
                return NotFound();

            return Ok();
        }

        [HttpPost("add-detail")]
        public async Task<IActionResult> AddDetail([FromBody] InfoTypeDetailModel detail)
        {
            var filter = Builders<Model.Contact>.Filter.Eq(c => c.Id, detail.ContactId);
            var update = Builders<Model.Contact>.Update.Push(c => c.InfoTypes, new InfoType
            {
                Type = Enum.TryParse<InfoValueType>(detail.Type, out var parsedType) ? parsedType : throw new ArgumentException("Geçersiz tür"),
                Value = detail.Value
            });

            var result = await _context.Contacts.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                return NotFound("Kayıt bulunamadı.");
            }

            return Ok();
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
