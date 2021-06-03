using System;

namespace VoidNetworking
{
    public static class VoidLog
    {
        private const string Header = "VoidNetworking generated log.  \t";
        private const string ErrorFormat = Header+"VoidError: {0}";
        private const string WarningFormat = Header+"VoidWarning: {0}";
        private const string MessageFormat = Header+"VoidMessage: {0}";

        public static event Action<string> OnError;
        public static event Action<string> OnWarning;
        public static event Action<string> OnMessage;

        internal static void LogError(string message)
        {
            OnError?.Invoke(string.Format(ErrorFormat, message));
        }

        internal static void LogWarning(string message)
        {
            OnWarning?.Invoke(string.Format(WarningFormat, message));
        }

        internal static void LogMessage(string message)
        {
            OnMessage?.Invoke(string.Format(MessageFormat, message));
        }
    }
}
