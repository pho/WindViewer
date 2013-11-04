using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWActorEdit
{
    public partial class ExceptionForm : Form
    {
        string ExceptionType = string.Empty;
        string ExceptionMessage = string.Empty;
        string ContinueExitMessage = string.Empty;
        string ApplicationStatus = string.Empty;
        string ExceptionStackTrace = string.Empty;

        public ExceptionForm(Exception Ex, bool Continue)
        {
            InitializeComponent();

            ExceptionType += string.Format("{0} in an instance of {1}", Ex.GetType().FullName, Ex.TargetSite.DeclaringType.FullName);

            ExceptionMessage += string.Format("The application has raised {0}. ", PrefixString(ExceptionType));
            if (Ex.Message != string.Empty)
                ExceptionMessage += string.Format("The exception message is as follows: {0}.", Ex.Message.TrimEnd('.', ',', '?', '!'));
            else
                ExceptionMessage += "The exception does not contain a message.";

            if (Ex.InnerException != null)
            {
                ExceptionMessage += string.Format("\n\nThe exception also contains an inner {0} ", Ex.InnerException.GetType().FullName);
                if (Ex.InnerException.Message != string.Empty)
                    ExceptionMessage += string.Format("with the following message: {0}.", Ex.InnerException.Message.TrimEnd('.', ',', '?'));
                else
                    ExceptionMessage += "without a message.";
            }

            if (Continue == true)
                ContinueExitMessage += "To resume the application execution, please press the 'Continue' button.\n";
            ContinueExitMessage += "To exit the application, please press the 'Exit' button.\nClick the 'Details' button to see more information about this error.";

            ApplicationStatus = string.Format("{0} v{1}; assembly version {2}; raised on {3} UTC", Application.ProductName, Application.ProductVersion.ToString(),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                DateTime.UtcNow.ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo));

            if (Ex.InnerException != null && Ex.InnerException.StackTrace != null)
                ExceptionStackTrace += string.Format("Inner Stack Trace:\r\n{0}\r\n", Ex.InnerException.StackTrace);
            ExceptionStackTrace += string.Format("Stack Trace:\r\n{0}", Ex.StackTrace);

            this.Text = string.Format("Exception - {0}", Ex.GetType().FullName);
            lblException.Text = string.Format("{0}\n\n{1}", ExceptionMessage, ContinueExitMessage);
            lblStatusInfo.Text = ApplicationStatus;
            txtDetails.Text = ExceptionStackTrace;

            btnContinue.Enabled = Continue;

            System.Media.SystemSounds.Exclamation.Play();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            btnDetails.Enabled = false;
            txtDetails.Visible = true;

            this.Height += txtDetails.Height;
        }

        private void ExceptionForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                string Spacer = Environment.NewLine + "----------------------------------------" + Environment.NewLine;
                Clipboard.SetText(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", ExceptionType, Spacer, lblStatusInfo.Text, Spacer, ExceptionMessage, Spacer, ExceptionStackTrace, Spacer));
            }
        }

        public static string PrefixString(string Str)
        {
            return string.Format("a{0} {1}", (IsVowel(Str.ToLower()[0]) ? "n" : ""), Str);
        }

        public static bool IsVowel(char Letter)
        {
            return (Letter == 'e' || Letter == 'a' || Letter == 'o' || Letter == 'i' || Letter == 'u');
        }
    }
}
