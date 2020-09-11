Imports Components
Imports DataLayer
Imports System.Configuration
Imports System.Configuration.ConfigurationManager
Imports System.Text

Public Class EnteredProjectionsReminder

    Public Shared Sub Run(ByVal DB As Database)
        Dim objDB As New Database
        Dim dbTaskLog As TaskLogRow = Nothing
        Dim sql As String = String.Empty
        Try

            'Test New Project Style
            'Dim dt As New DataTable
            'dt.Columns.Add("TwoPriceCampaignId")
            'dt.Columns.Add("BuilderId")
            'Dim dr As DataRow = dt.NewRow
            'dr("TwoPriceCampaignId") = "883"
            'dr("BuilderId") = "24380"
            'dt.Rows.Add(dr)
            'GetProjectsDetailsBody(DB, dt.Rows(0))


            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CPEnteredProjectionsReminder"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            '================= WRITE CODE HERE ====================
            sql = "Select distinct  tpc.TwoPriceCampaignId, tpcb.BuilderId,  tpc.Name  as EventTitle,  b.CompanyName, BldrInvi.PrimaryContact,"
            sql &= " Convert(VARCHAR(10), CAST(tpc.StartDate As Date), 101)  As EventStart,  "
            sql &= " Convert(VARCHAR(10), CAST(tpc.EndDate As Date), 101)  As EventEnd,  "
            sql &= " BldrInvi.EventDescription As EventDescription,  "
            sql &= " BldrInvi.ResponseDeadline  As ResponseDeadline,"
            sql &= " cast(getdate()+2 As Date) As Today,  "
            sql &= " Format(BldrInvi.ResponseDeadline, 'MMMM dd,yyyy') AS ResponseDeadlineFull,  "
            sql &= " Format(BldrInvi.ResponseDeadline, 'dddd,MM/dd/yyyy') as ResponseDeadlineDay,  "
            sql &= " Format(BldrInvi.ResponseDeadline,'D','en-US') AS ResponseDeadlineFullDay, "
            sql &= " amd.FirstName+' '+ amd.LastName AS ContactName,  "
            sql &= " amd.Email AS ContactEmail,  "
            sql &= " amd.Contact As ContactPhone,  "
            sql &= " amd.Email as AdminEmail,  "
            sql &= " tpc.twopricecampaignid  "
            sql &= " from TwoPriceBuilderInvitation BldrInvi "
            sql &= " inner join twopricecampaign tpc                On tpc.twopricecampaignid   = bldrinvi.twopricecampaignid  "
            sql &= " inner join TwoPriceCampaignBuilder_Rel	tpcb	on tpcb.TwoPriceCampaignId  = tpc.TwoPriceCampaignId "
            sql &= " inner join Builder b							on tpcb.BuilderId           = b.BuilderId"
            sql &= " inner join admin amd                           on BldrInvi.PrimaryContact  = amd.AdminId "
            sql &= " inner join TwoPriceBuilderProject tpbp         On tpc.twopricecampaignid   = tpbp.twopricecampaignid AND tpcb.BuilderId = tpbp.BuilderID"
            sql &= " where cast(BldrInvi.ResponseDeadline As Date)  =cast(getdate()+2 As Date)"
            sql &= " And cast(tpc.enddate As Date)  >=cast(getdate() As Date)"
            sql &= " And BldrInvi.InvitationStatus                  =   1 "
            sql &= " And tpc.Status != 'Awarded' and tpc.isactive = 1 "

            Dim dtTwoPriceEvent As DataTable = DB.GetDataTable(sql)
            Dim bHasErrors As Boolean = False
            Dim errorMsg As New StringBuilder
            Dim ecount As Integer = 0
            If dtTwoPriceEvent.Rows.Count > 0 Then

                errorMsg.AppendLine("CPEnteredProjectionsReminder Task: " & DateTime.Now)
                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "CPEnteredProjectionsReminder")
                For Each row As DataRow In dtTwoPriceEvent.Rows
                    Try

                        Dim sSubject As String = FormatSubject(DB, row, dbMsg.Subject)

                        Dim sBody As String = FormatMessage(DB, row, dbMsg.Message)
                        sBody = sBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")


                        'Automatic Message Recipients
                        If dbMsg.IsEmail And dbMsg.IsActive Then

                            Dim sqlRecipients As String =
                                "   SELECT Distinct ba.FirstName , ba.LastName,ba.Email  " _
                                & " FROM BuilderAccount ba" _
                                & " WHERE ba.BuilderId=" & row("BuilderId") & "  AND IsPrimary = 1 AND Isactive = 1 "

                            Dim dtRecipients As DataTable = DB.GetDataTable(sqlRecipients)
                            If dtRecipients.Rows.Count > 0 Then
                                For Each rowRecipients As DataRow In dtRecipients.Rows
                                    Dim addr As String = rowRecipients("Email")
                                    If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                                        addr = SysParam.GetValue(DB, "AdminEmail")
                                    End If
                                    If Core.IsEmail(addr) Then
                                        Dim FullName As String = Core.BuildFullName(Core.GetString(rowRecipients("FirstName")), "", Core.GetString(rowRecipients("LastName")))
                                        sBody = sBody.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")
                                        Dim fromEmail As String = "customerservice@cbusa.us"
                                        Dim fromName As String = row("CompanyName")
                                        Core.SendSimpleMail(fromEmail, fromName, addr, FullName, sSubject, sBody)
                                    End If
                                Next
                            End If
                        End If

                        'SendEmail(row, sSubject, sBody)

                    Catch ex As Exception
                        bHasErrors = True
                        errorMsg.AppendLine("----------------------------------")
                        errorMsg.AppendLine("Error sending to :")
                        errorMsg.AppendLine("Builder: " & Core.GetString(row("CompanyName")))
                        errorMsg.AppendLine("Event Title: " & Core.GetString(row("EventTitle")))
                        errorMsg.AppendLine("Response Dead line Day: " & Core.GetString(row("ResponseDeadlineDay")))
                        errorMsg.AppendLine(ex.Message)
                        errorMsg.AppendLine("----------------------------------")
                    End Try
                Next

                dbTaskLog = New TaskLogRow(DB)
                dbTaskLog.TaskName = "CPEnteredProjectionsReminder"
                dbTaskLog.Status = "Completed"
                dbTaskLog.LogDate = Now()
                dbTaskLog.Msg = "CP Entered Projections Reminders Sent "
                dbTaskLog.Insert()
            End If
            If bHasErrors Then
                Logger.Error(errorMsg.ToString)
                Console.WriteLine("Errors occured in this task.  Please review log4net to see detailed information regarding these errors")
            End If

        Catch ex As Exception
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "CPEnteredProjectionsReminder"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ex.Message
            dbTaskLog.Insert()
        Finally
            objDB.Close()
            DB.Close()
        End Try
    End Sub



    Private Shared Sub SendEmail(ByVal drEvent As DataRow, ByVal sSubject As String, ByVal sBody As String)

        Dim fromEmail As String = drEvent("AdminEmail")
        Dim fromName As String = drEvent("CompanyName")

        Dim ToEmail As String = drEvent("ContactEmail")
        Dim ToName As String = drEvent("CompanyName")

        If Core.IsEmail(ToEmail) Then
            Core.SendSimpleMail(fromEmail, fromName, ToEmail, ToName, sSubject, sBody)
            Console.WriteLine("Email Sent to: " & ToEmail & " | Company: " & drEvent("CompanyName") & " | Event: " & drEvent("EventTitle"))
        Else
            Throw New Exception("Builder Email was invalid")
        End If
    End Sub

    Private Shared Function FormatSubject(ByVal DB As Database, ByVal drEvent As DataRow, ByVal Subject As String) As String
        Subject = Subject.Replace("%%EventName%%", drEvent("EventTitle"))
        Return Subject
    End Function

    Private Shared Function FormatMessage(ByVal DB As Database, ByVal drEvent As DataRow, ByVal Msg As String) As String

        Dim BuilderLink As String = AppSettings("GlobalRefererName") & "/builder/CpEvent/DataEntry.aspx?Tcam=" & drEvent("TwoPriceCampaignId") & ""

        Msg = Msg.Replace("%%EventName%%", drEvent("EventTitle"))
        Msg = Msg.Replace("%%ResponseDeadlineFullDay%%", drEvent("ResponseDeadlineFullDay"))
        Msg = Msg.Replace("%%BUILDER_URL%%", BuilderLink)
        Msg = Msg.Replace("%%NAME_CONTACT_INFO%%", GetEventCreatorInfo(DB, drEvent))
        Msg = Msg.Replace("%%PROJECTS_TABLE%%", GetProjectsDetailsBody(DB, drEvent))
        Return Msg
    End Function
    Private Shared Function GetProjectsDetailsBody(ByVal DB As Database, ByVal drEvent As DataRow) As String

        Dim sBody As String = vbCrLf & vbCrLf
        Dim sb As StringBuilder = New StringBuilder()

        Dim CampaignType As Integer = DB.ExecuteScalar("SELECT TOP 1 ISNULL(Id,0)  FROM TwoPriceProjectData WHERE TwoPriceCampaignId =" & drEvent("TwoPriceCampaignId") & "")
        If CampaignType > 0 Then

            Dim dt As New DataTable

            Dim strQuery As String = "exec Proc_TwoPriceCampaignProjectData_horizontal " & drEvent("TwoPriceCampaignId") & ", " & drEvent("BuilderId") & ""
            dt = DB.GetDataTable(strQuery)
            If dt.Rows.Count > 0 Then

                sb.Append("<div style='width:650px; overflow-x:auto;'>")
                'Table start.
                sb.Append("<table cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:Arial'>")
                sb.Append("<tr><td colspan='" & dt.Columns.Count - 3 & "' style='padding:5px; text-align:center;'><b>Project Details</b></td></tr>")
                'Adding HeaderRow.
                sb.Append("<tr>")
                For Each col As DataColumn In dt.Columns
                    If col.ColumnName <> "TwoPriceCampaignId" And col.ColumnName <> "BuilderId" And col.ColumnName <> "BuilderProjectId" Then
                        sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>" & col.ColumnName & "</th>"))
                    End If
                Next
                sb.Append("</tr>")
                'Adding DataRow.
                For Each row As DataRow In dt.Rows
                    sb.Append("<tr>")
                    For Each col As DataColumn In dt.Columns
                        If col.ColumnName <> "TwoPriceCampaignId" And col.ColumnName <> "BuilderId" And col.ColumnName <> "BuilderProjectId" Then
                            sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row(col.ColumnName.ToString()).ToString & "</td>")))
                        End If
                    Next
                    sb.Append("</tr>")
                Next
                sb.Append("</table></div><br>")

            End If

        Else

            Dim sTwoPriceBuilderProjectsQuery As String = String.Empty
            sTwoPriceBuilderProjectsQuery &= "select  BuilderProjectId  , TwoPriceCampaignId  , BuilderId  , ProjectName  , Status  , ConstructionType  , Community , FloorPlan  , 
                                                 case when  TakeOffInSystem=0 then 'No' when TakeOffInSystem= 1 then 'Yes' end  TakeOffInSystem  , 
                                                 SquareFeet  , Stories  , SpecialMaterials  , Notes  , ExtraData  , RecordState  , 
                                                 tpbp.CreatedOn  , tpbp.CreatedBy  , tpbp.Modifiedon  , tpbp.ModifiedBy ,
		                                         case when ConstructionType= 1 then 'PortfolioHomes'  when ConstructionType= 2 then 'Custom Homes' end as ConstructionTypeValue,
			                                     DisplayValue as StatusValue
	                                             from TwoPriceBuilderProject  tpbp
	                                             inner join TwoPriceBuilderProjectStatus tpbps on tpbp.Status=tpbps.TwoPriceBuilderProjectStatusId  
                                                 where TwoPriceCampaignId=" & drEvent("TwoPriceCampaignId") & " And BuilderId = " & drEvent("BuilderId") & "
                                                 order by ConstructionType asc"

            Dim dtTwoPriceBuilderProjects As DataTable = DB.GetDataTable(sTwoPriceBuilderProjectsQuery)

            If dtTwoPriceBuilderProjects.Rows.Count > 0 Then

                Dim drPortfolioHomes() As DataRow = dtTwoPriceBuilderProjects.Select("ConstructionType=1")
                If drPortfolioHomes.Length > 0 Then
                    'Table start.
                    sb.Append("<table cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:Arial'>")
                    sb.Append("<tr><td colspan='4' style='padding:5px; text-align:center;'><b>Portfolio Homes</b></td></tr>")
                    'Adding HeaderRow.
                    sb.Append("<tr>")
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Community</th>"))
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Lot Number or Address</th>"))
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Plan</th>"))
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Status</th>"))
                    sb.Append("</tr>")
                    'Adding DataRow.
                    For Each row As DataRow In drPortfolioHomes
                        sb.Append("<tr>")
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("Community").ToString & "</td>")))
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("ProjectName").ToString & "</td>")))
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("FloorPlan").ToString & "</td>")))
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("StatusValue").ToString & "</td>")))
                        sb.Append("</tr>")
                    Next
                    sb.Append("</table><br>")
                    'Table end.
                End If

                Dim drCustomHomes() As DataRow = dtTwoPriceBuilderProjects.Select("ConstructionType=2")
                If drCustomHomes.Length > 0 Then
                    'Table start.
                    sb.Append("<table cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:Arial'>")
                    sb.Append("<tr><td colspan='6' style='padding:5px; text-align:center;'><b>Custom Homes</b></td></tr>")
                    'Adding HeaderRow.
                    sb.Append("<tr>")
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Project Title/Address</th>"))
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Takeoff</th>"))
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>SF</th>"))
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Stories</th>"))
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Special Materials/Notes</th>"))
                    sb.Append(("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Status</th>"))

                    sb.Append("</tr>")
                    'Adding DataRow.
                    For Each row As DataRow In drCustomHomes
                        sb.Append("<tr>")
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("ProjectName").ToString & "</td>")))
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("TakeOffInSystem").ToString & "</td>")))
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("SquareFeet").ToString & "</td>")))
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("Stories").ToString & "</td>")))
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("SpecialMaterials").ToString & "</td>")))
                        sb.Append(("<td style='width:100px;border: 1px solid #ccc'>" & (row("StatusValue").ToString & "</td>")))
                        sb.Append("</tr>")
                    Next
                    sb.Append("</table>")
                    'Table end.
                End If
            End If


        End If

        sBody = sBody & sb.ToString()
        Return sBody

    End Function

    Private Shared Function GetEventCreatorInfo(ByVal DB As Database, ByVal drEvent As DataRow) As String

        Dim sContactInfo As String = String.Empty
        Dim sb As StringBuilder = New StringBuilder()
        Dim sQuery As String = String.Empty
        sQuery &= "Select  * From Admin WHERE AdminId = " & drEvent("PrimaryContact") & ""

        Dim dtAdmin As DataTable = DB.GetDataTable(sQuery)
        If dtAdmin.Rows.Count > 0 Then
            Dim dr As DataRow = dtAdmin.Rows(0)
            Dim sBody As String = "<b> " & dr("FirstName") & " " & dr("LastName") & "</b>" & vbCrLf
            sBody &= "" & dr("Contact") & vbCrLf
            sBody &= "" & dr("Email") & vbCrLf
            sBody &= "<a href='https://cbusa.us'>www.cbusa.us</a>" & vbCrLf
            Return sContactInfo & sBody
        End If

        Return sContactInfo


    End Function

End Class
