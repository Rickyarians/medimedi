using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;
using CRUDServices.BindingModel;

namespace CRUDServices.BusinessObjects.Code
{
    public class ApiHelper : IApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly List<BInterfaceAPI> _interfaceApi;
        private readonly Dictionary<string, string> _interfaceTokens;

        public ApiHelper(IConfiguration configuration)
        {
            var clientHandler = new HttpClientHandler();

            if (configuration.GetSection("AppConfig").GetValue<bool>("HttpAllowInvalidHttpSCertificate"))
                clientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            _httpClient = new HttpClient(clientHandler);

            if (configuration.GetSection("AppConfig").GetValue<int>("HttpClientTimeoutInSecond") > 0)
                _httpClient.Timeout =
                    TimeSpan.FromSeconds(configuration.GetSection("AppConfig").GetValue<int>("HttpClientTimeoutInSecond"));

            _interfaceTokens = new Dictionary<string, string>();
            _interfaceApi =
                configuration.GetSection("InterfaceApi").Get<List<BInterfaceAPI>>() ??
                new List<BInterfaceAPI>();
            if (_interfaceApi is not { Count: > 0 }) return;
            foreach (var i in _interfaceApi) _interfaceTokens.Add(i.InterfaceName, "");
        }

        public async Task<ResultBase<string>> GetAsync(string interfaceName, string apiPath, string queryString = "", Dictionary<string, string>? headerValue = null)
        {
            var result = new ResultBase<string>();
            try
            {
                queryString = CleanQueryString(queryString);

                var httpClient = _httpClient;
                httpClient.DefaultRequestHeaders.Clear();

                result = HandleInterfaceAuthorize(interfaceName, out var interfaceApi, result);
                if (result.ResultCode != "1000") return result;

                if (_interfaceApi.Where(x => x.InterfaceName == interfaceName).Select(x => x.DefaultHeader).Any())
                {
                    _interfaceApi.Where(x => x.InterfaceName == interfaceName).Select(x => x.DefaultHeader).FirstOrDefault()?.ForEach(x =>
                    {
                        httpClient.DefaultRequestHeaders.Add(x.Key, x.Value);
                    });
                }

                if (headerValue != null)
                {
                    headerValue.ToList().ForEach(x =>
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(x.Key, x.Value);
                    });
                }
                var data = await httpClient.GetAsync(interfaceApi!.BaseAddress + apiPath + queryString);

                if (!data.IsSuccessStatusCode)
                {
                    if (data.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        result = await HandleRetryGet(interfaceName, apiPath, queryString, interfaceApi, result);
                        return result;
                    }

                    result.SetError(((int)data.StatusCode).ToString(), data.ReasonPhrase ?? "No reason.");
                    return result;
                }

                result.Data = await data.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result;
        }

