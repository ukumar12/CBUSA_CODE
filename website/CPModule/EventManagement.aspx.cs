
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;
using Components;
using DataAccessLayer;
using Newtonsoft.Json;
using System.Web;
using System.Web.Services;

public partial class EventManagement : System.Web.UI.Page
{
    int TwoPriceCampaignId;
    DataAccess Da;

    protected void Page_Load(object sender, EventArgs e)
    {
        Da = new DataAccess();
        if (!Page.IsPostBack)
        {
            // TwoPriceCampaignId = 415;
            if (Request.QueryString["TwoPriceCampaignId"] != null)
            {
                TwoPriceCampaignId = Convert.ToInt32(Request.QueryString["TwoPriceCampaignId"].ToString());
                ViewState["TwoPriceCampaignId"] = TwoPriceCampaignId;
                Session["TwoPriceCampaignId"] = TwoPriceCampaignId;
            }
            else
            {
                //Response.Redirect("https://app.custombuilders-usa.com//admin/twoprice/campaigns/default.aspx");//Prod Link
                //Response.Redirect("https://app.custombuilders-usa.com//admin/twoprice/campaigns/default.aspx");
            }

            PopulateCopyFromTPCList();
            if (ViewState["TwoPriceCampaignId"] != null)
            {
                BindDropDownList();
                FetchDataByTwoPriceCampaignId(ViewState["TwoPriceCampaignId"].ToString());
            }

        }

    }
    //public string BindMultiselectDropDown()
    //{
    //    DataSet ds = Da.ExecuteSelectQry("select AdminId,FirstName,LastName,Email from Admin");
    //    List<PrimaryContact> PrimaryContactList = ds.Tables[0].AsEnumerable().Select(x => new PrimaryContact()
    //    {
    //        AdminId = x.Field<int>("AdminId"),
    //        FirstName = x.Field<string>("FirstName"),
    //        LastName = x.Field<string>("LastName"),
    //        Email = x.Field<string>("Email")
    //    }).ToList();

    //    string Result = JsonConvert.SerializeObject(PrimaryContactList);

    //    return Result;

