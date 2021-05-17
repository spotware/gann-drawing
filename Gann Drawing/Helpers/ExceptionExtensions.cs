using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace cAlgo.Helpers
{
    public static class ExceptionExtensions
    {
        public static string GetLog(this Exception exception)
        {
            return exception.GetLog(null, false);
        }

        private static string GetLog(this Exception exception, string intend, bool isInnerException)
        {
            var stringBuilder = new StringBuilder();

            intend = intend ?? string.Empty;

            if (isInnerException)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("{0}InnerException:", intend);
            }
            else
            {
                var systemType = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";

                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("OS Version: {0}", Environment.OSVersion.VersionString);
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("System Type: {0}", systemType);
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("{0}Source: {1}", intend, exception.Source);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("{0}Message: {1}", intend, exception.Message);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("{0}TargetSite: {1}", intend, exception.TargetSite);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("{0}Type: {1}", intend, exception.GetType());
            stringBuilder.AppendLine();

            var stackTrace = exception.GetStackTrace(intend);

            stringBuilder.AppendLine(stackTrace);

            stringBuilder.AppendLine();

            if (exception.InnerException != null)
            {
                var innerExceptionIntent = new string(' ', intend.Length + 4);

                var innerExceptionSummary = exception.InnerException.GetLog(innerExceptionIntent, true);

                stringBuilder.Append(innerExceptionSummary);
            }

            return stringBuilder.ToString();
        }

        public static string GetStackTrace(this Exception exception)
        {
            return exception.GetStackTrace(null);
        }

        private static string GetStackTrace(this Exception exception, string intend)
        {
            if (string.IsNullOrEmpty(exception.StackTrace))
            {
                return string.Empty;
            }

            var stackTrace = new StackTrace(exception, true);

            var frames = stackTrace.GetFrames();

            if (frames == null || !frames.Any())
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{0}StackTrace:", intend);
            stringBuilder.AppendLine();

            var tracesIntend = new string(' ', string.IsNullOrEmpty(intend) ? 4 : intend.Length + 4);

            foreach (var stackFram in frames)
            {
                var fileName = stackFram.GetFileName();

                fileName = !string.IsNullOrEmpty(fileName)
                    ? fileName.Substring(fileName.LastIndexOf(@"\", StringComparison.InvariantCultureIgnoreCase) + 1)
                    : string.Empty;

                stringBuilder.AppendFormat("{0}File: {1} | Line: {2} | Col: {3} | Offset: {4} | Method: {5}", tracesIntend, fileName, stackFram.GetFileLineNumber(), stackFram.GetFileColumnNumber(), stackFram.GetILOffset(), stackFram.GetMethod());

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}