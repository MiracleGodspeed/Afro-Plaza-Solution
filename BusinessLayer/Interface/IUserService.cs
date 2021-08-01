using DataLayer.Dtos;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IUserService
    {
        Task<ResponseModel> PostUser(AddUserDto userDto);
        Task<UserDto> AuthenticateUser(LoginDto dto, string injectkey);
    }
}
