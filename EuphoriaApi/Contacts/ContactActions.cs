using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace EuphoriaApi.Contacts {
    public interface IContactActions {
        //https://apidocs.euphoria.co.za/Pages/PublicSection.aspx?CallName=GetContacts&Section=Contacts
        Task<List<Contact>> GetAsync(int pageSize, int startAt, string tmsUsername, string tmsPassword, string? searchTerm = null);
        //https://apidocs.euphoria.co.za/Pages/PublicSection.aspx?CallName=AddContact&Section=Contacts
        Task<int> AddContact(string tmsUsername, string tmsPassword, Contact contact);
        //https://apidocs.euphoria.co.za/Pages/PublicSection.aspx?CallName=DeleteContact&Section=Contacts
        Task DeleteContact(string tmsUsername, string tmsPassword, string contactID);
    }

    public class ContactActions : IContactActions {
        private readonly IEuphoriaApiClient client;
        public ContactActions(IEuphoriaApiClient client) {
            this.client = client;
        }

        public async Task<List<Contact>> GetAsync(int pageSize, int startAt, string tmsUsername, string tmsPassword, string? searchTerm = null) {
            string request = "<ActionName>GetContacts</ActionName>" +
                    "<pageSize>" + pageSize + "</pageSize>" +
                    "<startAt>" + startAt + "</startAt>" +
                    "<TmsUsername>" + tmsUsername + "</TmsUsername>" +
                    "<TmsPassword>" + tmsPassword + "</TmsPassword>";

            if (searchTerm != null)
                request += "<SearchTerm>" + searchTerm + "</SearchTerm>";

            XmlDocument xmlDoc = await client.PostXML(request);
            client.ThrowIfError(xmlDoc);

            return Convert(xmlDoc);
        }
        private List<Contact> Convert(XmlDocument responseXML) {
            var contacts = new List<Contact>();

            XmlNode documentElement = responseXML.DocumentElement;
            if (documentElement.HasChildNodes) {
                XmlNodeList childNodes = documentElement.ChildNodes;
                foreach (XmlNode childNode in childNodes) {
                    var contact = new Contact() {
                        UniqueID = childNode.SelectSingleNode("uID").InnerText.Trim(),
                        Owner = childNode.SelectSingleNode("Owner").InnerText.Trim(),
                        IsShared = childNode.SelectSingleNode("isShared").InnerText.Trim() == "1",
                        Tenant = childNode.SelectSingleNode("Tenant").InnerText.Trim(),
                        ContactName = childNode.SelectSingleNode("ContactName").InnerText.Trim(),
                        ContactCompany = childNode.SelectSingleNode("ContactCompany").InnerText.Trim(),
                        ContactNumber1 = childNode.SelectSingleNode("ContactNum1").InnerText.Trim(),
                        ContactNumber2 = childNode.SelectSingleNode("ContactNum2").InnerText.Trim(),
                        ContactNumber3 = childNode.SelectSingleNode("ContactNum3").InnerText.Trim(),
                        ContactNumber4 = childNode.SelectSingleNode("ContactNum4").InnerText.Trim(),
                        DataSource = childNode.SelectSingleNode("dataSource").InnerText.Trim(),
                        DateUpdated = childNode.SelectSingleNode("DateUpdated").InnerText.Trim(),
                        DateCreated = childNode.SelectSingleNode("DateCreated").InnerText.Trim()
                    };

                    contacts.Add(contact);
                }
            }

            return contacts;
        }

        public async Task<int> AddContact(string tmsUsername, string tmsPassword, Contact contact) {
            string request = "<ActionName>AddContact</ActionName>" +
                    "<TmsUsername>" + tmsUsername + "</TmsUsername>" +
                    "<TmsPassword>" + tmsPassword + "</TmsPassword>" +
                    "<ContactName><![CDATA[" + contact.ContactName + "]]></ContactName>" +
                    "<ContactCompany><![CDATA[" + contact.ContactCompany + "]]></ContactCompany>" +
                    "<ContactNum1><![CDATA[" + contact.ContactNumber1 + "]]></ContactNum1>";

            if (contact.ContactNumber2 != null)
                request += "<ContactNum2><![CDATA[" + contact.ContactNumber2 + "]]></ContactNum2>";
            if (contact.ContactNumber3 != null)
                request += "<ContactNum3><![CDATA[" + contact.ContactNumber2 + "]]></ContactNum3>";
            if (contact.ContactNumber4 != null)
                request += "<ContactNum4><![CDATA[" + contact.ContactNumber2 + "]]></ContactNum4>";
            request += "<IsShared>" + (contact.IsShared ? "1" : "0") + "</IsShared>";

            XmlDocument xmlDoc = await client.PostXML(request);
            client.ThrowIfError(xmlDoc);

            XmlNode documentElement = xmlDoc.DocumentElement;
            XmlNodeList childNodes = documentElement.ChildNodes;
            var childNodeID = childNodes[1];
            return int.Parse(childNodeID.InnerText.Trim());
        }

        public async Task DeleteContact(string tmsUsername, string tmsPassword, string contactID) {
            string request = "<ActionName>DeleteContact</ActionName>" +
                "<TmsUsername>" + tmsUsername + "</TmsUsername>" +
                "<TmsPassword>" + tmsPassword + "</TmsPassword>" +
                "<contactID>" + contactID + "</contactID>";

            XmlDocument xmlDoc = await client.PostXML(request);
            client.ThrowIfError(xmlDoc);

            XmlNode documentElement = xmlDoc.DocumentElement;
            XmlNodeList childNodes = documentElement.ChildNodes;
            var childNodeRtnVal = childNodes[0];

            if (childNodeRtnVal.InnerText.Trim() != "OK")
                throw new XmlException(childNodeRtnVal.InnerText);
        }
    }
}
