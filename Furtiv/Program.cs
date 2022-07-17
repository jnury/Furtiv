using System;
using System.Windows.Forms;

namespace Furtiv
{
    internal static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FurtivAppContext(args));
        }
    }
}
