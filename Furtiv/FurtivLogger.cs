using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace Furtiv
{
    internal class FurtivLogger
    {
        private String LogFolder;
        private String FurtivLogFilePath;
        private String AppOutputLogFilePath;
        private String AppErrorLogFilePath;
        private Boolean DecodeCliXml = false;
        public Boolean LoggingEnabled = true;
        public Boolean AppLoggingEnabled = false;

        public FurtivLogger(FurtivParameters Parameters)
        {
            DateTime Now = DateTime.Now;

            LogFolder = Parameters.LogFolder;
            
            FurtivLogFilePath = Path.Combine(LogFolder, $"Furtiv_{Now:yyyyMMdd}.log");

            if (!String.IsNullOrEmpty(Parameters.AppName))
            {
                AppOutputLogFilePath = Path.Combine(LogFolder, $"{Parameters.AppName}_{Now:yyyyMMdd_HHmmss}.log");
                AppErrorLogFilePath = Path.Combine(LogFolder, $"{Parameters.AppName}_{Now:yyyyMMdd_HHmmss}_Error.log");
                AppLoggingEnabled = true;
            }

            // Create log folder if it doesn't exist
            try
            {
                Directory.CreateDirectory(LogFolder);
            }
            catch
            {
                LoggingEnabled = false;
                AppLoggingEnabled = false;
            }

            if (Parameters.AppType == "PowerShell")
            {
                DecodeCliXml = true;
            }
        }

        private void OutFile(String FilePath, String Message)
        {
            Process currentProcess = Process.GetCurrentProcess();

            Message = $"{DateTime.Now:yyyy.MM.dd HH:mm:ss} - {currentProcess.Id} - {Message} \r\n";

            using (var mutex = new Mutex(false, Path.GetFileName(FilePath)))
            {
                // Wait a maximum of 500 milliseconds for writing
                if (mutex.WaitOne(500))
                {
                    File.AppendAllText(FilePath, Message);
                    mutex.ReleaseMutex();
                }
            }
        }

        // Log in Furtiv main log file
        public void Log(String Message)
        {
            if (LoggingEnabled)
            {
                OutFile(FurtivLogFilePath, Message);
            }
        }

        // Log in application log file
        public void AppOutputLog(String Message)
        {
            if (AppLoggingEnabled)
            {
                OutFile(AppOutputLogFilePath, Message);
            }
        }

        // Log in application error log file
        public void AppErrorLog(String Message)
        {
            if (AppLoggingEnabled)
            {
                if (DecodeCliXml)
                {
                    XmlDocument xmlMessage = new XmlDocument();
                    String level;
                    String rawMessage;
                    String cleanMessage;

                    try
                    {
                        xmlMessage.LoadXml(Message);
                        XmlNodeList nodeList = xmlMessage.GetElementsByTagName("S");
                        foreach (XmlNode node in nodeList)
                        {
                            XmlNode xmlLevel = node.Attributes.GetNamedItem("S");
                            if (xmlLevel != null)
                            {
                                switch (xmlLevel.Value)
                                {
                                    case "error":
                                        level = "Error";
                                        break;
                                    case "warning":
                                        level = "Warning";
                                        break;
                                    default:
                                        level = xmlLevel.Value;
                                        break;
                                }
                                
                                rawMessage = node.FirstChild.Value;
                                cleanMessage = Regex.Replace(rawMessage, "_x000D_|_x000A_", ""); // Remove LF and CR
                                OutFile(AppErrorLogFilePath, level + " - " + cleanMessage);
                            }
                        }
                    }
                    catch
                    {
                        // Nothing to do here :-/
                    }
                }
                else
                {
                    OutFile(AppErrorLogFilePath, Message);
                }
            }
        }
    }
}
