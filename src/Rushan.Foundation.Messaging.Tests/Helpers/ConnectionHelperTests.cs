using Rushan.Foundation.Messaging.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging.Tests.Helpers
{
    public class ConnectionHelperTests
    {
        [Test]
        public void WhenCallGetApplicationName_ReturnCurrentAssemblyName()
        {
            var messageBrokerUri = "amqp://guest:guest@localhost/";
            
            var expected = "guest";            
            var actual = ConnectionHelper.GetAuthUser(messageBrokerUri);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
