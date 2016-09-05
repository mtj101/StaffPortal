using System;
using StaffPortal.Models;
using StaffPortal.Business;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace StaffPortal.Service
{
    public class MessageService
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public void MarkAsRead(int messageId)
        {
            var message = _db.Messages.Find(messageId);
            if (message == null)
                throw new ArgumentException(nameof(messageId));

            message.IsRead = true;
            _db.SaveChanges();
        }

        private void AddMessage(Message message)
        {
            _db.Messages.Add(message);
            _db.SaveChanges();
        }

        public void SendMessage(string senderId, string receiverId, string subject, string body)
        {
            var sender = _db.Users.Find(senderId);
            var receiver = _db.Users.Find(receiverId);

            if (sender == null || receiver == null)
                throw new ArgumentException();

            var message = new Message
            {
                Created = DateTime.UtcNow,
                Sender = sender,
                Receiver = receiver,
                Subject = subject,
                Body = body,
                IsRead = false
            };

            AddMessage(message);
        }

        public void DeleteMessages(int[] messageIds, string userId)
        {
            foreach (var messageId in messageIds)
            {
                var message = _db.Messages.Include(m => m.Receiver).Single(m => m.Id == messageId);

                // ensure message is only deleted by logged-in user
                if (message.Receiver.Id == userId)
                {
                    _db.Messages.Remove(message);
                }
            }

            _db.SaveChanges();
        }

        public Message GetMessageForUser(string userId, int messageId)
        {
            
            var message = _db.Messages
                .Include(m => m.Receiver)
                .Include(m => m.Receiver.StaffMember)
                .Include(m => m.Sender)
                .Include(m => m.Sender.StaffMember)
                .Where(m => m.Receiver.Id == userId || m.Sender.Id == userId) // ensure only owner of message can retrieve it
                .SingleOrDefault(m => m.Id == messageId);

            if (message != null)
            {
                return message;
            }
            throw new ArgumentException();
        }

        public List<Message> GetAllMessagesForUser(string userId, MessageType messageType)
        {
            var user = _db.Users.Find(userId);

            if (user == null)
                throw new ArgumentException();

            List<Message> messages = new List<Message>();

            if (messageType == MessageType.Received)
            {
                messages = _db.Messages
                    .Include(m => m.Receiver)
                    .Include(m => m.Receiver.StaffMember)
                    .Include(m => m.Sender)
                    .Include(m => m.Sender.StaffMember)
                    .Where(m => m.Receiver.Id == user.Id)
                    .OrderByDescending(m => m.Created)
                    .ToList();
            }
            if (messageType == MessageType.Sent)
            {
                messages = _db.Messages
                    .Include(m => m.Receiver)
                    .Include(m => m.Receiver.StaffMember)
                    .Include(m => m.Sender)
                    .Include(m => m.Sender.StaffMember)
                    .Where(m => m.Sender.Id == user.Id)
                    .OrderByDescending(m => m.Created)
                    .ToList();
            }


            return messages;
        }

        public int GetUnreadTotalForUser(string userId)
        {
            var user = _db.Users.Find(userId);

            int total = _db.Messages.Where(m => m.Receiver.Id == user.Id).Where(m => !m.IsRead).Count();

            return total;
        }
    }

    public enum MessageType
    {
        Sent,
        Received
    }
}