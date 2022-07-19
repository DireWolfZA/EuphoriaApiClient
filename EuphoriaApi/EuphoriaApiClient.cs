using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EuphoriaApi.Contacts;
using EuphoriaApi.OutboundCallHistory;

namespace EuphoriaApi {
    public class EuphoriaApiClient : IEuphoriaApiClient {
        private readonly string _baseUrl = "https://api.euphoria.co.za";
        private readonly string _apiUrl = "/files/1010107057/api/Euphoria.Api.aspx"; // from decompiling official APIDLL source code: https://dt46w9nqlye04.cloudfront.net/Downloads/Euphoria.APIDLL-64Bit-1.0.0.23.zip

        private readonly string _tenantName;
        private readonly string _authCode;

        public EuphoriaApiClient(string tenantName, string authCode) {
            if (string.IsNullOrWhiteSpace(tenantName))
                throw new ArgumentNullException(nameof(tenantName));
            if (string.IsNullOrWhiteSpace(authCode))
                throw new ArgumentNullException(nameof(authCode));

            _tenantName = tenantName;
            _authCode = authCode;
        }

        public IOutboundCallHistoryActions OutboundCallHistory => new OutboundCallHistoryActions(this);
        public IContactActions Contacts => new ContactActions(this);

        public async Task<XmlDocument> PostXML(string requestXML) {
            requestXML = "<?xml version='1.0' encoding='utf-8'?>" +
                         $"<XML><Tenant><Name>{_tenantName}</Name><Auth>{_authCode}</Auth></Tenant>" +
                         requestXML + "</XML>";
            string requestString = _baseUrl + _apiUrl;
            string rtn;
            byte[] bytes = Encoding.ASCII.GetBytes(requestXML);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestString);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = bytes.Length;
            httpWebRequest.ContentType = "text/xml; encoding='UTF-8'";

            using (Stream requestStream = await httpWebRequest.GetRequestStreamAsync()) {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync()) {
                if (httpWebResponse.StatusCode == HttpStatusCode.OK) {
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream())) {
                        rtn = await streamReader.ReadToEndAsync();
                    }
                } else {
                    throw new HttpRequestException($"Invalid Response: {httpWebResponse.StatusCode}: {httpWebResponse.StatusDescription}");
                }
            }

            var xmlDocument = new XmlDocument() {
                PreserveWhitespace = false
            };
            try {
                xmlDocument.LoadXml(rtn);
            } catch (XmlException ex) {
                throw new XmlException($"{ex.Message} Invalid XML:{Environment.NewLine}{rtn}", ex, ex.LineNumber, ex.LinePosition);
            }
            return xmlDocument;
        }

        public void ThrowIfError(XmlDocument xmlDocument) {
            var nodes = xmlDocument.DocumentElement.ChildNodes.Cast<XmlNode>();
            XmlNode statusNode = nodes.FirstOrDefault(n => n.Name == "Status");
            XmlNode errorNode = nodes.FirstOrDefault(n => n.Name == "Error");
            if (statusNode != null || errorNode != null) {
                throw new XmlException($"{statusNode?.InnerXml}: {errorNode?.InnerXml}");
            }
        }
    }
}
