using System;
using System.Collections.Generic;
using System.Web;

using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Security.Cryptography;

namespace PASHelpers
{
    /// <summary>
    /// Primarily used to connect to the server and retrieve XML results.
    /// Also includes a few "extra" utility methods.
    /// </summary>
    public class PASConnect
    {
        //get your API token and secret by logging in and visiting https://publisher.pokeraffiliatesolutions.com/api
        private const string _sAPIToken = "YOUR_API_TOKEN";                           // Your "API Token" (provided by PAS) goes here.
        private const string _sAPISecret = "YOUR_API_ACCESS_KEY";                     // Your "API Access Key" (provided by PAS) goes here.
        private const string _sURL = "https://publisher.pokeraffiliatesolutions.com"; // Should not need to change this!

        private const int _iRequestTimeout = 5000;

        
        public PASConnect()
        {

        }

        public XmlDocument GetDocumentErrors(XmlDocument xml)
        {
            XmlDocument xmlErrs = new XmlDocument();
            xmlErrs.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><errors></errors>");
            foreach (XmlNode xn in xml.SelectNodes("//error"))
            {
                XmlNode xNew = xmlErrs.ImportNode(xn, true);
                xmlErrs["errors"].AppendChild(xNew);
            }

            return xmlErrs;
        }

        /// <summary>
        /// Get the encoded HMAC signature as a byte array. Uses SHA1. sAPISecret is the key used by SHA1, other params are concatenated to a data glob. 
        /// </summary>
        private byte[] GetHMACSignature(string sAPIToken, string sMethod, string sPath, string sUnixTimeStamp, string sAPISecret)
        {
            HMACSHA1 oCrypto = new HMACSHA1(System.Text.Encoding.UTF8.GetBytes(sAPISecret));
            string sSigData = sAPIToken + sMethod + sPath + sUnixTimeStamp;

            return oCrypto.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sSigData));
        }

        public static List<string> GetOffers(int iWebsiteID)
        {
            PASConnect oConn = new PASConnect();
            XmlDocument xmlWrapper = oConn.SendRequest("/website_offers.xml", "GET", null, "&website_id=" + iWebsiteID.ToString());

            List<string> lst = new List<string>();
            foreach (XmlNode xn in xmlWrapper.SelectNodes("//offer"))
            {
                lst.Add(xn.InnerXml);
            }

            return lst;
        }

        private string GetSignature(string sMethod, string sPath)
        {
            string sUnixTS = GetUnixTimeStampString();

            return "?api_token=" + _sAPIToken + "&timestamp=" + sUnixTS
                + "&signature="
                + HttpUtility.UrlEncode(Convert.ToBase64String(
                    GetHMACSignature(_sAPIToken, sMethod, sPath, sUnixTS, _sAPISecret)
                    ));
        }

        private string GetUnixTimeStampString()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return ((long)ts.TotalSeconds).ToString();
        }

        public bool IsDocumentWithErrors(XmlDocument xml)
        {
            if (xml.SelectNodes("//error").Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Send a request to the PAS server, using the API credentials defined in the source code.
        /// Path and method are required; you can send "null" for payload and extra GET params when appropriate.
        /// This method does some processing of exceptions. Notably, if a 400 or 404 response includes an error 
        /// message from the server, that message will be returned. Other exceptions will be rethrown.
        /// </summary>
        public XmlDocument SendRequest(string sPath, string sMethod, string sPayload, string sExtraGETParams)
        {
            sPayload = sPayload ?? "";
            sExtraGETParams = sExtraGETParams ?? "";

            string sRequestURL = _sURL + sPath + GetSignature(sMethod, sPath) + sExtraGETParams;
            WebRequest oRequest = WebRequest.Create(sRequestURL);
            oRequest.Method = sMethod;
            oRequest.ContentType = "application/xml";
            oRequest.Timeout = _iRequestTimeout;

            if (sMethod == "GET")
            {
                oRequest.ContentLength = 0;
            }
            else
            {
                byte[] bytesPayload = Encoding.UTF8.GetBytes(sPayload);
                oRequest.ContentLength = bytesPayload.Length;

                using (Stream streamSendData = oRequest.GetRequestStream())
                {
                    streamSendData.Write(bytesPayload, 0, bytesPayload.Length);
                }
            }

            string sXML = "";
            try
            {
                using (WebResponse oResponse = oRequest.GetResponse())
                {
                    using (StreamReader streamResponse = new StreamReader(oResponse.GetResponseStream()))
                    {
                        sXML = streamResponse.ReadToEnd();
                    }
                }
            }
            catch (WebException ew)
            {
                if (ew.Response != null)
                {
                    HttpWebResponse oResponseErr = (HttpWebResponse)ew.Response;
                    if (oResponseErr.StatusCode == HttpStatusCode.BadRequest || oResponseErr.StatusCode == HttpStatusCode.NotFound)
                    {
                        using (StreamReader streamResponse = new StreamReader(oResponseErr.GetResponseStream()))
                        {
                            sXML = streamResponse.ReadToEnd();
                        }
                    }
                }

                //if we didn't retrieve an error message back from the server, rethrow exception.
                if (sXML == "")
                    throw ew;
            }

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(sXML);
            }
            catch
            {
                xmlDoc.LoadXml("<errors><error>Fatal Exception LOCALLY attempting to create XML from server response.</error></errors>");
            }

            return xmlDoc;
        }

    
    }

}