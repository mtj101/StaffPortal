using System;
using StaffPortal.Models;

namespace StaffPortal.Business
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public ApplicationUser Sender { get; set; }
        public ApplicationUser Receiver { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }

        public static Message Create(ApplicationUser sender, ApplicationUser receiver, string subject, string body)
        {
            var message = new Message
            {
                Created = DateTime.UtcNow,
                Sender = sender,
                Receiver = receiver,
                Subject = subject,
                Body = body,
                IsRead = false
            };
            return message;
        }
    }
}