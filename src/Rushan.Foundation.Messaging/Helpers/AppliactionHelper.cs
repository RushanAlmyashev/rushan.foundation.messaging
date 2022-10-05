using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Rushan.Foundation.Messaging.Helpers
{
    public static class ApplicationHelper
    {
        private static string _applicationName = string.Empty;

        public static string GetApplicationName()
        {
            if (!string.IsNullOrEmpty(_applicationName))
            {
                return _applicationName;
            }

            var entryAssembly =  Assembly.GetEntryAssembly();

            if (entryAssembly == null)
                return null;

            string version = null;

            var versionInfo = FileVersionInfo.GetVersionInfo(entryAssembly.Location);

            var assemblyInformationalVersion =
                (AssemblyInformationalVersionAttribute)entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).
                    SingleOrDefault();

            if (assemblyInformationalVersion != null)
                version = assemblyInformationalVersion.InformationalVersion;
            else
            {
                if (!string.IsNullOrWhiteSpace(versionInfo.FileVersion))
                    version = versionInfo.FileVersion;
            }

            _applicationName = version == null ?
                versionInfo.ProductName :
                $"{versionInfo.ProductName} v{version}";

            return _applicationName;
        }

        public static string GetMessagingVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }
    }
}
