using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

        public async Task<XmlDocument> PostXML(string requestXML) {
            requestXML = "<?xml version='1.0' encoding='utf-8'?>\n" +
                         "<XML>\n" +
                            "<Tenant>\n\t" +
                                "<Name>" + _tenantName + "</Name>\n\t" +
                                "<Auth>" + _authCode + "</Auth>\n" +
                            "</Tenant>\n" +
                            requestXML +
                         "</XML>";
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
            xmlDocument.LoadXml(rtn);
            return xmlDocument;
        }
    }
}
