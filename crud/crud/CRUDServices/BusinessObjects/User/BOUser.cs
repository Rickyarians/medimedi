using System.ComponentModel.DataAnnotations;
using CRUDServices.BindingModel;
using CRUDServices.BindingModel.User;
using CRUDServices.BusinessObjects.User;
using CRUDServices.DataAccess.Context;
using CRUDServices.DataAccess.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CRUDServices.BusinessObjects.Code;
public class BOUser : IBOUser
{

    private readonly CRUDContext _dbContext;

    public BOUser(CRUDContext dbContext
        )
    {
        _dbContext = dbContext;

    }



    public async Task<ResultBasePaginated<List<Users>>> GetListUser(UserDtoList req)
    {
        var result = new ResultBasePaginated<List<Users>>();

        try
        {
            var query = _dbContext.Users.Where(x => x.IsDeleted == false).AsNoTracking();


            query = query
              .Skip((req.page - 1) * req.size)
            .Take(req.size);

            result.Pagination = ResultBaseExtensions.SetPagination(query, req.page, req.size);
            result.Data = await query.ToListAsync();

            if (!result.Data.Any())
            {
                result.SetNotFound();
            }
        }
        catch (Exception ex)
        {
            result.SetException(ex);
        }

        return result;
    }

    public async Task<ResultBase<Users>> GetDetailUser(string userId)
    {
        var result = new ResultBase<Users>();

        try
        {
            Users query = await _dbContext.Users.Where(x => x.IsDeleted == false && x.UserId.ToString() == userId).AsNoTracking().FirstOrDefaultAsync();

            result.Data = query;
            if (result.Data == null)
            {
                result.SetNotFound();
            }
        }
        catch (Exception ex)
        {
            result.SetException(ex);
        }

        return result;
    }


    public async Task<ResultBase<string>> DeleteUser(string userId)
    {
        var result = new ResultBase<string>();

        try
        {
            Users query = await _dbContext.Users.Where(x => x.UserId.ToString() == userId).FirstOrDefaultAsync();

            query.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
            result.Data = query.UserId.ToString();
            if (result.Data == null)
            {
                result.SetNotFound();
            }
        }
        catch (Exception ex)
        {
            result.SetException(ex);
        }

        return result;
    }

    public async Task<ResultBase<Users>> updateUser(UserDto req, string userId)
    {
        var result = new ResultBase<Users>();

        try
        {
            Users query = await _dbContext.Users.Where(x => x.IsDeleted == false && x.UserId.ToString() == userId).FirstOrDefaultAsync();


            if(query == null)
            {
                result.SetNotFound();
            } else
            {
                query.FirstName = req.FirstName;
                query.LastName = req.LastName;
                query.Email = req.Email;

                await _dbContext.SaveChangesAsync();

                result.Data = query;
                result.ResultMessage = "Success Update Data";
            }
        }
        catch (Exception ex)
        {
            result.SetException(ex);
        }

        return result;
    }


    public async Task<ResultBase<Users>> insertUser(UserDto req)
    {
        var result = new ResultBase<Users>();

        try
        {
            Users data = new Users()
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email
            };

            await _dbContext.Users.AddAsync(data);
            await _dbContext.SaveChangesAsync();

            result.Data = data;
        }
        catch (Exception ex)
        {
            result.SetException(ex);
        }

        return result;
    }
}