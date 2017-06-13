using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Minion.Core.ServiceModel
{
    public interface IApiService<TService>
    {
        Task<TObject> Call<TObject>(ApiVerbs verb, Expression<Action<TService>> initiator) 
            where TObject : class, new();
    }

    public class ApiService<TService> : IApiService<TService>
    {
        private readonly ILogger _logger;
        private readonly ApiServiceRoutes _services;
        private readonly HttpClient _client;

        public ApiService(ILogger<TService> logger,
            ApiServiceRoutes services)
        {
            _logger = logger;
            _services = services;

            var type = typeof (TService);
            var host = "";
            var isRegistered = _services.TryGetValue(type.Name, out host);

            _client = new HttpClient
            {
                BaseAddress = new Uri(host),
                Timeout = TimeSpan.FromSeconds(30),
            };

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<TObject> Delete<TObject>(Expression<Action<TService>> initiator)
            where TObject : class, new()
        {
            return await Call<TObject>(ApiVerbs.Delete, initiator);
        }

        public async Task<TObject> Get<TObject>(Expression<Action<TService>> initiator)
            where TObject : class, new()
        {
            return await Call<TObject>(ApiVerbs.Get, initiator);
        }

        public async Task<TObject> Post<TObject>(Expression<Action<TService>> initiator)
            where TObject : class, new()
        {
            return await Call<TObject>(ApiVerbs.Post, initiator);
        }

        public async Task<TObject> Put<TObject>(Expression<Action<TService>> initiator)
            where TObject : class, new()
        {
            return await Call<TObject>(ApiVerbs.Put, initiator);
        }

        public async Task<TObject> Call<TObject>(ApiVerbs verb,
            Expression<Action<TService>> initiator)
            where TObject : class, new()
        {
            var body = (MethodCallExpression) initiator.Body;
            var name = body.Method.Name;

            if(body.Arguments.Count > 1 || body.Arguments.Count < 1)
            {
                throw new ArgumentException("The ApiService client operates on services that have a single input parameter object");
            }

            var argument = body.Arguments[0].ExtractValue();
            var content = new StringContent(JsonConvert.SerializeObject(argument), Encoding.UTF8, "application/json");
            HttpResponseMessage request = null;

            switch (verb)
            {
                case ApiVerbs.Delete:
                    request = await _client.DeleteAsync(name);
                    break;

                case ApiVerbs.Post:
                    request = await _client.PostAsync(name, content);
                    break;

                case ApiVerbs.Put:
                    request = await _client.PostAsync(name, content);
                    break;

                case ApiVerbs.Get:
                default:
                    var queryString = name + "?" + argument.GetQueryString();
                    request = await _client.GetAsync(queryString);
                    break;
            }

            var response = await request.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            var output = JsonConvert.DeserializeObject<TObject>(response);

            return output;
        }
    }

    public static class ApiServiceExtensions
    {
        public static IServiceCollection AddApiServiceManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            services.TryAdd(ServiceDescriptor.Singleton(typeof (IApiService<>), typeof (ApiService<>)));

            return services;
        }
    }
}
