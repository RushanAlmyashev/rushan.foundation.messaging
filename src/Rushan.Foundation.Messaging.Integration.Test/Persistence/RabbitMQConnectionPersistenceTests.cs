using Moq;
using Rushan.Foundation.Messaging.Integration.Tests.Utils;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Persistence;

namespace Rushan.Foundation.Messaging.Integration.Tests.Persistence
{
    [Category("Integration")]
    [Category("LongRunning")]
    [NonParallelizable]
    public class RabbitMQConnectionPersistenceTests
    {
        private IRabbitMQConnection _target;

        private Mock<ILogger> _logger;
        private string _connectionString;

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger>();

            _connectionString = BrokerHelper.GetConnectionString();
            _target = new RabbitMQConnectionPersistence(_connectionString, _logger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _target.Stop();
        }


        [Test]
        public void TestConnectnion()
        {
            var actual = _target.GetConnection().IsOpen;

            Assert.True(actual);
        }

        [Test]
        public async Task WhenBrockerShuttingDown_ConnectionTryingRestore()
        {
            var actualBeforeWaiting = _target.GetConnection().IsOpen;

            await DockerServiceHelper.StopContainerAsync();

            var actualOnWaiting = _target.GetConnection().IsOpen;

            await DockerServiceHelper.StartContainerAsync();

            var actualAfterWaiting = _target.GetConnection().IsOpen;

            Assert.IsTrue(actualBeforeWaiting);
            Assert.IsFalse(actualOnWaiting);
            Assert.IsTrue(actualAfterWaiting);
        }
    }
}
