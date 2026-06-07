using System;
using System.Windows.Forms;

namespace ConfigWell.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new MainForm();
            if (args.Length > 0 && System.IO.File.Exists(args[0]))
                form.OpenFileOnLoad = args[0];
            Application.Run(form);
        }
    }
}
