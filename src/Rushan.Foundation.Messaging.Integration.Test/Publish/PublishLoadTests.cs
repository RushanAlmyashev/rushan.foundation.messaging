using AutoFixture;
using Rushan.Foundation.Messaging.Integration.Tests.Application.Messages;
using Rushan.Foundation.Messaging.Integration.Tests.RabbitMQDockering;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Persistence;
using Rushan.Foundation.Messaging.Publish;
using Rushan.Foundation.Messaging.Serialization;

namespace Rushan.Foundation.Messaging.Integration.Tests.Publish
{
    [Category("Integration")]
    public class PublishLoadTests
    {
        private Publisher _target;

        //private Autofac.IContainer _container;
        private const int NumberOfMessages = 100;
        private Fixture _fixture;
       
        private IRabbitMQConnection _rabbitMQConnection;
        private ISerializer _serializer;
        private string _exchange;
        private ILogger _logger;

        public PublishLoadTests()
        {
            _fixture = new Fixture();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            var connectionString = BrokerHelper.GetConnectionString();

            _logger = new EmptyLogger();
            _serializer = new JsonMessageSerializer();
            _exchange = BrokerConstants.ExchangeName;

            _rabbitMQConnection = new RabbitMQConnectionPersistence(connectionString, _logger);
            _rabbitMQConnection.Connect();
            
            

            _target = new Publisher(_rabbitMQConnection, _serializer, _logger, _exchange);



            //var builder = new ContainerBuilder();

            //builder.Register(ctx => new InternalService()).As<IInternalService>();
            //builder.Register(ctx => new FirstReciver(ctx.Resolve<IInternalService>()))
            //    .AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();

            //builder.Register(ctx => new SecondReciever(ctx.Resolve<IInternalService>()))
            //    .AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();

            //builder.Register(ctx => new IPublisher())

            //_container = builder.Build();


        }

        [Test]
        public void WhenCallPusblishInParallel_ShouldNotThrow()
        {
            var message = _fixture.Create<MessageTwo>();

            _target.Publish(message);

            var message2 = _fixture.Create<MessageTwo>();

            _target.Publish(message2);


            var message3 = _fixture.Create<MessageThree>();

            _target.Publish(message3);

            var taskOne = Task.Run(() => SendCollectionMessageOne());
            var taskTwo = Task.Run(() => SendCollectionMessageTwo());
            var taskThree = Task.Run(() => SendCollectionMessageThree());

            Task.WaitAll(taskOne, taskTwo, taskThree);
            Task.WaitAll(taskOne);

            Assert.IsTrue(true);
        }

        private void SendCollectionMessageOne()
        {
            Parallel.For(0, NumberOfMessages, indexer =>
            {
                var message = _fixture.Create<MessageOne>();

                _target.Publish(message);
            });
        }

        private void SendCollectionMessageTwo()
        {
            Parallel.For(0, NumberOfMessages, indexer =>
            {
                var message = _fixture.Create<MessageTwo>();

                _target.Publish(message);
            });
        }

        private void SendCollectionMessageThree()
        {
            Parallel.For(0, NumberOfMessages, indexer =>
            {
                var message = _fixture.Create<MessageThree>();

                _target.Publish(message);
            });
        }
    }
}