        public async Task<ResultBase<T>> GetAsync<T>(string interfaceName, string apiPath, string queryString = "", Dictionary<string, string>? headerValue = null)
        {
            var result = new ResultBase<T>();
            try
            {
                var resultString = await GetAsync(interfaceName, apiPath, queryString, headerValue);

                if (resultString.ResultCode != "1000")
                {
                    result.SetError(resultString.ResultCode, resultString.ResultMessage);
                    return result;
                }

                result.Data = JsonConvert.DeserializeObject<T>(resultString.Data!);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result;
        }
        public async Task<ResultBase<T>> GetAsyncResultBase<T>(string interfaceName, string apiPath, string queryString = "", Dictionary<string, string>? headerValue = null)
        {
            var result = new ResultBase<T>();
            try
            {
                var resultString = await GetAsync(interfaceName, apiPath, queryString, headerValue);

                if (resultString.ResultCode != "1000")
                {
                    result.SetError(resultString.ResultCode, resultString.ResultMessage);
                    return result;
                }

                var jo = JObject.Parse(resultString.Data ?? "");
                var jsonData = jo["data"]?.ToString() ?? "";
                result.Data = JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result;
        }

        public async Task<ResultBase<string>> PostAsync(ILogger logger, string interfaceName, string apiPath, string queryString = "",
            object? value = null, Dictionary<string, string>? headerValue = null)
        {
            var result = new ResultBase<string>();
            var url = string.Empty;
            var body = string.Empty;
            try
            {
                queryString = CleanQueryString(queryString);

                var httpClient = _httpClient;
                httpClient.DefaultRequestHeaders.Clear();

                result = HandleInterfaceAuthorize(interfaceName, out var interfaceApi, result);
                if (result.ResultCode != "1000") return result;

                url = interfaceApi!.BaseAddress + apiPath + queryString;
                body = JsonSerializer.Serialize(value);


                if (_interfaceApi.Where(x => x.InterfaceName == interfaceName).Select(x => x.DefaultHeader).Any())
                {
                    _interfaceApi.Where(x => x.InterfaceName == interfaceName).Select(x => x.DefaultHeader).FirstOrDefault()?.ForEach(x =>
                    {
                        httpClient.DefaultRequestHeaders.Add(x.Key, x.Value);
                    });
                }

                //Check if use an header or not
                if (headerValue != null)
                {
                    headerValue.ToList().ForEach(x =>
                    {
                        httpClient.DefaultRequestHeaders.Add(x.Key, x.Value);
                    });
                }
                var data = await _httpClient.PostAsync(url,
                    new StringContent(body, Encoding.UTF8, "application/json"));

                if (!data.IsSuccessStatusCode)
                {
                    if (data.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        result = await HandleRetryPost(interfaceName, apiPath, queryString, value, interfaceApi, result);

                        logger.LogInformation($"{interfaceName} | {url}, {body} | {JsonSerializer.Serialize(result)}");
                        return result;
                    }

                    result.SetError(((int)data.StatusCode).ToString(), data.ReasonPhrase ?? "No reason.");
                    try
                    {
                        result.Data = await data.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                        // ignored
                    }

                    logger.LogInformation($"{interfaceName} | {url}, {body} | {JsonSerializer.Serialize(result)} | {result.Data}");
                    return result;
                }

                result.Data = await data.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            logger.LogInformation($"{interfaceName} | {url}, {body} | {JsonSerializer.Serialize(result)}");
            return result;
        }

        public async Task<ResultBase<T>> PostAsync<T>(ILogger logger, string interfaceName, string apiPath, string queryString = "",
            object? value = null, Dictionary<string, string>? headerValue = null)
        {
            var result = new ResultBase<T>();
            try
            {
                var resultString = await PostAsync(logger, interfaceName, apiPath, queryString, value, headerValue);

                if (resultString.ResultCode != "1000")
                {
                    try
                    {
                        result.Data = JsonConvert.DeserializeObject<T>(resultString.Data!);
                    }
                    catch
                    {
                        // ignored
                    }

                    result.SetError(resultString.ResultCode, resultString.ResultMessage);
                    return result;
                }

                result.Data = JsonConvert.DeserializeObject<T>(resultString.Data!);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result;
        }


        private string CleanQueryString(string queryString)
        {
            if (queryString != "" && !queryString.StartsWith("?")) queryString = "?" + queryString;

            return queryString;
        }

        private async Task<ResultBase<string>> HandleRetryGet(string interfaceName, string apiPath, string queryString,
            BInterfaceAPI interfaceApi, ResultBase<string> result)
        {
            result = LoginInterface(interfaceName, interfaceApi, result);
            if (result.ResultCode != "1000") return result;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _interfaceTokens.GetValueOrDefault(interfaceName));

            var data = await _httpClient.GetAsync(interfaceApi.BaseAddress + apiPath + queryString);
            if (!data.IsSuccessStatusCode)
            {
                result.SetError(((int)data.StatusCode).ToString(), data.ReasonPhrase ?? "No reason.");
                return result;
            }

            result.Data = await data.Content.ReadAsStringAsync();
            return result;
        }

        private async Task<ResultBase<string>> HandleRetryPost(string interfaceName, string apiPath, string queryString,
            object? value, BInterfaceAPI interfaceApi, ResultBase<string> result)
        {
            result = LoginInterface(interfaceName, interfaceApi, result);
            if (result.ResultCode != "1000") return result;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _interfaceTokens.GetValueOrDefault(interfaceName));

            var data = await _httpClient.PostAsync(interfaceApi.BaseAddress + apiPath + queryString,
                new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json"));
            if (!data.IsSuccessStatusCode)
            {
                result.SetError(((int)data.StatusCode).ToString(), data.ReasonPhrase ?? "No reason.");
                return result;
            }

            result.Data = await data.Content.ReadAsStringAsync();
            return result;
        }

        private ResultBase<string> HandleInterfaceAuthorize(string interfaceName, out BInterfaceAPI? interfaceApi,
            ResultBase<string> result)
        {
            interfaceApi = _interfaceApi.FirstOrDefault(x => x.InterfaceName == interfaceName);

            if (interfaceApi == null)
            {
                result.SetError("9999", "Interface Api not registered. Please check configuration file.");
                return result;
            }

            if (!string.IsNullOrWhiteSpace(interfaceApi.LoginPath))
            {
                if (string.IsNullOrWhiteSpace(_interfaceTokens.GetValueOrDefault(interfaceName)))
                {
                    result = LoginInterface(interfaceName, interfaceApi, result);
                    if (result.ResultCode != "1000") return result;
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _interfaceTokens.GetValueOrDefault(interfaceName));
            }

            return result;
        }

        // Declare Private
        public static string EncodeBase64(string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public static string DecodeBase64(string value)
        {
            try
            {
                var valueBytes = Convert.FromBase64String(value);
                return Encoding.UTF8.GetString(valueBytes);
            }
            catch
            {
                return "";
            }
        }


        private ResultBase<string> LoginInterface(string interfaceName, BInterfaceAPI interfaceApi,
            ResultBase<string> result)
        {
            if (string.IsNullOrWhiteSpace(interfaceApi.Payload))
            {
                result.SetError("9999", $"Payload {interfaceName} are not set yet. Please set it up first.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(interfaceApi.UserEnc))
            {
                result.SetError("9999", $"User {interfaceName} are not set yet. Please set it up first.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(interfaceApi.PassEnc))
            {
                result.SetError("9999", $"Password {interfaceName} are not set yet. Please set it up first.");
                return result;
            }

            var requestBody =
                string.Format(interfaceApi.Payload!, DecodeBase64(interfaceApi.UserEnc!), DecodeBase64(interfaceApi.PassEnc!));
            var httpResponse = _httpClient.PostAsync(interfaceApi.BaseAddress + interfaceApi.LoginPath,
                new StringContent(requestBody, Encoding.UTF8, "application/json")).Result;

            if (!httpResponse.IsSuccessStatusCode)
            {
                result.SetError(((int)httpResponse.StatusCode).ToString(), httpResponse.ReasonPhrase ?? "No reason.");
                return result;
            }

            var jo = JObject.Parse(httpResponse.Content.ReadAsStringAsync().Result);
            _interfaceTokens[interfaceName] = GetValueByJsonPath(jo, (interfaceApi.TokenJsonPath?.ToString() ?? ""));
            return result;
        }

        public string GetValueByJsonPath(JObject json, string path)
        {
            var pathSegments = path.Split('.'); // Split the path into segments
            JToken token = json;

            foreach (var segment in pathSegments)
            {
                if (token is JObject obj)
                {
                    if (obj.TryGetValue(segment, StringComparison.InvariantCultureIgnoreCase, out var nextToken))
                    {
                        token = nextToken;
                    }
                    else
                    {
                        return null; // Path segment not found
                    }
                }
                else
                {
                    return null; // Invalid path or structure
                }
            }

            return token?.ToString() ?? "";
        }

        public async Task<ResultBase<T>> PostResultBaseAsync<T>(string interfaceName, string path, object parameters, string authValue = "")
        {
            var result = new ResultBase<T>();
            var auth = new ResultBase<string>();
            try
            {
                var baseUrl = _interfaceApi.FirstOrDefault(x => x.InterfaceName == interfaceName);
                using (var client = new HttpClient { BaseAddress = new Uri(baseUrl?.BaseAddress) })
                {
                    string serializedDto = JsonConvert.SerializeObject(parameters);

                    var inputMessage = new HttpRequestMessage
                    {
                        Content = new StringContent(serializedDto, Encoding.UTF8, "application/json")
                    };
                      auth = HandleInterfaceAuthorize(interfaceName, out var interfaceApi, auth);
                    if (result.ResultCode != "1000") return result;
                    inputMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.Data);
                    client.DefaultRequestHeaders.Authorization =
                                 new AuthenticationHeaderValue("Bearer", _interfaceTokens.GetValueOrDefault(interfaceName));
                   

                    var response = await client.PostAsync(baseUrl.BaseAddress + path, inputMessage.Content);
                    //var message = response.Result;

                    if (response.IsSuccessStatusCode)
                    {

                        var contentRes = await response.Content.ReadAsStringAsync(); //await response.Content.ReadAsStringAsync(); //response.Result.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<ResultBase<T>>(contentRes!);
                    }
                    else
                    {
                        result.SetError("4000", "Invalid Credentials");
                        return result;
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                return result;
            }
        }

        public async Task<ResultBase<T>> GetResultBaseAuthenticatedAsync<T>(string interfaceName, string path, string queryString = "", Dictionary<string, string>? authValue = null)
        {
            var result = new ResultBase<T>();
            try
            {
                var baseUrl = _interfaceApi.FirstOrDefault(x => x.InterfaceName == interfaceName);
                using (var client = new HttpClient { BaseAddress = new Uri(baseUrl?.BaseAddress) })
                {
                    if (authValue != null)
                    {
                        authValue.ToList().ForEach(x =>
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation(x.Key, x.Value);
                        });
                    }

                    var response = client.GetAsync(path + queryString);
                    var message = response.Result;

                    var contents = await response.Result.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResultBase<T>>(contents!);

                    return result;
                }
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                return result;
            }
        }


        public async Task<ResultBase<string>> GetResultFile(string interfaceName, string path, string queryString = "", Dictionary<string, string>? authValue = null)
        {
            var result = new ResultBase<string> ();
            try
            {
                var baseUrl = _interfaceApi.FirstOrDefault(x => x.InterfaceName == interfaceName);
                using (var client = new HttpClient { BaseAddress = new Uri(baseUrl?.BaseAddress) })
                {
                    if (authValue != null)
                    {
                        authValue.ToList().ForEach(x =>
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation(x.Key, x.Value);
                        });
                    } else
                    {
                        if (_interfaceApi.Where(x => x.InterfaceName == interfaceName).Select(x => x.DefaultHeader).Any())
                        {
                            _interfaceApi.Where(x => x.InterfaceName == interfaceName).Select(x => x.DefaultHeader).FirstOrDefault()?.ForEach(x =>
                            {
                                client.DefaultRequestHeaders.Add(x.Key, x.Value);
                            });
                        }
                    }

                    var response = client.GetAsync(path + queryString);
                    var message = response.Result;
                    var fileContent = await response.Result.Content.ReadAsByteArrayAsync();

                    // Convert the byte array to a Base64 string
                    var base64String = Convert.ToBase64String(fileContent);
                    result.Data = base64String;
             
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                return result;
            }
        }




        public async Task<ResultBase<T>> PostResultBaseAuthenticatedAsync<T>(string interfaceName, string path, object parameters, Dictionary<string, string>? authValue = null)
        {
            var result = new ResultBase<T>();
            try
            {
                var baseUrl = _interfaceApi.FirstOrDefault(x => x.InterfaceName == interfaceName);
                using (var client = new HttpClient { BaseAddress = new Uri(baseUrl?.BaseAddress) })
                {
                    string serializedDto = JsonConvert.SerializeObject(parameters);

                    var inputMessage = new HttpRequestMessage
                    {
                        Content = new StringContent(serializedDto, Encoding.UTF8, "application/json")
                    };

                    if (authValue != null)
                    {
                        authValue.ToList().ForEach(x =>
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation(x.Key, x.Value);
                        });
                    } else
                    {
                       

                        if (_interfaceApi.Where(x => x.InterfaceName == interfaceName).Select(x => x.DefaultHeader).Any())
                        {
                            _interfaceApi.Where(x => x.InterfaceName == interfaceName).Select(x => x.DefaultHeader).FirstOrDefault()?.ForEach(x =>
                            {
                                client.DefaultRequestHeaders.Add(x.Key, x.Value);
                            });
                        }
                    }

                    inputMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authValue);

                    var response = await client.PostAsync(baseUrl.BaseAddress + path, inputMessage.Content);
                    //var message = response.Result;

                    if (response.IsSuccessStatusCode)
                    {

                        var contentRes = await response.Content.ReadAsStringAsync(); //await response.Content.ReadAsStringAsync(); //response.Result.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<ResultBase<T>>(contentRes!);
                    }
                    else
                    {
                        result.SetError("4000", "Invalid Credentials");
                        return result;
                    }


                    return result;
                }

            }
            catch (Exception ex)
            {
                result.SetException(ex);
                return result;
            }
        }

        public async Task<ResultBasePaginated<T>> GetResultPaginatedAuthenticatedAsync<T>(string interfaceName, string path, string queryString, Dictionary<string, string>? authValue = null)
        {
            var result = new ResultBasePaginated<T>();
            try
            {
                var baseUrl = _interfaceApi.FirstOrDefault(x => x.InterfaceName == interfaceName);
                using (var client = new HttpClient { BaseAddress = new Uri(baseUrl?.BaseAddress) })
                {
                    if (authValue != null)
                    {
                        authValue.ToList().ForEach(x =>
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation(x.Key, x.Value);
                        });
                    }

                    var response = client.GetAsync(path + queryString);
                    var message = response.Result;

                    var contents = await response.Result.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResultBasePaginated<T>>(contents!);

                    return result;
                }
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                return result;
            }
        }
    }
}
