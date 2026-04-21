using System;

namespace EuphoriaApi.CallRecordings {
    public class CallRecording {
        public string? UID { get; set; }
        public string? MD5 { get; set; }
        public string? Direction { get; set; }
        public string? Source { get; set; }
        public string? Destination { get; set; }
        public DateTime? DateStamp { get; set; }
        public string? FileName { get; set; }
        public int? FileSize { get; set; }
        public string? HasComment { get; set; }
        public int? Flagged { get; set; }
        public string? UniqueID { get; set; }
        public int? IsDeleted { get; set; }
        public string? Status { get; set; }
        public string? CrmTag { get; set; }
        public string? Note { get; set; }
        public string? SIPCallID { get; set; }
    }
}
