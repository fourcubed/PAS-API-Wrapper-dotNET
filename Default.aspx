<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="base.css" />     
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div id="content">
        <div id="sidebar">
            <p>
                Date range (when used) is <asp:Literal ID="litStartDate" runat="server"></asp:Literal> - <asp:Literal ID="litEndDate" runat="server"></asp:Literal>.
                <br />
                Changeable in codebehind page.
            </p>
            <p>
                Website ID for Offers/Save New Mbr: <asp:TextBox ID="txtWebsiteID" runat="server" Columns="6" Text="67" ></asp:TextBox>
            </p>
            <p>
                <asp:Button ID="btnGetOffers" runat="server" Text="Get Offers" onclick="btnGetOffers_Click" />
            </p>
            <p>
                <asp:Button ID="btnGetMemberByLogin" runat="server" Text="Get Member (Login)" onclick="btnGetMemberByLogin_Click"  />
                <asp:TextBox ID="txtGetMemberByLogin" runat="server" Columns="6" ></asp:TextBox>
            </p>
            <p>
                <asp:Button ID="btnSaveNewMember" runat="server" Text="Save New Member (Login Name)" onclick="btnSaveNewMember_Click"  />
                <asp:TextBox ID="txtSaveNewMemberLoginName" runat="server" Columns="6" MaxLength="15" ></asp:TextBox>
            </p>
            <div id="memberstuff" style="border: solid thin black;">
                <p>
                    Member ID for below: 
                    <asp:TextBox ID="txtGetMemberByID" runat="server" Columns="6" Text="40403" ></asp:TextBox>
                </p>
                <p>
                    <asp:Button ID="btnGetMemberByID" runat="server" Text="Get Member (Member ID)" onclick="btnGetMemberByID_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberStatsByDateRangeFixed" runat="server" Text="Get Member Stats (use date range)" onclick="btnGetMemberStatsByDateRangeFixed_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberStatsByDateRangeBlank" runat="server" Text="Get Member Stats (use blank dates)" onclick="btnGetMemberStatsByDateRangeBlank_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberStatsByDay" runat="server" Text="Get Member Stats By Day (uses date range)" onclick="btnGetMemberStatsByDay_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberRAFStatsByDateRangeFixed" runat="server" Text="Get RAF Stats (use date range)" onclick="btnGetMemberRAFStatsByDateRangeFixed_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberRAFStatsByDateRangeBlank" runat="server" Text="Get RAF Stats (use blank dates)" onclick="btnGetMemberRAFStatsByDateRangeBlank_Click"  />
                </p>
                <p>
                    Page # for payments:
                    <asp:TextBox ID="txtPage" runat="server" Columns="2" Text="1" ></asp:TextBox>
                </p>
                <p>
                    <asp:Button ID="btnGetMemberPaymentsByDateRangeFixed" runat="server" Text="Get Payments (use date range)" onclick="btnGetMemberPaymentsByDateRangeFixed_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberPaymentsByDateRangeBlank" runat="server" Text="Get Payments (use blank dates)" onclick="btnGetMemberPaymentsByDateRangeBlank_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberCashoutsByDateRangeFixed" runat="server" Text="Get Cashouts (use date range)" onclick="btnGetMemberCashoutsByDateRangeFixed_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberCashoutsByDateRangeBlank" runat="server" Text="Get Cashouts (use blank dates)" onclick="btnGetMemberCashoutsByDateRangeBlank_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberBalances" runat="server" Text="Get Balances" onclick="btnGetMemberBalances_Click"  />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberCashoutMethods" runat="server" Text="Get Cashout Methods" onclick="btnGetMemberCashoutMethods_Click"   />
                </p>
                <p>
                    <asp:Button ID="btnGetRemoteAuthToken" runat="server" Text="Get Remote Auth Token" onclick="btnGetRemoteAuthToken_Click"   />
                </p>
                <p>
                    <asp:Button ID="btnSaveEditedMember" runat="server" Text="Save Edited Member (Outside ID)" onclick="btnSaveEditedMember_Click"   />
                    <asp:TextBox ID="txtOutsideID" runat="server" Columns="5" Text="1234" MaxLength="9" ></asp:TextBox>
                </p>
                <p>
                    <asp:Button ID="btnSaveNewTicket" runat="server" Text="Save New Ticket (Subject Append)" onclick="btnSaveNewTicket_Click"   />
                    <asp:TextBox ID="txtNewTicketSubjectTag" runat="server" Columns="5" Text="001" MaxLength="9" ></asp:TextBox>
                </p>
                <p>
                    <asp:Button ID="btnGetMemberTicketsXML" runat="server" Text="Get Tickets XML" onclick="btnGetMemberTicketsXML_Click"   />
                </p>
                <p>
                    <asp:Button ID="btnGetMemberTickets" runat="server" Text="Get Tickets" onclick="btnGetMemberTickets_Click"   />
                </p>
                <p>
                    <asp:Button ID="btnAddTicketReply" runat="server" Text="Add Ticket Reply (Body Append)" onclick="btnAddTicketReply_Click"   />
                    <asp:TextBox ID="txtAddReplyBodyTag" runat="server" Columns="5" Text="001" MaxLength="9" ></asp:TextBox>
                </p>
                <p>
                    <asp:Button ID="btnGetMemberTrackers" runat="server" Text="Get Trackers" onclick="btnGetMemberTrackers_Click"   />
                </p>
                <p>
                    <asp:Button ID="btnAddTracker" runat="server" Text="Add Tracker (OfferID, Trk Tag)" onclick="btnAddTracker_Click"   />
                    <asp:TextBox ID="txtAddTrackerOfferID" runat="server" Columns="5" Text="" MaxLength="9" ></asp:TextBox>
                    <asp:TextBox ID="txtAddTrackerTrackerTag" runat="server" Columns="5" Text="TestTag" MaxLength="9" ></asp:TextBox>
                </p>
                <p>
                    <asp:Button ID="btnNewMemberCashout" runat="server" Text="New Cashout (MethodID, Details)" onclick="btnNewMemberCashout_Click"   />
                    <asp:TextBox ID="txtNewMemberCashoutMethodID" runat="server" Columns="5" Text="" MaxLength="9" ></asp:TextBox>
                    <asp:TextBox ID="txtNewMemberCashoutTrackerDetails" runat="server" Columns="5" Text="TestDetails" MaxLength="9" ></asp:TextBox>
                </p>
            </div>
        </div>
        <div id="main">
            <div id="results">
                <asp:Literal ID="litResults" runat="server"></asp:Literal>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
