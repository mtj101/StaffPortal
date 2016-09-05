using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StaffPortal.Business;
using StaffPortal.Models;
using StaffPortal.Service;
using Microsoft.AspNet.Identity;


namespace StaffPortal.Controllers
{
    //[Authorize]
    [RoutePrefix("message")]
    public class MessageApiController : ApiController
    {
        private MessageService _messageService = new MessageService();

        [Route("all")]
        [HttpGet]
        public IHttpActionResult GetAllMessagesForUser()
        {
            var userId = User.Identity.GetUserId();
            try
            {
                var messages = _messageService.GetAllMessagesForUser(userId, MessageType.Received).Select(m => new { m.Subject, m.Body }).ToList();
                return Ok(messages);
            }
            catch (ArgumentException e)
            {
                return BadRequest();
            }

        }

        [Route("{messageId:int}")]
        [HttpGet]
        public IHttpActionResult GetMessageById(int messageId)
        {
            var userId = User.Identity.GetUserId();
            try
            {
                var message = _messageService.GetMessageForUser(userId, messageId);
                var jsonMessage = new
                {
                    message.Subject,
                    message.Body,
                    Created = message.Created.ToString("dd/MM/yyyy HH:mm"),
                    Sender = $"{message.Sender.StaffMember.FirstNames} {message.Sender.StaffMember.Surname}",
                    SenderId = message.Sender.Id,
                    Receiver = $"{message.Receiver.StaffMember.FirstNames} {message.Receiver.StaffMember.Surname}",
                    ReceiverId = message.Receiver.Id,
                    IsRead = message.IsRead
                };
                return Ok(jsonMessage);
            }
            catch (ArgumentException e)
            {
                return BadRequest();
            }

        }

        [Route("read/{messageId:int}")]
        [HttpPut]
        public IHttpActionResult MarkMessageRead(int messageId)
        {
            try
            {
                _messageService.MarkAsRead(messageId);
            }
            catch (ArgumentException e)
            {
                return BadRequest();
            }

            return Ok();
        }

    }
}

