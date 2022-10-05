using Autofac;
using AutoFixture;
using Rushan.Foundation.Messaging.Tests.InteranlServices;
using Rushan.Foundation.Messaging.Tests.Messages;
using Rushan.Foundation.Messaging.Tests.Recievers;

namespace Rushan.Foundation.Messaging.Tests
{
    [TestFixture]
    public class AutofacComponentResolverTests
    {
        private Autofac.IContainer _container;
        private Fixture _fixture;

        public AutofacComponentResolverTests()
        {
            _fixture = new Fixture();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();

            builder.Register(ctx => new InternalService()).As<IInternalService>();
            builder.Register(ctx => new FirstReciver(ctx.Resolve<IInternalService>()))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();



            builder.Register<Func<MessageOne, IMessageReceiver<MessageOne>>>(ctx =>
            {                
                var cc = ctx.Resolve<IComponentContext>();               

                return message => cc.Resolve<IMessageReceiver<MessageOne>>();
            }).As<Func<MessageOne, IMessageReceiver<MessageOne>>>();

            _container = builder.Build();
        }

        [Test]
        public void ResolverTest()
        {
            var messageOne = _fixture.Create<MessageOne>();

            var recieversFunc = _container.Resolve<Func<MessageOne, IMessageReceiver<MessageOne>>>();            

            var reciver = recieversFunc(messageOne);
            reciver.ReceiveMessageAsync(messageOne);
        }
    }
}
