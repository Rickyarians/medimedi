using System.ComponentModel.DataAnnotations;
using CRUDServices.BindingModel;
using CRUDServices.BindingModel.User;
using CRUDServices.BusinessObjects.Code;
using CRUDServices.BusinessObjects.User;
using CRUDServices.DataAccess.Models;
using CRUDServices.Extensions;
using Microsoft.AspNetCore.Mvc;
namespace CollectionServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IBOUser _boUser;
        public UserController(IBOUser boUser)
        {
            _boUser = boUser;
        }


        /*
         
        ● Create: POST /api/users

        ● Read (All): GET /api/users

        ● Read (Single): GET /api/users/{userId}

        ● Update: PUT /api/users/{userId}

        ● Delete: DELETE /api/users/{userId}
        
        */

        [HttpPost]
        [Route("/api/users")]
        public async Task<IActionResult> InsertUser([FromBody] UserDto request)
        {
            var result = new ResultBase<Users>();
            try
            {
                result = await _boUser.insertUser(request);
                return ActionResultExtensions.SetHttpStatus(result);
            }
            catch (Exception ex)
            {
                return ActionResultExtensions.SetHttpException(result, ex);
            }
        }

        [HttpGet]
        [Route("/api/users")]
        public async Task<IActionResult> getUserList([FromQuery] UserDtoList request)
        {
            var result = new ResultBasePaginated<List<Users>>();
            try
            {
                result = await _boUser.GetListUser(request);
                return ActionResultExtensions.SetHttpStatus(result);
            }
            catch (Exception ex)
            {
                return ActionResultExtensions.SetHttpException(result, ex);
            }
        }


        [HttpGet]
        [Route("/api/users/{userId}")]
        public async Task<IActionResult> getUserDetail([Required]string userId)
        {
            var result = new ResultBase<Users>();
            try
            {
                result = await _boUser.GetDetailUser(userId);
                return ActionResultExtensions.SetHttpStatus(result);
            }
            catch (Exception ex)
            {
                return ActionResultExtensions.SetHttpException(result, ex);
            }
        }

        [HttpPut]
        [Route("/api/users/{userId}")]
        public async Task<IActionResult> updateUser([FromBody] UserDto request, [Required]string userId)
        {
            var result = new ResultBase<Users>();
            try
            {
                result = await _boUser.updateUser(request, userId);
                return ActionResultExtensions.SetHttpStatus(result);
            }
            catch (Exception ex)
            {
                return ActionResultExtensions.SetHttpException(result, ex);
            }
        }


        [HttpDelete]
        [Route("/api/users/{userId}")]
        public async Task<IActionResult> deleteUser([Required]string userId)
        {
            var result = new ResultBase<string>();
            try
            {
                result = await _boUser.DeleteUser(userId);
                return ActionResultExtensions.SetHttpStatus(result);
            }
            catch (Exception ex)
            {
                return ActionResultExtensions.SetHttpException(result, ex);
            }
        }
    }
}
