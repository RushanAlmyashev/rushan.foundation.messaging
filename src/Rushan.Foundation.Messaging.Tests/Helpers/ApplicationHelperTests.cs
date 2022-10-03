using Rushan.Foundation.Messaging.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging.Tests.Helpers
{
    
    public class ApplicationHelperTests
    {
        [Test]
        public void WhenCallGetApplicationName_ReturnCurrentAssemblyName()
        {            
            var expected = $"Microsoft.TestHost v17.1.0";
            var actual = ApplicationHelper.GetApplicationName();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
