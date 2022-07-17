using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Furtiv
{
    internal class FurtivParameters
    {
        public String AppType;
        public String AppName;
        public String FileName;
        public String Arguments;
        public Boolean ShowHelp;
        public Boolean EncodedCommand;
        public String LogFolder;
        public FurtivParameters(string[] args)
        {
            // Default values
            AppType        = "Executable";
            Arguments      = "";
            ShowHelp       = false;
            EncodedCommand = false;
            LogFolder      = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "Furtiv\\Logs");

            // Parse received arguments
            for (int i = 0; i < args.Length; i++)
            {
                if (!String.IsNullOrEmpty(FileName))
                {
                    // We already have started building the commandline -> all remaings args are added to arguments
                    if (args[i].Contains(" "))
                    {
                        Arguments = $"{Arguments} \"{args[i]}\"";
                    }
                    else
                    {
                        Arguments = $"{Arguments} {args[i]}";
                    }
                }
                else
                {
                    // Evaluate lower case argument with leading / replaced by -
                    switch (Regex.Replace(args[i].ToLower(), "^/", "-"))
                    {
                        case "-help":
                        case "-h":
                            ShowHelp = true;
                            break;
                        case "-powershell":
                            AppType = "PowerShell";
                            break;
                        case "-encodedcommand":
                            EncodedCommand = true;
                            break;
                        case "-logfolder":
                            if (i < (args.Length - 1))
                            {
                                i++;
                                LogFolder = args[i];
                            }
                            break;
                        default:
                            FileName = args[i];
                            break;
                    }
                }
            }

            if (!String.IsNullOrEmpty(FileName))
            {
                AppName = Path.GetFileNameWithoutExtension(FileName);
                Arguments = Arguments.Trim();

                if (FileName.Contains(" "))
                {
                    FileName = $"\"{FileName}\"";
                }

                if (AppType == "PowerShell")
                {
                    if (EncodedCommand)
                    {
                        Arguments = $"-ExecutionPolicy Unrestricted -NoLogo -WindowStyle Hidden -EncodedCommand {FileName}";
                    }
                    else
                    {
                        Arguments = $"-ExecutionPolicy Unrestricted -NoLogo -WindowStyle Hidden -Command {FileName} {Arguments}";
                    }

                    FileName = "powershell.exe";
                    Arguments = Arguments.Trim();
                }
            }
        }
    }
}
