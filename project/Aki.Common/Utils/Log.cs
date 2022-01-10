using System;

namespace Aki.Common.Utils
{
    public static class Log
    {
        private static string _filepath;

        static Log()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);
            _filepath = VFS.Combine(VFS.Cwd, "./user/logs/modules.log");

            if (VFS.Exists(_filepath))
            {
                VFS.DeleteFile(_filepath);
            }
        }

        public static void Write(string text)
        {
            VFS.WriteTextFile(_filepath, $"{text}{Environment.NewLine}", true);
        }

        private static void Formatted(string type, string text)
        {
            Write($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}|{type}| {text}");
        }

        public static void Info(string text)
        {
            Formatted("INFO", text);
        }

        public static void Warning(string text)
        {
            Formatted("WARNING", text);
        }

        public static void Error(string text)
        {
            Formatted("ERROR", text);
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            Write(ex.Message);
            Write(ex.StackTrace);
        }
    }
}
