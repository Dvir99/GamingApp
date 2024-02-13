using API.DTOs;
using API.Helpers;
using API.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Services
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUsername);
    

    }
}