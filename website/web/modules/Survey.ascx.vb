Option Strict Off

Imports Components
Imports DataLayer

Partial Class Survey
    Inherits ModuleControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim SQL As String = String.Empty
            SQL = "Select * From Survey Where IsActive = 1 And (StartDate Is Null Or DateDiff(dd, StartDate, GetDate()) >= 0) And (EndDate Is Null Or DateDiff(dd, EndDate, GetDate()) <= 0) "
            If Not Session("BuilderId") Is Nothing Then
                SQL &= " And IsBuilder = 1 "
            ElseIf Not Session("VendorId") Is Nothing Then
                SQL &= " And IsVendor = 1 "
            ElseIf Not Session("PIQId") Is Nothing Then
                SQL &= " And IsPIQ = 1 "
            End If

            Dim dt As DataTable = DB.GetDataTable(SQL)
            rptSurveys.DataSource = dt
            rptSurveys.DataBind()

            If dt.Rows.Count = 0 Then
                ltlMsg.Text = "<b>There are no survey available at this time.</b>"
            End If
        End If
    End Sub
    Protected Sub rptSurveys_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptSurveys.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ltlSurveyName As Literal = e.Item.FindControl("ltlSurveyName")
        Dim ltlSurveyDesc As Literal = e.Item.FindControl("ltlSurveyDesc")
        Dim ltlSurveyEndDate As Literal = e.Item.FindControl("ltlSurveyEndDate")
        Dim ltlSurveyStart As Literal = e.Item.FindControl("ltlSurveyStart")

        ltlSurveyName.Text = Core.GetString(e.Item.DataItem("DisplayTitle"))
        ltlSurveyDesc.Text = Core.GetString(e.Item.DataItem("Description"))
        If Not IsDBNull(e.Item.DataItem("EndDate")) Then
            ltlSurveyEndDate.Text = FormatDateTime(e.Item.DataItem("EndDate"), DateFormat.ShortDate)
        Else
            ltlSurveyEndDate.Text = "Open"
        End If

        ltlSurveyStart.Text = "<a class=""btnblue"" href=""/surveys/start.aspx?SurveyId=" & e.Item.DataItem("SurveyId") & """>Start Survey</a>"

    End Sub
End Class
