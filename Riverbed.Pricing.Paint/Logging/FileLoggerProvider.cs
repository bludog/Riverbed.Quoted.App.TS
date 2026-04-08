using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace Riverbed.Pricing.Paint.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _path;
        
        public FileLoggerProvider(string path)
        {
            _path = path;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(_path, categoryName);
        }
        
        public void Dispose() { }
        
        private class FileLogger : ILogger
        {
            private readonly string _path;
            private readonly string _categoryName;
            private static readonly object _lock = new object();
            
            public FileLogger(string path, string categoryName)
            {
                _path = path;
                _categoryName = categoryName;
            }
            
            public IDisposable BeginScope<TState>(TState state) => default!;
            
            public bool IsEnabled(LogLevel logLevel) => true;
            
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                var formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
                var actualPath = _path.Replace("{Date}", formattedDate);
                
                var logEntry = new StringBuilder();
                logEntry.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] [{_categoryName}] ");
                logEntry.Append(formatter(state, exception));
                
                if (exception != null)
                {
                    logEntry.AppendLine();
                    logEntry.Append(exception.ToString());
                }
                
                lock (_lock)
                {
                    File.AppendAllText(actualPath, logEntry.ToString() + Environment.NewLine);
                }
            }
        }
    }
    
    // Extension method to simplify registering the file logger
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string path)
        {
            builder.AddProvider(new FileLoggerProvider(path));
            return builder;
        }
    }
}