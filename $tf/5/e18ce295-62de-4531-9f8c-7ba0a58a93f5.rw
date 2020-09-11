using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using Components;
using DataAccessLayer;

namespace CPModuleScheduleEmail
{
    public class Program
    {
        DataAccess Da = new DataAccess();
        static void Main(string[] args)
        {
            Program prg = new Program();
            prg.SendEmailScheduledInvitation();
            prg.SendEmailToNewlyAddedBuilders();
        }

        public void SendEmailScheduledInvitation()
        {
            SaveTaskLog("Scheduled Invitation", "Start", "Start");//Task Check
            DataSet DsResult = new DataSet();
            List<SqlParameter> SpList = new List<SqlParameter>();

            int RetValue = Da.CallStoreProcedure("SendScheduledInvitationEmail", SpList, ref DsResult);
            if (DsResult != null && DsResult.Tables.Count > 0)
            {
                if (DsResult.Tables[0] != null && DsResult.Tables[0].Rows.Count > 0)
                {
                    SaveTaskLog("Scheduled Invitation", "No of Event", "No of Event = " + DsResult.Tables[0].Rows.Count); //Task Check
                    for (int i = 0; i < DsResult.Tables[0].Rows.Count; i++)
                    {
                        string MailSubject = DsResult.Tables[0].Rows[i]["MailSubject"].ToString();
                        string MailBody = System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[i]["MailBody"].ToString());
                        if (!string.IsNullOrEmpty(MailSubject))
                        {
                            MailSubject = MailSubject.Replace("{{%EventTitle%}}", DsResult.Tables[0].Rows[i]["EventTitle"].ToString());
                            MailSubject = MailSubject.Replace("{{%EventStart%}}", DsResult.Tables[0].Rows[i]["EventStart"].ToString());
                            MailSubject = MailSubject.Replace("{{%EventEnd%}}", DsResult.Tables[0].Rows[i]["EventEnd"].ToString());
                            MailSubject = MailSubject.Replace("{{%EventDescription%}}", System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[i]["EventDescription"].ToString()));
                            MailSubject = MailSubject.Replace("{{%ResponseDeadline%}}", DsResult.Tables[0].Rows[i]["ResponseDeadline"].ToString());
                            MailSubject = MailSubject.Replace("{{%ResponseDeadlineFull%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineFull"].ToString());
                            MailSubject = MailSubject.Replace("{{%ResponseDeadlineDay%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineDay"].ToString());
                            MailSubject = MailSubject.Replace("{{%ResponseDeadlineFullDay%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineFullDay"].ToString());

                            MailSubject = MailSubject.Replace("{{%ContactName%}}", DsResult.Tables[0].Rows[i]["ContactName"].ToString());
                            MailSubject = MailSubject.Replace("{{%ContactEmail%}}", DsResult.Tables[0].Rows[i]["ContactEmail"].ToString());
                            MailSubject = MailSubject.Replace("{{%ContactPhone%}}", DsResult.Tables[0].Rows[i]["ContactPhone"].ToString());

                            string EventID = DsResult.Tables[0].Rows[i]["TwoPriceCampaignId"].ToString();

                            string OptInLink = ConfigurationManager.AppSettings["CPAppHosting"] + "/default.aspx?mod=tpc&Tcam=" + EventID + "&Opt=1";
                            //ConfigurationManager.AppSettings["CPAppHosting"] + "/builder/CpEvent/DataEntry.aspx?Tcam=" + EventID + "&Opt=1";
                            string OptIn = "<a style=\"padding: 2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;\" href=\"" + OptInLink + "\">" + DsResult.Tables[0].Rows[i]["OptIn"].ToString() + "</a>";
                            MailSubject = MailSubject.Replace("{{%OptIn%}}", OptIn);

