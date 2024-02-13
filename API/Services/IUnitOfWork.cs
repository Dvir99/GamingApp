using API.Data;

namespace API.Services
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository {get;}
        IMessageRepository MessageRepository {get;}
        LikesRepository LikesRepository {get;}
        Task<bool> Complete();
        bool HasChanges();

    }
}