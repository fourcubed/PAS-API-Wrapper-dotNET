using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for PASMembers
/// </summary>
namespace PASHelpers
{
    public class PASMember
    {
        PASConnect _oConn = new PASConnect();
        private int? _iMemberID = null;
        private XmlDocument _xmlObject = null;

        public XmlDocument XmlObject
        {
            get { return _xmlObject; }
            set { _xmlObject = value; }
        }

        public bool HasErrors
        {
            get { return _oConn.IsDocumentWithErrors(_xmlObject); }
        }

        /// <summary>
        /// Return errors as an XML document.
        /// </summary>
        public XmlDocument ErrorsAsXML
        {
            get { return _oConn.GetDocumentErrors(_xmlObject); }
        }

        /// <summary>
        /// Return errors as a list of error messages.
        /// </summary>
        public List<string> ErrorMessageList
        {
            get
            {
                List<string> lst = new List<string>();
                foreach (XmlNode xn in _oConn.GetDocumentErrors(_xmlObject).SelectNodes("//error"))
                {
                    lst.Add(xn.InnerText);
                }
                return lst;
            }
        }

        /// <summary>
        /// Create blank new member. Must complete required fields before attempting to save.
        /// </summary>
        public PASMember()
        {
            _xmlObject = GetBlankMember();
        }

        /// <summary>
        /// Retrieve member from PAS servers. 
        /// </summary>
        public PASMember(int iMemberID)
        {
            _iMemberID = iMemberID;
            _xmlObject = _oConn.SendRequest("/publisher_members/" + _iMemberID + ".xml", "GET", null, null);
        }

        public XmlDocument AddTracker(string sTrackerTag, int iOfferID)
        {
            string sPayload = "<member_tracker><identifier>"
                + System.Security.SecurityElement.Escape(sTrackerTag) + "</identifier>"
                + "<website_offer_id>" + iOfferID + "</website_offer_id></member_tracker>";

            return _oConn.SendRequest("/publisher_members/" + _iMemberID + "/publisher_member_trackers.xml", "POST", sPayload, null);
        }

        private XmlDocument GetBlankMember()
        {
            string sTemplate = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>"
                + "<member>"
                + "  <outside_id/>"
                + "  <website_id></website_id>"
                + "  <login></login>"
                + "  <password></password>"
                + "  <email></email>"
                + "  <first_name></first_name>"
                + "  <last_name></last_name>"
                + "  <referring_member_id></referring_member_id>"
                + "  <address_1/>"
                + "  <address_2/>"
                + "  <aim/>"
                + "  <skype/>"
                + "  <phone/>"
                + "  <yahoo/>"
                + "  <city/>"
                + "  <state/>"
                + "  <zip_code/>"
                + "  <country/>"
                + "  <date_of_birth></date_of_birth>"
                + "  <gender/>"
                + "  <member_trackers>"
                + "  </member_trackers>"
                + "</member>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sTemplate);
            return xml;
        }

