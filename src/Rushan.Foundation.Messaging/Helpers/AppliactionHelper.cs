using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Rushan.Foundation.Messaging.Helpers
{
    public static class ApplicationHelper
    {
        public static string GetApplicationName()
        {
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

            var applicationName = version == null ?
                versionInfo.ProductName :
                $"{versionInfo.ProductName} v{version}";

            return applicationName;
        }
    }
}
