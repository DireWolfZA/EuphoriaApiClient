using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace EuphoriaApi.CallRecordings {
    public interface ICallRecordingActions {
        //https://apidocs.euphoria.co.za/Pages/PublicSection.aspx?CallName=GetCallRecordings%20and%20GetTenantCallRecordings&Section=Call%20Recordings
        /// <summary>Get call recordings for a tenant</summary>
        /// <param name="pageSize">Size of page with entries</param>
        /// <param name="startAt">Start at number of the array</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <param name="extension">Extension number</param>
        /// <param name="sortOrder">Sorting of calls - ASC or DESC</param>
        /// <param name="searchFilter">Search filter</param>
        Task<List<CallRecording>> GetAsync(int pageSize, int startAt, DateTime? startDate = null, DateTime? endDate = null, int? extension = null, string? sortOrder = null, string? searchFilter = null);
    }

    public class CallRecordingActions : ICallRecordingActions {
        private readonly IEuphoriaApiClient client;
        public CallRecordingActions(IEuphoriaApiClient client) {
            this.client = client;
        }

        public async Task<List<CallRecording>> GetAsync(int pageSize, int startAt, DateTime? startDate = null, DateTime? endDate = null, int? extension = null, string? sortOrder = null, string? searchFilter = null) {
            string request = "<ActionName>GetCallRecordings</ActionName>" +
                "<pageSize>" + pageSize + "</pageSize>" +
                "<startAt>" + startAt + "</startAt>";

            if (startDate != null)
                request += "<StartDate>" + startDate.Value.ToString("yyyy-MM-dd") + "</StartDate>";
            if (endDate != null)
                request += "<EndDate>" + endDate.Value.ToString("yyyy-MM-dd") + "</EndDate>";
            if (extension != null)
                request += "<Extension>" + extension + "</Extension>";
            if (sortOrder != null)
                request += "<direction>" + sortOrder + "</direction>";
            if (searchFilter != null)
                request += "<SearchFilter>" + searchFilter + "</SearchFilter>";

            XmlDocument xmlDoc = await client.PostXML(request);
            client.ThrowIfError(xmlDoc);

            return convert(xmlDoc);
        }
        private List<CallRecording> convert(XmlDocument responseXML) {
            var callRecordings = new List<CallRecording>();

            XmlNode documentElement = responseXML.DocumentElement;
            if (documentElement.HasChildNodes) {
                XmlNodeList childNodes = documentElement.ChildNodes;
                foreach (XmlNode childNode in childNodes) {
                    var callRecording = new CallRecording() {
                        UID = childNode.SelectSingleNode("uID").InnerText.Trim(),
                        MD5 = childNode.SelectSingleNode("MD5").InnerText.Trim(),
                        Direction = childNode.SelectSingleNode("Direction").InnerText.Trim(),
                        Source = childNode.SelectSingleNode("Source").InnerText.Trim(),
                        Destination = childNode.SelectSingleNode("Destination").InnerText.Trim(),
                        DateStamp = DateTime.Parse(childNode.SelectSingleNode("DateStamp").InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture),
                        FileName = childNode.SelectSingleNode("FileName").InnerText.Trim(),
                        FileSize = int.Parse(childNode.SelectSingleNode("FileSize").InnerText.Trim()),
                        HasComment = childNode.SelectSingleNode("HasComment").InnerText.Trim(),
                        Flagged = int.Parse(childNode.SelectSingleNode("Flagged").InnerText.Trim()),
                        UniqueID = childNode.SelectSingleNode("UniqueID").InnerText.Trim(),
                        IsDeleted = int.Parse(childNode.SelectSingleNode("IsDeleted").InnerText.Trim()),
                        Status = childNode.SelectSingleNode("Status").InnerText.Trim(),
                        CrmTag = childNode.SelectSingleNode("CrmTag").InnerText.Trim(),
                        Note = childNode.SelectSingleNode("Note").InnerText.Trim(),
                        SIPCallID = childNode.SelectSingleNode("SipCallId").InnerText.Trim(),
                    };

                    callRecordings.Add(callRecording);
                }
            }

            return callRecordings;
        }
    }
}
