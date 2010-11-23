using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text;
using System.Xml;
using PASHelpers;

public partial class _Default : System.Web.UI.Page 
{
    private PASHelpers.PASConnect _oPASConnect;

    private DateTime _dtStart = new DateTime(2010, 10, 03);
    private DateTime _dtEnd = new DateTime(2010, 10, 27);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //do setup stuff
        }
        else
        {
            _oPASConnect = new PASConnect();
        }

        litStartDate.Text = _dtStart.ToShortDateString();
        litEndDate.Text = _dtEnd.ToShortDateString();
    }

    private string GetHTMLEncodedErrors(List<string> sErrors)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string s in sErrors)
        {
            sb.Append(HttpUtility.HtmlEncode(s) + "<br />");
        }

        return sb.ToString();
    }

    private PASMember GetMemberByID(string sMemberID, out string sErrorMessage)
    {
        sErrorMessage = "";
        int iMemberID;

        if (!int.TryParse(txtGetMemberByID.Text, out iMemberID))
        {
            sErrorMessage = "Invalid Member ID";
            return null;
        }

        return new PASMember(iMemberID);
    }

    protected void btnGetOffers_Click(object sender, EventArgs e)
    {
        int iWebsiteID;
        if (!int.TryParse(txtWebsiteID.Text, out iWebsiteID))
        {
            litResults.Text = "Invalid Website ID";
            return;
        }

        List<string> lstOffers = PASConnect.GetOffers(iWebsiteID);

        StringBuilder sb = new StringBuilder();
        foreach(string s in lstOffers)
        {
            sb.Append(HttpUtility.HtmlEncode(s) + "<br /><br />");
        }

        litResults.Text = sb.ToString();
    }

    protected void btnGetMemberByLogin_Click(object sender, EventArgs e)
    {
        if (this.txtGetMemberByLogin.Text.Trim() == "")
        {
            litResults.Text = "Invalid Login Name";
            return;
        }

        PASMember oMember = PASMember.GetMemberByLogin(txtGetMemberByLogin.Text);

        if (oMember != null)
            litResults.Text = HttpUtility.HtmlEncode(oMember.XmlObject.OuterXml);
        else
            litResults.Text = "Null value returned indicating no results (or multiple results) for " + txtGetMemberByLogin.Text + ".";
    }

    protected void btnGetMemberByID_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.XmlObject.OuterXml);
    }

    protected void btnGetMemberStatsByDateRangeFixed_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberStats(_dtStart, _dtEnd).OuterXml);
    }

    protected void btnGetMemberStatsByDateRangeBlank_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberStats(null, null).OuterXml);
    }

    protected void btnGetMemberStatsByDay_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        {
            Dictionary<DateTime, XmlDocument> dictStatsByDate = oMember.GetMemberStatsByDay(_dtStart, _dtEnd);
            StringBuilder sb = new StringBuilder();
            foreach (DateTime k in dictStatsByDate.Keys)
            {
                sb.Append(k.ToShortDateString() + "<br />" + HttpUtility.HtmlEncode(dictStatsByDate[k].OuterXml) + "<br />");
            }
            litResults.Text = sb.ToString();
        }
    }

    protected void btnGetMemberRAFStatsByDateRangeFixed_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberRAFStats(_dtStart, _dtEnd).OuterXml);
    }

    protected void btnGetMemberRAFStatsByDateRangeBlank_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberRAFStats(null, null).OuterXml);
    }

    protected void btnGetMemberPaymentsByDateRangeFixed_Click(object sender, EventArgs e)
    {
        int? iPage = null;
        int iParsed;
        if (int.TryParse(txtPage.Text, out iParsed))
            iPage = iParsed;

        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberPayments(_dtStart, _dtEnd, iPage).OuterXml);
    }

    protected void btnGetMemberPaymentsByDateRangeBlank_Click(object sender, EventArgs e)
    {
        int? iPage = null;
        int iParsed;
        if (int.TryParse(txtPage.Text, out iParsed))
            iPage = iParsed;

        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberPayments(null, null, iPage).OuterXml);
    }

    protected void btnGetMemberCashoutsByDateRangeFixed_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberCashouts(_dtStart, _dtEnd).OuterXml);
    }

    protected void btnGetMemberCashoutsByDateRangeBlank_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberCashouts(null, null).OuterXml);
    }

    protected void btnGetMemberBalances_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
        {
            Dictionary<string, decimal> dictBalances = oMember.GetMemberBalances();
            StringBuilder sb = new StringBuilder();
            foreach (string k in dictBalances.Keys)
            {
                sb.Append(k + " : " + dictBalances[k].ToString() + "<br />");
            }
            litResults.Text = sb.ToString();
        }
    }

    protected void btnGetMemberCashoutMethods_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
        {
            Dictionary<string, Dictionary<string, string>> dictCOM = oMember.GetMemberCashoutMethods();
            StringBuilder sb = new StringBuilder();
            foreach (string k in dictCOM.Keys)
            {
                sb.Append(k + "<br />");
                foreach (string ik in dictCOM[k].Keys)
                {
                    sb.Append(ik + " : " + dictCOM[k][ik] + "<br />");
                }
                sb.Append("<br />");
            }
            litResults.Text = sb.ToString();
        }
    }

    protected void btnGetRemoteAuthToken_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = oMember.GetRemoteAuthToken();
    }

    protected void btnSaveNewMember_Click(object sender, EventArgs e)
    {
        int iWebsiteID;
        if (!int.TryParse(txtWebsiteID.Text, out iWebsiteID))
        {
            litResults.Text = "Invalid Website ID";
            return;
        }

        if (txtSaveNewMemberLoginName.Text.Trim() == "")
        {
            litResults.Text = "Invalid Login Name";
            return;
        }


        PASMember oMember = new PASMember();

        oMember.UpdateMemberElement("website_id", txtWebsiteID.Text);                           //Required
        oMember.UpdateMemberElement("login", txtSaveNewMemberLoginName.Text);                   //Required    
        oMember.UpdateMemberElement("password", "examplepasswordonly");                         //Required; recommended to set to a long random string.
        oMember.UpdateMemberElement("email", txtSaveNewMemberLoginName.Text + "@example.com");  //Required
        oMember.UpdateMemberElement("first_name", "FirstName");
        oMember.UpdateMemberElement("last_name", "LastName");

        XmlDocument xml = oMember.Save();
        if (xml != null)
            litResults.Text = HttpUtility.HtmlEncode(oMember.Save().OuterXml);
        else
            litResults.Text = "error saving new member";
    }

    protected void btnSaveEditedMember_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        oMember.UpdateMemberElement("outside_id", txtOutsideID.Text);
        XmlDocument xml = oMember.Save();
        if (xml != null)
            litResults.Text = HttpUtility.HtmlEncode(xml.OuterXml);
        else
            litResults.Text = "error saving edited member";

    }

    protected void btnSaveNewTicket_Click(object sender, EventArgs e)
    {
        int iMemberID;
        if (!int.TryParse(txtGetMemberByID.Text, out iMemberID))
        {
            litResults.Text = "Invalid Member ID";
            return;
        }

        PASTicket oTicket = new PASTicket(iMemberID);
        XmlDocument xml = oTicket.SaveNew("TEST TICKET ONLY PLEASE IGNORE  " + txtNewTicketSubjectTag.Text, "This is a test ticket only. Please ignore.");

        if (xml != null)
            litResults.Text = HttpUtility.HtmlEncode(xml.OuterXml);
        else
            litResults.Text = "error saving new ticket";

    }

    protected void btnGetMemberTicketsXML_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberTicketsXML().OuterXml);
    }

    protected void btnGetMemberTickets_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
        {
            List<PASTicket> lst = oMember.GetMemberTickets();

            StringBuilder sb = new StringBuilder();
            foreach (PASTicket t in lst)
            {
                sb.Append(t.XmlObject.SelectSingleNode("//id").InnerText + "<br />");
                sb.Append(t.XmlObject.SelectSingleNode("//subject").InnerText + "<br />");
                sb.Append(t.XmlObject.SelectSingleNode("//body").InnerText + "<br />");
                sb.Append("<br />");
            }
            litResults.Text = sb.ToString();
        }

    }

    protected void btnAddTicketReply_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
        {
            List<PASTicket> lstT = oMember.GetMemberTickets();

            if (lstT.Count == 0)
            {
                litResults.Text = "No tickets found for this member.";
                return;
            }
            else
            {
                lstT[0].AddReply("This is a test reply to a test ticket. Please ignore. " + txtAddReplyBodyTag.Text);
                litResults.Text = HttpUtility.HtmlEncode(oMember.GetMemberTicketsXML().OuterXml);
            }
        }

    }

    protected void btnAddTracker_Click(object sender, EventArgs e)
    {
        int iOfferID;
        if (!int.TryParse(txtAddTrackerOfferID.Text, out iOfferID))
        {
            litResults.Text = "Invalid Offer ID";
            return;
        }

        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.AddTracker(txtAddTrackerTrackerTag.Text, iOfferID).OuterXml);
    }

    protected void btnGetMemberTrackers_Click(object sender, EventArgs e)
    {
        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in oMember.GetMemberTrackers())
            {
                sb.Append(HttpUtility.HtmlEncode(s) + "<br /><br />");
            }

            litResults.Text = (sb.ToString());
        }

    }

    protected void btnNewMemberCashout_Click(object sender, EventArgs e)
    {
        int iCashoutMethodID;
        if (!int.TryParse(txtNewMemberCashoutMethodID.Text, out iCashoutMethodID))
        {
            litResults.Text = "Invalid Cashout Method ID";
            return;
        }

        PASMember oMember;
        string sError;
        oMember = GetMemberByID(txtGetMemberByID.Text, out sError);

        if (sError != "")
            litResults.Text = sError;
        else if (oMember.HasErrors)
            litResults.Text = GetHTMLEncodedErrors(oMember.ErrorMessageList);
        else
            litResults.Text = HttpUtility.HtmlEncode(oMember.NewMemberCashout(txtNewMemberCashoutTrackerDetails.Text, iCashoutMethodID).OuterXml);
    }
}
