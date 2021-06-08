using System;
using System.IO;
using System.Text;
using System.Threading;

namespace cAlgo.Helpers
{
    public interface ILogger
    {
        void Fatal(Exception exception);

        void Log(string text);

        void Print(string text);

        void Print(string format, params object[] args);
    }

    public sealed class Logger : ILogger
    {
        private readonly string _rootDirectoryPath;

        private readonly Action<string> _print;

        public Logger(string rootDirectoryName, Action<string> print)
        {
            if (string.IsNullOrWhiteSpace(rootDirectoryName))
            {
                throw new ArgumentException("rootDirectoryName");
            }

            if (print == null)
            {
                throw new ArgumentException("print");
            }

            _rootDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "cAlgo", rootDirectoryName);

            Directory.CreateDirectory(_rootDirectoryPath);

            _print = print;
        }

        public void Fatal(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("Local Time: {0:o} | UTC Time: {1:o}", DateTime.Now, DateTime.UtcNow);

            stringBuilder.AppendLine(exception.GetLog());

            Log(stringBuilder.ToString());
        }

        public void Log(string text)
        {
            Print(text);

            var filePath = GetFilePath();

            var waitHandleName = filePath.Replace(Path.DirectorySeparatorChar, '_');

            using (var eventWaitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, waitHandleName))
            {
                eventWaitHandle.WaitOne();

                using (var fileStream = File.Open(filePath, FileMode.Append, FileAccess.Write))
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.Write(text);
                    writer.WriteLine("------------------------------------------------------------------------------------------------");
                }

                eventWaitHandle.Set();
            }
        }

        public void Print(string text)
        {
            _print(text);
        }

        public void Print(string format, params object[] args)
        {
            Print(string.Format(format, args));
        }

        private string GetFilePath()
        {
            return Path.Combine(_rootDirectoryPath, string.Format("{0:yyyy-MM-dd}.txt", DateTime.UtcNow));
        }
    }
}