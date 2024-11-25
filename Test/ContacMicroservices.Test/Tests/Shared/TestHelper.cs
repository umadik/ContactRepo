using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContacMicroservices.Test.Tests.Shared
{
    public static class TestHelper
    {
        public static IAsyncCursor<T> MockCursor<T>(IEnumerable<T> items)
        {
            var mockCursor = new Mock<IAsyncCursor<T>>();
            mockCursor.SetupSequence(cursor => cursor.MoveNext(It.IsAny<System.Threading.CancellationToken>()))
                      .Returns(true)
                      .Returns(false);
            mockCursor.Setup(cursor => cursor.Current).Returns(items);
            return mockCursor.Object;
        }
    }
}
