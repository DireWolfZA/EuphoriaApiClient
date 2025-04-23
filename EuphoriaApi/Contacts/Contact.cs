namespace EuphoriaApi.Contacts {
    public class Contact {
        public string? UniqueID { get; set; }
        public string? Owner { get; set; }
        public bool IsShared { get; set; }
        public string? Tenant { get; set; }
        public string? ContactName { get; set; }
        public string? ContactCompany { get; set; }
        public string? ContactNumber1 { get; set; }
        public string? ContactNumber2 { get; set; }
        public string? ContactNumber3 { get; set; }
        public string? ContactNumber4 { get; set; }
        public string? DataSource { get; set; }
        public string? DateUpdated { get; set; }
        public string? DateCreated { get; set; }
    }
}
