using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace GAPI
{
	public static class Logger
	{
		private static string logFileName = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"app.log");

        public static bool ConsoleOutput { get; set; } = true;

		public static string LogFileName
		{
			get
			{
				return logFileName;
			}
			set
			{
				logFileName = value;
			}
		}

		public static void WriteToLog(string message)
		{
			WriteToLog(message,null);
		}

        public static void WriteToLog(Exception ex)
        {
            WriteToLog(null, ex);
        }

        public static void WriteToLog(List<string> lines)
		{
			foreach (string line in lines)
			{
				WriteToLog(line);
			}
		}

        public static void WriteToLog()
        {
            WriteToLog(null, null);
        }

        public static void WriteToLog(string message, Exception ex)
		{
            var dt = "[" + DateTime.Now.ToString("yyyy-MM-dd--HH:mm:ss") + "] ";

			if (ex != null)
			{
				message += " ---> Error: " + ex.ToString();

                if (ex is WebException)
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
            }

            using (FileStream fs = new FileStream(logFileName,
                                                 FileMode.Append,
                                                 FileAccess.Write,
                                                 FileShare.Read)
                  )
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(dt + message);
                    sw.Close();
                }
                fs.Close();
            }

            if (ConsoleOutput)
            {
                Console.WriteLine(message);
            }
        }
	}
}
