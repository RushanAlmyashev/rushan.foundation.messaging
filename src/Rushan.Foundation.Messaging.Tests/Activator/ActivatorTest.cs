using AutoFixture;
using Moq;
using Rushan.Foundation.Messaging.Activator;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Serialization;
using Rushan.Foundation.Messaging.Tests.Messages;

namespace Rushan.Foundation.Messaging.Tests.Activator
{
    [TestFixture]
    public class ActivatorTest
    {
        private IActivator _target;

        private Mock<ILogger> _logger;
        private ISerializer _serializer;

        private Fixture _fixture;

        public ActivatorTest()
        {
            _fixture = new Fixture();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger>();
            _serializer = new JsonMessageSerializer();

            _target = new Messaging.Activator.Activator(_serializer, _logger.Object);
        }

        [Test]
        public void WhenCallActivateMessage_ReturnExpectedMessage()
        {
            var message = _fixture.Create<MessageOne>();

            var typeHint = message.GetType().FullName;
            var byteArrayContent = _serializer.Serialize(message);

            var actual = _target.CreateMessageInstance(typeHint, byteArrayContent);

            Assert.That(actual, Is.EqualTo(message));
        }
    }
}
