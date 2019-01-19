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
            _logger.Error(ex.ToString());
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }
    }
}
