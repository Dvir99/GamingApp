using API.DTOs;
using API.Helpers;
using API.Models;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);

        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
            .OrderBy(x => x.MessageSent)
            .AsQueryable();

            query = messageParams.Container switch 
            {
                "Inbox" => query.Where(u => u.RecipientUsename == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u =>u.SenderUsename == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsename == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>
            .CreateAsync(messages, messageParams.pageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUsername)
        {
            var query = _context.Messages
            .Where(
                m => m.RecipientUsename == currentUserName && m.RecipientDeleted == false &&
                m.SenderUsename == recipientUsername ||
                m.RecipientUsename == recipientUsername && m.SenderDeleted == false &&
                m.SenderUsename == currentUserName
            )
            .OrderBy(m => m.MessageSent)
            .AsQueryable();

            var unreadMessages = query.Where(m =>m.DateRead == null 
            && m.RecipientUsename == currentUserName).ToList();

            if(unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            return await query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

    }
}