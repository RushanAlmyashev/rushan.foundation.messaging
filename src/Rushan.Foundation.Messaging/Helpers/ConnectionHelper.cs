namespace Rushan.Foundation.Messaging.Helpers
{
    internal static class ConnectionHelper
    {
        internal static string GetAuthUser(string messageBrokerUri)
        {
            var credentials = messageBrokerUri.SubstringBetween("://", "@");
            var login = credentials.SubstringBefore(":");

            return login;
        }

        private static string SubstringBetween(this string s, string start, string end)
        {
            return s.SubstringAfter(start).SubstringBefore(end);

        }

        private static string SubstringAfter(this string s, string substring)
        {
            var start = s.IndexOf(substring);

            return start == -1 ?
                string.Empty :
                s.Substring(start + substring.Length);
        }

        private static string SubstringBefore(this string s, string substring)
        {
            var substringOffset = s.IndexOf(substring);

            return substringOffset == -1 ?
                s :
                s.Substring(0, substringOffset);
        }
    }
}
