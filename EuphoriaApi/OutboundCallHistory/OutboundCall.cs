namespace EuphoriaApi.OutboundCallHistory {
    public class OutboundCall {
        public string UniqueID { get; set; }
        public string Extension { get; set; }
        public string ExtensionName { get; set; }
        public string Duration { get; set; }
        public string DialledNumber { get; set; }
        public string StartTime { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string CallBill { get; set; }
    }
}
