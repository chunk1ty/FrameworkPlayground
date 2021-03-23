using System;

namespace IoCContainer.Services
{
    public  interface IEmailSender
    {
    }
    
    public class SmtpEmailSender : IEmailSender
    {
        public SmtpEmailSender()
        {
            Console.WriteLine("SmtpEmailSender constructor.");
        } 
    }
}