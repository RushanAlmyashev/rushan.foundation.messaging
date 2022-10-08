using Autofac;
using AutoFixture;
using Moq;
using Rushan.Foundation.Messaging.Configuration;
using Rushan.Foundation.Messaging.Integration.Tests.Application.InteranlServices;
using Rushan.Foundation.Messaging.Integration.Tests.Application.Messages;
using Rushan.Foundation.Messaging.Integration.Tests.Application.Recievers;
using Rushan.Foundation.Messaging.Integration.Tests.RabbitMQDockering;

namespace Rushan.Foundation.Messaging.Integration.Tests
{
    [Category("Integration")]
    [NonParallelizable]
    public class RabbitMqMessageBusTest
    {
        private const int NumberOfMessageOne = 8;
        private const int NumberOfMessageTwo = 7322;
        private const int NumberOfMessageThree = 100000;

        private RabbitMqMessaging _target;

        private IContainer _container;
        private Fixture _fixture;

        private MessagingConfiguration _messagingConfiguration;

        public RabbitMqMessageBusTest()
        {
            _fixture = new Fixture();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _messagingConfiguration = new MessagingConfiguration
            {
                MessageBrokerUri = BrokerHelper.GetConnectionString(),
                Exchange = BrokerConstants.ExchangeName
            };

            var messaging = new RabbitMqMessaging(_messagingConfiguration);

            var builder = new ContainerBuilder();

            builder.Register(ctx => new InternalService()).As<IInternalService>().AsSelf();
            builder.Register(ctx => new FirstReciver(ctx.Resolve<IInternalService>()))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.Register(ctx => new SecondReciever(ctx.Resolve<IInternalService>()))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.Register(ctx => _target)
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            _container = builder.Build();
        }

        [TearDown]
        public void TearDown()
        {
            var messaging = _container.Resolve<RabbitMqMessaging>();

            messaging.Stop();
            _container.Dispose();
        }

        [Test]
        public void WhenParallelSendDifferentMessages_SameMessageAreRecievedWithWriteSequences()
        {

            var internlService = _container.Resolve<InternalService>();
            var recievers = _container.Resolve<IEnumerable<IMessageReceiver>>();
            var messaging = _container.Resolve<RabbitMqMessaging>();

            foreach (var reciever in recievers)
            {
                messaging.Subscribe(reciever);
            }

            messaging.Start();

            var expectedMessageOnes = _fixture.CreateMany<MessageOne>(NumberOfMessageOne).ToArray();
            var expectedMessageTwos = _fixture.CreateMany<MessageTwo>(NumberOfMessageTwo).ToArray();
            var expectedMessageThrees = _fixture.CreateMany<MessageThree>(NumberOfMessageThree).ToArray();

            var taskOne = Task.Run(() => SendCollectionMessageOne(messaging, expectedMessageOnes));
            var taskTwo = Task.Run(() => SendCollectionMessageTwo(messaging, expectedMessageTwos));
            var taskThree = Task.Run(() => SendCollectionMessageThree(messaging, expectedMessageThrees));

            Task.WaitAll(taskOne, taskTwo, taskThree);

            Task.Delay(3000);//For some messages

            var actualMessageOnes = internlService.MessageOnes.ToArray();
            var actualMessageTwos = internlService.MessageTwos.ToArray();
            var actualMessageThrees = internlService.MessageThrees.ToArray();

            Assert.That(actualMessageOnes.Length, Is.EqualTo(expectedMessageOnes.Length));
            for (int i = 0; i < actualMessageOnes.Length; i++)
            {
                Assert.That(actualMessageOnes[i], Is.EqualTo(expectedMessageOnes[i]));
            }

            Assert.That(actualMessageTwos.Length, Is.EqualTo(expectedMessageTwos.Length));
            for (int i = 0; i < actualMessageTwos.Length; i++)
            {
                Assert.That(actualMessageTwos[i], Is.EqualTo(expectedMessageTwos[i]));
            }

            Assert.That(actualMessageThrees.Length, Is.EqualTo(expectedMessageThrees.Length));
            for (int i = 0; i < actualMessageThrees.Length; i++)
            {
                Assert.That(actualMessageThrees[i], Is.EqualTo(expectedMessageThrees[i]));
            }
        }


        private void SendCollectionMessageOne(IMessaging messaging, MessageOne[] messages)
        {
            foreach (var message in messages)
            {
                messaging.Publish(message);
            }
        }

        private void SendCollectionMessageTwo(IMessaging messaging, MessageTwo[] messages)
        {
            foreach (var message in messages)
            {
                messaging.Publish(message);
            }
        }

        private void SendCollectionMessageThree(IMessaging messaging, MessageThree[] messages)
        {
            foreach (var message in messages)
            {
                messaging.Publish(message);
            }
        }
    }
}