    //}
    public void FetchDataByTwoPriceCampaignId(string TwoPriceCampaignId)
    {
        //set by default colour of not schedule
        lblDistributionStatus.Attributes.Remove("class");
        lblDistributionStatus.Attributes.Add("class", "alert alert-danger");
        //set Deadline By default
        string After2Week = DateTime.Now.AddDays(14).ToString();
        dtPickerResponseDeadline.Value = After2Week;
        //set Distribution DtPicker By default 
        string After1day = DateTime.Now.AddDays(1).ToString();
        dtPickerDistribution.Value = After1day;

        //set utc by default
        DateTime DateDeadLineDefault = Convert.ToDateTime(dtPickerResponseDeadline.Value);
        DateTime DateTimeReminder1Default = DateDeadLineDefault.AddDays(-Convert.ToInt32(ddlReminder1DaysBefore.SelectedValue));
        lblReminder1ScheduledUTC.Text = string.Format("{0:dddd,MMMM dd}", DateTimeReminder1Default);

        DateTime DateTimeReminder2Default = DateDeadLineDefault.AddDays(-Convert.ToInt32(ddlReminder2DaysBefore.SelectedValue));
        lblReminder2ScheduledUTC.Text = string.Format("{0:dddd,MMMM dd}", DateTimeReminder2Default);


        DataSet DsResult = new DataSet();
        List<SqlParameter> SpList = new List<SqlParameter>();

        SpList.Add(new SqlParameter("@TwoPriceCampaignId", TwoPriceCampaignId));

        int RetValue = Da.CallStoreProcedure("FetchDataByTwoPriceCampaignId", SpList, ref DsResult);
        if (DsResult != null && DsResult.Tables.Count > 0)
        {
            if (DsResult.Tables[0] != null && DsResult.Tables[0].Rows.Count > 0)
            {
                //Table :  TwoPriceCampaign
                lblNameWdMonth.Text = DsResult.Tables[0].Rows[0]["NameWdMonth"].ToString();
                lblStatus.Text = DsResult.Tables[0].Rows[0]["Status"].ToString();
                lblStartToEndDate.Text = DsResult.Tables[0].Rows[0]["StartToEndDate"].ToString();
                lblActiveStatus.Text = DsResult.Tables[0].Rows[0]["ActiveStatus"].ToString();

                bool  SendNotificationToVendor = Convert.ToBoolean(DsResult.Tables[0].Rows[0]["SendNotificationToVendor"].ToString());
                chkOptionSendToTheseVendors.Checked = SendNotificationToVendor;

                string strParticipationOptions = DsResult.Tables[0].Rows[0]["ParticipationOptions"].ToString();

                if (!string.IsNullOrEmpty(strParticipationOptions) && strParticipationOptions.Length > 0)
                {
                    string[] arrParticipationOptions = strParticipationOptions.Split(',');
                    if (arrParticipationOptions.Length > 0)
                    {
                        foreach (ListItem itm in chkParticipationOption.Items)
                        {
                            if (arrParticipationOptions.Contains(itm.Value))
                            {
                                itm.Selected = true;
                            }
                        }
                    }
                }
            }

            if (DsResult.Tables[2] != null && DsResult.Tables[2].Rows.Count > 0)
            {
                lblMarketList.Text = DsResult.Tables[2].Rows[0]["LLCNAME"].ToString();
            }
            if (DsResult.Tables[1] != null && DsResult.Tables[1].Rows.Count > 0)
            {
                //Table :  TwoPriceBuilderInvitation

                //*****************Event
                hdnSelectedAdmin.Value = DsResult.Tables[1].Rows[0]["AdditionalContact"].ToString();
                editorEventDescription.InnerHtml = DsResult.Tables[1].Rows[0]["EventDescription"].ToString();
                if (!string.IsNullOrEmpty(DsResult.Tables[1].Rows[0]["ResponseDeadline"].ToString()))
                {
                    dtPickerResponseDeadline.Value = DsResult.Tables[1].Rows[0]["ResponseDeadline"].ToString();
                    lblResponseDeadline.Text = DsResult.Tables[1].Rows[0]["ResponseDeadline"].ToString();
                }
                ListItem selectedListItemPrimaryContact = ddlPrimaryContact.Items.FindByValue(DsResult.Tables[1].Rows[0]["PrimaryContact"].ToString());
                if (selectedListItemPrimaryContact != null)
                {
                    ddlPrimaryContact.ClearSelection();
                    selectedListItemPrimaryContact.Selected = true;
                }

                //*****************Invitation
                txtInvitationSubLine.Text = DsResult.Tables[1].Rows[0]["InvitationSubject"].ToString();
                editorInvitationMsgBody.InnerHtml = DsResult.Tables[1].Rows[0]["InvitationMessage"].ToString();
                txtOptInLinkText.Text = DsResult.Tables[1].Rows[0]["InvitationOptInText"].ToString();
                txtOptOutLinkText.Text = DsResult.Tables[1].Rows[0]["InvitationOptOutText"].ToString();

                //*****************Reminder
                if (string.IsNullOrEmpty(DsResult.Tables[1].Rows[0]["Reminder1DaysBefore"].ToString()))
                {
                    lblReminderNotScheduled.Visible = true;
                }
                else
                {
                    lblReminderNotScheduled.Visible = false;
                }

                ListItem SelectedListItemReminder1DaysBefore = ddlReminder1DaysBefore.Items.FindByValue(DsResult.Tables[1].Rows[0]["Reminder1DaysBefore"].ToString());
                if (SelectedListItemReminder1DaysBefore != null)
                {
                    ddlReminder1DaysBefore.ClearSelection();
                    SelectedListItemReminder1DaysBefore.Selected = true;
                }
                if (!string.IsNullOrEmpty(DsResult.Tables[1].Rows[0]["Reminder1Time"].ToString()))
                {
                    timepickerReminder1Time.Value = DsResult.Tables[1].Rows[0]["Reminder1Time"].ToString();
                }


                ListItem SelectedListItemddlReminder1TimeZone = ddlReminder1TimeZone.Items.FindByValue(DsResult.Tables[1].Rows[0]["Reminder1TimeZone"].ToString());
                if (SelectedListItemddlReminder1TimeZone != null)
                {
                    ddlReminder1TimeZone.ClearSelection();
                    SelectedListItemddlReminder1TimeZone.Selected = true;
                }
                if (!string.IsNullOrEmpty(DsResult.Tables[1].Rows[0]["Reminder1ScheduledUTC"].ToString()))
                {
                    DateTime dateDeadLine = Convert.ToDateTime(DsResult.Tables[1].Rows[0]["Reminder1ScheduledUTC"].ToString());
                    lblReminder1ScheduledUTC.Text = string.Format("{0:dddd,MMMM dd}", dateDeadLine);
                }
                //Bind Send Reminder 1 and Reminder 2
                else if (!string.IsNullOrEmpty(lblResponseDeadline.Text))
                {
                    DateTime dateDeadLine = Convert.ToDateTime(lblResponseDeadline.Text.Trim());
                    DateTime dateTimeReminder1 = dateDeadLine.AddDays(-Convert.ToInt32(ddlReminder1DaysBefore.SelectedValue));
                    lblReminder1ScheduledUTC.Text = string.Format("{0:dddd,MMMM dd}", dateTimeReminder1);

                }

                txtReminder1EnrollSubject.Text = DsResult.Tables[1].Rows[0]["Reminder1EnrollSubject"].ToString();
                editorReminder1EnrollMessage.InnerHtml = DsResult.Tables[1].Rows[0]["Reminder1EnrollMessage"].ToString();
                txtReminder1ProjectsSubject.Text = DsResult.Tables[1].Rows[0]["Reminder1ProjectsSubject"].ToString();
                editorReminder1ProjectsMessage.InnerHtml = DsResult.Tables[1].Rows[0]["Reminder1ProjectsMessage"].ToString();
                //----

                ListItem SelectedListItemReminder2DaysBefore = ddlReminder2DaysBefore.Items.FindByValue(DsResult.Tables[1].Rows[0]["Reminder2DaysBefore"].ToString());
                if (SelectedListItemReminder2DaysBefore != null)
                {
                    ddlReminder2DaysBefore.ClearSelection();
                    SelectedListItemReminder2DaysBefore.Selected = true;
                }
                if (!string.IsNullOrEmpty(DsResult.Tables[1].Rows[0]["Reminder2Time"].ToString()))
                {
                    timepickerReminder2Time.Value = DsResult.Tables[1].Rows[0]["Reminder2Time"].ToString();
                }


                ListItem SelectedListItemddlReminder2TimeZone = ddlReminder2TimeZone.Items.FindByValue(DsResult.Tables[1].Rows[0]["Reminder2TimeZone"].ToString());
                if (SelectedListItemddlReminder2TimeZone != null)
                {
                    ddlReminder2TimeZone.ClearSelection();
                    SelectedListItemddlReminder2TimeZone.Selected = true;
                }
                if (!string.IsNullOrEmpty(DsResult.Tables[1].Rows[0]["Reminder2ScheduledUTC"].ToString()))
                {
                    DateTime dateDeadLine = Convert.ToDateTime(DsResult.Tables[1].Rows[0]["Reminder2ScheduledUTC"].ToString());
                    lblReminder2ScheduledUTC.Text = string.Format("{0:dddd,MMMM dd}", dateDeadLine);
                }
                //Bind Send Reminder 1 and Reminder 2
                else if (!string.IsNullOrEmpty(lblResponseDeadline.Text))
                {
                    DateTime dateDeadLine = Convert.ToDateTime(lblResponseDeadline.Text.Trim());

                    DateTime dateTimeReminder2 = dateDeadLine.AddDays(-Convert.ToInt32(ddlReminder2DaysBefore.SelectedValue));
                    lblReminder2ScheduledUTC.Text = string.Format("{0:dddd,MMMM dd}", dateTimeReminder2);
                }

                txtReminder2EnrollSubject.Text = DsResult.Tables[1].Rows[0]["Reminder2EnrollSubject"].ToString();
                editorReminder2EnrollMessage.InnerHtml = DsResult.Tables[1].Rows[0]["Reminder2EnrollMessage"].ToString();
                txtReminder2ProjectsSubject.Text = DsResult.Tables[1].Rows[0]["Reminder2ProjectsSubject"].ToString();
                editorReminder2ProjectsMessage.InnerHtml = DsResult.Tables[1].Rows[0]["Reminder2ProjectsMessage"].ToString();

                //*****************Distribution
                lblSendInfo.Text = "";
                rblInvitationSendType.Enabled = true;
                ddlDistributionTimeZone.Enabled = true;
                dvCancelSchedule.Attributes.Add("style", "display:none;");
                chkOptionSendToNewBuilder.Checked = true;
                hdnScheduledCanclationCheck.Value = "";
                lblDistributionStatus.Text = "NOT SCHEDULED";

                if (!string.IsNullOrEmpty(DsResult.Tables[1].Rows[0]["InvitationStatus"].ToString()))
                {

                    ListItem SelectedListItemrblInvitationSendType = rblInvitationSendType.Items.FindByValue(DsResult.Tables[1].Rows[0]["InvitationSendType"].ToString());
                    if (SelectedListItemrblInvitationSendType != null)
                    {
                        rblInvitationSendType.ClearSelection();
                        SelectedListItemrblInvitationSendType.Selected = true;
                    }
                    lblInvitationStatus.Text = DsResult.Tables[1].Rows[0]["InvitationStatus"].ToString();
                    if (DsResult.Tables[1].Rows[0]["OptionSendToNewBuilder"].ToString().ToUpper() == "TRUE")
                    {
                        chkOptionSendToNewBuilder.Checked = true;
                    }
                    else if (DsResult.Tables[1].Rows[0]["OptionSendToNewBuilder"].ToString().ToUpper() == "FALSE")
                    {
                        chkOptionSendToNewBuilder.Checked = false;
                    }

                    //Depend on condition InvitationStatus
                    if (DsResult.Tables[1].Rows[0]["InvitationStatus"].ToString() == "1")
                    {
                        btnDistributionScheduled.InnerText = "RESEND";
                        lblDistributionStatus.Text = "SENT";
                        lblDistributionStatus.Attributes.Remove("class");
                        lblDistributionStatus.Attributes.Add("class", "alert alert-success");

                        lblSendInfo.Text = "Invitation Already Sent";
                        dvCancelSchedule.Attributes.Add("style", "display:none;");
                    }
                    else if (DsResult.Tables[1].Rows[0]["InvitationStatus"].ToString() == "2")
                    {
                        if (!string.IsNullOrEmpty(DsResult.Tables[1].Rows[0]["InvitationSheduledLocal"].ToString()))
                        {
                            timepickerDistributionTime.Value = DsResult.Tables[1].Rows[0]["SheduledTime"].ToString();
                            dtPickerDistribution.Value = DsResult.Tables[1].Rows[0]["InvitationSheduledLocal"].ToString();
                        }

                        btnDistributionScheduled.InnerText = "CANCEL";
                        lblDistributionStatus.Text = "SCHEDULED";
                        lblDistributionStatus.Attributes.Remove("class");
                        lblDistributionStatus.Attributes.Add("class", "alert alert-info");
                        rblInvitationSendType.Enabled = false;
                        ddlDistributionTimeZone.Enabled = false;

                        dvCancelSchedule.Attributes.Add("style", "display:block;");

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "EnableDisableDateTimePicker('2')", true);
                    }
                }

            }

            //Project Data
            ViewState["ProjectData"] = DsResult.Tables[3];
            gvProjectData.DataSource = DsResult.Tables[3];
            gvProjectData.DataBind();

            //All Available Vendor List 

            list1.DataSource = DsResult.Tables[4];
            list1.DataTextField = "VendorName";
            list1.DataValueField = "VendorID";
            list1.DataBind();

            //Selected Vendors List
            list2.DataSource = DsResult.Tables[5];
            list2.DataTextField = "VendorName";
            list2.DataValueField = "VendorID";
            list2.DataBind();




        }
    }
    public void BindDropDownList()
    {
        DataSet DsResult = new DataSet();
        List<SqlParameter> SpList = new List<SqlParameter>();

        string AdminId = "";
        //Session["AdminId"] = "7";
        if (Session["AdminId"] != null)
        {
            AdminId = Session["AdminId"].ToString();
        }

        SpList.Add(new SqlParameter("@Status", 1));
        SpList.Add(new SqlParameter("@Variable1", ""));
        SpList.Add(new SqlParameter("@Variable2", ""));
        SpList.Add(new SqlParameter("@Variable3", ""));

        int RetValue = Da.CallStoreProcedure("BindDropDownList", SpList, ref DsResult);
        if (DsResult != null && DsResult.Tables.Count > 0)
        {
            Da.BindDropDownListFromDB(ref ddlPrimaryContact, DsResult.Tables[0], AdminId, "", DataAccess.DefaultType.NotItem);
        }

        //Bind Reminder1DaysBefore and Reminder2DaysBefore
        DataTable DtReminder1DaysBefore = new DataTable();
        DtReminder1DaysBefore.Columns.Add("ID", typeof(string));
        DtReminder1DaysBefore.Columns.Add("Name", typeof(string));
        for (int i = 0; i <= 30; i++)
        {
            DtReminder1DaysBefore.Rows.Add(i.ToString(), i.ToString());
        }
        if (DtReminder1DaysBefore != null && DtReminder1DaysBefore.Rows.Count > 0)
        {
            Da.BindDropDownListFromDB(ref ddlReminder1DaysBefore, DtReminder1DaysBefore, "3", "", DataAccess.DefaultType.NotItem);
            Da.BindDropDownListFromDB(ref ddlReminder2DaysBefore, DtReminder1DaysBefore, "0", "", DataAccess.DefaultType.NotItem);
        }

    }
    public void KeepEditorDataSame()
    {
        //Event
        string EditorDescription = editorEventDescription.InnerText;
        editorEventDescription.InnerHtml = EditorDescription;
        //Invitation
        string EditorReload = editorInvitationMsgBody.InnerText;
        editorInvitationMsgBody.InnerHtml = EditorReload;
        //Reminders
        string Reminder1EnrollMessage = editorReminder1EnrollMessage.InnerText;
        editorReminder1EnrollMessage.InnerHtml = Reminder1EnrollMessage;
        string Reminder1ProjectsMessage = editorReminder1ProjectsMessage.InnerText;
        editorReminder1ProjectsMessage.InnerHtml = Reminder1ProjectsMessage;

        string Reminder2EnrollMessage = editorReminder2EnrollMessage.InnerText;
        editorReminder2EnrollMessage.InnerHtml = Reminder2EnrollMessage;
        string Reminder2ProjectsMessage = editorReminder2ProjectsMessage.InnerText;
        editorReminder2ProjectsMessage.InnerHtml = Reminder2ProjectsMessage;

    }
    public void CustomAlertMessegeBox(System.Web.UI.HtmlControls.HtmlControl HtmlContrl, Label lblErrorMsg, int? MsgType, string Msg)
    {
        lblErrorMsg.Text = "";
        HtmlContrl.Attributes.Remove("class");
        HtmlContrl.Visible = false;

        if (MsgType == 1)
        {
            //Danger
            HtmlContrl.Attributes.Add("class", "alert alert-danger");
            lblErrorMsg.Text = Msg;
            HtmlContrl.Visible = true;
        }
        else if (MsgType == 2)
        {
            //Success
            HtmlContrl.Attributes.Add("class", "alert alert-success");
            lblErrorMsg.Text = Msg;
            HtmlContrl.Visible = true;
        }
        else if (MsgType == 3)
        {
            //Warning
            HtmlContrl.Attributes.Add("class", "alert alert-warning");
            lblErrorMsg.Text = Msg;
            HtmlContrl.Visible = true;
        }
        else if (MsgType == 4)
        {
            //Info
            HtmlContrl.Attributes.Add("class", "alert alert-info");
            lblErrorMsg.Text = Msg;
            HtmlContrl.Visible = true;
        }


    }

    protected void btnSaveEvent_Click(object sender, EventArgs e)
    {
        DataSet DsResult = new DataSet();
        List<SqlParameter> SpList = new List<SqlParameter>();
        if (ViewState["TwoPriceCampaignId"] != null)
        {
            SpList.Add(new SqlParameter("@Status", "Event"));

            SpList.Add(new SqlParameter("@TwoPriceCampaignId", ViewState["TwoPriceCampaignId"].ToString()));
            SpList.Add(new SqlParameter("@EventDescription", editorEventDescription.InnerText.Replace("amp;", "")));
            SpList.Add(new SqlParameter("@ResponseDeadline", dtPickerResponseDeadline.Value));
            SpList.Add(new SqlParameter("@PrimaryContact", ddlPrimaryContact.SelectedValue));
            SpList.Add(new SqlParameter("@AdditionalContact", hdnSelectedAdmin.Value));
            SpList.Add(new SqlParameter("@InvitationSubject", ""));
            SpList.Add(new SqlParameter("@InvitationMessage", ""));
            SpList.Add(new SqlParameter("@InvitationOptInText", ""));
            SpList.Add(new SqlParameter("@InvitationOptOutText", ""));


            int RetValue = Da.CallStoreProcedure("OperationEventInvitation", SpList, ref DsResult);
            if (RetValue == 1 || RetValue == 2)
            {
                CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 2, "Event Details Saved Successfully.");

                FetchDataByTwoPriceCampaignId(ViewState["TwoPriceCampaignId"].ToString());
                KeepEditorDataSame();


            }
            else
            {
                CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 1, "Something went wrong.Please Try again !!!");
            }
        }
    }
    protected void btnSaveInvitation_Click(object sender, EventArgs e)
    {
        DataSet DsResult = new DataSet();
        List<SqlParameter> SpList = new List<SqlParameter>();
        if (ViewState["TwoPriceCampaignId"] != null)
        {
            SpList.Add(new SqlParameter("@Status", "Invitation"));

            SpList.Add(new SqlParameter("@TwoPriceCampaignId", ViewState["TwoPriceCampaignId"].ToString()));
            SpList.Add(new SqlParameter("@EventDescription", ""));
            SpList.Add(new SqlParameter("@ResponseDeadline", ""));
            SpList.Add(new SqlParameter("@PrimaryContact", ""));
            SpList.Add(new SqlParameter("@AdditionalContact", hdnSelectedAdmin.Value));
            SpList.Add(new SqlParameter("@InvitationSubject", txtInvitationSubLine.Text));
            SpList.Add(new SqlParameter("@InvitationMessage", editorInvitationMsgBody.InnerText));
            SpList.Add(new SqlParameter("@InvitationOptInText", txtOptInLinkText.Text));
            SpList.Add(new SqlParameter("@InvitationOptOutText", txtOptOutLinkText.Text));

            int RetValue = Da.CallStoreProcedure("OperationEventInvitation", SpList, ref DsResult);
            if (RetValue == 1 || RetValue == 2)
            {
                CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 2, "Invitation Details Saved Successfully.");

                FetchDataByTwoPriceCampaignId(ViewState["TwoPriceCampaignId"].ToString());
                KeepEditorDataSame();
            }
            else
            {
                CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 1, "Something went wrong.Please Try again !!!");
            }
        }
    }
    protected void btnSaveReminders_Click(object sender, EventArgs e)
    {
        DataSet DsResult = new DataSet();
        List<SqlParameter> SpList = new List<SqlParameter>();
        if (ViewState["TwoPriceCampaignId"] != null)
        {
            SpList.Add(new SqlParameter("@TwoPriceCampaignId", ViewState["TwoPriceCampaignId"].ToString()));

            SpList.Add(new SqlParameter("@Reminder1DaysBefore", ddlReminder1DaysBefore.SelectedValue));
            SpList.Add(new SqlParameter("@Reminder1Time", timepickerReminder1Time.Value));
            SpList.Add(new SqlParameter("@Reminder1TimeZone", ddlReminder1TimeZone.SelectedValue));
            DateTime Reminder1ScheduledUTC;
            if (!string.IsNullOrEmpty(lblResponseDeadline.Text))
            {
                DateTime dateDeadLine = Convert.ToDateTime(lblResponseDeadline.Text);
                DateTime dateTime = dateDeadLine.AddDays(-Convert.ToInt32(ddlReminder1DaysBefore.SelectedValue));
                Reminder1ScheduledUTC = Convert.ToDateTime(string.Format("{0:d}", dateTime) + " " + timepickerReminder1Time.Value);
                SpList.Add(new SqlParameter("@Reminder1ScheduledUTC", Reminder1ScheduledUTC));
            }
            else if (!string.IsNullOrEmpty(dtPickerResponseDeadline.Value))
            {
                DateTime dateDeadLine = Convert.ToDateTime(dtPickerResponseDeadline.Value);
                DateTime dateTime = dateDeadLine.AddDays(-Convert.ToInt32(ddlReminder1DaysBefore.SelectedValue));
                Reminder1ScheduledUTC = Convert.ToDateTime(string.Format("{0:d}", dateTime) + " " + timepickerReminder1Time.Value);
                SpList.Add(new SqlParameter("@Reminder1ScheduledUTC", Reminder1ScheduledUTC));
            }
            SpList.Add(new SqlParameter("@Reminder1EnrollSubject", txtReminder1EnrollSubject.Text));
            SpList.Add(new SqlParameter("@Reminder1EnrollMessage", editorReminder1EnrollMessage.InnerText));
            SpList.Add(new SqlParameter("@Reminder1ProjectsSubject", txtReminder1ProjectsSubject.Text));
            SpList.Add(new SqlParameter("@Reminder1ProjectsMessage", editorReminder1ProjectsMessage.InnerText));


            SpList.Add(new SqlParameter("@Reminder2DaysBefore", ddlReminder2DaysBefore.SelectedValue));
            SpList.Add(new SqlParameter("@Reminder2Time", timepickerReminder2Time.Value));
            SpList.Add(new SqlParameter("@Reminder2TimeZone", ddlReminder2TimeZone.SelectedValue));
            DateTime Reminder2ScheduledUTC;
            if (!string.IsNullOrEmpty(lblResponseDeadline.Text))
            {
                DateTime dateDeadLine = Convert.ToDateTime(lblResponseDeadline.Text);
                DateTime dateTime = dateDeadLine.AddDays(-Convert.ToInt32(ddlReminder2DaysBefore.SelectedValue));
                Reminder2ScheduledUTC = Convert.ToDateTime(string.Format("{0:d}", dateTime) + " " + timepickerReminder2Time.Value);
                SpList.Add(new SqlParameter("@Reminder2ScheduledUTC", Reminder2ScheduledUTC));
            }
            else if (!string.IsNullOrEmpty(dtPickerResponseDeadline.Value))
            {
                DateTime dateDeadLine = Convert.ToDateTime(dtPickerResponseDeadline.Value);
                DateTime dateTime = dateDeadLine.AddDays(-Convert.ToInt32(ddlReminder2DaysBefore.SelectedValue));
                Reminder2ScheduledUTC = Convert.ToDateTime(string.Format("{0:d}", dateTime) + " " + timepickerReminder2Time.Value);
                SpList.Add(new SqlParameter("@Reminder2ScheduledUTC", Reminder2ScheduledUTC));
            }
            SpList.Add(new SqlParameter("@Reminder2EnrollSubject", txtReminder2EnrollSubject.Text));
            SpList.Add(new SqlParameter("@Reminder2EnrollMessage", editorReminder2EnrollMessage.InnerText));
            SpList.Add(new SqlParameter("@Reminder2ProjectsSubject", txtReminder2ProjectsSubject.Text));
            SpList.Add(new SqlParameter("@Reminder2ProjectsMessage", editorReminder2ProjectsMessage.InnerText));

            int RetValue = Da.CallStoreProcedure("OperationReminders", SpList, ref DsResult);
            if (RetValue == 1 || RetValue == 2)
            {
                CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 2, "Reminders Details Saved Successfully.");
                FetchDataByTwoPriceCampaignId(ViewState["TwoPriceCampaignId"].ToString());
                KeepEditorDataSame();
            }
            else
            {
                CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 1, "Something went wrong.Please Try again !!!");
            }
        }
    }
    protected void btnCancelationReminder_Click(object sender, EventArgs e)
    {
        DataSet DsResult = new DataSet();
        List<SqlParameter> SpList = new List<SqlParameter>();
        if (ViewState["TwoPriceCampaignId"] != null)
        {
            SpList.Add(new SqlParameter("@TwoPriceCampaignId", ViewState["TwoPriceCampaignId"].ToString()));

            int RetValue = Da.CallStoreProcedure("CancellationReminders", SpList, ref DsResult);
            if (RetValue == 1 || RetValue == 2)
            {
                CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 2, "Reminders Cancelled Successfully.");
                BindDropDownList();
                FetchDataByTwoPriceCampaignId(ViewState["TwoPriceCampaignId"].ToString());
                KeepEditorDataSame();
            }
            else
            {
                CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 1, "Something went wrong.Please Try again !!!");
            }
        }
    }
    protected void btnSaveDistribution_Click(object sender, EventArgs e)
    {

        //string strAvailableVendors = Request.Form[list1.UniqueID];  



        string confirmValue = Request.Form["confirm_value"];
        if (confirmValue == "Yes")
        {
            DataSet DsResult = new DataSet();
            List<SqlParameter> SpList = new List<SqlParameter>();
            if (ViewState["TwoPriceCampaignId"] != null)
            {
                SpList.Add(new SqlParameter("@TwoPriceCampaignId", ViewState["TwoPriceCampaignId"].ToString()));
                // SpList.Add(new SqlParameter("@InvitationSendType", rblInvitationSendType.SelectedValue));
                if (rblInvitationSendType.SelectedValue == "1")
                {
                    SpList.Add(new SqlParameter("@InvitationSendType", rblInvitationSendType.SelectedValue));
                    SpList.Add(new SqlParameter("@InvitationStatus", InvitationStatus.Sent));
                    SpList.Add(new SqlParameter("@InvitationSentUTC", DateTime.UtcNow));
                    SpList.Add(new SqlParameter("@OptionSendToNewBuilder", chkOptionSendToNewBuilder.Checked));
                }
                else if (rblInvitationSendType.SelectedValue == "2")
                {
                    DateTime InvitationSheduledUTC;


                    if (hdnScheduledCanclationCheck.Value == "CancledScheduled")
                    {
                        //SpList.Add(new SqlParameter("@InvitationStatus", InvitationStatus.Scheduled));
                        //SpList.Add(new SqlParameter("@InvitationSendType", rblInvitationSendType.SelectedValue));
                    }
                    else
                    {
                        SpList.Add(new SqlParameter("@InvitationStatus", InvitationStatus.Scheduled));
                        SpList.Add(new SqlParameter("@InvitationSendType", rblInvitationSendType.SelectedValue));
                        InvitationSheduledUTC = Convert.ToDateTime(string.Format("{0:d}", dtPickerDistribution.Value) + " " + timepickerDistributionTime.Value);
                        SpList.Add(new SqlParameter("@InvitationSheduledUTC", InvitationSheduledUTC));
                        SpList.Add(new SqlParameter("@InvitationSheduledLocal", InvitationSheduledUTC));
                        SpList.Add(new SqlParameter("@InvitationSheduledTimeZone", ddlDistributionTimeZone.SelectedValue));
                        SpList.Add(new SqlParameter("@OptionSendToNewBuilder", chkOptionSendToNewBuilder.Checked));
                    }
                }

                int RetValue = Da.CallStoreProcedure("OperationDistribution", SpList, ref DsResult);
                if (RetValue == 1 || RetValue == 2)
                {
                    if (chkOptionSendToTheseVendors.Checked)
                    {
                        string strSelectedVendors = Request.Form[list2.UniqueID];
                        DataSet DsResultNew = new DataSet();
                        List<SqlParameter> SpListNew = new List<SqlParameter>();
                        SpListNew.Add(new SqlParameter("@TwoPriceCampaignId", Convert.ToInt32(ViewState["TwoPriceCampaignId"].ToString())));
                        SpListNew.Add(new SqlParameter("@SelectedVendors", strSelectedVendors.TrimEnd(',')));
                        int Result = Da.CallStoreProcedure("TwoPrice_SendNotificationToVendor_Insert", SpListNew, ref DsResultNew);
                    }

                    CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 2, "Distribution Details Saved Successfully.");
                    FetchDataByTwoPriceCampaignId(ViewState["TwoPriceCampaignId"].ToString());
                    KeepEditorDataSame();

                    if (rblInvitationSendType.SelectedValue == "1")
                    {
                        if (DsResult != null && DsResult.Tables.Count > 0)
                        {
                            ///Mail Sending                           
                            string MailSubject = DsResult.Tables[0].Rows[0]["MailSubject"].ToString();
                            string MailBody = System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[0]["MailBody"].ToString());
                            if (!string.IsNullOrEmpty(MailSubject))
                            {
                                MailSubject = MailSubject.Replace("{{%EventTitle%}}", DsResult.Tables[0].Rows[0]["EventTitle"].ToString());
                                MailSubject = MailSubject.Replace("{{%EventStart%}}", DsResult.Tables[0].Rows[0]["EventStart"].ToString());
                                MailSubject = MailSubject.Replace("{{%EventEnd%}}", DsResult.Tables[0].Rows[0]["EventEnd"].ToString());
                                MailSubject = MailSubject.Replace("{{%EventDescription%}}", System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[0]["EventDescription"].ToString()));
                                MailSubject = MailSubject.Replace("{{%ResponseDeadline%}}", DsResult.Tables[0].Rows[0]["ResponseDeadline"].ToString());
                                MailSubject = MailSubject.Replace("{{%ResponseDeadlineFull%}}", DsResult.Tables[0].Rows[0]["ResponseDeadlineFull"].ToString());
                                MailSubject = MailSubject.Replace("{{%ResponseDeadlineDay%}}", DsResult.Tables[0].Rows[0]["ResponseDeadlineDay"].ToString());
                                MailSubject = MailSubject.Replace("{{%ResponseDeadlineFullDay%}}", DsResult.Tables[0].Rows[0]["ResponseDeadlineFullDay"].ToString());

                                MailSubject = MailSubject.Replace("{{%ContactName%}}", DsResult.Tables[0].Rows[0]["ContactName"].ToString());
                                MailSubject = MailSubject.Replace("{{%ContactEmail%}}", DsResult.Tables[0].Rows[0]["ContactEmail"].ToString());
                                MailSubject = MailSubject.Replace("{{%ContactPhone%}}", DsResult.Tables[0].Rows[0]["ContactPhone"].ToString());

                                string EventID = ViewState["TwoPriceCampaignId"].ToString();

                                string OptInLink = ConfigurationManager.AppSettings["CPAppHosting"] + "/default.aspx?mod=tpc&Tcam=" + EventID + "&Opt=1";
                                string OptIn = "<a style=\"padding: 2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;\" href=\"" + OptInLink + "\">" + DsResult.Tables[0].Rows[0]["OptIn"].ToString() + "</a>";
                                MailSubject = MailSubject.Replace("{{%OptIn%}}", OptIn);

                                MailSubject = MailSubject.Replace("{{%ReminderBlock%}}", "");
                            }
                            if (!string.IsNullOrEmpty(MailBody))
                            {
                                MailBody = MailBody.Replace("{{%EventTitle%}}", DsResult.Tables[0].Rows[0]["EventTitle"].ToString());
                                MailBody = MailBody.Replace("{{%EventStart%}}", DsResult.Tables[0].Rows[0]["EventStart"].ToString());
                                MailBody = MailBody.Replace("{{%EventEnd%}}", DsResult.Tables[0].Rows[0]["EventEnd"].ToString());
                                MailBody = MailBody.Replace("{{%EventDescription%}}", System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[0]["EventDescription"].ToString()));
                                MailBody = MailBody.Replace("{{%ResponseDeadline%}}", DsResult.Tables[0].Rows[0]["ResponseDeadline"].ToString());
                                MailBody = MailBody.Replace("{{%ResponseDeadlineFull%}}", DsResult.Tables[0].Rows[0]["ResponseDeadlineFull"].ToString());
                                MailBody = MailBody.Replace("{{%ResponseDeadlineDay%}}", DsResult.Tables[0].Rows[0]["ResponseDeadlineDay"].ToString());
                                MailBody = MailBody.Replace("{{%ResponseDeadlineFullDay%}}", DsResult.Tables[0].Rows[0]["ResponseDeadlineFullDay"].ToString());

                                MailBody = MailBody.Replace("{{%ContactName%}}", DsResult.Tables[0].Rows[0]["ContactName"].ToString());
                                MailBody = MailBody.Replace("{{%ContactEmail%}}", DsResult.Tables[0].Rows[0]["ContactEmail"].ToString());
                                MailBody = MailBody.Replace("{{%ContactPhone%}}", DsResult.Tables[0].Rows[0]["ContactPhone"].ToString());

                                string EventID = ViewState["TwoPriceCampaignId"].ToString();

                                string OptInLink = ConfigurationManager.AppSettings["CPAppHosting"] + "/default.aspx?mod=tpc&Tcam=" + EventID + "&Opt=1";

                                string OptIn = "<a style=\"padding: 2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;\" href=\"" + OptInLink + "\">" + DsResult.Tables[0].Rows[0]["OptIn"].ToString() + "</a>";
                                MailBody = MailBody.Replace("{{%OptIn%}}", OptIn);

                                MailBody = MailBody.Replace("{{%ReminderBlock%}}", "");
                            }
                            DataTable DtSendMail = DsResult.Tables[1];
                            if (DtSendMail != null && DtSendMail.Rows.Count > 0)
                            {
                                for (int i = 0; i < DtSendMail.Rows.Count; i++)
                                {
                                    //Subject
                                    string SendMailSubject = MailSubject;
                                    string SendMailBody = MailBody;

                                    SendMailSubject = SendMailSubject.Replace("{{%BuilderName%}}", DsResult.Tables[1].Rows[i]["BuilderName"].ToString());
                                    SendMailSubject = SendMailSubject.Replace("{{%RecipientFirstName%}}", DsResult.Tables[1].Rows[i]["RecipientFirstName"].ToString());
                                    SendMailSubject = SendMailSubject.Replace("{{%RecipientLastName%}}", DsResult.Tables[1].Rows[i]["RecipientLastName"].ToString());
                                    SendMailSubject = SendMailSubject.Replace("{{%RecipientFullName%}}", DsResult.Tables[1].Rows[i]["RecipientFullName"].ToString());
                                    //Body
                                    SendMailBody = SendMailBody.Replace("{{%BuilderName%}}", DsResult.Tables[1].Rows[i]["BuilderName"].ToString());
                                    SendMailBody = SendMailBody.Replace("{{%RecipientFirstName%}}", DsResult.Tables[1].Rows[i]["RecipientFirstName"].ToString());
                                    SendMailBody = SendMailBody.Replace("{{%RecipientLastName%}}", DsResult.Tables[1].Rows[i]["RecipientLastName"].ToString());
                                    SendMailBody = SendMailBody.Replace("{{%RecipientFullName%}}", DsResult.Tables[1].Rows[i]["RecipientFullName"].ToString());

                                    //////////   Opt Out Link Generate using parameter-- mod,Tcam,BuilderId,BuilderAccountID
                                    string EventID = ViewState["TwoPriceCampaignId"].ToString();
                                    string OptOutLink = ConfigurationManager.AppSettings["CPModule"] + "/OptOut.aspx?mod=tpc&Tcam=" + EventID + "&Opt=3&BuilderId=" + DsResult.Tables[1].Rows[i]["BuilderID"].ToString() + "&BuilderAccountID=" + DsResult.Tables[1].Rows[i]["BuilderAccountID"].ToString();
                                    string OptOut = "<a style=\"padding:2px 4px; border:1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#cc0000; text-decoration:none;\" href=\"" + OptOutLink + "\">" + DsResult.Tables[0].Rows[0]["OptOut"].ToString() + "</a>";
                                    SendMailBody = SendMailBody.Replace("{{%OptOut%}}", OptOut);
                                    SendMailSubject = SendMailSubject.Replace("{{%OptOut%}}", OptOut);

                                    if (i == 0)
                                    {
                                        //Copy to CBUSA Primary Contact

                                        //MailAddress msgFrom = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");
                                        //MailAddress msgTo = new MailAddress("dmetey@medullus.com", DsResult.Tables[1].Rows[i]["RecipientFullName"].ToString());

                                        if (!string.IsNullOrEmpty(DsResult.Tables[1].Rows[i]["RecipientEmail"].ToString()) && !string.IsNullOrEmpty(DsResult.Tables[0].Rows[0]["ContactEmail"].ToString()))
                                        {

                                            MailAddress msgFrom = new MailAddress(DsResult.Tables[0].Rows[0]["ContactEmail"].ToString(), DsResult.Tables[0].Rows[0]["ContactName"].ToString());
                                            MailAddress msgTo = new MailAddress(DsResult.Tables[1].Rows[i]["RecipientEmail"].ToString(), DsResult.Tables[1].Rows[i]["RecipientFullName"].ToString());

                                            MailMessage msg = new MailMessage(msgFrom, msgTo);
                                            msg.IsBodyHtml = false;
                                            msg.Subject = SendMailSubject;
                                            msg.Body = SendMailBody;

                                            Core.SendMail(msg, "", "");

                                            //*******************************Send Mail To CBUSA Additional Contact**************************************************
                                            if (!string.IsNullOrEmpty(hdnSelectedAdmin.Value))
                                            {
                                                string[] AdditionalMail = hdnSelectedAdmin.Value.Substring(0, hdnSelectedAdmin.Value.Length - 1).Split(',');
                                                MailAddress MsgFromAdditionalPrimary = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");

                                                foreach (string email in AdditionalMail)
                                                {
                                                    MailAddress MsgToAdditionalPrimary = new MailAddress(email, "");
                                                    MailMessage AdditionalMsg = new MailMessage(MsgFromAdditionalPrimary, MsgToAdditionalPrimary);
                                                    AdditionalMsg.IsBodyHtml = false;
                                                    AdditionalMsg.Subject = SendMailSubject;
                                                    AdditionalMsg.Body = SendMailBody;

                                                    Core.SendMail(AdditionalMsg, "", "");
                                                }
                                            }

                                            //Save Mail Sending Track Record
                                            DataSet DsResultMailTrack = new DataSet();
                                            List<SqlParameter> SpListMailTrack = new List<SqlParameter>();
                                            SpListMailTrack.Add(new SqlParameter("@TwoPriceCampaignId", DsResult.Tables[1].Rows[i]["TwoPriceCampaignId"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@BuilderId", DsResult.Tables[1].Rows[i]["BuilderID"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@BuilderAccountId", DsResult.Tables[1].Rows[i]["BuilderAccountID"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@InvitationSendUTC", DateTime.UtcNow));
                                            SpListMailTrack.Add(new SqlParameter("@MessageId", ""));
                                            SpListMailTrack.Add(new SqlParameter("@MessageDisposition", ""));
                                            SpListMailTrack.Add(new SqlParameter("@MessageDetails", ""));
                                            SpListMailTrack.Add(new SqlParameter("@EmailType", "1"));

                                            int RetValueMailTrack = Da.CallStoreProcedure("SaveMailTransaction", SpListMailTrack, ref DsResultMailTrack);


                                            //*******************************Send Mail To CBUSA Primary Contact**************************************************
                                            MailAddress msgFromPrimary = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");
                                            MailAddress msgToPrimary = new MailAddress(DsResult.Tables[0].Rows[0]["ContactEmail"].ToString(), DsResult.Tables[0].Rows[0]["ContactName"].ToString());
                                            // MailAddress msgToPrimary = new MailAddress("dmetey@medullus.com", DsResult.Tables[0].Rows[0]["ContactName"].ToString());

                                            MailMessage msgPrimary = new MailMessage(msgFromPrimary, msgToPrimary);
                                            msgPrimary.IsBodyHtml = false;
                                            msgPrimary.Subject = SendMailSubject;
                                            msgPrimary.Body = SendMailBody;

                                            Core.SendMail(msgPrimary, "", "");
                                        }
                                    }
                                    else //if (i == 5000)
                                    {
                                        // MailAddress msgFrom = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");
                                        // MailAddress msgTo = new MailAddress("dmetey@medullus.com", DsResult.Tables[1].Rows[i]["RecipientFullName"].ToString());

                                        if (!string.IsNullOrEmpty(DsResult.Tables[1].Rows[i]["RecipientEmail"].ToString()) && !string.IsNullOrEmpty(DsResult.Tables[0].Rows[0]["ContactEmail"].ToString()))
                                        {
                                            MailAddress msgFrom = new MailAddress(DsResult.Tables[0].Rows[0]["ContactEmail"].ToString(), DsResult.Tables[0].Rows[0]["ContactName"].ToString());
                                            MailAddress msgTo = new MailAddress(DsResult.Tables[1].Rows[i]["RecipientEmail"].ToString(), DsResult.Tables[1].Rows[i]["RecipientFullName"].ToString());

                                            MailMessage msg = new MailMessage(msgFrom, msgTo);
                                            msg.IsBodyHtml = false;
                                            msg.Subject = SendMailSubject;
                                            msg.Body = SendMailBody;

                                            Core.SendMail(msg, "", "");

                                            //Save Mail Sending Track Record
                                            DataSet DsResultMailTrack = new DataSet();
                                            List<SqlParameter> SpListMailTrack = new List<SqlParameter>();
                                            SpListMailTrack.Add(new SqlParameter("@TwoPriceCampaignId", DsResult.Tables[1].Rows[i]["TwoPriceCampaignId"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@BuilderId", DsResult.Tables[1].Rows[i]["BuilderID"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@BuilderAccountId", DsResult.Tables[1].Rows[i]["BuilderAccountID"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@InvitationSendUTC", DateTime.UtcNow));
                                            SpListMailTrack.Add(new SqlParameter("@MessageId", ""));
                                            SpListMailTrack.Add(new SqlParameter("@MessageDisposition", ""));
                                            SpListMailTrack.Add(new SqlParameter("@MessageDetails", ""));
                                            SpListMailTrack.Add(new SqlParameter("@EmailType", "1"));

                                            int RetValueMailTrack = Da.CallStoreProcedure("SaveMailTransaction", SpListMailTrack, ref DsResultMailTrack);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 1, "Something went wrong.Please Try again !!!");
                }
            }
        }
        else
        {
            FetchDataByTwoPriceCampaignId(ViewState["TwoPriceCampaignId"].ToString());
            KeepEditorDataSame();
        }
    }

    public enum InvitationStatus
    {
        NotScheduled = 0,
        Sent = 1,
        Scheduled = 2
    }

    //-------------------- COPY CONTENT FEATURE -----------------------------
    private void PopulateCopyFromTPCList()
    {
        ddlCopyFromTPCampaignList.Items.Clear();

        ListItem liSelect = new ListItem("-- SELECT --", "0");
        ddlCopyFromTPCampaignList.Items.Add(liSelect);

        string sqlGetTwoPriceList = "SELECT TwoPriceCampaignId, [Name] FROM TwoPriceCampaign WHERE TwoPriceCampaignId IN(SELECT DISTINCT TwoPriceCampaignId FROM TwoPriceBuilderInvitation) AND TwoPriceCampaignId <> @CurrTwoPriceCampaignId AND IsActive = 1";

        SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        SqlCommand sqlComm = new SqlCommand(sqlGetTwoPriceList, sqlConn);

        sqlComm.Parameters.AddWithValue("@CurrTwoPriceCampaignId", ViewState["TwoPriceCampaignId"]);

        try
        {
            sqlConn.Open();

            SqlDataReader sqlDA;

            sqlDA = sqlComm.ExecuteReader();

            if (sqlDA.HasRows)
            {
                while (sqlDA.Read())
                {
                    ListItem liTwoPriceCampaign = new ListItem(sqlDA.GetValue(1).ToString(), sqlDA.GetValue(0).ToString());
                    ddlCopyFromTPCampaignList.Items.Add(liTwoPriceCampaign);
                }
            }
        }
        catch
        {
        }
        finally
        {
            sqlComm.Dispose();
            sqlComm = null;

            sqlConn.Close();
            sqlConn = null;
        }
    }

    protected void ddlCopyFromTPCampaignList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCopyFromTPCampaignList.SelectedValue == "0")
            return;

        string sqlUpdateCampaignContent = "sp_CopyTwoPriceBuilderContent";

        SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        SqlCommand sqlComm = new SqlCommand(sqlUpdateCampaignContent, sqlConn);

        try
        {
            sqlConn.Open();

            sqlComm.CommandType = CommandType.StoredProcedure;

            sqlComm.Parameters.AddWithValue("@CopyFromTwoPriceCampaignId", ddlCopyFromTPCampaignList.SelectedValue);
            sqlComm.Parameters.AddWithValue("@CopyToTwoPriceCampaignId", ViewState["TwoPriceCampaignId"]);

            sqlComm.ExecuteNonQuery();

            FetchDataByTwoPriceCampaignId(ViewState["TwoPriceCampaignId"].ToString());
        }
        catch (Exception ex)
        {
        }
        finally
        {
            sqlComm.Dispose();
            sqlComm = null;

            sqlConn.Close();
            sqlConn = null;
        }
    }

    //-------------------- COPY CONTENT FEATURE -----------------------------

    #region Project Data Section
    protected void btnSaveProjectData_Click(object sender, EventArgs e)
    {
        int errorCount = 0;
        DataSet DsResult = new DataSet();
        List<SqlParameter> SpList = new List<SqlParameter>();
        SpList.Add(new SqlParameter("@TwoPriceCampaignId", ViewState["TwoPriceCampaignId"].ToString()));
        Da.CallStoreProcedure("TwoPriceCampaignProjectDataClear", SpList, ref DsResult);

        string strParticipationOptions = "";
        foreach (ListItem itm in chkParticipationOption.Items)
        {
            if (itm.Selected)
            {
                strParticipationOptions += itm.Value + ",";
            }
        }
        strParticipationOptions = strParticipationOptions.TrimEnd(',');

        SpList = new List<SqlParameter>();
        SpList.Add(new SqlParameter("@TwoPriceCampaignId", ViewState["TwoPriceCampaignId"].ToString()));
        SpList.Add(new SqlParameter("@ParticipationOptions", strParticipationOptions));
        errorCount = Da.CallStoreProcedure("TwoPriceCampaignParticipationOptions_Insert", SpList, ref DsResult);


        if (errorCount == 0)
        {

            foreach (GridViewRow gr in gvProjectData.Rows)
            {
                SpList = new List<SqlParameter>();
                CheckBox chk = gr.FindControl("chkId") as CheckBox;
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        string QuestionId = (gr.Cells[1].FindControl("hdnQuestionId") as HiddenField).Value;
                        string Question = (gr.Cells[1].FindControl("lblQuestion") as Label).Text;
                        string Label = (gr.Cells[2].FindControl("hdnQuestionLabel") as HiddenField).Value;
                        string HintText = (gr.Cells[3].FindControl("hdnHintText") as HiddenField).Value;
                        string sortOrder = (gr.Cells[3].FindControl("hdnSortOrder") as HiddenField).Value;
                        CheckBox chkRequired = gr.FindControl("chkRequired") as CheckBox;
                        SpList.Add(new SqlParameter("@TwoPriceCampaignId", ViewState["TwoPriceCampaignId"].ToString()));
                        SpList.Add(new SqlParameter("@QuestionId", QuestionId));
                        SpList.Add(new SqlParameter("@QuestionLabel", Label));
                        SpList.Add(new SqlParameter("@HintText", HintText));
                        SpList.Add(new SqlParameter("@SortOrder", "0"));
                        SpList.Add(new SqlParameter("@IsRequired", Convert.ToBoolean(chkRequired.Checked)));

                        errorCount += Da.CallStoreProcedure("TwoPriceCampaignProjectDataInsert", SpList, ref DsResult);
                    }
                }
            }
        }

        if (errorCount == 0)
        {
            CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 2, "Project Data Details Saved Successfully.");
            FetchDataByTwoPriceCampaignId(ViewState["TwoPriceCampaignId"].ToString());
            KeepEditorDataSame();
        }
        else
        {
            CustomAlertMessegeBox(MessageDiv, lblErrorMsg, 1, "Something went wrong.Please Try again !!!");
        }
    }
    #endregion Project Data Section

    [WebMethod]
    public static int UpdateVendorEventNotificationOption(int flag)
    {
        DataAccess db = new DataAccess();
        int errorCount = 0;

        string TwoPriceCampaignId = HttpContext.Current.Session["TwoPriceCampaignId"].ToString();
        DataSet DsResult = new DataSet();
        List<SqlParameter> SpList = new List<SqlParameter>();
        SpList.Add(new SqlParameter("@TwoPriceCampaignId", TwoPriceCampaignId.ToString()));
        SpList.Add(new SqlParameter("@Flag", flag));
        errorCount = db.CallStoreProcedure("PROC_UPDATE_VENDOR_EVENT_NOTIFICATION_FLAG", SpList, ref DsResult);
        return errorCount;
    }
}
