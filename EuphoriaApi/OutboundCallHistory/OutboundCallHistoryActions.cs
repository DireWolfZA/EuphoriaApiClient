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
        Task<List<OutboundCall>> GetAsync(int pageSize, int startAt, bool onlyAnswered, DateTime startDate, DateTime? endDate = null, int? extension = null);
    }
}
