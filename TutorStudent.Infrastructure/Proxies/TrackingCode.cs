using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using TutorStudent.Domain.ProxyServices;
using TutorStudent.Domain.ProxyServices.Dto;

namespace TutorStudent.Infrastructure.Proxies
{
    
    public class TrackingCode : ITrackingCode
    {
        private readonly HttpClient _client;
        //private const string TrackingCodeUrl = "http://localhost:5029/api/TrackingCodeModelAppService/TrackingCodeModel";
        private const string TrackingCodeUrl = "http://87.248.139.36:5001/api/TrackingCodeModelAppService/TrackingCodeModel";

        
        public TrackingCode()
        {
            var bypassSslHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _client = new HttpClient(bypassSslHandler);
             
        }
        
        public async Task<string> GetTrackingCode(TrackingCodeProxyData trackingCodeProxyData)
        {
            HttpContent content =
                new StringContent(JsonConvert.SerializeObject(trackingCodeProxyData), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(TrackingCodeUrl, content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TrackingCodeProxyResponseDto>(json);
            return result.TrackingCodeGenerated;
        }
    }
}