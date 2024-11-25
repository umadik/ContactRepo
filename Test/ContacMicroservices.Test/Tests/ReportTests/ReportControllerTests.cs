using ContactMicroservices.Services.Report.Controllers;
using ContactMicroservices.Services.Report.Data;
using ContactMicroservices.Services.Report.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;

namespace ContacMicroservices.Test.Tests.ReportTests
{

    public class ReportControllerTests
    {
        private readonly MongoDbContext _mockDbContext;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly ReportController _controller;
        private readonly IOptions<ReportDatabaseSettings> _settings;

        public ReportControllerTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var settings = new ReportDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                ContactsDatabaseName = "ContactDb",
                DatabaseName = "ReportDb",
                ReportCollectionName = "TestReports"
            };
            _settings = Options.Create(settings);

            // Sınıf seviyesindeki _mockDbContext'i başlatıyoruz
            _mockDbContext = new ContactMicroservices.Services.Report.Data.MongoDbContext(_settings);

            _controller = new ReportController(_mockDbContext, _mockHttpClientFactory.Object);
        }

        [Fact]
        public async Task GetAllReports_ShouldReturnReports()
        {
            // Arrange
            var report = new Report { Id = "", Location = "Report1" };

            // Act
            var result = await _controller.GetAllReports();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedContacts = Assert.IsAssignableFrom<IEnumerable<Report>>(okResult.Value);
            int currentRecord = returnedContacts.Count();

            var result1 = await _controller.CreateReport(report);

            var result2 = await _controller.GetAllReports();
            var okResult2 = Assert.IsType<OkObjectResult>(result2);
            var returnedContacts2 = Assert.IsAssignableFrom<IEnumerable<Report>>(okResult2.Value);
            int newRecord = returnedContacts2.Count();

            // Assert
            Assert.Equal(currentRecord + 1, newRecord);
        }
    }

}
