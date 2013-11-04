using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WWActorEdit
{
    static class Program
    {
        public static ExceptionHandler ExHandler;

        [STAThread]
        static void Main()
        {
            ExHandler = new ExceptionHandler();
            Application.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
