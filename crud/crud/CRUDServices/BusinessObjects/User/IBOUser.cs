using System.ComponentModel.DataAnnotations;
using CRUDServices.BindingModel;
using CRUDServices.BindingModel.User;
using CRUDServices.DataAccess.Models;

namespace CRUDServices.BusinessObjects.User
{
    public interface IBOUser
    {
        Task<ResultBasePaginated<List<Users>>> GetListUser(UserDtoList req);

        Task<ResultBase<Users>> GetDetailUser(string userId);

        Task<ResultBase<string>> DeleteUser([Required] string userId);

        Task<ResultBase<Users>> updateUser(UserDto req, string userId);

        Task<ResultBase<Users>> insertUser(UserDto req);
    }
}
