using System;
using System.Windows.Forms;

namespace WinFormsApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new MainForm());
            Application.Run(new Form1());
        }
    }
}
