global using NUnit.Framework;

using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Rushan.Foundation.Messaging.Integration.Tests.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Rushan.Foundation.Messaging.Integration.Tests
{

    [SetUpFixture]
    public static class Setup
    {
        [OneTimeSetUp]
        public static async Task RunBeforeAnyTestsAsync()
        {
            await DockerServiceHelper.StartContainerAsync();
        }

        [OneTimeTearDown]
        public static async Task RunAfterAnyTestsAsync()
        {
            await DockerServiceHelper.StopContainerAsync();
        }
    }
}