        /// <summary>
        /// Static method. Return member object matching the login name, or null if no matches (or multiple matches) on name.
        /// </summary>
        public static PASMember GetMemberByLogin(string sLogin)
        {
            PASConnect oConn = new PASConnect();
            XmlDocument xmlMembers = oConn.SendRequest("/publisher_members.xml", "GET", null, "&search[order]=&criteria_0=login&operator_0=equals&query_0=" + sLogin);
            XmlNode xmlWrapper = xmlMembers.SelectSingleNode("//members");

            if (xmlWrapper.Attributes["total_entries"].InnerText == "1")
            {
                // int.Parse should never fail here. if it does, exception is appropriate.
                return new PASMember(int.Parse(xmlWrapper["member"]["id"].InnerText));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a dictionary with balance info containing keys "available_balance", "total_balance", "problem_balance", with Decimal values.
        /// </summary>
        public Dictionary<string, Decimal> GetMemberBalances()
        {
            XmlDocument xml = _oConn.SendRequest("/publisher_members/" + _iMemberID + "/cashouts/new.xml", "GET", null, null);

            decimal decAvail;
            decimal decTotal;
            decimal decProblem;
            //note that decimal.tryparse will set the decimal value to zero if the input does not parse.
            decimal.TryParse(xml.SelectSingleNode("//available_balance").InnerText, out decAvail);
            decimal.TryParse(xml.SelectSingleNode("//total_balance").InnerText, out decTotal);
            decimal.TryParse(xml.SelectSingleNode("//problem_balance").InnerText, out decProblem);

            Dictionary<string, Decimal> dict = new Dictionary<string, decimal>();
            dict["available_balance"] = decAvail;
            dict["total_balance"] = decTotal;
            dict["problem_balance"] = decProblem;

            return dict;
        }

        /// <summary>
        /// Returns a dictionary of dictionaries for cashout methods. Outer dictionary has keys for cashout method ids.
        /// Inner dictionary has keys for "id", "name", "minimum_amount", "instructions".
        /// Uses strings for ID, even though ID is parseable to int.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> GetMemberCashoutMethods()
        {
            XmlDocument xml = _oConn.SendRequest("/publisher_members/" + _iMemberID + "/cashouts/new.xml", "GET", null, null);

            Dictionary<string, Dictionary<string, string>> dictOuter = new Dictionary<string, Dictionary<string, string>>();
            foreach (XmlNode xmln in xml.SelectNodes("//cashout_method"))
            {
                Dictionary<string, string> dictInner = new Dictionary<string, string>();
                string sID = xmln["id"].InnerText;
                dictInner["id"] = sID;
                dictInner["name"] = xmln["name"].InnerText;
                dictInner["minimum_amount"] = xmln["minimum_amount"].InnerText;
                dictInner["instructions"] = xmln["instructions"].InnerText;
                dictOuter[sID] = dictInner;
            }

            return dictOuter;
        }

        public XmlDocument GetMemberCashouts(DateTime? dtStart, DateTime? dtEnd)
        {
            string sExtraGET = "";

            if (dtStart.HasValue && dtEnd.HasValue)
                sExtraGET = "&start_date=" + dtStart.Value.ToString("yyyy-MM-dd") + "&end_date" + dtEnd.Value.ToString("yyyy-MM-dd");

            return _oConn.SendRequest("/publisher_members/" + _iMemberID + "/cashouts.xml", "GET", null, sExtraGET);
        }

        public XmlDocument GetMemberPayments(DateTime? dtStart, DateTime? dtEnd, int? iPage)
        {
            string sExtraGET = "";
            iPage = iPage ?? 1;

            if (dtStart.HasValue && dtEnd.HasValue)
                sExtraGET = "&start_date=" + dtStart.Value.ToString("yyyy-MM-dd") + "&end_date" + dtEnd.Value.ToString("yyyy-MM-dd") + "&page=" + iPage.ToString();
            else
                sExtraGET = "&page=" + iPage;

            return _oConn.SendRequest("/publisher_members/" + _iMemberID + "/payments.xml", "GET", null, sExtraGET);
        }

        public XmlDocument GetMemberRAFStats(DateTime? dtStart, DateTime? dtEnd)
        {
            string sExtraGET = "";

            if (dtStart.HasValue && dtEnd.HasValue)
                sExtraGET = "&start_date=" + dtStart.Value.ToString("yyyy-MM-dd") + "&end_date" + dtEnd.Value.ToString("yyyy-MM-dd");

            return _oConn.SendRequest("/publisher_members/" + _iMemberID + "/referrals.xml", "GET", null, sExtraGET);
        }

        public XmlDocument GetMemberStats(DateTime? dtStart, DateTime? dtEnd) 
        {
            string sExtraGET = "";
            
            if (dtStart.HasValue && dtEnd.HasValue)
                sExtraGET = "&start_date=" + dtStart.Value.ToString("yyyy-MM-dd") + "&end_date" + dtEnd.Value.ToString("yyyy-MM-dd");

            return _oConn.SendRequest("/publisher_members/" + _iMemberID + "/stats.xml", "GET", null, sExtraGET);
        }

        /// <summary>
        /// Returns a dictionary of member stats, with each day in date range as a key.
        /// Do not cross month boundaries with your date range or otherwise try to return > 1 month with this method, it will time out.
        /// </summary>
        public Dictionary<DateTime, XmlDocument> GetMemberStatsByDay(DateTime dtStart, DateTime dtEnd)
        {
            //strip off any time portion that might have been sent
            dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);
            dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day);

            //if someone accidentally breaks year/month boundaries, return null, don't try to process.
            if (dtStart.Month != dtEnd.Month || dtStart.Year != dtEnd.Year)
                return null;

            //if end date is in future, we can truncate end date to today instead, no need to ask for future stats.
            if (dtEnd > DateTime.Today)
                dtEnd = DateTime.Today;

            Dictionary<DateTime, XmlDocument> dict = new Dictionary<DateTime, XmlDocument>();
            DateTime dtCurrent = dtStart;
            while (dtCurrent <= dtEnd)
            {
                string sExtraGET = "&start_date=" + dtCurrent.ToString("yyyy-MM-dd") + "&end_date=" + dtCurrent.ToString("yyyy-MM-dd");
                XmlDocument xml = _oConn.SendRequest("/publisher_members/" + _iMemberID + "/stats.xml", "GET", null, sExtraGET);
                dict[dtCurrent] = xml;

                dtCurrent = dtCurrent.AddDays(1);
            }

            return dict;
        }

