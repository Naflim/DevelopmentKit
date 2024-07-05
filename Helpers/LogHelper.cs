using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naflim.DevelopmentKit.Helpers
{
    internal class LogHelper
    {
        private static ILogger? _logger;

        public static void SetLogger(ILogger logger) 
        {
            _logger = logger;
        }

        public static void Info(string message)
        {
            _logger?.LogInformation(message);
        }

        public static void Error(Exception exception)
        {
            _logger?.LogError(exception, exception.Message);
        }
    }
}
