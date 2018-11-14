using System;
using System.IO;
using TxFlow.DebugAdapter.Protocol;

namespace TxFlow.DebugAdapter
{
    internal class Program
	{
		private static bool trace_requests = true;
		private static bool trace_responses = true;
		static string LOG_FILE_PATH = @"E:\__DELETEME__\Logs\" + DateTime.Now.ToFileTime() + ".txt";

		private static void Main(string[] argv)
		{
            if (argv.Length > 0 && "HostService" == argv[0])
            {
                new Service.WorkflowDebugService().HostService(new Uri("http://localhost:8071"));
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
            else
            {

                System.Diagnostics.Debugger.Launch();

                // stdin/stdout
                Program.Log("waiting for debug protocol on stdin/stdout");
                RunSession(Console.OpenStandardInput(), Console.OpenStandardOutput());
            }
        }

		static TextWriter logFile;

		public static void Log(bool predicate, string format, params object[] data)
		{
			if (predicate)
			{
				Log(format, data);
			}
		}
		
		public static void Log(string format, params object[] data)
		{
			try
			{
				Console.Error.WriteLine(format, data);

				if (LOG_FILE_PATH != null)
				{
					if (logFile == null)
					{
						logFile = File.CreateText(LOG_FILE_PATH);
					}

					string msg = string.Format(format, data);
					logFile.WriteLine(string.Format("{0} {1}", DateTime.UtcNow.ToLongTimeString(), msg));
				}
			}
			catch (Exception ex)
			{
				if (LOG_FILE_PATH != null)
				{
					try
					{
						File.WriteAllText(LOG_FILE_PATH + ".err", ex.ToString());
					}
					catch
					{
					}
				}

				throw;
			}
		}

		private static void RunSession(Stream inputStream, Stream outputStream)
		{
			DebugSession debugSession = new TxFlowDebugSession();
			debugSession.TRACE = trace_requests;
			debugSession.TRACE_RESPONSE = trace_responses;
			debugSession.Start(inputStream, outputStream).Wait();

			if (logFile!=null)
			{
				logFile.Flush();
				logFile.Close();
				logFile = null;
			}
		}
	}
}
