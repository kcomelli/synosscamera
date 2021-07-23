using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synosscamera.testsuite
{
    /// <summary>
    /// Inspectable logger factory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InspectableTestLoggerFactory<T>
    {
        private readonly Mock<ILoggerFactory> _mockedLoggerFactory;
        private readonly Mock<ILogger<T>> _mockedLogger;
        private readonly List<string> _logMessages = new List<string>();
        private readonly List<LogLevel> _logLevels = new List<LogLevel>();

        /// <summary>
        /// Constructor of the class
        /// </summary>
        public InspectableTestLoggerFactory()
        {
            _mockedLoggerFactory = new Mock<ILoggerFactory>();
            _mockedLogger = new Mock<ILogger<T>>();

            _mockedLogger.Setup(f => f.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                        .Callback(new InvocationAction(invocation =>
                        {
                            // some code
                            var level = (LogLevel)invocation.Arguments[0];

                            if (invocation.Arguments[2] is IReadOnlyList<KeyValuePair<string, object>> values)
                            {
                                _logMessages.AddRange(values.Where(v => v.Value is string && !string.IsNullOrWhiteSpace((string)v.Value)).Select(v => v.Value as string));
                                _logLevels.Add(level);
                            }
                        }));

            _mockedLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns<string>((category) =>
            {
                return (ILogger)_mockedLogger.Object;
            });
        }
        /// <summary>
        /// Factory access
        /// </summary>
        public ILoggerFactory Factory => _mockedLoggerFactory.Object;
        /// <summary>
        /// Messages logged
        /// </summary>
        public List<string> Messages => _logMessages;
        /// <summary>
        /// Log levels contained
        /// </summary>
        public List<LogLevel> Levels => _logLevels;
        /// <summary>
        /// Mocked logger
        /// </summary>
        public Mock<ILogger<T>> MockedLogger => _mockedLogger;
        /// <summary>
        /// Create logger
        /// </summary>
        /// <returns></returns>
        public ILogger<T> CreateLogger()
        {
            return Factory.CreateLogger<T>();
        }
        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="categoryName">Logger name</param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return Factory.CreateLogger(categoryName);
        }
        /// <summary>
        /// Clear data
        /// </summary>
        public void Clear()
        {
            _logMessages.Clear();
            _logLevels.Clear();
        }
    }
}
