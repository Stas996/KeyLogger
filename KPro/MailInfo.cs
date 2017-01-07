using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPro
{
    class MailInfo
    {
        public string SmtpServerAddress { get; set; }
        public string MailFrom { get; set; }
        public string Password { get; set; }
        public string MailTo { get; set; }
        public string AttFile { get; set; }

        public MailInfo(string smtpServerAddress, string mailFrom, string password, string mailTo, string attFile = null)
        {
            SmtpServerAddress = smtpServerAddress;
            MailFrom = mailFrom;
            Password = password;
            MailTo = mailTo;
            AttFile = attFile;
        }
    }
}
