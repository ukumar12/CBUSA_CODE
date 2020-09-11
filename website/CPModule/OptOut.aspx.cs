using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using Components;
using DataAccessLayer;

public partial class OptOut : System.Web.UI.Page
{
    DataAccess Da = new DataAccess();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DataSet DsResult = new DataSet();
            List<SqlParameter> SpList = new List<SqlParameter>();
            if (Request.QueryString["Tcam"] != null && Request.QueryString["BuilderId"] != null && Request.QueryString["BuilderAccountID"] != null)
            {

                SpList.Add(new SqlParameter("@TwoPriceCampaignId", Request.QueryString["Tcam"].ToString()));
                SpList.Add(new SqlParameter("@BuilderId", Request.QueryString["BuilderId"].ToString()));
                SpList.Add(new SqlParameter("@ParticipationType", "3"));
                SpList.Add(new SqlParameter("@RecordState", "1"));
                SpList.Add(new SqlParameter("@CreatedBy", Request.QueryString["BuilderAccountID"].ToString()));
                SpList.Add(new SqlParameter("@ModifiedBy", Request.QueryString["BuilderAccountID"].ToString()));

                int RetValue = Da.CallStoreProcedure("SaveOptout", SpList, ref DsResult);
                if (RetValue == 1)
                {
                    if (DsResult != null && DsResult.Tables.Count > 0)
                    {
                        if (DsResult.Tables[0] != null && DsResult.Tables[0].Rows.Count > 0)
                        {
                            lblNameWdMonth.Text = DsResult.Tables[0].Rows[0]["NameWdMonth"].ToString();
                            lblNameWdMonthBody.Text = DsResult.Tables[0].Rows[0]["NameWdMonth"].ToString();
                            lblResponseDeadlineFull.Text = DsResult.Tables[0].Rows[0]["ResponseDeadlineFull"].ToString();
                            lblEventDescription.InnerHtml = System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[0]["EventDescription"].ToString());
                            aDataEntryPage.HRef = ConfigurationManager.AppSettings["CPAppHosting"] + "/default.aspx?mod=tpc&Tcam=" + Request.QueryString["Tcam"].ToString() + "&Opt=1";
                            lbllblBuilderName.Text = DsResult.Tables[1].Rows[0]["BuilderName"].ToString();

                            lblNameWdMonth1.Text = DsResult.Tables[0].Rows[0]["Name"].ToString();
                            lblNameWdMonthBody1.Text = DsResult.Tables[0].Rows[0]["Name"].ToString();
                            lblResponseDeadlineFull1.Text = DsResult.Tables[0].Rows[0]["ResponseDeadlineFull"].ToString();
                            aDataEntryPage1.HRef = ConfigurationManager.AppSettings["CPAppHosting"] + "/default.aspx?mod=tpc&Tcam=" + Request.QueryString["Tcam"].ToString() + "&Opt=1";
                            lblBuilderName1.Text = DsResult.Tables[1].Rows[0]["BuilderName"].ToString();

                            //string MailBody = dvEventDetails.InnerHtml;
                            var sb = new StringBuilder();
                            dvEventDetails.RenderControl(new HtmlTextWriter(new StringWriter(sb)));

                            string MailBody = System.Net.WebUtility.HtmlDecode(sb.ToString().Trim());
                            string MailSubject = "You have chosen NOT to participate in the " + DsResult.Tables[0].Rows[0]["Name"].ToString() + " committed-purchase event.";

                            //MailAddress msgFrom = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");
                            //MailAddress msgTo = new MailAddress("dmetey@medullus.com", DsResult.Tables[1].Rows[0]["RecipientFullName"].ToString());

                            MailAddress msgFrom = new MailAddress(DsResult.Tables[0].Rows[0]["ContactEmail"].ToString(), DsResult.Tables[0].Rows[0]["ContactName"].ToString());
                            MailAddress msgTo = new MailAddress(DsResult.Tables[1].Rows[0]["RecipientEmail"].ToString(), DsResult.Tables[1].Rows[0]["RecipientFullName"].ToString());
                            
                            

                            MailMessage msg = new MailMessage(msgFrom, msgTo);
                            msg.IsBodyHtml = false;
                            msg.Subject = MailSubject;
                            msg.Body = MailBody;

                            Core.SendMail(msg, "", "");
                            

                            //*******************************Send Mail To CBUSA Primary Contact**************************************************
                            string MailBodyPri = DsResult.Tables[1].Rows[0]["BuilderName"].ToString() + " has opted out of the " + DsResult.Tables[0].Rows[0]["Name"].ToString() + ".";
                            string MailSubjectPri = "OptOut-Builder";
                            MailAddress msgFromPri = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");
                            MailAddress msgToPri = new MailAddress(DsResult.Tables[0].Rows[0]["ContactEmail"].ToString(), DsResult.Tables[0].Rows[0]["ContactName"].ToString());
                            //MailAddress msgToPri = new MailAddress("dmetey@medullus.com", DsResult.Tables[0].Rows[0]["ContactName"].ToString());
                            MailMessage msgPri = new MailMessage(msgFromPri, msgToPri);
                            msgPri.IsBodyHtml = false;
                            msgPri.Subject = MailSubjectPri;
                            msgPri.Body = MailBodyPri;

                            Core.SendMail(msgPri, "", "");
                            MailAddress AdditionalmsgTo;
                            MailMessage AdditionalmsgPri;
                            foreach (string temp in DsResult.Tables[0].Rows[0]["AdditionalContact"].ToString().Split(','))
                            {
                                if(!string.IsNullOrEmpty(temp))
                                {
                                    AdditionalmsgTo = new MailAddress(temp, "");
                                    AdditionalmsgPri = new MailMessage(msgFromPri, AdditionalmsgTo);
                                    AdditionalmsgPri.IsBodyHtml = false;
                                    AdditionalmsgPri.Subject = MailSubjectPri;
                                    AdditionalmsgPri.Body = MailBodyPri;

                                    Core.SendMail(AdditionalmsgPri, "", "");
                                }
                                

                            }

                        }
                    }
                }
                else if (RetValue == 2)
                {
                    //ScriptManager.RegisterStartupScript(this, typeof(Page), "alert", "alert('Event Deadline Already Over');", true);
                    mailErrorContent.Visible = true;
                    mailSuccessContent.Visible = false;

                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "alert", "alert('This url not accessable');", true);
            }
        }
    }
}