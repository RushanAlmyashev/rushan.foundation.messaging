using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Rushan.Foundation.Messaging.Helpers
{
    internal static class ApplicationHelper
    {
        private static string _applicationName = string.Empty;
        private static string _messagingVersion = string.Empty;

        /// <summary>
        /// Get an executed product name
        /// </summary>
        /// <returns>application name</returns>
        internal static string GetApplicationName()
        {
            if (!string.IsNullOrEmpty(_applicationName))
            {
                return _applicationName;
            }

            var entryAssembly =  Assembly.GetEntryAssembly();

            if (entryAssembly == null)
            {
                return null;
            }

            string version = null;

            var versionInfo = FileVersionInfo.GetVersionInfo(entryAssembly.Location);

            var assemblyInformationalVersion =
                (AssemblyInformationalVersionAttribute)entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).
                    SingleOrDefault();

            if (assemblyInformationalVersion != null)
            {
                version = assemblyInformationalVersion.InformationalVersion;
            }
            else if(!string.IsNullOrWhiteSpace(versionInfo.FileVersion))
            {
                version = versionInfo.FileVersion;
            }

            _applicationName = version == null ?
                versionInfo.ProductName :
                $"{versionInfo.ProductName} v{version}";

            return _applicationName;
        }

        /// <summary>
        /// Get Version of current Messaging assembly
        /// </summary>
        /// <returns>Get Messaging Version</returns>
        internal static string GetMessagingVersion()
        {
            if (!string.IsNullOrEmpty(_messagingVersion))
            {
                return _messagingVersion;
            }

            _messagingVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            return _messagingVersion;
        }
    }
}
