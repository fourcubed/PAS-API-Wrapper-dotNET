using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace PASHelpers
{
    public class PASTicket
    {
        PASConnect _oConn = new PASConnect();
        private int? _iMemberID = null;
        private string _sTicketSlug = null;
        private XmlDocument _xmlObject = null;

        public PASTicket(int iMemberID) : this(iMemberID, null)
        {
        }

        public PASTicket(int iMemberID, string sTicketSlug)
        {
            _iMemberID = iMemberID;
            _sTicketSlug = sTicketSlug;

            if (_sTicketSlug != null)
                _xmlObject = _oConn.SendRequest("/publisher_members/" + _iMemberID + "/tickets/" + _sTicketSlug + ".xml", "GET", null, null);
            else
                _xmlObject = GetBlankTicket();
        }

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
        /// This method adds a reply to an existing ticket only. Will NOT work for new tickets.
        /// </summary>
        public XmlDocument AddReply(string sReplyBody)
        {
            if (_sTicketSlug == null)
            {
                return null;
            }
            else
            {
                string sPayload = "<ticket_reply><body>" + System.Security.SecurityElement.Escape(sReplyBody) + "</body></ticket_reply>";
                XmlDocument xml = _oConn.SendRequest("/publisher_members/" + _iMemberID + "/tickets/" + _sTicketSlug + "/reply.xml", "POST", sPayload, null);

                if (_oConn.IsDocumentWithErrors(xml))
                    return _oConn.GetDocumentErrors(xml);
                else
                    return xml;
            }
        }

        private XmlDocument GetBlankTicket()
        {
            string sTemplate = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>"
             + "<ticket>"
             + "  <subject></subject>"
             + "  <body></body>"
             + "</ticket>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sTemplate);
            return xml;
        }

        /// <summary>
        /// This method saves a NEW ticket only.  Will not work for existing tickets.
        /// </summary>
        public XmlDocument SaveNew(string sSubject, string sBody)
        {
            if (_sTicketSlug != null)
            {
                return null;
            }
            else
            {
                XmlNode xN;
                xN = _xmlObject.SelectSingleNode("/ticket/subject");
                xN.InnerText = System.Security.SecurityElement.Escape(sSubject);
                xN = _xmlObject.SelectSingleNode("/ticket/body");
                xN.InnerText = System.Security.SecurityElement.Escape(sBody);

                XmlDocument xml = _oConn.SendRequest("/publisher_members/" + _iMemberID + "/tickets.xml", "POST", _xmlObject.OuterXml, null);

                if (_oConn.IsDocumentWithErrors(xml))
                    return _oConn.GetDocumentErrors(xml);
                else
                    return xml;
            }
        }


    }

}