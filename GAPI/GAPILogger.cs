using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace GAPI
{
	public static class Logger
	{
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		public static void Info(string message)
		{
            _logger.Info(message);
        }

        public static void Info()
        {
            Info(String.Empty);
        }

        public static void Info(List<string> lines)
        {
            foreach (string line in lines)
            {
                Info(line);
            }
        }

        public static void Error(Exception ex)
        {
            Error(null, ex);
        }

        public static void Error(string message, Exception ex = null)
        {
            if ((ex != null ) && (ex is WebException))
            {
                if ((ex as WebException).Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (ex as WebException).Response as HttpWebResponse;
                    if (response != null)
                    {
                        message += $" Status code: {(int)response.StatusCode}";
                    }
                }
            }

            _logger.Error(ex, message);
        }

        public static void Warning(string message)
        {
            _logger.Warn(message);
        }

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }
    }
}