                            MailSubject = MailSubject.Replace("{{%ReminderBlock%}}", "");
                        }
                        if (!string.IsNullOrEmpty(MailBody))
                        {
                            MailBody = MailBody.Replace("{{%EventTitle%}}", DsResult.Tables[0].Rows[i]["EventTitle"].ToString());
                            MailBody = MailBody.Replace("{{%EventStart%}}", DsResult.Tables[0].Rows[i]["EventStart"].ToString());
                            MailBody = MailBody.Replace("{{%EventEnd%}}", DsResult.Tables[0].Rows[i]["EventEnd"].ToString());
                            MailBody = MailBody.Replace("{{%EventDescription%}}", System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[i]["EventDescription"].ToString()));
                            MailBody = MailBody.Replace("{{%ResponseDeadline%}}", DsResult.Tables[0].Rows[i]["ResponseDeadline"].ToString());
                            MailBody = MailBody.Replace("{{%ResponseDeadlineFull%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineFull"].ToString());
                            MailBody = MailBody.Replace("{{%ResponseDeadlineDay%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineDay"].ToString());
                            MailBody = MailBody.Replace("{{%ResponseDeadlineFullDay%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineFullDay"].ToString());

                            MailBody = MailBody.Replace("{{%ContactName%}}", DsResult.Tables[0].Rows[i]["ContactName"].ToString());
                            MailBody = MailBody.Replace("{{%ContactEmail%}}", DsResult.Tables[0].Rows[i]["ContactEmail"].ToString());
                            MailBody = MailBody.Replace("{{%ContactPhone%}}", DsResult.Tables[0].Rows[i]["ContactPhone"].ToString());


                            string EventID = DsResult.Tables[0].Rows[i]["TwoPriceCampaignId"].ToString();

                            string OptInLink = ConfigurationManager.AppSettings["CPAppHosting"] + "/default.aspx?mod=tpc&Tcam=" + EventID + "&Opt=1";
                            //ConfigurationManager.AppSettings["CPAppHosting"] + "/builder/CpEvent/DataEntry.aspx?Tcam=" + EventID + "&Opt=1";
                            string OptIn = "<a style=\"padding: 2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;\" href=\"" + OptInLink + "\">" + DsResult.Tables[0].Rows[i]["OptIn"].ToString() + "</a>";
                            MailBody = MailBody.Replace("{{%OptIn%}}", OptIn);

                            MailBody = MailBody.Replace("{{%ReminderBlock%}}", "");
                        }

                        string TwoPriceCampaignId = DsResult.Tables[0].Rows[i]["TwoPriceCampaignId"].ToString();

                        DataSet DsResultReciept = new DataSet();
                        List<SqlParameter> SpListReciept = new List<SqlParameter>();
                        SpListReciept.Add(new SqlParameter("@TwoPriceCampaignId", TwoPriceCampaignId));
                        SpListReciept.Add(new SqlParameter("@ListType", 1));
                        int RetValueReciept = Da.CallStoreProcedure("GetReciepientList", SpListReciept, ref DsResultReciept);
                        if (DsResultReciept != null && DsResultReciept.Tables.Count > 0)
                        {
                            if (DsResultReciept.Tables[0] != null && DsResultReciept.Tables[0].Rows.Count > 0)
                            {
                                SaveTaskLog("Scheduled Invitation", "No of Builder ", "Event Name = " + DsResult.Tables[0].Rows[i]["EventTitle"].ToString() + "No of Builder = " + DsResultReciept.Tables[0].Rows.Count); //Task Check

                                for (int j = 0; j < DsResultReciept.Tables[0].Rows.Count; j++)
                                {
                                    string SendMailSubject = MailSubject;
                                    string SendMailBody = MailBody;
                                    //Subject
                                    SendMailSubject = SendMailSubject.Replace("{{%BuilderName%}}", DsResultReciept.Tables[0].Rows[j]["BuilderName"].ToString());
                                    SendMailSubject = SendMailSubject.Replace("{{%RecipientFirstName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientFirstName"].ToString());
                                    SendMailSubject = SendMailSubject.Replace("{{%RecipientLastName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientLastName"].ToString());
                                    SendMailSubject = SendMailSubject.Replace("{{%RecipientFullName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString());
                                    //Body
                                    SendMailBody = SendMailBody.Replace("{{%BuilderName%}}", DsResultReciept.Tables[0].Rows[j]["BuilderName"].ToString());
                                    SendMailBody = SendMailBody.Replace("{{%RecipientFirstName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientFirstName"].ToString());
                                    SendMailBody = SendMailBody.Replace("{{%RecipientLastName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientLastName"].ToString());
                                    SendMailBody = SendMailBody.Replace("{{%RecipientFullName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString());

                                    //////////   Opt Out Link Generate using parameter-- mod,Tcam,BuilderId,BuilderAccountID
                                    string EventID = DsResult.Tables[0].Rows[i]["TwoPriceCampaignId"].ToString();
                                    string OptOutLink = ConfigurationManager.AppSettings["CPModule"] + "/OptOut.aspx?mod=tpc&Tcam=" + EventID + "&Opt=3&BuilderId=" + DsResultReciept.Tables[0].Rows[j]["BuilderID"].ToString() + "&BuilderAccountID=" + DsResultReciept.Tables[0].Rows[j]["BuilderAccountID"].ToString();
                                    string OptOut = "<a style=\"padding:2px 4px; border:1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#cc0000; text-decoration:none;\" href=\"" + OptOutLink + "\">" + DsResult.Tables[0].Rows[i]["OptOut"].ToString() + "</a>";
                                    SendMailBody = SendMailBody.Replace("{{%OptOut%}}", OptOut);
                                    SendMailSubject = SendMailSubject.Replace("{{%OptOut%}}", OptOut);

                                    if (j == 0)
                                    {
                                        //MailAddress msgFrom = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");
                                        //MailAddress msgTo = new MailAddress("dmetey@medullus.com", DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString());

                                        if (!string.IsNullOrEmpty(DsResultReciept.Tables[0].Rows[j]["RecipientEmail"].ToString()) && !string.IsNullOrEmpty(DsResult.Tables[0].Rows[i]["ContactEmail"].ToString()))
                                        {

                                            MailAddress msgFrom = new MailAddress(DsResult.Tables[0].Rows[i]["ContactEmail"].ToString(), DsResult.Tables[0].Rows[i]["ContactName"].ToString());
                                            MailAddress msgTo = new MailAddress(DsResultReciept.Tables[0].Rows[j]["RecipientEmail"].ToString(), DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString()); //Close for Testing

                                            MailMessage msg = new MailMessage(msgFrom, msgTo);
                                            msg.IsBodyHtml = false;
                                            msg.Subject = SendMailSubject;
                                            msg.Body = SendMailBody;
                                            try
                                            {
                                                Core.SendMail(msg, "", "");
                                            }
                                            catch (Exception ex)
                                            {
                                                SaveTaskLog("Scheduled Invitation", "Mail Send", ex.Message);//Task Check
                                            }

                                            //Save Mail Sending Track Record
                                            DataSet DsResultMailTrack = new DataSet();
                                            List<SqlParameter> SpListMailTrack = new List<SqlParameter>();
                                            SpListMailTrack.Add(new SqlParameter("@TwoPriceCampaignId", DsResultReciept.Tables[0].Rows[j]["TwoPriceCampaignId"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@BuilderId", DsResultReciept.Tables[0].Rows[j]["BuilderID"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@BuilderAccountId", DsResultReciept.Tables[0].Rows[j]["BuilderAccountID"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@InvitationSendUTC", DateTime.UtcNow));
                                            SpListMailTrack.Add(new SqlParameter("@MessageId", ""));
                                            SpListMailTrack.Add(new SqlParameter("@MessageDisposition", ""));
                                            SpListMailTrack.Add(new SqlParameter("@MessageDetails", ""));
                                            SpListMailTrack.Add(new SqlParameter("@EmailType", "1"));

                                            int RetValueMailTrack = Da.CallStoreProcedure("SaveMailTransaction", SpListMailTrack, ref DsResultMailTrack);

                                            //*******************************Send Mail To CBUSA Primary Contact**************************************************
                                            MailAddress msgFromPrimary = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");

                                            MailAddress msgToPrimary = new MailAddress(DsResult.Tables[0].Rows[i]["ContactEmail"].ToString(), DsResult.Tables[0].Rows[i]["ContactName"].ToString());
                                            //MailAddress msgToPrimary = new MailAddress("abasu@medullus.com", DsResult.Tables[0].Rows[i]["ContactName"].ToString());

                                            MailMessage msgPrimary = new MailMessage(msgFromPrimary, msgToPrimary);
                                            msgPrimary.IsBodyHtml = false;
                                            msgPrimary.Subject = SendMailSubject;
                                            msgPrimary.Body = SendMailBody;

                                            try
                                            {
                                                Core.SendMail(msgPrimary, "", "");
                                            }
                                            catch (Exception ex)
                                            {
                                                SaveTaskLog("Scheduled Invitation", "Mail Send", ex.Message);//Task Check
                                            }
                                        }
                                        else
                                        {
                                            SaveTaskLog("Scheduled Invitation", "Mail Send", "mailid not configure properly"+ DsResultReciept.Tables[0].Rows[j]["BuilderID"].ToString() +" Or " + DsResult.Tables[0].Rows[i]["ContactName"].ToString());//Task Check
                                        }
                                    }
                                    else //if (j == 1)
                                    {
                                        //MailAddress msgFrom = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");
                                        //MailAddress msgTo = new MailAddress("dmetey@medullus.com", DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString());
                                        if (!string.IsNullOrEmpty(DsResultReciept.Tables[0].Rows[j]["RecipientEmail"].ToString()) && !string.IsNullOrEmpty(DsResult.Tables[0].Rows[i]["ContactEmail"].ToString()))
                                        {
                                            MailAddress msgFrom = new MailAddress(DsResult.Tables[0].Rows[i]["ContactEmail"].ToString(), DsResult.Tables[0].Rows[i]["ContactName"].ToString());
                                            MailAddress msgTo = new MailAddress(DsResultReciept.Tables[0].Rows[j]["RecipientEmail"].ToString(), DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString());//Close for Testing

                                            MailMessage msg = new MailMessage(msgFrom, msgTo);
                                            msg.IsBodyHtml = false;
                                            msg.Subject = SendMailSubject;
                                            msg.Body = SendMailBody;
                                            try
                                            {
                                                Core.SendMail(msg, "", "");
                                            }
                                            catch (Exception ex)
                                            {
                                                SaveTaskLog("Scheduled Invitation", "Mail Send", ex.Message);//Task Check
                                            }

                                            //Save Mail Sending Track Record
                                            DataSet DsResultMailTrack = new DataSet();
                                            List<SqlParameter> SpListMailTrack = new List<SqlParameter>();
                                            SpListMailTrack.Add(new SqlParameter("@TwoPriceCampaignId", DsResultReciept.Tables[0].Rows[j]["TwoPriceCampaignId"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@BuilderId", DsResultReciept.Tables[0].Rows[j]["BuilderID"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@BuilderAccountId", DsResultReciept.Tables[0].Rows[j]["BuilderAccountID"].ToString()));
                                            SpListMailTrack.Add(new SqlParameter("@InvitationSendUTC", DateTime.UtcNow));
                                            SpListMailTrack.Add(new SqlParameter("@MessageId", ""));
                                            SpListMailTrack.Add(new SqlParameter("@MessageDisposition", ""));
                                            SpListMailTrack.Add(new SqlParameter("@MessageDetails", ""));
                                            SpListMailTrack.Add(new SqlParameter("@EmailType", "1"));

                                            int RetValueMailTrack = Da.CallStoreProcedure("SaveMailTransaction", SpListMailTrack, ref DsResultMailTrack);
                                        }
                                        else
                                        {
                                            SaveTaskLog("Scheduled Invitation", "Mail Send", "mailid not configure properly" + DsResultReciept.Tables[0].Rows[j]["BuilderID"].ToString() + " Or " + DsResult.Tables[0].Rows[i]["ContactName"].ToString());//Task Check
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SaveTaskLog("Scheduled Invitation", "No of Builder", "No of Builder = 0"); //Task Check
                            }
                        }

                    }

                }
                else
                {
                    SaveTaskLog("Scheduled Invitation", "No of Event", "No of Event = 0"); //Task Check
                }
            }
            SaveTaskLog("Scheduled Invitation", "Complete", "Complete");
        }

        public void SendEmailToNewlyAddedBuilders()
        {
            SaveTaskLog("Scheduled Invitation NewlyAddedBuilders", "Start", "Start"); //Task Check
            DataSet DsResult = new DataSet();
            List<SqlParameter> SpList = new List<SqlParameter>();

            int RetValue = Da.CallStoreProcedure("SendEmailToNewlyAddedBuilders", SpList, ref DsResult);
            if (DsResult != null && DsResult.Tables.Count > 0)
            {
                if (DsResult.Tables[0] != null && DsResult.Tables[0].Rows.Count > 0)
                {
                    SaveTaskLog("Scheduled Invitation NewlyAddedBuilders", "No of Event", "No of Event = " + DsResult.Tables[0].Rows.Count); //Task Check
                    for (int i = 0; i < DsResult.Tables[0].Rows.Count; i++)
                    {
                        string MailSubject = DsResult.Tables[0].Rows[i]["MailSubject"].ToString();
                        string MailBody = System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[i]["MailBody"].ToString());
                        if (!string.IsNullOrEmpty(MailSubject))
                        {
                            MailSubject = MailSubject.Replace("{{%EventTitle%}}", DsResult.Tables[0].Rows[i]["EventTitle"].ToString());
                            MailSubject = MailSubject.Replace("{{%EventStart%}}", DsResult.Tables[0].Rows[i]["EventStart"].ToString());
                            MailSubject = MailSubject.Replace("{{%EventEnd%}}", DsResult.Tables[0].Rows[i]["EventEnd"].ToString());
                            MailSubject = MailSubject.Replace("{{%EventDescription%}}", System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[i]["EventDescription"].ToString()));
                            MailSubject = MailSubject.Replace("{{%ResponseDeadline%}}", DsResult.Tables[0].Rows[i]["ResponseDeadline"].ToString());
                            MailSubject = MailSubject.Replace("{{%ResponseDeadlineFull%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineFull"].ToString());
                            MailSubject = MailSubject.Replace("{{%ResponseDeadlineDay%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineDay"].ToString());
                            MailSubject = MailSubject.Replace("{{%ResponseDeadlineFullDay%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineFullDay"].ToString());

                            MailSubject = MailSubject.Replace("{{%ContactName%}}", DsResult.Tables[0].Rows[i]["ContactName"].ToString());
                            MailSubject = MailSubject.Replace("{{%ContactEmail%}}", DsResult.Tables[0].Rows[i]["ContactEmail"].ToString());
                            MailSubject = MailSubject.Replace("{{%ContactPhone%}}", DsResult.Tables[0].Rows[i]["ContactPhone"].ToString());

                            string EventID = DsResult.Tables[0].Rows[i]["TwoPriceCampaignId"].ToString();

                            string OptInLink = ConfigurationManager.AppSettings["CPAppHosting"] + "/default.aspx?mod=tpc&Tcam=" + EventID + "&Opt=1";
                            // ConfigurationManager.AppSettings["CPAppHosting"] + "/builder/CpEvent/DataEntry.aspx?Tcam=" + EventID + "&Opt=1";
                            string OptIn = "<a style=\"padding: 2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;\" href=\"" + OptInLink + "\">" + DsResult.Tables[0].Rows[i]["OptIn"].ToString() + "</a>";
                            MailSubject = MailSubject.Replace("{{%OptIn%}}", OptIn);

                            MailSubject = MailSubject.Replace("{{%ReminderBlock%}}", "");
                        }
                        if (!string.IsNullOrEmpty(MailBody))
                        {
                            MailBody = MailBody.Replace("{{%EventTitle%}}", DsResult.Tables[0].Rows[i]["EventTitle"].ToString());
                            MailBody = MailBody.Replace("{{%EventStart%}}", DsResult.Tables[0].Rows[i]["EventStart"].ToString());
                            MailBody = MailBody.Replace("{{%EventEnd%}}", DsResult.Tables[0].Rows[i]["EventEnd"].ToString());
                            MailBody = MailBody.Replace("{{%EventDescription%}}", System.Net.WebUtility.HtmlDecode(DsResult.Tables[0].Rows[i]["EventDescription"].ToString()));
                            MailBody = MailBody.Replace("{{%ResponseDeadline%}}", DsResult.Tables[0].Rows[i]["ResponseDeadline"].ToString());
                            MailBody = MailBody.Replace("{{%ResponseDeadlineFull%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineFull"].ToString());
                            MailBody = MailBody.Replace("{{%ResponseDeadlineDay%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineDay"].ToString());
                            MailBody = MailBody.Replace("{{%ResponseDeadlineFullDay%}}", DsResult.Tables[0].Rows[i]["ResponseDeadlineFullDay"].ToString());

                            MailBody = MailBody.Replace("{{%ContactName%}}", DsResult.Tables[0].Rows[i]["ContactName"].ToString());
                            MailBody = MailBody.Replace("{{%ContactEmail%}}", DsResult.Tables[0].Rows[i]["ContactEmail"].ToString());
                            MailBody = MailBody.Replace("{{%ContactPhone%}}", DsResult.Tables[0].Rows[i]["ContactPhone"].ToString());

                            string EventID = DsResult.Tables[0].Rows[i]["TwoPriceCampaignId"].ToString();

                            string OptInLink = ConfigurationManager.AppSettings["CPAppHosting"] + "/default.aspx?mod=tpc&Tcam=" + EventID + "&Opt=1";
                            //ConfigurationManager.AppSettings["CPAppHosting"] + "/builder/CpEvent/DataEntry.aspx?Tcam=" + EventID + "&Opt=1";
                            string OptIn = "<a style=\"padding: 2px 4px; border: 1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#38761d;text-decoration:none;\" href=\"" + OptInLink + "\">" + DsResult.Tables[0].Rows[i]["OptIn"].ToString() + "</a>";
                            MailBody = MailBody.Replace("{{%OptIn%}}", OptIn);

                            MailBody = MailBody.Replace("{{%ReminderBlock%}}", "");
                        }

                        string TwoPriceCampaignId = DsResult.Tables[0].Rows[i]["TwoPriceCampaignId"].ToString();

                        DataSet DsResultReciept = new DataSet();
                        List<SqlParameter> SpListReciept = new List<SqlParameter>();
                        SpListReciept.Add(new SqlParameter("@TwoPriceCampaignId", TwoPriceCampaignId));
                        SpListReciept.Add(new SqlParameter("@ListType", 2));
                        int RetValueReciept = Da.CallStoreProcedure("GetReciepientList", SpListReciept, ref DsResultReciept);
                        if (DsResultReciept != null && DsResultReciept.Tables.Count > 0)
                        {
                            if (DsResultReciept.Tables[0] != null && DsResultReciept.Tables[0].Rows.Count > 0)
                            {
                                SaveTaskLog("Scheduled Invitation", "No of Builder ", "Event Name = " + DsResult.Tables[0].Rows[i]["EventTitle"].ToString() + "No of Builder = " + DsResultReciept.Tables[0].Rows.Count); //Task Check

                                for (int j = 0; j < DsResultReciept.Tables[0].Rows.Count; j++)
                                {
                                    string SendMailSubject = MailSubject;
                                    string SendMailBody = MailBody;
                                    //Subject
                                    SendMailSubject = SendMailSubject.Replace("{{%BuilderName%}}", DsResultReciept.Tables[0].Rows[j]["BuilderName"].ToString());
                                    SendMailSubject = SendMailSubject.Replace("{{%RecipientFirstName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientFirstName"].ToString());
                                    SendMailSubject = SendMailSubject.Replace("{{%RecipientLastName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientLastName"].ToString());
                                    SendMailSubject = SendMailSubject.Replace("{{%RecipientFullName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString());
                                    //Body
                                    SendMailBody = SendMailBody.Replace("{{%BuilderName%}}", DsResultReciept.Tables[0].Rows[j]["BuilderName"].ToString());
                                    SendMailBody = SendMailBody.Replace("{{%RecipientFirstName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientFirstName"].ToString());
                                    SendMailBody = SendMailBody.Replace("{{%RecipientLastName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientLastName"].ToString());
                                    SendMailBody = SendMailBody.Replace("{{%RecipientFullName%}}", DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString());

                                    //////////   Opt Out Link Generate using parameter-- mod,Tcam,BuilderId,BuilderAccountID
                                    string EventID = DsResult.Tables[0].Rows[i]["TwoPriceCampaignId"].ToString();
                                    string OptOutLink = ConfigurationManager.AppSettings["CPModule"] + "/OptOut.aspx?mod=tpc&Tcam=" + EventID + "&Opt=3&BuilderId=" + DsResultReciept.Tables[0].Rows[j]["BuilderID"].ToString() + "&BuilderAccountID=" + DsResultReciept.Tables[0].Rows[j]["BuilderAccountID"].ToString();
                                    string OptOut = "<a style=\"padding:2px 4px; border:1px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#cc0000; text-decoration:none;\" href=\"" + OptOutLink + "\">" + DsResult.Tables[0].Rows[i]["OptOut"].ToString() + "</a>";
                                    SendMailBody = SendMailBody.Replace("{{%OptOut%}}", OptOut);
                                    SendMailSubject = SendMailSubject.Replace("{{%OptOut%}}", OptOut);

                                    //if (j == 0)
                                    //{
                                    //MailAddress msgFrom = new MailAddress("customerservice@cbusa.us", "CBUSA Customer Service");
                                    //MailAddress msgTo = new MailAddress("dmetey@medullus.com", DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString());
                                    if (!string.IsNullOrEmpty(DsResultReciept.Tables[0].Rows[j]["RecipientEmail"].ToString()) && !string.IsNullOrEmpty(DsResult.Tables[0].Rows[i]["ContactEmail"].ToString()))
                                    {
                                        MailAddress msgFrom = new MailAddress(DsResult.Tables[0].Rows[i]["ContactEmail"].ToString(), DsResult.Tables[0].Rows[i]["ContactName"].ToString());
                                        MailAddress msgTo = new MailAddress(DsResultReciept.Tables[0].Rows[j]["RecipientEmail"].ToString(), DsResultReciept.Tables[0].Rows[j]["RecipientFullName"].ToString());

                                        MailMessage msg = new MailMessage(msgFrom, msgTo);
                                        msg.IsBodyHtml = false;
                                        msg.Subject = SendMailSubject;
                                        msg.Body = SendMailBody;

                                        try
                                        {
                                            Core.SendMail(msg, "", "");
                                        }
                                        catch (Exception ex)
                                        {
                                            SaveTaskLog("Scheduled Invitation NewlyAddedBuilders", "Mail Send", ex.Message);//Task Check
                                        }

                                        //Save Mail Sending Track Record
                                        DataSet DsResultMailTrack = new DataSet();
                                        List<SqlParameter> SpListMailTrack = new List<SqlParameter>();
                                        SpListMailTrack.Add(new SqlParameter("@TwoPriceCampaignId", DsResultReciept.Tables[0].Rows[j]["TwoPriceCampaignId"].ToString()));
                                        SpListMailTrack.Add(new SqlParameter("@BuilderId", DsResultReciept.Tables[0].Rows[j]["BuilderID"].ToString()));
                                        SpListMailTrack.Add(new SqlParameter("@BuilderAccountId", DsResultReciept.Tables[0].Rows[j]["BuilderAccountID"].ToString()));
                                        SpListMailTrack.Add(new SqlParameter("@InvitationSendUTC", DateTime.UtcNow));
                                        SpListMailTrack.Add(new SqlParameter("@MessageId", ""));
                                        SpListMailTrack.Add(new SqlParameter("@MessageDisposition", ""));
                                        SpListMailTrack.Add(new SqlParameter("@MessageDetails", ""));
                                        SpListMailTrack.Add(new SqlParameter("@EmailType", "1"));

                                        int RetValueMailTrack = Da.CallStoreProcedure("SaveMailTransaction", SpListMailTrack, ref DsResultMailTrack);
                                        // }
                                    }
                                    else
                                    {
                                        SaveTaskLog("Scheduled Invitation", "Mail Send", "mailid not configure properly,BuilderID" + DsResultReciept.Tables[0].Rows[j]["BuilderID"].ToString() + " Or Primay ContactName " + DsResult.Tables[0].Rows[i]["ContactName"].ToString());//Task Check
                                    }
                                }
                            }
                            else
                            {
                                SaveTaskLog("Scheduled Invitation NewlyAddedBuilders", "No of Builder", "No of Builder = 0"); //Task Check
                            }
                        }

                    }

                }
                else
                {
                    SaveTaskLog("Scheduled Invitation NewlyAddedBuilders", "No of Event", "No of Event = 0"); //Task Check
                }
            }
            SaveTaskLog("Scheduled Invitation NewlyAddedBuilders", "Complete", "Complete");  //Task Check
        }

        public void SaveTaskLog(string TaskName, string Status, string Msg)
        {
            DataSet DsResult = new DataSet();
            List<SqlParameter> SpList = new List<SqlParameter>();
            SpList.Add(new SqlParameter("@TaskName", TaskName));
            SpList.Add(new SqlParameter("@Status", Status));
            SpList.Add(new SqlParameter("@Msg", Msg));

            int RetValue = Da.CallStoreProcedure("SaveTaskLog", SpList, ref DsResult);
        }

    }
}
