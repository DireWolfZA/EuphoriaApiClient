using System.Threading.Tasks;
using System.Xml;
using EuphoriaApi.OutboundCallHistory;

namespace EuphoriaApi {
    public interface IEuphoriaApiClient {
        Task<XmlDocument> PostXML(string requestXml);

        IOutboundCallHistoryActions OutboundCallHistory { get; }
    }
}
