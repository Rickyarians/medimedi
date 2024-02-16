
namespace CRUDServices.BindingModel
{
    public static class ResultBaseExtensions
    {

        public static ResultBase SetError(this ResultBase finalResult, string ResultCode, string ResultMessage)
        {
            finalResult.ResultMessage = ResultMessage;
            finalResult.ResultCode = ResultCode;
            return finalResult;
        }

        public static ResultBase<T> SetError<T>(this ResultBase<T> finalResult, string ResultCode, string ResultMessage)
        {
            finalResult.ResultMessage = ResultMessage;
            finalResult.ResultCode = ResultCode;
            return finalResult;
        }

        public static ResultBase<T> SetError<T>(this ResultBase<T> finalResult, ResultBase resultBase)
            where T : class
        {
            finalResult.ResultMessage = resultBase.ResultMessage;
            finalResult.ResultCode = resultBase.ResultCode;
            return finalResult;
        }
        
        public static ResultBasePaginated<T> SetError<T>(this ResultBasePaginated<T> finalResult, string ResultCode, string ResultMessage)
           where T : class
        {
            finalResult.ResultMessage = ResultMessage;
            finalResult.ResultCode = ResultCode;
            return finalResult;
        }
        
        public static ResultBasePaginated<T> SetError<T>(this ResultBasePaginated<T> finalResult, ResultBase resultBase)
            where T : class
        {
            finalResult.ResultMessage = resultBase.ResultMessage;
            finalResult.ResultCode = resultBase.ResultCode;
            return finalResult;
        }
        public static ResultBase SetNotFound(this ResultBase finalResult, string ResultMessage)
        {
            finalResult.ResultMessage = ResultMessage;
            finalResult.ResultCode = "2002";
            return finalResult;
        }
        public static ResultBase SetNotFound(this ResultBase finalResult)
        {
            finalResult.ResultMessage = "Data not found.";
            finalResult.ResultCode = "2002";
            return finalResult;
        }
        public static ResultBase<T> SetNotFound<T>(this ResultBase<T> finalResult)
            where T : class
        {
            finalResult.ResultMessage = "Data not found.";
            finalResult.ResultCode = "2002";
            return finalResult;
        }
        public static ResultBase<T> SetNotFound<T>(this ResultBase<T> finalResult, string ResultMessage)
            
        {
            finalResult.ResultMessage = ResultMessage;
            finalResult.ResultCode = "2002";
            return finalResult;
        }
        
        public static ResultBase SetBadRequest(this ResultBase finalResult, string ResultMessage)
        {
            finalResult.ResultMessage = ResultMessage;
            finalResult.ResultCode = "4000";
            return finalResult;
        }
        
        public static ResultBase<T> SetBadRequest<T>(this ResultBase<T> finalResult, string ResultMessage)
            where T : class
        {
            finalResult.ResultMessage = ResultMessage;
            finalResult.ResultCode = "4000";
            return finalResult;
        }

        public static ResultBase SetException(this ResultBase finalResult, Exception ex)
        {
            finalResult.ResultMessage = ex?.Message + (ex?.InnerException?.Message ?? "");
            finalResult.ResultCode = "9999";
            return finalResult;
        }

        public static ResultBase<T> SetException<T>(this ResultBase<T> finalResult, Exception ex)
        {
            finalResult.ResultMessage = ex?.Message + (ex?.InnerException?.Message ?? "");
            finalResult.ResultCode = "9999";
            return finalResult;
        }

        public static ResultBasePaginated<List<T>>.Paginated SetPagination<T>(IQueryable<T> query, int page, int size)
            where T : class
        {
            var total = query.Count();
            return new()
            {
                Page = page,
                Size = size,
                Total = total,
                TotalPage = (int)Math.Ceiling((double)total / (double)size),
            };
        }

        public static ResultBasePaginated<List<T>>.Paginated SetPagination<T>(long total, int page, int size)
            where T : class
        {
            return new()
            {
                Page = page,
                Size = size,
                Total = total,
                TotalPage = (int)Math.Ceiling((double)total / (double)size),
            };
        }
        public static ResultBasePaginated<T>.Paginated SetPaginationSingle<T>(long total, int page, int size)
            where T : class
        {
            return new()
            {
                Page = page,
                Size = size,
                Total = total,
                TotalPage = (int)Math.Ceiling((double)total / (double)size),
            };
        }
    }
}