        /// <summary>
        /// Note that this method can cause multiple requests to the PAS server. Use the
        /// alternate method GetMemberTicketsXML if you don't need PASTicket objects.
        /// </summary>
        public List<PASTicket> GetMemberTickets()
        {
            XmlDocument xml = _oConn.SendRequest("/publisher_members/" + _iMemberID + "/tickets.xml", "GET", null, null);

            List<PASTicket> lst = new List<PASTicket>();
            foreach (XmlNode xn in xml.SelectNodes("//ticket"))
            {
                PASTicket oT = new PASTicket(_iMemberID.Value, xn["id"].InnerText);
                lst.Add(oT);
            }

            return lst;
        }

        public XmlDocument GetMemberTicketsXML()
        {
            XmlDocument xml = _oConn.SendRequest("/publisher_members/" + _iMemberID + "/tickets.xml", "GET", null, null);

            return xml;
        }

        public List<string> GetMemberTrackers()
        {
            List<string> lst = new List<string>();
            foreach (XmlNode xn in _xmlObject.SelectNodes("//member_tracker"))
            {
                lst.Add(xn.InnerXml);
            }

            return lst;
        }

        public string GetRemoteAuthToken()
        {
            XmlDocument xml = _oConn.SendRequest("/remote_auth.xml", "POST", "<member_id>" + _iMemberID + "</member_id>", null);

            return xml["remote_auth_token"].InnerText;
        }

        public XmlDocument NewMemberCashout(string sCashoutDetails, int iCashoutMethodID)
        {
            string sPayload = "<cashout><details>"
                + System.Security.SecurityElement.Escape(sCashoutDetails) + "</details><cashout_method_id>"
                + iCashoutMethodID + "</cashout_method_id></cashout>";

            return _oConn.SendRequest("/publisher_members/" + _iMemberID + "/cashouts.xml", "POST", sPayload, null);
        }

        /// <summary>
        /// Return results of save, or xml errors if save fails.
        /// </summary>
        public XmlDocument Save()
        {
            XmlDocument xml;
            if (this._xmlObject.SelectSingleNode("//id") != null)
                xml = _oConn.SendRequest("/publisher_members/" + _iMemberID + ".xml", "PUT", this._xmlObject.OuterXml, null);
            else
                xml = _oConn.SendRequest("/publisher_members.xml", "POST", this._xmlObject.OuterXml, null);

            if (!_oConn.IsDocumentWithErrors(xml))
                return xml;
            else
                return _oConn.GetDocumentErrors(xml);
        }

        public XmlDocument UpdateMemberElement(string sName, string sValue)
        {
            XmlNode xN = _xmlObject.SelectSingleNode("/member/" + sName);
            xN.InnerText = System.Security.SecurityElement.Escape(sValue);

            return _xmlObject;
        }

    }
}
