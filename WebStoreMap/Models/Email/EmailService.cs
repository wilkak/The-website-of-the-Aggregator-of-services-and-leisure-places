using MimeKit;

using System;
using WebStoreMap.Models.ViewModels.Email;

namespace WebStoreMap.Models.Email
{
    public class EmailService
    {
        public void SendEmailCustom(EmailViewModel Model)
        {
            try
            {
                MimeMessage Message = new MimeMessage();
                Message.From.Add(new MailboxAddress(Properties.Settings.Default.Company, Properties.Settings.Default.EmailNameProd)); //отправитель сообщения
                Message.To.Add(new MailboxAddress("", Model.Email)); //адресат сообщения
                Message.Subject = Model.Subject; //тема сообщения

                Message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = Model.HtmlMessage
                };
                using (MailKit.Net.Smtp.SmtpClient Client = new MailKit.Net.Smtp.SmtpClient())
                {
                    Client.Connect(Properties.Settings.Default.HostSmtp, Properties.Settings.Default.EmailPort, Properties.Settings.Default.EmailUseSsl); // порты 465 587

                    Client.Authenticate(Properties.Settings.Default.EmailNameProd, Properties.Settings.Default.EmailPassProd); //логин-пароль от аккаунта
                    _ = Client.Send(Message);

                    Client.Disconnect(true);
                   
                    Console.WriteLine("Сообщение отправлено успешно!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }
    }


}