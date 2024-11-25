using ContactMicroservices.Services.Contact.Controllers;
using ContactMicroservices.Services.Contact.Model;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;

namespace ContacMicroservices.Test.Tests.ContacTests
{

    public class ContactControllerTests
    {
        private readonly Mock<ICapPublisher> _mockCapPublisher;
        private readonly ContactController _controller;
        private readonly IOptions<ContactDatabaseSettings> _settings;

        public ContactControllerTests()
        {
            _mockCapPublisher = new Mock<ICapPublisher>();
            var settings = new ContactDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "ContactDb",
                ContactCollectionName = "TestContacts"
            };
            _settings = Options.Create(settings);

            // Mock edilen MongoDbContext'i manuel olarak oluşturalım
            var mockContext = new ContactMicroservices.Services.Contact.Data.MongoDbContext(_settings);

            // ContactController için mock nesneleri ile yapılandırma
            _controller = new ContactController(mockContext, _mockCapPublisher.Object);
        }

        [Fact]
        public async Task GetAllContacts_ShouldReturnContacts()
        {
            // Arrange
            var contact = new Contact { Id = "", FirstName = "Umut", LastName = "Madik" };


            // Act
            var result = await _controller.GetAllContacts();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedContacts = Assert.IsAssignableFrom<IEnumerable<Contact>>(okResult.Value);
            int currentRecord = returnedContacts.Count();

            var result1 = await _controller.CreateContact(contact);

            var result2 = await _controller.GetAllContacts();
            var okResult2 = Assert.IsType<OkObjectResult>(result2);
            var returnedContacts2 = Assert.IsAssignableFrom<IEnumerable<Contact>>(okResult2.Value);
            int newRecord = returnedContacts2.Count();

            // Assert
            Assert.Equal(currentRecord + 1, newRecord);
        }
    }
}

