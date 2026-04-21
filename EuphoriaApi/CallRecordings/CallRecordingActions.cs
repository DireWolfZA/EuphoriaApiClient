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

        //https://apidocs.euphoria.co.za/Pages/PublicSection.aspx?CallName=DownloadCallRecordingByUniqueId&Section=Call%20Recordings
        /// <summary>Returns the recorded WAV file. The WAV file audio stream will contain GSM codec compression. The file can be played back with most audio players</summary>
        /// <param name="uniqueID">Unique ID of the call</param>
        /// <returns>BASE64 encoded text block containing the WAV file (Already decoded to byte array)</returns>
        Task<byte[]> DownloadByID(string uniqueID);

        //https://apidocs.euphoria.co.za/Pages/PublicSection.aspx?CallName=DownloadCallRecordingByFilename&Section=Call%20Recordings
        /// <summary>Returns the recorded WAV file. The WAV file audio stream will contain GSM codec compression. The file can be played back with most audio players.</summary>
        /// <param name="filename">Name of call recording file</param>
        /// <returns>BASE64 encoded text block containing the WAV file (Already decoded to byte array)</returns>
        Task<byte[]> DownloadByFilename(string filename);
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

        public async Task<byte[]> DownloadByID(string uniqueID) {
            string request = "<ActionName>DownloadCallRecordingByUniqueId</ActionName>" +
                "<uniqueId>" + uniqueID + "</uniqueId>";

            XmlDocument xmlDoc = await client.PostXML(request);
            client.ThrowIfError(xmlDoc);

            XmlNode documentElement = xmlDoc.DocumentElement;
            var officialAPIAudioData = documentElement.SelectSingleNode("CallRecording").SelectSingleNode("audioData").InnerText.Trim();
            return Convert.FromBase64String(officialAPIAudioData);
        }

        public async Task<byte[]> DownloadByFilename(string filename) {
            string request = "<ActionName>DownloadCallRecordingByFilename</ActionName>" +
                "<FileName>" + filename + "</FileName>";

            XmlDocument xmlDoc = await client.PostXML(request);
            client.ThrowIfError(xmlDoc);

            XmlNode documentElement = xmlDoc.DocumentElement;
            var officialAPIAudioData = documentElement.SelectSingleNode("CallRecording").SelectSingleNode("audioData").InnerText.Trim();
            return Convert.FromBase64String(officialAPIAudioData);
        }
    }
}
