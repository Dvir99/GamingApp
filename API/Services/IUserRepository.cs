using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Models;

namespace API.Services
{
    public interface IUserRepository
    {
        void UpdateUser(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<MemberDTO> GetMemberAsync(string username);

    }
}