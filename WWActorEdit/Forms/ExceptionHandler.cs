using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;

namespace WWActorEdit
{
    class ExceptionHandler
    {
        public ExceptionHandler()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            #if !DEBUG
            ProcessForm(e.Exception);
            #endif
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            #if !DEBUG
            ProcessForm((Exception)e.ExceptionObject, false);
            #endif
        }

        void ProcessForm(Exception Ex, bool Continue = true)
        {
            ExceptionForm ExForm = new ExceptionForm(Ex, Continue);
            
            try
            {
                DialogResult Result = ExForm.ShowDialog();

                switch (Result)
                {
                    case DialogResult.OK:
                        break;
                    case DialogResult.Cancel:
                        Environment.Exit(0);
                        break;
                }
            }
            finally
            {
                ExForm.Dispose();
            }
        }
    }
}
