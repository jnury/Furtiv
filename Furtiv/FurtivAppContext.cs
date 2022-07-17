using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace Furtiv
{
    internal class FurtivAppContext : ApplicationContext
    {
        private FurtivParameters Parameters;
        private FurtivLogger Logger;
        private Process ConsoleAppProcess;

        public FurtivAppContext(string[] args)
        {
            Parameters = new FurtivParameters(args);
            Logger = new FurtivLogger(Parameters);

            // Display help message if needed
            if (Parameters.ShowHelp || args.Length == 0)
            {
                string caption = "Furtiv App Console Wrapper Help";
                string helpText = @"Furtiv is a console application wrapper for Windows. It's intended to launch command line tools (like PowerShell scripts) without displaying a window.

Command line parameters:

-Help (or -h): display help message
-LogFolder path_to_log: create logs in specified log folder
-PowerShell: use PowerShell interpreter for the command
-EncodedCommand: use PowerShell EncodedCommand argument

See full documentation at https://github.com/jnury/Furtiv";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(helpText, caption, buttons);
                Environment.Exit(0);
            }

            // Exits if no command line was provided
            else if(String.IsNullOrEmpty(Parameters.FileName))
            {

                Logger.Log($"ERROR: No executable was provided. Received arguments: '{String.Join(" ", args)}'");

                string caption = "Furtiv App Console Wrapper";
                string errorMessage = "No executable was provided";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(errorMessage, caption, buttons, MessageBoxIcon.Error);

                Environment.Exit(1);
            }

            // Launch Console if everything is OK
            ConsoleAppProcess = new Process();
            ConsoleAppProcess.StartInfo.FileName = Parameters.FileName;

            if (Parameters.Arguments.Length > 0)
            {
                Logger.Log($"Launching Console App '{Parameters.FileName}' with arguments: '{Parameters.Arguments}'");
                ConsoleAppProcess.StartInfo.Arguments = Parameters.Arguments;
            }
            else
            {
                Logger.Log($"Launching Console App '{Parameters.FileName}' without argument");
            }

            // Process start info
            ConsoleAppProcess.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            ConsoleAppProcess.StartInfo.UseShellExecute        = false;
            ConsoleAppProcess.StartInfo.CreateNoWindow         = true;
            ConsoleAppProcess.StartInfo.RedirectStandardOutput = true;
            ConsoleAppProcess.StartInfo.RedirectStandardError  = true;

            // Process event handlers
            ConsoleAppProcess.EnableRaisingEvents = true;
            ConsoleAppProcess.Exited             += new EventHandler(ConsoleAppExited);
            ConsoleAppProcess.OutputDataReceived += new DataReceivedEventHandler(ConsoleAppOutputReceived);
            ConsoleAppProcess.ErrorDataReceived  += new DataReceivedEventHandler(ConsoleAppErrorReceived);

            try
            {
                ConsoleAppProcess.Start();
                ConsoleAppProcess.BeginOutputReadLine();
                ConsoleAppProcess.BeginErrorReadLine();
                Logger.Log($"Successfully created Console App process with ID {ConsoleAppProcess.Id}");
            }
            catch (Exception e)
            {
                Logger.Log($"ERROR: Failed to start process. Message: {e.Message}");
            }
        }
    
        // Event when Console App exits
        private void ConsoleAppExited(object sender, EventArgs e)
        {
            int ConsoleAppExitCode = ConsoleAppProcess.ExitCode;
            Logger.Log($"Console App exited with code {ConsoleAppExitCode}");
            Environment.Exit(ConsoleAppExitCode);
        }

        // Event when there is something to read in Console App StdOut
        private void ConsoleAppOutputReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Logger.AppOutputLog(e.Data);
            }
        }

        // Event when there is something to read in Console App StdErr
        private void ConsoleAppErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Logger.AppErrorLog(e.Data);
            }
        }
    }
}
