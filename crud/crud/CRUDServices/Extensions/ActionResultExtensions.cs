using CRUDServices.BindingModel;
using Microsoft.AspNetCore.Mvc;

namespace CRUDServices.Extensions
{
    public static class ActionResultExtensions
    {
        public static ActionResult SetHttpStatus(ResultBase result)
        {
            if (result.ResultCode is "1000" or "200")
            {
                return new OkObjectResult(result);
            }

            if (result.ResultCode is "1001" or "201")
            {
                return new ObjectResult(result) { StatusCode = 201 };
            }

            if (result.ResultCode == "2002")
            {
                return new NotFoundObjectResult(result);
            }

            if (result.ResultCode is "4000" or "400")
            {
                return new BadRequestObjectResult(result);
            }

            if (result.ResultCode is "4001" or "401")
            {
                return new UnauthorizedObjectResult(result);
            }

            if (result.ResultCode == "9999")
            {
                return new ObjectResult(result) { StatusCode = 500 };
            }

            return new ObjectResult(result) { StatusCode = 500 };
        }

        public static ActionResult SetHttpStatusReturnData<T>(ResultBase<T> result)
        {
            if (result.ResultCode == "1000")
            {
                return new OkObjectResult(result.Data);
            }

            if (result.ResultCode == "1001")
            {
                return new ObjectResult(result.Data) { StatusCode = 201 };
            }

            if (result.ResultCode == "2002")
            {
                return new NotFoundObjectResult(result.Data);
            }

            if (result.ResultCode == "4000")
            {
                return new BadRequestObjectResult(result.Data);
            }

            if (result.ResultCode == "4001")
            {
                return new UnauthorizedObjectResult(result.Data);
            }

            if (result.ResultCode == "9999")
            {
                return new ObjectResult(result.Data) { StatusCode = 500 };
            }

            return new ObjectResult(result.Data) { StatusCode = 500 };
        }

        public static ActionResult SetHttpException(ResultBase result, Exception ex)
        {
            result.SetException(ex);
            return new ObjectResult(result) { StatusCode = 500 };
        }

        public static ActionResult SetHttpExceptionReturnData<T>(ResultBase<T> result, Exception ex)
        {
            result.SetException(ex);
            return new ObjectResult(result.Data) { StatusCode = 500 };
        }
    }
}
