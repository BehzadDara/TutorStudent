using System.Threading.Tasks;
using TutorStudent.Domain.ProxyServices.Dto;

namespace TutorStudent.Domain.ProxyServices
{
    public interface ITrackingCode
    {
        Task<string> GetTrackingCode(TrackingCodeProxyData trackingCodeProxyData);
    }
}