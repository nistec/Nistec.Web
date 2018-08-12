using Nistec.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Nistec.Web.Security
{

    [Entity("EmailProvider", EntityMode.Config)]
    public class EmailProviderContext : EntityContext<EmailProvider>
    {
        #region ctor


        public EmailProviderContext()
        {
 
        }

        public EmailProviderContext(string Provider)
            : base(Provider)
        {

        }

        #endregion
    }


    public class EmailProvider
    {

        public static EmailProvider Get(string Provider = "Default")
        {
            if (Provider == null)
            {
                throw new Exception("EmailProvider error,null EmailProvider");
            }
            EmailProvider entity = null;
            using (EmailProviderContext context = new EmailProviderContext(Provider))
            {
                entity = context.Entity;// context.ExecuteCommand<EmailProvider>("select * from UserMailProvider where Provider=@Provider", DataParameter.Get("Provider", Provider));
            }
            if (entity == null)
            {
                throw new Exception("EmailProvider Exception, EmailProvider not found");
            }

            return entity;
        }

        public SmtpException SendEmail(string to, string subject, string body, bool enableException)
        {
            MailMessage msg = null;
            try
            {
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = EnableSsl;
                client.Host = Host;
                client.Port = Port;
                if (UseDefaultCredentials)
                {
                    client.UseDefaultCredentials = true;
                }
                else
                {
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(UserName, Password);
                    client.UseDefaultCredentials = false;
                    client.Credentials = credentials;
                }
                msg = new MailMessage();
                msg.From = new MailAddress(Sender);
                msg.To.Add(new MailAddress(to));

                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.Body = body;

                client.Send(msg);

                return new SmtpException(SmtpStatusCode.Ok, "Message Sent");

            }
            catch (System.Net.Mail.SmtpException sex)
            {
                Console.Write("sent error: status: {0}, message:{1}", sex.StatusCode, sex.Message);
                if (enableException)
                    throw sex;
                return sex;
            }
            catch (Exception ex)
            {
                Console.Write("sent error: status: {0}, message:{1}", -1, ex.Message);
                if (enableException)
                    throw ex;
                return new SmtpException(SmtpStatusCode.GeneralFailure, "Message Not Sent");

            }
            finally
            {
                if (msg != null)
                {
                    msg.Dispose();
                }
            }
        }



        [EntityProperty( EntityPropertyType.Key)]
        public string Provider { get; set; }//= "Default";
        [EntityProperty]
        public bool EnableSsl { get; set; }// = true;
        [EntityProperty]
        public string Host { get; set; }//= "smtp.gmail.com";
        [EntityProperty]
        public int Port { get; set; }//= 587;
        [EntityProperty]
        public string UserName { get; set; }
        [EntityProperty]
        public string Sender { get; set; }
        [EntityProperty]
        public string Password { get; set; }
        [EntityProperty]
        public bool UseDefaultCredentials { get; set; }
    }

}
