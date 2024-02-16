using CRUDServices.BindingModel;

namespace CRUDServices.BusinessObjects.Code
{
    public interface IApiHelper
    {
        Task<ResultBase<string>> GetAsync(string interfaceName, string apiPath, string queryString, Dictionary<string, string>? headerValue);
        Task<ResultBase<T>> GetAsync<T>(string interfaceName, string apiPath, string queryString, Dictionary<string, string>? headerValue);
        Task<ResultBase<T>> GetAsyncResultBase<T>(string interfaceName, string apiPath, string queryString, Dictionary<string, string>? headerValue);
        Task<ResultBase<string>> GetResultFile(string interfaceName, string apiPath, string queryString, Dictionary<string, string>? headerValue);
        Task<ResultBase<string>> PostAsync(ILogger logger, string interfaceName, string apiPath, string queryString, object? value, Dictionary<string, string>? headerValue);
        Task<ResultBase<T>> PostAsync<T>(ILogger logger, string interfaceName, string apiPath, string queryString, object? value, Dictionary<string, string>? headerValue);
        Task<ResultBase<T>> PostResultBaseAsync<T>(string interfaceName, string path, object parameters, string authValue = "");
        Task<ResultBase<T>> GetResultBaseAuthenticatedAsync<T>(string interfaceName, string path, string queryString = "", Dictionary<string, string>? authValue = null);
        Task<ResultBasePaginated<T>> GetResultPaginatedAuthenticatedAsync<T>(string interfaceName, string path, string queryString, Dictionary<string, string>? authValue = null);
        Task<ResultBase<T>> PostResultBaseAuthenticatedAsync<T>(string interfaceName, string path, object parameters, Dictionary<string, string>? authValue = null);
    }

}
