using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MessagesController(IUnitOfWork uow,IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO newMessage)
        {
            var username = User.GetUsername();

            if(username == newMessage.RecipientUsername.ToLower())
            return BadRequest("Can't Send Message To Yourself");

            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _uow.UserRepository.GetUserByUsernameAsync(newMessage.RecipientUsername);

            if(recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsename = sender.UserName,
                RecipientUsename = recipient.UserName,
                Content = newMessage.Content
            };

            _uow.MessageRepository.AddMessage(message);

            if(await _uow.Complete()) return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Failed with message sending");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagedForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, 
            messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await _uow.MessageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _uow.MessageRepository.GetMessage(id);

            if(message.SenderUsename != username && message.RecipientUsename != username) return Unauthorized();

            if(message.SenderUsename == username) message.SenderDeleted = true;
            if(message.RecipientUsename == username) message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted) 
            {
                _uow.MessageRepository.DeleteMessage(message);
            }

            if(await _uow.Complete()) return Ok();

            return BadRequest("Problem Deleting Message");
           
        }
    }
}