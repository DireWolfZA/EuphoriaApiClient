using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

//https://apidocs.euphoria.co.za/Pages/PublicSection.aspx?CallName=GetOutboundCallingHistory&Section=Call%20Histories
namespace EuphoriaApi.OutboundCallHistory {
    public interface IOutboundCallHistoryActions {
        /// <summary>Get outbound call history</summary>
        /// <param name="pageSize">Size of the page with entries</param>
        /// <param name="startAt">What number of the array to start at</param>
        /// <param name="onlyAnswered">If only answered calls should show</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <param name="extension">Extension number</param>
        Task<List<OutboundCall>> GetAsync(int pageSize, int startAt, bool? onlyAnswered = null, DateTime? startDate = null, DateTime? endDate = null, int? extension = null);
    }

    public class OutboundCallHistoryActions : IOutboundCallHistoryActions {
        private readonly IEuphoriaApiClient client;
        public OutboundCallHistoryActions(IEuphoriaApiClient client) {
            this.client = client;
        }

        public async Task<List<OutboundCall>> GetAsync(int pageSize, int startAt, bool? onlyAnswered = null, DateTime? startDate = null, DateTime? endDate = null, int? extension = null) {
            string request = "<ActionName>GetOutboundCallingHistory</ActionName>" +
                    "<pageSize>" + pageSize + "</pageSize>" +
                    "<startAt>" + startAt + "</startAt>";

            if (onlyAnswered != null)
                request += "<OnlyAnswered>" + (onlyAnswered.Value ? "yes" : "no") + "</OnlyAnswered>";
            if (startDate != null)
                request += "<startDate>" + startDate.Value.ToString("yyyy-MM-dd") + "</startDate>";
            if (endDate != null)
                request += "<endDate>" + endDate.Value.ToString("yyyy-MM-dd") + "</endDate>";
            if (extension != null)
                request += "<extension>" + extension + "</extension>";

            XmlDocument xmlDoc = await client.PostXML(request);
            client.ThrowIfError(xmlDoc);

            return Convert(xmlDoc);
        }

        private List<OutboundCall> Convert(XmlDocument responseXML) {
            var outboundCalls = new List<OutboundCall>();

            XmlNode documentElement = responseXML.DocumentElement;
            if (documentElement.HasChildNodes) {
                XmlNodeList childNodes = documentElement.ChildNodes;
                foreach (XmlNode childNode in childNodes) {
                    var outboundCall = new OutboundCall {
                        UniqueID = childNode.SelectSingleNode("uID").InnerText.Trim(),
                        Extension = childNode.SelectSingleNode("Extension").InnerText.Trim(),
                        ExtensionName = childNode.SelectSingleNode("ExtensionName").InnerText.Trim(),
                        Duration = childNode.SelectSingleNode("Duration").InnerText.Trim(),
                        DialledNumber = childNode.SelectSingleNode("DialedNumber").InnerText.Trim(),
                        StartTime = childNode.SelectSingleNode("StartTime").InnerText.Trim(),
                        Status = childNode.SelectSingleNode("Status").InnerText.Trim(),
                        StatusDescription = childNode.SelectSingleNode("StatusDescription").InnerText.Trim(),
                        CallBill = childNode.SelectSingleNode("CallBill").InnerText.Trim()
                    };

                    outboundCalls.Add(outboundCall);
                }
            }

            return outboundCalls;
        }
    }
}
