using Rushan.Foundation.Messaging.Tests.Models;

namespace Rushan.Foundation.Messaging.Tests
{
    public class ActivatorHelper
    {
        [Test]
        public void WhenCallGetType_ReturnExpectedResult()
        {
            var obj = new Dummy();

            var expected = typeof(Dummy);
            var actual = Activator.GetType(obj.GetType().FullName);
            
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
